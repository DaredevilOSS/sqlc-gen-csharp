# sqlc-gen-csharp

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

## Supported SQL Engines
- MySQL via [MySqlConnector](https://www.nuget.org/packages/MySqlConnector) package - [MySqlConnectorDriver](MySqlConnectorDriver/MySqlConnectorDriver.csproj)
- PostgreSQL via [Npgsql](https://www.nuget.org/packages/Npgsql) package - [NpgsqlDriver](NpgsqlDriver/NpgsqlDriver.csproj)

## Configuration
Options available for plugin:
```yaml
version: "2"
plugins:
  - name: csharp
    env:
      - DEBUG
    process:
      cmd: ./dist/SqlcGenCsharpProcess
sql:
  - schema: "examples/authors/postgresql/schema.sql"
    queries: "examples/authors/postgresql/query.sql"
    engine: "postgresql"
    codegen:
      - plugin: csharp
        out: NpgsqlExample
        options:
          driver: Npgsql
          minimalCsharp: 7.0
          filePerQuery: true
          generateCsproj: false
```

## Examples & Tests
The below examples in here are automatically tested:
- [MySqlConnectorExample](MySqlConnectorExample/MySqlConnectorExample.csproj)
- [NpgsqlExample](NpgsqlExample/NpgsqlExample.csproj)
