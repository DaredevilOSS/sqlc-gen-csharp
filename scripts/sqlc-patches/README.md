# sqlc patches for SQL Server (mssql) support

The mssql engine in sqlc `main` (v1.31.1+) is missing a few things this plugin relies on. These patches are
applied on top of sqlc commit `3ad8b20fb` and are meant to be upstreamed:

1. `0001` - the mssql dialect for the query preprocessor (`sqlc.arg`, `sqlc.narg`, `sqlc.slice`, `sqlc.embed`
   become `@name` / `/*SLICE:name*/@name` / `table.*`) and `sqlc.embed` support in the core analysis path.
2. `0002` - `INSERT ... VALUES` gets an empty FROM clause (without it `:copyfrom` panics with a nil dereference).
3. `0003` - `TRUNCATE TABLE` (statements the converter does not know are silently dropped from the output).

Build a patched sqlc:

```bash
git clone https://github.com/sqlc-dev/sqlc && cd sqlc && git checkout 3ad8b20fb
git am /path/to/sqlc-gen-csharp/scripts/sqlc-patches/*.patch
go build -o /usr/local/bin/sqlc ./cmd/sqlc
```
