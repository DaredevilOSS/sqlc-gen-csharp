#!/usr/bin/env bash

set -e

doc_file="docs/$1"
plugin_version=$(git tag | sort --version-sort | tail -n1)
plugin_url="https://github.com/DaredevilOSS/sqlc-gen-csharp/releases/download/${plugin_version}/sqlc-gen-csharp.wasm"
release_sha=$(curl -s "${plugin_url}" | shasum -a 256 | awk '{ print $1 }')
    
contents="[comment]: <> (do not edit - CI auto-generated)
# Quickstart
## Configuration
\`\`\`yaml
version: \"2\"
plugins:
- name: csharp
  wasm:
    url: https://github.com/DaredevilOSS/sqlc-gen-csharp/releases/download/${plugin_version}/sqlc-gen-csharp.wasm
    sha256: ${release_sha}
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
\`\`\`

## Examples
All of the examples can be found in [here](/docs/__Examples.md)."

echo "${contents}" > "${doc_file}"