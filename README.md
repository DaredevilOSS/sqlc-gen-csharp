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
    url: https://github.com/DaredevilOSS/sqlc-gen-csharp/releases/download/v0.13.1/sqlc-gen-csharp.wasm
    sha256: 4fcd5ac76ceecb81702fc94eb813b83ab18aedb93972d3250cf583506af06756
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
| Option                 | Possible values                                                                                                                                                                    | Optional | Info                                                                                                                                                                                                                                                                                                                                      |
|------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| overrideDriverVersion  | default:<br/> `2.3.6` for MySqlConnector  (mysql)<br/>`8.0.3` for Npgsql (postgresql)<br/>`8.0.10` for Microsoft.Data.Sqlite (sqlite)<br/><br/>values: The desired driver version  | Yes      | Determines the version of the driver to be used. |
| targetFramework        | default: `net8.0`<br/>values: `netstandard2.0`, `netstandard2.1`, `net8.0`                                                                                                         | Yes      | Determines the target framework for your generated code, meaning the generated code will be compiled to the specified runtime.<br/>For more information and help deciding on the right value, refer to the [Microsoft .NET Standard documentation](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-1-0). |
| generateCsproj         | default: `true`<br/>values: `false`,`true`                                                                                                                                         | Yes      | Assists you with the integration of SQLC and csharp by generating a `.csproj` file. This converts the generated output to a .dll, a project that you can easily incorporate into your build process.                                                                                                                                      |
| namespaceName          | default: the generated project name                                                                                                                                                | Yes      | Allows you to override the namespace name to be different than the project name                                                                                                                                                                                                                                                           |
| useDapper              | default: `false`<br/>values: `false`,`true`                                                                                                                                        | Yes      | Enables Dapper as a thin wrapper for the generated code. For more information, please refer to the [Dapper documentation](https://github.com/DapperLib/Dapper). |
| overrideDapperVersion  | default:<br/> `2.1.35`<br/>values: The desired Dapper version                                                                                                                      | Yes      | If `useDapper` is set to `true`, this option allows you to override the version of Dapper to be used. |

## Query Annotations
Basic functionality - same for all databases:
- `:one`          - returns 0...1 records
- `:many`         - returns 0...n records
- `:exec`         - DML / DDL that does not return anything
- `:execrows`     - returns number of affected rows by DML

Advanced functionality - varies between databases:
- `:execlastid`   - INSERT with returned last inserted id
- `:copyfrom`     - batch insert, implementation varies greatly
<br/>

| Annotation  | PostgresSQL | MySQL | SQLite |
|-------------|-------------|-------|--------|
| :one        | ✅          | ✅    | ✅     |
| :many       | ✅          | ✅    | ✅     |
| :exec       | ✅          | ✅    | ✅     |
| :execrows   | ✅          | ✅    | ✅     |
| :execlastid | ✅          | ✅    | ✅     |
| :copyfrom   | ✅          | ❌    | 🚫     |

- ✅ means the feature is fully supported.
- 🚫 means the database does not support the feature.
- ❌ means the feature is not supported by the plugin (but could be supported by the database).

More info can be found in [here](https://docs.sqlc.dev/en/latest/reference/query-annotations.html).
# Contributing
## Local plugin development
### Prerequisites
Make sure that the following applications are installed and added to your path.

Follow the instructions in each of these:
- Dotnet CLI - [Dotnet Installation](https://github.com/dotnet/sdk) - use version `.NET 8.0 (latest)` <br/>
- Buf build - [Buf Build](https://buf.build/docs/installation) <br/>
- WASM (follow this guide) - [WASM libs](https://www.strathweb.com/2023/09/dotnet-wasi-applications-in-net-8-0/)

### Protobuf
SQLC protobuf are defined in sqlc-dev/sqlc repository.
Generating C# code from protocol buffer files:
```
make protobuf-generate
```

### Generating code
SQLC utilizes our process / WASM plugin to generate code:
```
make sqlc-generate-process
make sqlc-generate-wasm
```

### Testing generated code
Testing the SQLC generated code via a predefined flow:
```
make test-process-plugin
make test-wasm-plugin
```

## Release flow
The release flow in this repo follows the semver conventions, building tag as `v[major].[minor].[patch]`.
In order to create a release you need to add `[release]` somewhere in your commit message when merging to master.

### Version bumping (built on tags)
By default, the release script will bump the patch version. Adding `[release]` to your commit message results in a new tag with `v[major].[minor].[patch]+1`. 
- Bump `minor` version by adding `[minor]` to your commit message resulting in a new tag with `v[major].[minor]+1.0` <br/>
- Bump `major` version by adding `[major]` to your commit message resulting in a new tag with `v[major]+1.0.0` <br/>

### Release structure
The new created tag will create a draft release with it, in the release there will be the wasm plugin embedded in the release. <br/>
# Examples
## Engine `postgresql`: [NpgsqlExample](examples/NpgsqlExample)

### [Schema](examples/config/postgresql/schema.sql) | [Queries](examples/config/postgresql/query.sql) | [End2End Test](EndToEndTests/NpgsqlTester.cs)

### Config
```yaml
useDapper: false
targetFramework: net8.0
generateCsproj: true
namespaceName: NpgsqlExampleGen
```
## Engine `postgresql`: [NpgsqlDapperExample](examples/NpgsqlDapperExample)

### [Schema](examples/config/postgresql/schema.sql) | [Queries](examples/config/postgresql/query.sql) | [End2End Test](EndToEndTests/NpgsqlDapperTester.cs)

### Config
```yaml
useDapper: true
targetFramework: net8.0
generateCsproj: true
namespaceName: NpgsqlDapperExampleGen
```
## Engine `postgresql`: [NpgsqlLegacyExample](examples/NpgsqlLegacyExample)

### [Schema](examples/config/postgresql/schema.sql) | [Queries](examples/config/postgresql/query.sql) | [End2End Test](LegacyEndToEndTests/NpgsqlTester.cs)

### Config
```yaml
useDapper: false
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: NpgsqlLegacyExampleGen
```
## Engine `postgresql`: [NpgsqlDapperLegacyExample](examples/NpgsqlDapperLegacyExample)

### [Schema](examples/config/postgresql/schema.sql) | [Queries](examples/config/postgresql/query.sql) | [End2End Test](LegacyEndToEndTests/NpgsqlDapperTester.cs)

### Config
```yaml
useDapper: true
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: NpgsqlDapperLegacyExampleGen
```
## Engine `mysql`: [MySqlConnectorExample](examples/MySqlConnectorExample)

### [Schema](examples/config/mysql/schema.sql) | [Queries](examples/config/mysql/query.sql) | [End2End Test](EndToEndTests/MySqlConnectorTester.cs)

### Config
```yaml
useDapper: false
targetFramework: net8.0
generateCsproj: true
namespaceName: MySqlConnectorExampleGen
```
## Engine `mysql`: [MySqlConnectorDapperExample](examples/MySqlConnectorDapperExample)

### [Schema](examples/config/mysql/schema.sql) | [Queries](examples/config/mysql/query.sql) | [End2End Test](EndToEndTests/MySqlConnectorDapperTester.cs)

### Config
```yaml
useDapper: true
targetFramework: net8.0
generateCsproj: true
namespaceName: MySqlConnectorDapperExampleGen
```
## Engine `mysql`: [MySqlConnectorLegacyExample](examples/MySqlConnectorLegacyExample)

### [Schema](examples/config/mysql/schema.sql) | [Queries](examples/config/mysql/query.sql) | [End2End Test](LegacyEndToEndTests/MySqlConnectorTester.cs)

### Config
```yaml
useDapper: false
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: MySqlConnectorLegacyExampleGen
```
## Engine `mysql`: [MySqlConnectorDapperLegacyExample](examples/MySqlConnectorDapperLegacyExample)

### [Schema](examples/config/mysql/schema.sql) | [Queries](examples/config/mysql/query.sql) | [End2End Test](LegacyEndToEndTests/MySqlConnectorDapperTester.cs)

### Config
```yaml
useDapper: true
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: MySqlConnectorDapperLegacyExampleGen
```
## Engine `sqlite`: [SqliteExample](examples/SqliteExample)

### [Schema](examples/config/sqlite/schema.sql) | [Queries](examples/config/sqlite/query.sql) | [End2End Test](EndToEndTests/SqliteTester.cs)

### Config
```yaml
useDapper: false
targetFramework: net8.0
generateCsproj: true
namespaceName: SqliteExampleGen
```
## Engine `sqlite`: [SqliteDapperExample](examples/SqliteDapperExample)

### [Schema](examples/config/sqlite/schema.sql) | [Queries](examples/config/sqlite/query.sql) | [End2End Test](EndToEndTests/SqliteDapperTester.cs)

### Config
```yaml
useDapper: true
targetFramework: net8.0
generateCsproj: true
namespaceName: SqliteDapperExampleGen
```
## Engine `sqlite`: [SqliteLegacyExample](examples/SqliteLegacyExample)

### [Schema](examples/config/sqlite/schema.sql) | [Queries](examples/config/sqlite/query.sql) | [End2End Test](LegacyEndToEndTests/SqliteTester.cs)

### Config
```yaml
useDapper: false
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: SqliteLegacyExampleGen
```
## Engine `sqlite`: [SqliteDapperLegacyExample](examples/SqliteDapperLegacyExample)

### [Schema](examples/config/sqlite/schema.sql) | [Queries](examples/config/sqlite/query.sql) | [End2End Test](LegacyEndToEndTests/SqliteDapperTester.cs)

### Config
```yaml
useDapper: true
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: SqliteDapperLegacyExampleGen
```