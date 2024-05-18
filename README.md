# sqlc-gen-csharp
## Usage
### Configuration
```yaml
version: "2"
plugins:
- name: csharp
  wasm:
    url: https://github.com/DionyOSS/sqlc-gen-csharp/releases/download/v0.10.0/sqlc-gen-csharp_0.10.0.wasm
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




## Examples & Tests
The below examples in here are automatically tested:
- [MySqlConnectorExample](MySqlConnectorExample/MySqlConnectorExample.csproj)
- [NpgsqlExample](NpgsqlExample/NpgsqlExample.csproj)


## Supported SQL Engines
- MySQL via [MySqlConnector](https://www.nuget.org/packages/MySqlConnector) package - [MySqlConnectorDriver](MySqlConnectorDriver/MySqlConnectorDriver.csproj)
- PostgreSQL via [Npgsql](https://www.nuget.org/packages/Npgsql) package - [NpgsqlDriver](NpgsqlDriver/NpgsqlDriver.csproj)


<br/>
<br/>
<br/>


# Local plugin development
## Prerequisites
make sure that the following applications are installed and exposed in your path

Follow the instructions in each of these:
* Dotnet CLI - [Dotnet Installation](https://github.com/dotnet/sdk) - use version `.NET 8.0 (latest)`
* buf build - [Buf Build](https://buf.build/docs/installation)
* WASM related - [WASM libs](https://www.strathweb.com/2023/09/dotnet-wasi-applications-in-net-8-0/)

## Protobuf
SQLC protobuf are defined in sqlc-dev/sqlc repository.
Generating C# code from protocol buffer files:
```
make protobuf-generate
```

## Generating code
SQLC utilizes our process / WASM plugin to generate code
```
make sqlc-generate-process
make sqlc-generate-wasm
```

## Testing generated code
Testing the SQLC generated code via a predefined flow:
```
make test-process-plugin
make test-wasm-plugin
```
