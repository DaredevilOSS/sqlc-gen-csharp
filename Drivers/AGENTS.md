# Drivers/AGENTS.md

This project owns **all database knowledge** and **all generated method bodies**. Read the root `AGENTS.md` first.

## Layout
| File                                   | Responsibility |
|----------------------------------------|----------------|
| `DbDriver.cs`                          | Abstract base: type resolution, nullability, overrides, parameter binding, readers, usings, package refs, hooks |
| `EnumDbDriver.cs`                      | Base for DBs with enums (Npgsql, MySQL): enum lookup, enum readers, enum extension classes |
| `NpgsqlDriver.cs`, `MySqlConnectorDriver.cs`, `SqliteDriver.cs` | One sealed class per DB: `ColumnMappings` + hook overrides + `I*` annotation implementations |
| `ColumnMapping.cs`                     | `ColumnMapping` record and the `ReaderFn` / `WriterFn` / `ConvertFunc` / `SqlMapperImplFunc` delegates |
| `QueryAnnotations.cs`                  | `IOne`, `IMany`, `IExec`, `IExecRows`, `IExecLastId`, `ICopyFrom` - the contract CodeGenerator calls |
| `Variable.cs`                          | Every identifier used inside generated code |
| `Generators/<Annotation>DeclareGen.cs` | DB-agnostic method body per annotation; calls driver hooks for anything DB-specific |
| `Generators/CommonGen.cs`              | Shared fragments: parameter list, Dapper params dict, reader loop, dataclass instantiation, slice SQL transform |
| `Generators/CancellationGen.cs`        | Every way a `CancellationToken` appears in generated code |

## Rules
- A `*DeclareGen` must never branch on the concrete driver type. If a DB needs something different, add or use a
  hook on `DbDriver` and override it in the driver.
- A driver must never emit a whole method for `:one/:many/:exec/:execrows/:execlastid`; it provides fragments via hooks.
  Only `ICopyFrom.GetCopyFromImpl` returns a whole method body (it is too DB-specific to share).
- `DbDriver` must not know about any specific DB (see "Existing violations" in the root file).
- Generated identifiers come from `Variable` (`Variable.Reader.AsVarName()` -> `reader`). Add a new enum member
  rather than writing a literal.
- Cancellation goes through `Cancellation.Argument()`, `TrailingArgument()`, `MethodParameter()`,
  `TrailingMethodParameter()`, `WrapDapperArgs()`. With the option off, output must not change.
- Every generator handles four combinations: {driver, Dapper} x {no transaction, `this.Transaction`}, and both
  `net*` and legacy (`Options.DotnetFramework.IsDotnetCore()` / `IsDotnetLegacy()`).

## Hooks a driver can override
| Hook                                    | Kind     | Purpose |
|-----------------------------------------|----------|---------|
| `ColumnMappings`                        | abstract | DB type -> C# type table (see below) |
| `TransactionClassName`                  | abstract | e.g. `NpgsqlTransaction` |
| `TransformQueryText(query)`             | abstract | rewrite sqlc SQL into the driver's parameter syntax (`$1`/`?` -> `@name`); return `""` to emit no `XxxSql` constant |
| `EstablishConnection(query)`            | abstract | connection/data-source acquisition + optional open statement |
| `CreateSqlCommand(sqlConst)`            | abstract | command creation, set text, prepare |
| `AddParametersToCommand(query)`         | virtual  | bind parameters (Npgsql adds `NpgsqlDbType` overrides) |
| `GetWriterFn(column, query)`            | virtual  | value conversion before binding (enums, sets, JSON, NodaTime, ...) |
| `GetColumnReader(column, ordinal, query)` | virtual | reader expression |
| `GetLastIdStatement(query)`             | virtual  | `:execlastid` tail; base = `ExecuteScalarAsync` + `ConvertFunc` of the id type |
| `GetCopyFromImpl(query, sqlConst)`      | `ICopyFrom` | full `:copyfrom` body |
| `GetPackageReferences()`                | virtual  | csproj `PackageReference`s (base adds Dapper, NodaTime, System.Text.Json when needed) |
| `GetUsingDirectivesForQueries/Models/Utils()` | virtual | usings per generated file |
| `GetClassBaseTypes()`, `GetConstructorStatements()`, `GetAdditionalClassMembers()` | virtual | query-class skeleton (data source, `IDisposable`) |
| `GetMemberDeclarationsForUtils()`, `GetConfigureSqlMappings()` | virtual | `Utils.cs` members, Dapper type handlers |
| `EnumDbDriver.GetEnumType / EnumToCsharpDataType / EnumToModelName / GetEnumReader / GetEnumExtensionsMembers` | abstract/virtual | enum behaviour |

## Type mappings (`ColumnMappings`)
`Dictionary<string csharpType, ColumnMapping>`; each `ColumnMapping` has:
- `DbTypes`: `dbTypeName -> DbTypeInfo(Length?, DbTypeOverride?)`. `Length` restricts the match
  (MySQL `tinyint` with length 1 -> `bool`). `DbTypeOverride` is the `NpgsqlDbType.X` passed when binding / binary copying.
