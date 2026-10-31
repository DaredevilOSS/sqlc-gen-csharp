# end2end/AGENTS.md

End-to-end tests run the **generated example projects** (`examples/*`) against real databases.
Read the root `AGENTS.md` first.

## Projects
| Project               | Target          | What it is |
|-----------------------|-----------------|------------|
| `EndToEndScaffold`    | `net10.0`       | Console app that **generates** the test classes from templates. |
| `EndToEndTests`       | `net10.0`       | Tests for the `net*` examples (`NpgsqlExample`, `MySqlConnectorDapperExample`, ...). |
| `EndToEndTestsLegacy` | `net472`        | Same tests for the `*LegacyExample` (`netstandard2.0`) projects. Runs only on Windows in CI (`.github/workflows/legacy-tests.yml`). |
| `EndToEndCommon`      | `netstandard2.0`| Shared helpers: env loading, SQLite DB setup. Also used by `benchmark/`. |

## How a tester class is built
Each tester (`NpgsqlTester`, `NpgsqlDapperTester`, `MySqlConnectorTester`, ... - 8 in total) is a `partial` class in two files:
- `<Tester>.cs` - **hand-written**: the `QuerySql` instance and the `[TearDown]` that empties every table.
- `<Tester>.generated.cs` - **generated, do not edit**: produced by `EndToEndScaffold` via `end2end/scripts/generate_tests.sh`
  (`make generate-end2end-tests`). CI regenerates and fails on `git diff`.

The same pair exists in both `EndToEndTests/` and `EndToEndTestsLegacy/`. The legacy hand-written files must stay
C# 7.3 / `net472` compatible (block-scoped namespaces, no target-typed `new`, no `!`).

## Adding or changing a test
1. Write the test body in `EndToEndScaffold/Templates/<Area>Tests.cs` as a `KnownTestType -> TestImpl` entry
   (`AnnotationTests`, `MacroTests`, `PostgresTests`, `MySqlTests`, `SqliteTests`).
2. Add the `KnownTestType` member and register it in the right set(s) in `EndToEndScaffold/Config.cs`
   (`_postgresTestTypes`, `_mysqlTestTypes`, `_sqliteTestTypes`). A test type is emitted into both the driver
   and Dapper tester of that DB, and into both the `net*` and legacy projects.
3. One template must compile for all four variants. Use the placeholders instead of branching:
   - `#UnknownValue#` (`Consts.UnknownRecordValuePlaceholder`) -> `.Value` where `:one` returns a nullable
     `record struct` (net*, non-Dapper), empty otherwise.
   - `#UnknownNullableIndicator#` (`Consts.UnknownNullableIndicatorPlaceholder`) -> `?` on net*, empty on legacy.
4. New table? Add its truncate/delete call to the hand-written `[TearDown]` in **both** `EndToEndTests/<Tester>.cs`
   and `EndToEndTestsLegacy/<Tester>.cs` (note: the legacy `NpgsqlTester` currently lacks
   `TruncatePostgresNotNullTypesAsync`).
5. `make generate-end2end-tests`, commit the `*.generated.cs` files, then `make run-end2end-tests`.

## Databases
- PostgreSQL and MySQL run in docker-compose. Their schemas are **baked into the images at build time**
  (`postgres.Dockerfile`, `mysql.Dockerfile` copy `examples/config/<db>/{types,authors}/schema.sql` plus
  `benchmark/config/<db>/schema.sql`). `end2end/scripts/run_tests.sh` rebuilds the images; if you start the
  containers yourself after a schema change, use `docker-compose up --build`.
- SQLite is a file DB created by `GlobalSetup` from the schemas copied into the test output
  (`authors.sqlite.schema.sql`, `types.sqlite.schema.sql`) and deleted afterwards.
- SQL Server (experimental) runs in docker-compose (`mssqldb`, plain image); its image has no init hook, so
  `EndToEndCommon.SetupTestsMssqlDb` (called from both `GlobalSetup`s) drops and recreates the database from
  `authors.mssql.schema.sql` / `types.mssql.schema.sql`, split on `GO`. The Windows legacy job uses LocalDB
  (`.github/scripts/start-mssql.ps1` sets `MSSQL_CONNECTION_STRING`).
- Connection strings come from the root `.env` (`POSTGRES_CONNECTION_STRING`, `MYSQL_CONNECTION_STRING`,
  `SQLITE_CONNECTION_STRING`, `MSSQL_CONNECTION_STRING`); a variable already set in the environment wins over the
  file. The MySQL one needs `AllowLoadLocalInfile=true` for `:copyfrom`.
- `Config.NotForLegacy` lists test types left out of the legacy testers (spatial types need a native library on
  .NET Framework).
- `end2end/MssqlSyntaxMatrix` is not a test project: it is the T-SQL support matrix (see its README).
- The legacy workflow (Windows) loads the PostgreSQL schemas itself with `psql`; keep it in mind when adding schema files.
