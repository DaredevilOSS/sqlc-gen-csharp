# AGENTS.md - sqlc-gen-csharp

**sqlc-gen-csharp** is a [sqlc](https://github.com/sqlc-dev/sqlc) plugin (written in C#) that turns a sqlc
`GenerateRequest` (protobuf: catalog + queries + options) into C# data access code for
PostgreSQL (Npgsql), MySQL (MySqlConnector), SQLite (Microsoft.Data.Sqlite) and, experimentally, SQL Server
(Microsoft.Data.SqlClient, `engine: mssql`), optionally wrapped with Dapper.

More specific instructions live next to the code - read them before working in those areas:
- `Drivers/AGENTS.md` - type mappings, driver hooks, annotation generators, enums, per-DB quirks
- `end2end/AGENTS.md` - end-to-end test scaffolding
- `docs/AGENTS.md` - documentation and README generation

---

## 1. Architecture and module ownership

### Project dependency graph (enforced by `unit-tests/RepositoryTests/ArchitectureTests.cs`)
```
GeneratedProtobuf   (sqlc protobuf, generated)            refs: -
Extensions          (generic string/collection/Roslyn helpers) refs: -
PluginOptions       (RawOptions -> Options, enums)        refs: GeneratedProtobuf
Drivers             (DB knowledge + method-body emission) refs: Extensions, PluginOptions
CodeGenerator       (orchestration + file assembly)       refs: PluginOptions, GeneratedProtobuf, Drivers
SqlcGenCsharp       (PluginRunner: stdin/stdout I/O)      refs: CodeGenerator
LocalRunner         (process plugin host, 1-liner)        refs: SqlcGenCsharp
WasmRunner          (WASM plugin host, 1-liner)           refs: SqlcGenCsharp
```
Never add a `ProjectReference` that is not in this graph. If a new edge is truly needed, update the
architecture test in the same change and explain why.

> Naming trap: `Drivers/` is not only "DB drivers". It also owns **all generated method bodies**
> (`Drivers/Generators/*DeclareGen.cs`). `CodeGenerator/Generators/` only assembles **files**
> (namespace, usings, header, which members go in which file).

### Where code goes
| You are changing...                                                    | Put it in                                                                       | Never in                                  |
|------------------------------------------------------------------------|---------------------------------------------------------------------------------|-------------------------------------------|
| Reading the request / writing the response                             | `SqlcGenCsharp/PluginRunner.cs`                                                 | the runners, CodeGenerator                |
| Which files are generated, their names, namespace style, usings order, header | `CodeGenerator/CodeGenerator.cs`, `CodeGenerator/Generators/*Gen.cs`     | Drivers                                   |
| Shape of Row / Args / Model types (record vs class, `required`/`init`) | `CodeGenerator/Generators/DataClassesGen.cs`                                    | Drivers                                   |
| Method body flow shared by all DBs (tx / no-tx, Dapper / driver)       | `Drivers/Generators/<Annotation>DeclareGen.cs`, shared bits in `CommonGen.cs`   | a concrete driver, CodeGenerator          |
| A DB-specific code fragment                                            | an override of a `DbDriver` hook in that driver (see `Drivers/AGENTS.md`)       | `*DeclareGen`, `CodeGenerator`, `DbDriver` |
| DB type -> C# type mapping                                             | that driver's `ColumnMappings`                                                  | `DbDriver` base                           |
| Enum generation / reading / writing                                    | `Drivers/EnumDbDriver.cs` (shared) or the driver's override                     | `CommonGen`, CodeGenerator                |
| `CancellationToken` in generated code                                  | only via `dbDriver.Cancellation` (`Drivers/Generators/CancellationGen.cs`)       | hand-written `cancellationToken` strings  |
| Names of generated locals/fields/properties                            | the `Variable` enum (`Drivers/Variable.cs`) + `AsVarName/AsPropertyName/AsFieldName` | string literals like `"reader"`       |
| Row/Args/Sql member naming                                             | `ClassMember` (`CodeGenerator/ClassMember.cs`)                                  | ad-hoc `$"{name}Row"`                     |
| A new plugin option                                                    | `PluginOptions/RawOptions.cs` + `PluginOptions/Options.cs` (see recipe below)   | code that parses raw JSON                 |
| Pure helpers (strings, sets, dictionaries, Roslyn trivia)              | `Extensions/` - must stay free of Query/Column/driver knowledge                 | -                                         |
| Protobuf types                                                         | regenerate `GeneratedProtobuf/` (`make generate-protobuf`)                      | hand edits                                |

### Interfaces between the layers
- **CodeGenerator -> Drivers**: CodeGenerator may only use the public surface of `DbDriver`, the
  annotation interfaces in `Drivers/QueryAnnotations.cs` (`IOne`, `IMany`, `IExec`, `IExecRows`, `IExecLastId`,
  `ICopyFrom`) and `EnumDbDriver`. It must not reference concrete drivers (`NpgsqlDriver`, ...) - the only exception
  is `CodeGenerator.InstantiateDriver`. The same applies to `Drivers/Generators/`. Enforced by
  `ArchitectureTests.TestConcreteDriversAreNotReferenced`.
- **Annotation dispatch** is a hard cast in `QueriesGen.AddMethodDeclaration` (`((IExec)dbDriver).ExecDeclare(...)`).
  A driver that doesn't implement an interface fails at runtime with a wrapped `SystemException`, not at compile time.
- **Drivers -> generated code**: drivers return C# source as strings / `MemberDeclarationSyntax`. Hooks return
  fragments (`ConnectionGenCommands`, `CommandGenCommands`, `GenCommand.WrapBlock`), never whole files.
- **Opt-in contract**: every option that is off by default (e.g. `withCancellationToken`, `useProperSingularization`)
  must leave generated output **byte-identical** when off. `sqlc diff` on the examples is the check.

### Code generation flow
```
PluginRunner.Run          read GenerateRequest from stdin
  CodeGenerator.Generate
    InitGenerators        Options(request); debugRequest -> dump request.json/request.message and stop
    InstantiateDriver     engine (postgresql|mysql|sqlite) -> NpgsqlDriver | MySqlConnectorDriver | SqliteDriver
    group queries by query *file name* -> one QueriesGen file per group
    ModelsGen (Models.cs), UtilsGen (Utils.cs, only if it has members), CsprojGen (if generateCsproj)
PluginRunner              write GenerateResponse to stdout
```

### Generated code shape (excerpt of `examples/NpgsqlExample/QuerySql.cs`)
```csharp
public class QuerySql : IDisposable                    // Npgsql/MySQL: IDisposable (own a DataSource); SQLite: no base type
{
    public QuerySql(string connectionString) : this() { ... }
    public static QuerySql WithTransaction(NpgsqlTransaction transaction) { ... }

    private const string GetAuthorSql = @"SELECT id, name, bio FROM authors WHERE name = @name LIMIT 1";
    public readonly record struct GetAuthorRow(long Id, string Name, string? Bio);   // nested in the query class
    public readonly record struct GetAuthorArgs(string Name);
    public async Task<GetAuthorRow?> GetAuthorAsync(GetAuthorArgs args) { ... }      // :many -> Task<List<Row>>
}
```
- `net8.0/net9.0/net10.0` without Dapper: Row/Args/Models are `readonly record struct` (so `:one` returns `Row?`).
- With Dapper, or on `netstandard*`: classes. `net*` -> `{ get; init; }` + `required` on not-null; legacy -> `{ get; set; }`.
- Every method has a no-transaction branch and a `this.Transaction` branch.

---

## 2. Generated / derived artifacts - never hand-edit

| Artifact                                                              | Source of truth                                          | Regenerate with                                  |
|-----------------------------------------------------------------------|----------------------------------------------------------|--------------------------------------------------|
| `examples/**` (code, `.csproj`, `request.json`, `request.message`)    | plugin code + `examples/config/**` + `sqlc.ci.yaml`      | `make sqlc-generate`                             |
| `sqlc.local.generated.yaml`, `sqlc.request.generated.yaml`            | `sqlc.ci.yaml` (`.sql` section)                          | `make sync-sqlc-options` (needs `yq`; part of `make sqlc-generate`) |
| `end2end/EndToEndTests*/**/*.generated.cs`                            | `end2end/EndToEndScaffold/` templates + `Config.cs`      | `make generate-end2end-tests`                    |
| `unit-tests/CodegenTests/test-requests/*/request.*`                   | `unit-tests/sqlc.yaml` + sibling `schema.sql`/`query.sql` | `cd unit-tests && sqlc generate` (part of `make unit-tests`) |
| `GeneratedProtobuf/Codegen.cs`                                        | sqlc's `plugin/codegen.proto`                            | `make generate-protobuf`                         |
| `docs/09_Examples.md`                                                 | `sqlc.ci.yaml`                                           | `dotnet run --project docs/ExamplesDocGen/ExampleDocGen.csproj > docs/09_Examples.md` |
| `README.md`, `docs/02_Quickstart.md`                                  | `docs/0*.md`, latest release                             | **release flow only** - see `docs/AGENTS.md`     |
| `sqlc.ci.yaml` `wasm.sha256`                                          | `dist/plugin.wasm`                                       | `make setup-wasm-plugin` (CI recomputes it)      |

`sqlc.ci.yaml` is the single source of truth for example configurations. Edit it, never the `*.generated.yaml` files.
CI fails if any of the generated artifacts above is stale (`sqlc diff`, `git diff --quiet`).

---

## 3. Outliers - things you would not guess from the design

**Naming**
- The query class name comes from the query **file name** including the extension: `query.sql` -> `QuerySql`.
  Query files with the same basename in different directories are **merged into one class** (all examples rely on
  this: `authors/query.sql` + `types/query.sql` -> one `QuerySql`).
- Model names: `ToModelName` defaults to `TrimEnd('s')`, which strips **all** trailing `s` (`address` -> `Addre`).
  `useProperSingularization` fixes it but is opt-in because it renames classes.
  Tables in a non-default schema get the schema as a prefix (`DummySchemaDummyTable`).
- The default schema is stored under the key `""` in `DbDriver.Tables` and `EnumDbDriver.Enums`.

**Options and overrides**
- Overrides match `"<QueryName>:<column>"` or `"*:<column>"`; the first matching entry in list order wins, and
  `csharp_type.notNull` replaces the column's nullability.
- Overrides apply only where a query is in context: Row/Args types and query method bodies. They **never** affect
  `Models.cs` (table models are generated with `query == null`).
- An override `type` must be a key in the driver's `ColumnMappings`, otherwise reader generation throws.
- `debugRequest: true` makes the plugin dump the request instead of generating code; it is rejected for the WASM plugin.
- `targetFramework` `net9.0`/`net10.0` generate exactly the same code as `net8.0`; only `<TargetFramework>` differs.
  `netstandard2.0/2.1` ("legacy") output: block-scoped namespace, no nullable reference types, different multi-line
  SQL constant indentation (`QueriesGen.GetQueryTextConstant`), `System.Text.Json` package reference for `JsonElement`.

**Code emission**
- Generated code is built from interpolated strings, parsed with `ParseMemberDeclaration(...)!` and formatted with
  `NormalizeWhitespace()`. Template indentation does not matter (except inside SQL string literals).
  A malformed template **does not throw** - it produces broken C#. The only real check is building the examples
  (`dotnet build`) and running the end-to-end tests.
- Generated files mix `\n` (`JoinByNewLine`) and CRLF trivia. Do not normalize line endings in `examples/` -
  `sqlc diff` will fail. Plugin sources are CRLF (`.editorconfig` `end_of_line = crlf`).
- The `:one`/`:many` generators silently fall back from Dapper to the raw driver when the query uses `sqlc.embed`.
- Dapper mode mutates global static state in the generated constructor: `Utils.ConfigureSqlMapper()` and
  `DefaultTypeMap.MatchNamesWithUnderscores = true`.

**WASM**
- The WASM build is trimmed; only `PluginOptions` is rooted (System.Text.Json reflection). New reflection-based code
  can work in the process plugin and break only in WASM.
- `WasmRunner.csproj` forces a `wasip1` core module because sqlc's wazero cannot load the wasip2 component the .NET 10
  WASI SDK produces. The override relies on private SDK item names - re-verify after any SDK upgrade.

**Repository**
- `.env` is shared by docker-compose, the end-to-end tests and CI; `RepositoryTests` asserts it is consistent with
  `benchmark/.env`.
- `make unit-tests` needs the process plugin published to `unit-tests/dist/` (`unit-tests/sqlc.yaml` uses
  `./dist/LocalRunner`); CI does `dotnet publish ../LocalRunner -c release --output dist/` from `unit-tests/`.
- `benchmark/` has its own `sqlc.yaml` and committed `*SqlcImpl` generated projects; it is excluded from format/lint.

---

## 4. Recipes (definition of done)

**Attribution**: the repository is public. Never put Claude session links (`claude.ai/code/session_...`, `Claude-Session:` trailers) in commit messages, PR descriptions or comments. Other attribution lines are fine.

**Every change**
1. `make sqlc-generate` (process plugin -> examples + requests) and commit the regenerated files.
2. `make unit-tests`.
3. `make test-plugin` (needs Docker) - unit tests, regeneration, `dotnet build`, end-to-end tests.
4. For changes that can affect the WASM build: `make setup-wasm-plugin && sqlc -f sqlc.ci.yaml diff`.
5. `make dotnet-format` (CI lints the same scope: everything except `GeneratedProtobuf`, `examples`, `benchmark/*SqlcImpl`).
6. PR label: `major` / `minor` / `patch`, or `skip-release` for non-release changes.

**Add support for a DB type** - see `Drivers/AGENTS.md` ("Adding a type mapping").

**Add a plugin option**
1. `RawOptions` property with `[JsonPropertyName("camelCase")]` and a default that preserves current output.
2. Matching read-only property in `Options` (constructor maps it); validate there.
3. Consume it via `dbDriver.Options` / `Options` - never re-read the raw JSON.
4. Unit test in `unit-tests/CodegenTests/OptionsTests.cs` (and a codegen test if it changes output).
5. Enable it in at least one example in `sqlc.ci.yaml` so it is covered end-to-end, then `make sqlc-generate`.
6. Document it in the options table in `docs/03_Usage.md` and in section 5 below.

**Add a query annotation**
1. New `I<Annotation>` interface in `Drivers/QueryAnnotations.cs`.
2. New `Drivers/Generators/<Annotation>DeclareGen.cs` following the existing ones (tx / no-tx, Dapper / driver).
3. New case in the `QueriesGen.AddMethodDeclaration` switch.
4. Implement the interface in every driver (or throw `NotSupportedException` with a clear message).
5. Queries in `examples/config/<db>/authors/query.sql`, templates in `end2end/EndToEndScaffold/Templates/AnnotationTests.cs`.
6. Update the annotation tables in `docs/03_Usage.md` and this file.

**Add a database driver**
1. `Drivers/<Name>Driver.cs` deriving from `DbDriver` (or `EnumDbDriver` if the DB has enums), implementing every `I*` interface.
2. Enum value in `PluginOptions/DriverName.cs`, engine mapping in `Options.EngineToDriverMapping`, case in `CodeGenerator.InstantiateDriver`.
3. Examples (normal / Dapper / Legacy / DapperLegacy) in `sqlc.ci.yaml`, `examples/config/<db>/`, e2e testers, docker-compose service, `docs/0X_<Db>.md`.

---

## 5. Reference

### Options (`PluginOptions/RawOptions.cs`)
| Option                        | Default                                                        | Notes |
|-------------------------------|----------------------------------------------------------------|-------|
| `targetFramework`             | `net8.0`                                                       | `netstandard2.0`, `netstandard2.1`, `net8.0`, `net9.0`, `net10.0` |
| `overrideDriverVersion`       | `2.4.0` MySqlConnector, `8.0.6` Npgsql, `10.0.13` Microsoft.Data.Sqlite | constants in each driver |
| `generateCsproj`              | `true`                                                         | |
| `namespaceName`               | output directory name                                          | |
| `useDapper`                   | `false`                                                        | |
| `overrideDapperVersion`       | `2.1.66`                                                       | `DbDriver.DefaultDapperVersion` |
| `useCentralPackageManagement` | `false`                                                        | omits `Version=` in the generated csproj |
| `withAsyncSuffix`             | `true`                                                         | |
| `withCancellationToken`       | `false`                                                        | must be byte-identical when off |
| `useProperSingularization`    | `false`                                                        | renames models when on |
| `overrides`                   | -                                                              | `Query:column` / `*:column`, see section 3 |
| `debugRequest`                | `false`                                                        | internal: dumps the request; not allowed with WASM |

### Annotation support
| Annotation / macro | PostgreSQL | MySQL | SQLite | Implementation |
|--------------------|-----------|-------|--------|----------------|
| `:one`, `:many`, `:exec`, `:execrows` | ✅ | ✅ | ✅ | shared `*DeclareGen` |
| `:execlastid`      | ✅ | ✅ | ✅ | PG/SQLite: user writes `RETURNING`; MySQL: plugin appends `; SELECT LAST_INSERT_ID()` |
| `:copyfrom`        | ✅ | ✅ | ✅ | PG: binary `COPY`; MySQL: CSV + `MySqlBulkLoader`; SQLite: multi-row `VALUES` |
| `sqlc.arg`, `sqlc.narg`, `sqlc.embed` | ✅ | ✅ | ✅ | |
| `sqlc.slice`       | 🚫 (use arrays) | ✅ | ✅ | runtime `Utils.TransformQueryForSliceArgs` |

SQL Server (experimental, `MssqlDriver`) supports every annotation and macro above. `:execlastid` relies on a user-written
`OUTPUT INSERTED.id`, `:copyfrom` uses `SqlBulkCopy`. It needs a sqlc build with the mssql macro fixes.

### Target frameworks
- The plugin projects (section 1 graph) and the tooling/test projects target `net10.0`; SDK pinned in `global.json`.
  Exceptions: `end2end/EndToEndCommon` (`netstandard2.0`) and `end2end/EndToEndTestsLegacy` (`net472`).
- Core examples are spread over net8.0 / net9.0 / net10.0 so every framework is built and tested.
- Legacy examples (`netstandard2.0`) are tested by `end2end/EndToEndTestsLegacy` (`net472`, Windows, `.github/workflows/legacy-tests.yml`).

### Commands
```bash
make unit-tests            # RepositoryTests + CodegenTests
make sqlc-generate         # sync yaml, publish LocalRunner, regenerate requests + examples
make generate-end2end-tests
make test-plugin           # everything above + build + end-to-end tests (Docker)
make setup-wasm-plugin     # WASM plugin -> dist/plugin.wasm (needs wasi-experimental workload + WASI SDK 25 in WASI_SDK_PATH)
sqlc -f sqlc.ci.yaml diff  # WASM output must equal the committed examples
make dotnet-format
```

### Release
1. `release-assistant.yml` - every PR to `main` needs `major`/`minor`/`patch` or `skip-release`.
2. `gen-release-pr.yml` - after Build on `main`, aggregates labeled PRs, computes the version, regenerates README/quickstart with the wasm sha, opens a `release-prep` PR.
3. `release.yml` - on merge of `release-prep`, tags, builds release notes from PR titles, uploads the wasm.

---

## 6. Known issues and existing layering violations

Do not copy these patterns; fix them only in a dedicated change (they may change generated output).
- `DbDriver.NullableTypes` hardcodes Npgsql types (`NpgsqlPoint`, `NpgsqlCidr`, ...) - should be added by `NpgsqlDriver`.
- `NpgsqlDriver.ColumnMappings` `string` / `byte[]` include MySQL type names (`longtext`, `tinyblob`, ...).
- `JsonElement` / `object` reader lambdas hardcode `reader` instead of `Variable.Reader.AsVarName()`.
- `EnumDbDriver` is declared in the global namespace.
- `CommonGen.ConstructDapperParamsDict` enum branch is dead: `Enums` is keyed by schema, not enum name, and the
  `ToEnumString()` it would emit does not exist.
- SQLite `:copyfrom` ignores overrides and writer functions (uses `p.Column.NotNull` directly).
- MySQL `:copyfrom` writes a fixed `input.csv` in the process working directory (not concurrency-safe).
- `GetIdColumnType` (`:execlastid` return type) is a heuristic: column `id`, else first column containing `id`, else the first column.
- Not supported: PostgreSQL `time with time zone`; MySQL geometry types; array columns get no `NpgsqlDbType` override (TODO in `DbDriver.GetColumnDbTypeOverride`).
