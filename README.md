# <p align="center">sqlc-gen-csharp</p>
<p align="center">
    <img src="https://github.com/DaredevilOSS/sqlc-gen-csharp/actions/workflows/build.yml/badge.svg?branch=main" alt="Build">
    <img src="https://github.com/DaredevilOSS/sqlc-gen-csharp/actions/workflows/tests.yml/badge.svg?branch=main" alt=".Net Core Tests">
    <img src="https://github.com/DaredevilOSS/sqlc-gen-csharp/actions/workflows/legacy-tests.yml/badge.svg?branch=main" alt=".Net Framework Tests (Legacy)">
</p>

sqlc-gen-csharp is a .Net plugin for [sqlc](https://github.com/sqlc-dev/sqlc).<br/> It leverages the SQLC plugin system to generate type-safe C# code for SQL queries, supporting  PostgresSQL, MySQL & SQLite via the corresponding driver or suitable Dapper abstraction.
## Quickstart
```yaml
version: "2"
plugins:
- name: csharp
  wasm:
    url: https://github.com/DaredevilOSS/sqlc-gen-csharp/releases/download/v0.25.0/sqlc-gen-csharp.wasm
    sha256: 2b1693010e41fb10fe5f0677bfcf6666e6cb471b6cc9ab3d459a23d6b42f17d5
sql:
  # For PostgresSQL
  - schema: schema.sql
    queries: queries.sql
    engine: postgresql
    codegen:
      - plugin: csharp
        out: PostgresDalGen
  # For MySQL
  - schema: schema.sql
    queries: queries.sql
    engine: mysql
    codegen:
      - plugin: csharp
        out: MySqlDalGen
  # For SQLite
  - schema: schema.sql
    queries: queries.sql
    engine: sqlite
    codegen:
      - plugin: csharp
        out: SqliteDalGen
```
# Usage
## Options

| Option                      | Possible values                                                                                                                                                                   | Optional | Info                                                                                                                                                                                                                                                                                                                                      |
|-----------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| driverVersion               | default:<br/> `2.4.0` for MySqlConnector  (mysql)<br/>`8.0.6` for Npgsql (postgresql)<br/>`10.0.13` for Microsoft.Data.Sqlite (sqlite)<br/><br/>values: The desired driver version | Yes      | Allows you to override the version of DB driver to be used.                                                                                                                                                                                                                                                                               |
| targetFramework             | default: `net8.0`<br/>values: `netstandard2.0`, `netstandard2.1`, `net8.0`, `net9.0`, `net10.0`                                                                                                        | Yes      | Determines the target framework for your generated code, meaning the generated code will be compiled to the specified runtime.<br/>For more information and help deciding on the right value, refer to the [Microsoft .NET Standard documentation](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-1-0). |
| generateCsproj              | default: `true`<br/>values: `false`,`true`                                                                                                                                        | Yes      | Assists you with the integration of SQLC and csharp by generating a `.csproj` file. This converts the generated output to a .dll, a project that you can easily incorporate into your build process.                                                                                                                                      |
| namespaceName               | default: the generated project name                                                                                                                                               | Yes      | Allows you to override the namespace name to be different than the project name                                                                                                                                                                                                                                                           |
| useDapper                   | default: `false`<br/>values: `false`,`true`                                                                                                                                       | Yes      | Enables Dapper as a thin wrapper for the generated code. For more information, please refer to the [Dapper documentation](https://github.com/DapperLib/Dapper).                                                                                                                                                                           |
| dapperVersion               | default:<br/> `2.1.66`<br/>values: The desired Dapper version                                                                                                                     | Yes      | If `useDapper` is set to `true`, this option allows you to override the version of Dapper to be used.                                                                                                                                                                                                                                     |
| Override                    | values: A nested override value like [this](#override-option).                                                                                                                    | Yes      | Allows you to override the generated C# data types for specific columns in specific queries. This option accepts a `query_name:column_name` mapping and the overriden data type.                                                                                                                                                          |                                                                                     |
| useCentralPackageManagement | default: `false`<br/>values: `false`,`true`                                                                                                                                       | Yes      | If true, the code is generated to support central package management.                                                                                                                                                                                                                                                                     |
| useProperSingularization    | default: `false`<br/>values: `false`,`true`                                                                                                                                       | Yes      | When true, model names are singularized using English grammar rules (`policies` -> `Policy`, `statuses` -> `Status`). Defaults to the legacy behavior of trimming trailing `s` characters, which mangles some names (`policies` -> `Policie`). Opt-in because enabling it renames existing generated classes.                          |
| useAsyncSuffix              | default: `true`<br/>values: `false`,`true`                                                                                                                                       | Yes      | When true, async methods will have the "Async" suffix appended to their names (e.g., `GetAuthorAsync`). When false, async methods will not have the suffix (e.g., `GetAuthor`).                                                                                                                                                        |
| useCancellationToken        | default: `false`<br/>values: `false`,`true`                                                                                                                                      | Yes      | When true, every generated method takes an optional trailing `CancellationToken cancellationToken = default`, threaded into every async DB call so in-flight queries can be cancelled. See [Cancellation](#cancellation). When false (default), generated output is unchanged.                                                          |
| useAsyncEnumerable          | default: `false`<br/>values: `false`,`true` | Yes | When true, `:many` queries return `IAsyncEnumerable<Row>` streamed row by row (`yield return`; Dapper uses `QueryUnbufferedAsync`) instead of `Task<List<Row>>`. Not supported with `netstandard2.0`. With `useCancellationToken` the token is marked `[EnumeratorCancellation]`; with Dapper it only cancels the enumeration (Dapper's unbuffered query takes no cancellation token). When false (default), generated output is unchanged. |

**Renamed options:** `overrideDriverVersion` -> `driverVersion`, `overrideDapperVersion` -> `dapperVersion`, `withAsyncSuffix` -> `useAsyncSuffix`, `withCancellationToken` -> `useCancellationToken`. The old names are deprecated but still accepted; setting both spellings to different values is an error.

### Target framework
`targetFramework` sets the framework of the generated `.csproj`, and is independent of the .NET version the plugin itself runs on.

| Value | Notes |
|-------|-------|
| `net8.0` (default) | Nullable reference types enabled, file-scoped namespaces |
| `net9.0`, `net10.0` | Same generated code as `net8.0`, only the `<TargetFramework>` differs |
| `netstandard2.1`, `netstandard2.0` | Legacy output - nullable reference types disabled, block-scoped namespaces |

The default stays `net8.0` so that upgrading the plugin never changes the target of existing projects.

### Override option
Override for a specific query:
```yaml
overrides:
  - column: "<query-name>:<field-name>"
    csharp_type:
      type: "<csharp-datatype>"
      notNull: true|false
```

Override for all queries:
```yaml
overrides:
  - column: "*:<field-name>"
    csharp_type:
      type: "<csharp-datatype>"
      notNull: true|false
```

## Supported Features
- ✅ means the feature is fully supported.
- ⚠️ means SQLC does not yet support this feature, so it cannot be supported by the plugin yet.
- 🚫 means the database does not support the feature.
- ❌ means the feature is not supported by the plugin (but could be supported by the database).

### Query Annotations
Basic functionality - same for all databases:
- `:one`          - returns 0...1 records
- `:many`         - returns 0...n records
- `:exec`         - DML / DDL that does not return anything
- `:execrows`     - returns number of affected rows by DML

Advanced functionality - varies between databases:
- `:execlastid`   - INSERT with returned last inserted id
- `:copyfrom`     - batch insert, implementation varies greatly
<br/>

| Annotation  | PostgresSQL | MySQL | SQLite  |
|-------------|-------------|-------|---------|
| :one        | ✅          | ✅    | ✅      |
| :many       | ✅          | ✅    | ✅      |
| :exec       | ✅          | ✅    | ✅      |
| :execrows   | ✅          | ✅    | ✅      |
| :execlastid | ✅          | ✅    | ✅      |
| :copyfrom   | ✅          | ✅    | ✅      |

More info can be found in [here](https://docs.sqlc.dev/en/stable/reference/query-annotations.html).

### Macro Annotations
- `sqlc.arg`       - Attach a name to a parameter in a SQL query
- `sqlc.narg`      - The same as `sqlc.arg`, but always marks the parameter as nullable
- `sqlc.slice`     - For databases that do not support passing arrays to the `IN` operator, generates a dynamic query at runtime with the correct number of parameters
- `sqlc.embed`     - Embedding allows you to reuse existing model structs in more queries
<br/>

| Annotation  | PostgresSQL | MySQL | SQLite  |
|-------------|-------------|-------|---------|
| sqlc.arg    | ✅          | ✅    | ✅       |
| sqlc.narg   | ✅          | ✅    | ✅       |
| sqlc.slice  | 🚫          | ✅    | ✅       |
| sqlc.embed  | ✅          | ✅    | ✅       |

More info can be found in [here](https://docs.sqlc.dev/en/stable/reference/macros.html#macros).

### Transactions
Transactions are supported by the plugin.
<br/>

#### Example using a transaction
```C#
public async Task ExampleTransaction(IDbConnection connection)
{
    // Begin a transaction
    using (var transaction = connection.BeginTransaction())
    {
        try
        {
            // Create a new Queries object with the transaction instead of the connection
            var queries = QuerySql.WithTransaction(transaction);

            // Example: Insert a new author
            var newAuthor = await queries.CreateAuthor(new CreateAuthorParams { Name = "Jane Doe", Bio = "Another author" });

            // Example: Get the author by ID within the same transaction
            var author = await queries.GetAuthor(newAuthor.AuthorID);

            // Example: Update the author's bio
            await queries.UpdateAuthorBio(new UpdateAuthorBioParams { AuthorID = author.AuthorID, Bio = "Updated bio for Jane Doe" });

            // Commit the transaction if all operations are successful
            transaction.Commit();
            Console.WriteLine("Transaction committed successfully.");
        }
        catch (Exception ex)
        {
            // Rollback the transaction if any error occurs
            transaction.Rollback();
            Console.WriteLine($"Transaction rolled back due to error: {ex.Message}");
            throw;
        }
    }
}
```

More info can be found in [here](https://docs.sqlc.dev/en/stable/howto/transactions.html).

### Cancellation
Set `useCancellationToken: true` to make every generated method accept an optional
`CancellationToken`. The token is threaded into every async database call, so a cancelled
token interrupts the in-flight query and releases the locks that statement holds. The
option is off by default; when off, generated output is identical to before.

```C#
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
try
{
    var author = await queries.GetAuthor(authorId, cts.Token);
}
catch (OperationCanceledException)
{
    // the query was cancelled; its statement-level locks are released
}
```

Notes:
- **PostgreSQL & MySQL** interrupt the running statement server-side (Npgsql sends a cancel
  request; MySqlConnector issues `KILL QUERY`), releasing that statement's locks immediately.
- **SQLite** runs synchronously and only honours the token *before* the statement starts;
  an already-running SQLite statement is not interrupted.
- **Transactions:** cancelling a query inside a `WithTransaction(...)` flow releases only that
  statement's locks. Locks held by earlier statements in the transaction (and, on PostgreSQL,
  the aborted-transaction state) are released when **you** roll back — the generated code never
  rolls back for you.
# PostgresSQL
<details>
<summary>:execlastid - Implementation</summary>

Implemented via a `RETURNING` clause, allowing the `INSERT` command to return the newly created id.
The data types that can be used as id data types for this annotation are:
1. uuid
2. bigint
3. integer
4. smallint (less recommended due to small id range, but possible)

```sql
INSERT INTO tab1 (field1, field2) VALUES ('a', 1) RETURNING id_field;
```
</details>

<details>
<summary>:copyfrom - Implementation</summary>

Implemented via the `COPY FROM` command which can load binary data directly from `stdin`.
</details>

<details open>
<summary>Supported Data Types</summary>

Since in batch insert the data is not validated by the SQL itself but written in a binary format, 
we consider support for the different data types separately for batch inserts and everything else.

| DB Type                                 | Supported? | Supported in Batch? |
|-----------------------------------------|------------|-------------------- |
| boolean                                 | ✅         | ✅                  |
| smallint                                | ✅         | ✅                  |
| integer                                 | ✅         | ✅                  |
| bigint                                  | ✅         | ✅                  |
| real                                    | ✅         | ✅                  |
| decimal, numeric                        | ✅         | ✅                  |
| double precision                        | ✅         | ✅                  |
| date                                    | ✅         | ✅                  |
| timestamp, timestamp without time zone  | ✅         | ✅                  |
| timestamp with time zone                | ✅         | ✅                  |
| time, time without time zone            | ✅         | ✅                  |
| time with time zone                     | 🚫         | 🚫                  |
| interval                                | ✅         | ✅                  |
| char                                    | ✅         | ✅                  |
| bpchar                                  | ✅         | ✅                  |
| varchar, character varying              | ✅         | ✅                  |
| text                                    | ✅         | ✅                  |
| bytea                                   | ✅         | ✅                  |
| 2-dimensional arrays (e.g text[],int[]) | ✅         | ✅                  |
| money                                   | ✅         | ✅                  |
| point                                   | ✅         | ✅                  |
| line                                    | ✅         | ✅                  |
| lseg                                    | ✅         | ✅                  |
| box                                     | ✅         | ✅                  |
| path                                    | ✅         | ✅                  |
| polygon                                 | ✅         | ✅                  |
| circle                                  | ✅         | ✅                  |
| cidr                                    | ✅         | ✅                  |
| inet                                    | ✅         | ✅                  |
| macaddr                                 | ✅         | ✅                  |
| macaddr8                                | ✅         | ⚠️                  |
| tsvector                                | ✅         | ❌                   |
| tsquery                                 | ✅         | ❌                   |
| uuid                                    | ✅         | ✅                  |
| json                                    | ✅         | ✅                  |
| jsonb                                   | ✅         | ✅                  |
| jsonpath                                | ✅         | ⚠️                  |
| xml                                     | ✅         | ⚠️                  |
| enum                                    | ✅         | ⚠️                  |
| any                                     | ✅         | ❌                   |
| hstore                                  | ✅         | ❌                   |

*** `time with time zone` is not useful and not recommended to use by Postgres themselves - 
see [here](https://www.postgresql.org/docs/current/datatype-datetime.html#DATATYPE-DATETIME) -
so we decided not to implement support for it.

*** Some data types require conversion in the INSERT statement, and SQLC disallows argument conversion in queries with `:copyfrom` annotation, which are used for batch inserts. 
These are the data types that require this conversion:
1. `macaddr8`
2. `jsonpath`
3. `xml`
4. `enum`

An example of this conversion:
```sql
INSERT INTO tab1 (macaddr8_field) VALUES (sqlc.narg('macaddr8_field')::macaddr8);
```

*** `any` is a pseudo-type reported by sqlc when it cannot infer a column's type; it maps to `object` in C#.
*** `hstore` is a key-value store type that maps to `object` in C#; readers use `GetValue()` to retrieve the value.

</details>

# MySQL
<details>
<summary>:execlastid - Implementation</summary>

The `LAST_INSERT_ID` query is appended to the original query, both with and without `Dapper`:

```sql
INSERT INTO tab1 (field1, field2) VALUES ('a', 1); 
SELECT LAST_INSERT_ID();
```
The result is read as a scalar and converted to the C# type of the table's id column
(`int` for `int`/`integer`/`mediumint`, `long` for `bigint`/`serial`).

</details>

<details>
<summary>:copyfrom - Implementation</summary>
Implemented via the `LOAD DATA` command which can load data from a `CSV` file to a table.
Requires us to first save the input batch as a CSV, and then load it via the driver.

</details>

<details open>
<summary>Supported Data Types</summary>

Since in batch insert the data is not validated by the SQL itself but written and read from a CSV,
we consider support for the different data types separately for batch inserts and everything else.

| DB Type                   | Supported? | Supported in Batch? |
|---------------------------|----|-------------|
| bool, boolean, tinyint(1) | ✅ | ✅          |
| bit                       | ✅ | ✅          |
| tinyint                   | ✅ | ✅          |
| smallint                  | ✅ | ✅          |
| mediumint                 | ✅ | ✅          |
| integer, int              | ✅ | ✅          |
| bigint                    | ✅ | ✅          |
| real                      | ✅ | ✅          |
| numeric                   | ✅ | ✅          |
| decimal                   | ✅ | ✅          |
| double precision          | ✅ | ✅          |
| year                      | ✅ | ✅          |
| date                      | ✅ | ✅          |
| timestamp                 | ✅ | ✅          |
| char                      | ✅ | ✅          |
| nchar, national char      | ✅ | ✅          |
| varchar                   | ✅ | ✅          |
| tinytext                  | ✅ | ✅          |
| mediumtext                | ✅ | ✅          |
| text                      | ✅ | ✅          |
| longtext                  | ✅ | ✅          |
| binary                    | ✅ | ✅          |
| varbinary                 | ✅ | ✅          |
| tinyblob                  | ✅ | ✅          |
| blob                      | ✅ | ✅          |
| mediumblob                | ✅ | ✅          |
| longblob                  | ✅ | ✅          |
| enum                      | ✅ | ✅          |
| set                       | ✅ | ✅          |
| json                      | ✅ | ✅          |
| geometry                  | ⚠️  | ⚠️         |
| point                     | ⚠️  | ⚠️         |
| linestring                | ⚠️  | ⚠️         |
| polygon                   | ⚠️  | ⚠️         |
| multipoint                | ⚠️  | ⚠️         |
| multilinestring           | ⚠️  | ⚠️         |
| multipolygon              | ⚠️  | ⚠️         |
| geometrycollection        | ⚠️  | ⚠️         |

</details>

# SQLite3
<details>
<summary>:execlastid - Implementation</summary>

## :execlastid - Implementation
Implemented via a `RETURNING` clause, allowing the `INSERT` command to return the newly created id.
Only integer data type is supported as id for this annotation.
   
```sql
INSERT INTO tab1 (field1, field2) VALUES ('a', 1) RETURNING id_field;
```
</details>

<details>
<summary>:copyfrom - Implementation</summary>
Implemented via a multi `VALUES` clause, like this:

```sql
INSERT INTO tab1 (field1, field2) VALUES 
('a', 1),
('b', 2),
('c', 3);
```

</details>

<details open>
<summary>Supported Data Types</summary>

| DB Type | Supported? |
|---------|------------|
| integer | ✅         |
| real    | ✅         |
| text    | ✅         |
| blob    | ✅         |

## Useful Overrides
It's possible to use the override data type functionality of the plugin thus overcoming the limited
amount of data types that are supported by SQLite. The supported overrides are specified below:

| DB Type | C# Type Override | Supported? | Description                                                      |
|---------|------------------|------------|------------------------------------------------------------------|
| integer | DateTime         | ✅         | Unix Epoch - seconds since 1-1-1970                              |
| text    | DateTime         | ✅         | String representation of the datetime in a configurable format   |
| integer | bool             | ✅         | If x equals 0 -> False, otherwise -> True                        |
| text    | bool             | ✅         | Converts string to a boolean value using Convert.ToBoolean rules |

</details>

# Benchmark Results

## Read Benchmarks
`L=Query Limit, C=Concurrency, Q=Total Queries To Submit`

[PostgreSQL - Reads](https://github.com/DaredevilOSS/sqlc-gen-csharp/blob/main/benchmark/BenchmarkDotNet.Artifacts/postgresql/reads/results/BenchmarkRunner.Benchmarks.PostgresqlReadBenchmark-report-github.md) 
<br/>

[MySQL - Reads](https://github.com/DaredevilOSS/sqlc-gen-csharp/blob/main/benchmark/BenchmarkDotNet.Artifacts/mysql/reads/results/BenchmarkRunner.Benchmarks.MysqlReadBenchmark-report-github.md) 
<br/>

[SQLite - Reads](https://github.com/DaredevilOSS/sqlc-gen-csharp/blob/main/benchmark/BenchmarkDotNet.Artifacts/sqlite/reads/results/BenchmarkRunner.Benchmarks.SqliteReadBenchmark-report-github.md) 
<br/>

## Write Benchmarks
`R=Total Records to Load, B=Batch Size`

[PostgreSQL - Writes](https://github.com/DaredevilOSS/sqlc-gen-csharp/blob/main/benchmark/BenchmarkDotNet.Artifacts/postgresql/writes/results/BenchmarkRunner.Benchmarks.PostgresqlWriteBenchmark-report-github.md) 
<br/>

[MySQL - Writes](https://github.com/DaredevilOSS/sqlc-gen-csharp/blob/main/benchmark/BenchmarkDotNet.Artifacts/mysql/writes/results/BenchmarkRunner.Benchmarks.MysqlWriteBenchmark-report-github.md) 
<br/>

[SQLite - Writes](https://github.com/DaredevilOSS/sqlc-gen-csharp/blob/main/benchmark/BenchmarkDotNet.Artifacts/sqlite/writes/results/BenchmarkRunner.Benchmarks.SqliteWriteBenchmark-report-github.md)

# Contributing
## Local plugin development
### Prerequisites
Make sure that the following applications are installed and added to your path.

Follow the instructions in each of these:
- Dotnet CLI - [Dotnet Installation](https://github.com/dotnet/sdk) - the SDK version pinned in `global.json` (`.NET 10`) <br/>
- Buf build - [Buf Build](https://buf.build/docs/installation) <br/>
- sqlc - [sqlc Installation](https://docs.sqlc.dev/en/latest/overview/install.html) <br/>
- Docker - for running the end-to-end tests against real PostgreSQL / MySQL instances <br/>
- WASM build tooling (only needed for building the WASM plugin):
  - the `wasi-experimental` workload - `dotnet workload install wasi-experimental`
  - [WASI SDK](https://github.com/WebAssembly/wasi-sdk/releases) version `25`, extracted somewhere and exposed through the `WASI_SDK_PATH` environment variable

### Target frameworks
Two different .NET versions are involved, and they are independent of each other:
- The **plugin itself** (all projects except `examples`) targets `net10.0`.
- The **generated code** targets whatever `targetFramework` is configured - `netstandard2.0`, `netstandard2.1`, `net8.0` (default), `net9.0` or `net10.0`.
  The `examples` cover every supported value (see `sqlc.ci.yaml`) and are built and tested against real databases in CI.

### Why the WASM plugin targets `wasip1`
sqlc runs WASM plugins with [wazero](https://wazero.io/), which can load core modules (`wasm32-wasip1`) but not WebAssembly components.
The .NET 10 WASI SDK hardcodes `wasm32-unknown-wasip2` and links a component-model world, which produces a component that sqlc rejects (`invalid version header`).
`WasmRunner/WasmRunner.csproj` therefore overrides the compile and link steps to produce a `wasip1` core module.
These overrides rely on private item names of the WASI SDK, so after any .NET SDK or WASI SDK upgrade, verify the plugin with `make setup-wasm-plugin && sqlc -f sqlc.ci.yaml diff`.

## Pre-commit Setup
This repository uses [pre-commit](https://pre-commit.com/). To set up pre-commit hooks, run:

```bash
pip install pre-commit
pre-commit install
```

### Protobuf
SQLC protobuf are defined in sqlc-dev/sqlc repository.
Generating C# code from protocol buffer files:
```
make generate-protobuf
```

### Generating code
SQLC utilizes our process / WASM plugin to generate code:
```
make sqlc-generate        # process plugin - regenerates the examples and the codegen requests
make setup-wasm-plugin    # builds the WASM plugin into dist/plugin.wasm
sqlc -f sqlc.ci.yaml diff # verifies the WASM plugin generates exactly the committed examples
```

### Testing generated code
Testing the SQLC generated code via a predefined flow:
```
make unit-tests   # codegen and repository unit tests
make test-plugin  # regenerates everything, builds the solution and runs the end-to-end tests (needs Docker)
```

## Release flow

The release flow in this repo follows the semver conventions, building tag as `v[major].[minor].[patch]`.

### PR Gate

Every PR that modifies source code, build configuration, or similar must be labeled with one of: `major`, `minor`, `patch`, or `skip-release`.

A bot posts a status check (`release-assistant/requirements`) that blocks merging until a version label is present.

### Generate Release PR

When Build completes on `main`, the `gen-release-pr.yml` workflow runs and:

1. Aggregates all unreleased, labeled PRs since the last release tag.
2. Determines the release type from the labels (`major` > `minor` > `patch`).
3. Computes the new version from the latest tag.
4. Downloads the latest wasm artifact and computes its sha256.
5. Regenerates documentation (README, quickstart) with the new version and wasm sha.
6. Opens a `release-prep` PR with the regenerated docs.

### Release

When the `release-prep` PR is merged, `release.yml`:

1. Detects the `chore: prepare release vX` commit.
2. Downloads the wasm artifact from the triggering Build.
3. Reconstructs release notes from merged PR titles since the previous tag.
4. Creates the release (with tag) and uploads the wasm asset.

# Examples
<details>
<summary>QuickStartPostgresDalGen</summary>

## Engine `postgresql`: [QuickStartPostgresDalGen](examples/QuickStartPostgresDalGen)
### [Schema](examples/config/postgresql/authors/schema.sql) | [Queries](examples/config/postgresql/authors/query.sql) | [End2End Test](end2end/EndToEndTests/QuickStartPostgresDalGen.cs)
### Config
```yaml
```

</details>

<details>
<summary>QuickStartMySqlDalGen</summary>

## Engine `mysql`: [QuickStartMySqlDalGen](examples/QuickStartMySqlDalGen)
### [Schema](examples/config/mysql/authors/schema.sql) | [Queries](examples/config/mysql/authors/query.sql) | [End2End Test](end2end/EndToEndTests/QuickStartMySqlDalGen.cs)
### Config
```yaml
```

</details>

<details>
<summary>QuickStartSqliteDalGen</summary>

## Engine `sqlite`: [QuickStartSqliteDalGen](examples/QuickStartSqliteDalGen)
### [Schema](examples/config/sqlite/authors/schema.sql) | [Queries](examples/config/sqlite/authors/query.sql) | [End2End Test](end2end/EndToEndTests/QuickStartSqliteDalGen.cs)
### Config
```yaml
```

</details>

<details>
<summary>Npgsql</summary>

## Engine `postgresql`: [NpgsqlExample](examples/NpgsqlExample)
### [Schema](examples/config/postgresql/authors/schema.sql) | [Queries](examples/config/postgresql/authors/query.sql) | [End2End Test](end2end/EndToEndTests/NpgsqlTester.cs)
### Config
```yaml
driverVersion: 8.0.6
useAsyncSuffix: true
useCancellationToken: false
useDapper: false
targetFramework: net8.0
generateCsproj: true
namespaceName: NpgsqlExampleGen
overrides:
- column: "GetPostgresFunctions:max_integer"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetPostgresFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetPostgresFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "GetPostgresSpecialTypesCnt:c_json"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "GetPostgresSpecialTypesCnt:c_jsonb"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_xml_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_macaddr8"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
    notNull: false
```

</details>

<details>
<summary>NpgsqlDapper</summary>

## Engine `postgresql`: [NpgsqlDapperExample](examples/NpgsqlDapperExample)
### [Schema](examples/config/postgresql/authors/schema.sql) | [Queries](examples/config/postgresql/authors/query.sql) | [End2End Test](end2end/EndToEndTests/NpgsqlDapperTester.cs)
### Config
```yaml
dapperVersion: 2.1.66
useDapper: true
targetFramework: net10.0
generateCsproj: true
namespaceName: NpgsqlDapperExampleGen
overrides:
- column: "GetPostgresFunctions:max_integer"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetPostgresFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetPostgresFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "GetPostgresSpecialTypesCnt:c_json"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "GetPostgresSpecialTypesCnt:c_jsonb"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_xml_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_macaddr8"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
    notNull: false
```

</details>

<details>
<summary>NpgsqlLegacy</summary>

## Engine `postgresql`: [NpgsqlLegacyExample](examples/NpgsqlLegacyExample)
### [Schema](examples/config/postgresql/authors/schema.sql) | [Queries](examples/config/postgresql/authors/query.sql) | [End2End Test](end2end/EndToEndTestsLegacy/NpgsqlTester.cs)
### Config
```yaml
useDapper: false
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: NpgsqlLegacyExampleGen
overrides:
- column: "GetPostgresFunctions:max_integer"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetPostgresFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetPostgresFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "GetPostgresSpecialTypesCnt:c_json"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "GetPostgresSpecialTypesCnt:c_jsonb"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_xml_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_macaddr8"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
    notNull: false
```

</details>

<details>
<summary>NpgsqlDapperLegacy</summary>

## Engine `postgresql`: [NpgsqlDapperLegacyExample](examples/NpgsqlDapperLegacyExample)
### [Schema](examples/config/postgresql/authors/schema.sql) | [Queries](examples/config/postgresql/authors/query.sql) | [End2End Test](end2end/EndToEndTestsLegacy/NpgsqlDapperTester.cs)
### Config
```yaml
useDapper: true
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: NpgsqlDapperLegacyExampleGen
overrides:
- column: "GetPostgresFunctions:max_integer"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetPostgresFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetPostgresFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "GetPostgresSpecialTypesCnt:c_json"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "GetPostgresSpecialTypesCnt:c_jsonb"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_xml_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_macaddr8"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
    notNull: false
```

</details>

<details>
<summary>MySqlConnector</summary>

## Engine `mysql`: [MySqlConnectorExample](examples/MySqlConnectorExample)
### [Schema](examples/config/mysql/authors/schema.sql) | [Queries](examples/config/mysql/authors/query.sql) | [End2End Test](end2end/EndToEndTests/MySqlConnectorTester.cs)
### Config
```yaml
useDapper: false
targetFramework: net9.0
generateCsproj: true
namespaceName: MySqlConnectorExampleGen
overrides:
- column: "GetMysqlFunctions:max_int"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetMysqlFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetMysqlFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
    notNull: false
```

</details>

<details>
<summary>MySqlConnectorDapper</summary>

## Engine `mysql`: [MySqlConnectorDapperExample](examples/MySqlConnectorDapperExample)
### [Schema](examples/config/mysql/authors/schema.sql) | [Queries](examples/config/mysql/authors/query.sql) | [End2End Test](end2end/EndToEndTests/MySqlConnectorDapperTester.cs)
### Config
```yaml
useDapper: true
targetFramework: net8.0
generateCsproj: true
namespaceName: MySqlConnectorDapperExampleGen
overrides:
- column: "GetMysqlFunctions:max_int"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetMysqlFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetMysqlFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
    notNull: false
```

</details>

<details>
<summary>MySqlConnectorLegacy</summary>

## Engine `mysql`: [MySqlConnectorLegacyExample](examples/MySqlConnectorLegacyExample)
### [Schema](examples/config/mysql/authors/schema.sql) | [Queries](examples/config/mysql/authors/query.sql) | [End2End Test](end2end/EndToEndTestsLegacy/MySqlConnectorTester.cs)
### Config
```yaml
overrideDriverVersion: 2.4.0
withAsyncSuffix: true
withCancellationToken: false
useDapper: false
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: MySqlConnectorLegacyExampleGen
overrides:
- column: "GetMysqlFunctions:max_int"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetMysqlFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetMysqlFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
    notNull: false
```

</details>

<details>
<summary>MySqlConnectorDapperLegacy</summary>

## Engine `mysql`: [MySqlConnectorDapperLegacyExample](examples/MySqlConnectorDapperLegacyExample)
### [Schema](examples/config/mysql/authors/schema.sql) | [Queries](examples/config/mysql/authors/query.sql) | [End2End Test](end2end/EndToEndTestsLegacy/MySqlConnectorDapperTester.cs)
### Config
```yaml
overrideDapperVersion: 2.1.66
useDapper: true
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: MySqlConnectorDapperLegacyExampleGen
overrides:
- column: "GetMysqlFunctions:max_int"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetMysqlFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetMysqlFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
    notNull: false
```

</details>

<details>
<summary>Sqlite</summary>

## Engine `sqlite`: [SqliteExample](examples/SqliteExample)
### [Schema](examples/config/sqlite/authors/schema.sql) | [Queries](examples/config/sqlite/authors/query.sql) | [End2End Test](end2end/EndToEndTests/SqliteTester.cs)
### Config
```yaml
useDapper: false
targetFramework: net10.0
generateCsproj: true
namespaceName: SqliteExampleGen
overrides:
- column: "GetSqliteFunctions:max_integer"
  csharp_type:
    type: "int"
- column: "GetSqliteFunctions:max_varchar"
  csharp_type:
    type: "string"
- column: "GetSqliteFunctions:max_real"
  csharp_type:
    type: "decimal"
- column: "*:c_text_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_integer_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_text_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_integer_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_text_noda_instant_override"
  csharp_type:
    type: "Instant"
- column: "*:c_integer_noda_instant_override"
  csharp_type:
    type: "Instant"
```

</details>

<details>
<summary>SqliteDapper</summary>

## Engine `sqlite`: [SqliteDapperExample](examples/SqliteDapperExample)
### [Schema](examples/config/sqlite/authors/schema.sql) | [Queries](examples/config/sqlite/authors/query.sql) | [End2End Test](end2end/EndToEndTests/SqliteDapperTester.cs)
### Config
```yaml
useDapper: true
targetFramework: net9.0
generateCsproj: true
namespaceName: SqliteDapperExampleGen
overrides:
- column: "GetSqliteFunctions:max_integer"
  csharp_type:
    type: "int"
- column: "GetSqliteFunctions:max_varchar"
  csharp_type:
    type: "string"
- column: "GetSqliteFunctions:max_real"
  csharp_type:
    type: "decimal"
- column: "*:c_text_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_integer_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_text_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_integer_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_text_noda_instant_override"
  csharp_type:
    type: "Instant"
- column: "*:c_integer_noda_instant_override"
  csharp_type:
    type: "Instant"
```

</details>

<details>
<summary>SqliteLegacy</summary>

## Engine `sqlite`: [SqliteLegacyExample](examples/SqliteLegacyExample)
### [Schema](examples/config/sqlite/authors/schema.sql) | [Queries](examples/config/sqlite/authors/query.sql) | [End2End Test](end2end/EndToEndTestsLegacy/SqliteTester.cs)
### Config
```yaml
useDapper: false
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: SqliteLegacyExampleGen
overrides:
- column: "GetSqliteFunctions:max_integer"
  csharp_type:
    type: "int"
- column: "GetSqliteFunctions:max_varchar"
  csharp_type:
    type: "string"
- column: "GetSqliteFunctions:max_real"
  csharp_type:
    type: "decimal"
- column: "*:c_text_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_integer_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_text_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_integer_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_text_noda_instant_override"
  csharp_type:
    type: "Instant"
- column: "*:c_integer_noda_instant_override"
  csharp_type:
    type: "Instant"
```

</details>

<details>
<summary>SqliteDapperLegacy</summary>

## Engine `sqlite`: [SqliteDapperLegacyExample](examples/SqliteDapperLegacyExample)
### [Schema](examples/config/sqlite/authors/schema.sql) | [Queries](examples/config/sqlite/authors/query.sql) | [End2End Test](end2end/EndToEndTestsLegacy/SqliteDapperTester.cs)
### Config
```yaml
useDapper: true
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: SqliteDapperLegacyExampleGen
overrides:
- column: "GetSqliteFunctions:max_integer"
  csharp_type:
    type: "int"
- column: "GetSqliteFunctions:max_varchar"
  csharp_type:
    type: "string"
- column: "GetSqliteFunctions:max_real"
  csharp_type:
    type: "decimal"
- column: "*:c_text_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_integer_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_text_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_integer_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_text_noda_instant_override"
  csharp_type:
    type: "Instant"
- column: "*:c_integer_noda_instant_override"
  csharp_type:
    type: "Instant"
```

</details>

<details>
<summary>SqliteAsyncEnumerable</summary>

## Engine `sqlite`: [SqliteAsyncEnumerableExample](examples/SqliteAsyncEnumerableExample)
### [Schema](examples/config/sqlite/authors/schema.sql) | [Queries](examples/config/sqlite/authors/query.sql) | [End2End Test](end2end/EndToEndTests/SqliteAsyncEnumerableTester.cs)
### Config
```yaml
useDapper: false
useAsyncEnumerable: true
useCancellationToken: true
targetFramework: net10.0
generateCsproj: true
namespaceName: SqliteAsyncEnumerableExampleGen
```

</details>

<details>
<summary>NpgsqlDapperAsyncEnumerable</summary>

## Engine `postgresql`: [NpgsqlDapperAsyncEnumerableExample](examples/NpgsqlDapperAsyncEnumerableExample)
### [Schema](examples/config/postgresql/authors/schema.sql) | [Queries](examples/config/postgresql/authors/query.sql) | [End2End Test](end2end/EndToEndTests/NpgsqlDapperAsyncEnumerableTester.cs)
### Config
```yaml
useDapper: true
useAsyncEnumerable: true
useCancellationToken: true
targetFramework: net10.0
generateCsproj: true
namespaceName: NpgsqlDapperAsyncEnumerableExampleGen
```

</details>