- `ReaderFn(ordinal, dbType)`, `ReaderArrayFn` (arrays, Npgsql), `WriterFn(el, dbType, notNull, isDapper, isLegacy)`.
- `ConvertFunc` - converts `ExecuteScalarAsync` output; **required** if the type can be an `:execlastid` id.
- `UsingDirectives`, `SqlMapper` (Dapper registration line), `SqlMapperImpl` (Dapper `TypeHandler` class source).

Outliers:
- **First match wins, in insertion order.** `bool` (`tinyint`, length 1) must stay before `short` (`tinyint`) in MySQL.
- DB type names are compared **lowercased**; `NpgsqlDriver`'s constructor also registers every type as `pg_catalog.<type>`.
- Entries with empty `DbTypes` (`[]`) - `Instant` everywhere, SQLite `DateTime` and `bool` - are reachable only through
  overrides. Their reader/writer receives the *underlying* DB type (`dbType`) to choose a conversion
  (SQLite: `integer` -> unix seconds, `text` -> formatted string `yyyy-MM-dd HH:mm:ss`).
- Usings from `ColumnMapping.UsingDirectives` are collected only from types that appear in **query result columns**
  (`GetUsingDirectivesForColumnMappings`), not from parameters or table-only columns.
- Dapper type handlers and `SqlMapper` registrations are emitted only for types that appear in a query (`TypeExistsInQueries`).

### Nullability
- `AddNullableSuffixIfNeeded`: not-null -> no suffix. Nullable -> `?` if the type is in `NullableTypes`, or always on `net*`.
- So on **legacy** targets only types listed in `NullableTypes` get `?`. Every new **value-type** mapping must be added
  to `NullableTypes`, otherwise nullable columns generate a non-nullable value type on `netstandard*`.
- `IsColumnNotNull` honours `overrides[].csharp_type.notNull`; always use it instead of `column.NotNull`.

## Per-database specifics
**PostgreSQL (`NpgsqlDriver`)**
- Params: `$N` -> `@name` via regex. Connections from a lazy `NpgsqlDataSource`; query class is `IDisposable`.
- Enums: type may be `schema.enum` in `column.Type.Name` with an empty `Schema`; enum C# name = PascalCase(`schema.enum`)
  (schema omitted for the default schema). Writes use generated `Stringify()`, reads `To<Enum>()`.
- `:copyfrom`: SQL replaced by `COPY <table> (<cols>) FROM STDIN (FORMAT BINARY)`; rows written with `BeginBinaryImportAsync`.
- `:execlastid`: relies on a user-written `RETURNING`.
- `sqlc.slice` is not supported (sqlc limitation) - use array parameters.

**MySQL (`MySqlConnectorDriver`)**
- Params: each `?` replaced in order. Lazy `MySqlDataSource` with `ConnectionReset = true`, `MinimumPoolSize >= 1`.
- `:execlastid`: `TransformQueryText` appends `; SELECT LAST_INSERT_ID()` (driver and Dapper alike).
- `:copyfrom`: the SQL constant is empty; generated code writes the batch with CsvHelper to `input.csv` in the
  current working directory, then `MySqlBulkLoader` (needs `AllowLoadLocalInfile=true` and server `--local-infile=1`).
  Nulls are written as `\N` via `Utils.NullToStringCsvConverter`; bool/byte/byte[]/SET have dedicated converters.
- Enums are all looked up in the default schema by `column.Type.Name`; C# name = PascalCase of that name
  (sqlc names them `<table>_<column>`).
- `SET` columns are detected heuristically: `column.Length > sum(enum value lengths)` -> `HashSet<Enum>`.

**SQLite (`SqliteDriver`)**
- Params: numbered `?N` replaced by number, otherwise `?` replaced in order. A new `SqliteConnection` per call; no `IDisposable`.
- Only `integer`, `real` (-> `decimal`), `text`, `blob` map natively; `DateTime`, `bool`, `Instant` come via overrides.
- `:copyfrom`: `Utils.TransformQueryForSqliteBatch` expands the `VALUES (...)` clause at runtime with suffixed parameters.
  Does not apply writer functions or overrides.
- `:execlastid`: relies on a user-written `RETURNING`.

## Adding a type mapping
1. Add/extend the `ColumnMappings` entry in the right driver (reader, array reader, writer, `ConvertFunc`, usings, Dapper handler as needed).
2. Value type? Add it to `NullableTypes` (for a DB-specific type, add it from the driver, not the base list).
3. Add a column to `examples/config/<db>/types/schema.sql` and use it in `types/query.sql` (normal + `:copyfrom` if supported).
4. Cover it in `end2end/EndToEndScaffold/Templates/<Db>Tests.cs` (see `end2end/AGENTS.md`).
5. Update the "Supported Data Types" table in `docs/0X_<Db>.md`.
6. `make sqlc-generate && make test-plugin`.
