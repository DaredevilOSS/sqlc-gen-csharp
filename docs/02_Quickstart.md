[comment]: <> (do not edit - CI auto-generated)
# Quickstart
## Configuration
```yaml
version: "2"
plugins:
- name: csharp
  wasm:
    url: https://github.com/DaredevilOSS/sqlc-gen-csharp/releases/download/v0.13.0/sqlc-gen-csharp.wasm
    sha256: e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855
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

## Examples
All of the examples can be found in [here](/docs/__Examples.md).
