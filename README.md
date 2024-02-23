# sqlc-gen-csharp

> [!CAUTION]
> Here be dragons! This plugin is still in early access. Expect breaking changes, missing functionality, and sub-optimal output. Please report all issues and errors. Good luck!

## Prerequisites
make sure that the following applications are installed and exposed in your path

* dotnet cli - https://github.com/dotnet/sdk - follow the instructions in the repo, currently we supports version `.NET 8.0 (latest)`
* buf build - https://buf.build/docs/installation - follow the instructions in here

## Protobuf
Defined in protos/ (should be identical to the protocol buffers in sqlc repo).
Generating C# code from protocol buffer file:
```
make buf-gen
```

## Usage
you can run the c# project by the terminal using this commnad `make dotnet-publish`
Use the following to run sqlc with the csharp code locally
```
make sqlc-generate
```

```yaml
version: '2'
plugins:
- name: csharp
  wasm:
    url: https://downloads.sqlc.dev/plugin/sqlc-gen-csharp_0.1.3.wasm
    sha256: 287df8f6cc06377d67ad5ba02c9e0f00c585509881434d15ea8bd9fc751a9368
sql:
- schema: "schema.sql"
  queries: "query.sql"
  engine: postgresql
  codegen:
  - out: src/authors
    plugin: csharp
    options:
      runtime: dotnet
      driver: postgres
```

## Supported engines and drivers

- PostgreSQL via [Npgsql](https://www.nuget.org/packages/Npgsql).
- MySQL via [MySqlConnector](https://www.nuget.org/packages/MySqlConnector).
- SqlServer via [sqlclient](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/data-providers?redirectedfrom=MSDN#net-framework-data-provider-for-sql-server-sqlclient).

## Getting started


### Setting up


### Schema and queries


### Generating code


### Using generated code
