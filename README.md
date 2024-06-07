[![CI](https://github.com/DaredevilOSS/sqlc-gen-csharp/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/DaredevilOSS/sqlc-gen-csharp/actions/workflows/main.yml)

# sqlc-gen-csharp
## Usage
### Configuration
```yaml
version: "2"
plugins:
- name: csharp
  wasm:
    url: https://github.com/DaredevilOSS/sqlc-gen-csharp/releases/download/v0.10.0/sqlc-gen-csharp.wasm
    sha256: 613ae249a541ab95c97b362bd1b0b572970edcad5eb2a11806a52d3f95e0f65f
sql:
  # PostgreSQL Example
  - schema: "examples/authors/postgresql/schema.sql"
    queries: "examples/authors/postgresql/query.sql"
    engine: "postgresql"
    codegen:
      - plugin: csharp
        out: NpgsqlExample
        options:
          driver: Npgsql
          targetFramework: net8.0
          generateCsproj: true
          filePerQuery: false
  # MySQL Example
  - schema: "examples/authors/mysql/schema.sql"
    queries: "examples/authors/mysql/query.sql"
    engine: "mysql"
    codegen:
      - plugin: csharp
        out: MySqlConnectorExample
        options:
          driver: MySqlConnector
```
### Options Documentation
| Option     | Possible values | Info |
|------------|---------------------------|-|
| targetFramework | default: `net8.0`<br/>vaults: `netstandard2.0`, `netstandard2.1`, `net8.0` |Decide on the right target framework for your generated code, meaning the generated code will be compiled to the specified runtime.<br/>For more information and help deciding on the right value, refer to the [Microsoft .NET Standard documentation](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-1-0). |
| generateCsproj      | default: `true`<br/>values: `false`,`true`  | This option is designed to assist you with the integration of SQLC and csharp by generating a `.csproj` file. This converts the generated output to a dynamic link library (DLL), simply a project that you can easily incorporate into your build process.  |
| filePerQuery | default: `false`<br/>values: `false`,`true` | This option allows users control on which `.cs` files to generate, when false it's one file per `.sql` SQLC query file, and when true it's one file per query. |


## Supported SQL Engines
- MySQL via [MySqlConnector](https://www.nuget.org/packages/MySqlConnector) package - [MySqlConnectorDriver](MySqlConnectorDriver/MySqlConnectorDriver.csproj)
- PostgreSQL via [Npgsql](https://www.nuget.org/packages/Npgsql) package - [NpgsqlDriver](NpgsqlDriver/NpgsqlDriver.csproj)


## Examples & Tests
The below examples in here are automatically tested:
- [MySqlConnectorExample](MySqlConnectorExample/MySqlConnectorExample.csproj)
- [NpgsqlExample](NpgsqlExample/NpgsqlExample.csproj)



<br/>
<br/>
<br/>

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

### Version bumping (build on tags)
**The default behavior is to bump the `patch` in the last version**, by adding `[release]` to your commit message the release script will create a new tag with `v[major].[minor].[patch]+1`.
* Bump `minor` version by adding `[minor]` to your commit message resulting in a new tag with `v[major].[minor]+1.0`<br/>
* Bump `major` version by adding `[major]` to your commit message resulting in a new tag with `v[major]+1.0.0`

### Release structure
The new created tag will create a draft release with it, in the release there will be the wasm plugin embedded in the release.<br/>
> All we have left to do now is to add the changelog and publish the draft

