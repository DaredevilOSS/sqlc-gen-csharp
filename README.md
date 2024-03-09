# sqlc-gen-csharp

## Prerequisites
make sure that the following applications are installed and exposed in your path

* Dotnet CLI - https://github.com/dotnet/sdk - follow the instructions in the repo, we use version `.NET 8.0 (latest)`
* buf build - https://buf.build/docs/installation - follow the instructions in here

## Protobuf
SQLC protobuf are defined in sqlc-dev/sqlc repository.
Generating C# code from protocol buffer file:
```
make buf-gen
```

## Usage
Use the following to run sqlc with the C# code locally:
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
To be true:
- MySQL via [MySqlConnector](https://www.nuget.org/packages/MySqlConnector).
- PostgreSQL via [Npgsql](https://www.nuget.org/packages/Npgsql).
- SqlServer via [sqlclient](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/data-providers?redirectedfrom=MSDN#net-framework-data-provider-for-sql-server-sqlclient).

## Getting started


### Setting up


### Schema and queries


### Generating code


### Using generated code
