## Quickstart
```yaml
version: "2"
plugins:
- name: csharp
  wasm:
    url: https://github.com/DaredevilOSS/sqlc-gen-csharp/releases/download/v0.18.0/sqlc-gen-csharp.wasm
    sha256: 5564052d3133c4127c067b7528e2b62e78ccf03cb99c7fc2aade0764b9455125
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
