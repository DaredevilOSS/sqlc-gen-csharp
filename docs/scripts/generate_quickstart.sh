#!/usr/bin/env bash

set -e

plugin_version=$(git tag | sort --version-sort | tail -n1)
plugin_url="https://github.com/DaredevilOSS/sqlc-gen-csharp/releases/download/${plugin_version}/sqlc-gen-csharp.wasm"
curl -sL --output /tmp/plugin.wasm "${plugin_url}"
release_sha=$(shasum -a 256 /tmp/plugin.wasm | awk '{ print $1 }')
    
contents="## Quickstart
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
\`\`\`"

echo "${contents}"