# <p align="center">sqlc-gen-csharp</p>
<p align="center"><img src="https://github.com/DaredevilOSS/sqlc-gen-csharp/actions/workflows/ci.yml/badge.svg?branch=main" alt="CI"></p>


sqlc-gen-csharp is a .Net plugin for [sqlc](https://github.com/sqlc-dev/sqlc).<br/> It leverages the SQLC plugin system to generate type-safe C# code for SQL queries, supporting  PostgresSQL, MySQL & SQLite via the corresponding driver or suitable Dapper abstraction.

## Usage

### Options
| Option          | Possible values                                                            | Optional | Info                                                                                                                                                                                                                                                                                                                                      |
|-----------------|----------------------------------------------------------------------------|----------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| OverrideDriverVersion | default:<br/> `2.3.6` for MySqlConnector  (mysql)<br/>`8.0.3` for Npgsql (postgresql)<br/>`8.0.10` for Microsoft.Data.Sqlite (sqlite)<br/><br/>values: The desired driver version  | Yes      | Determines the version of the driver to be used. |
| targetFramework | default: `net8.0`<br/>values: `netstandard2.0`, `netstandard2.1`, `net8.0` | Yes      | Determines the target framework for your generated code, meaning the generated code will be compiled to the specified runtime.<br/>For more information and help deciding on the right value, refer to the [Microsoft .NET Standard documentation](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-1-0). |
| generateCsproj  | default: `true`<br/>values: `false`,`true`                                 | Yes      | Assists you with the integration of SQLC and csharp by generating a `.csproj` file. This converts the generated output to a .dll, a project that you can easily incorporate into your build process.                                                                                                                                      |
| namespaceName   | default: the generated project name                                        | Yes      | Allows you to override the namespace name to be different than the project name                                                                                                                                                                                                                                                           |
| useDapper   | default: `false`<br/>values: `false`,`true`            | Yes      | Enables Dapper as a thin wrapper for the generated code. For more information, please refer to the [Dapper documentation](https://github.com/DapperLib/Dapper). |
| OverrideDapperVersion | default:<br/> `2.1.35`<br/><br/>values: The desired Dapper version  | Yes      | If `useDapper` is set to `true`, this option allows you to override the version of Dapper to be used. |

### Configuration
```yaml
version: "2"
plugins:
- name: csharp
  wasm:
    url: https://github.com/DaredevilOSS/sqlc-gen-csharp/releases/download/${PLUGIN_VERSION}/sqlc-gen-csharp.wasm
    sha256: ${RELEASE_SHA}
sql:
  # PostgreSQL Example
  - schema: "examples/config/postgresql/schema.sql"
    queries: "examples/config/postgresql/query.sql"
    engine: "postgresql"
    codegen:
      - plugin: csharp
        out: NpgsqlExample
  # MySQL Example
  - schema: "examples/config/mysql/schema.sql"
    queries: "examples/config/mysql/query.sql"
    engine: "mysql"
    codegen:
      - plugin: csharp
        out: MySqlConnectorExample
  # Sqlite Example
  - schema: "examples/config/sqlite/schema.sql"
    queries: "examples/config/sqlite/query.sql"
    engine: "sqlite"
    codegen:
      - plugin: csharp
        out: SqliteExample
```

### [Generated Code Examples](docs/Examples.md)

# Contributing
## Local plugin development
### Prerequisites
make sure that the following applications are installed and exposed in your path

Follow the instructions in each of these:
* Dotnet CLI - [Dotnet Installation](https://github.com/dotnet/sdk) - use version `.NET 8.0 (latest)`
* buf build - [Buf Build](https://buf.build/docs/installation)
* WASM related - [WASM libs](https://www.strathweb.com/2023/09/dotnet-wasi-applications-in-net-8-0/)

### Protobuf
SQLC protobuf are defined in sqlc-dev/sqlc repository.
Generating C# code from protocol buffer files:
```
make protobuf-generate
```

### Generating code
SQLC utilizes our process / WASM plugin to generate code
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

* In order to create a release you need to add `[release]` somewhere in your commit message when merging to master

### Version bumping (built on tags)
**By default, the release script will bump the patch version.**, by adding `[release]` to your commit message the release script will create a new tag with `v[major].[minor].[patch]+1`.
* Bump `minor` version by adding `[minor]` to your commit message resulting in a new tag with `v[major].[minor]+1.0`<br/>
* Bump `major` version by adding `[major]` to your commit message resulting in a new tag with `v[major]+1.0.0`

### Release structure
The new created tag will create a draft release with it, in the release there will be the wasm plugin embedded in the release.<br/>
> All we have left to do now is to add the changelog and publish the draft

