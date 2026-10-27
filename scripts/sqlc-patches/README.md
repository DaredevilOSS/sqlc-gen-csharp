# sqlc patches for SQL Server (mssql) support

The mssql engine on sqlc `main` is missing a few things this plugin relies on. These patches apply on top of the
sqlc commit in [`SQLC_REF`](SQLC_REF) and are meant to be upstreamed:

1. `0001` - the mssql dialect for the query preprocessor (`sqlc.arg`, `sqlc.narg`, `sqlc.slice`, `sqlc.embed`
   become `@name` / `/*SLICE:name*/@name` / `table.*`) and `sqlc.embed` support in the core analysis path
   (which every engine on that path lacked).
2. `0002` - `INSERT ... VALUES` gets an empty FROM clause (without it `:copyfrom` panics with a nil dereference).
3. `0003` - `TRUNCATE TABLE` (statements the converter does not know are silently dropped from the output).
4. `0004` - parameter names are case-insensitive (`@Name` and `@name` are one parameter), as in SQL Server.

`build_sqlc.sh [dir]` builds `dir/sqlc-upstream` (the pinned commit as is) and `dir/sqlc` (with the patches); CI
(`.github/workflows/mssql.yml`) and `make sqlc-generate-mssql` / `make mssql-syntax-matrix` use them.

Everything else sqlc misses for SQL Server is listed, case by case, in the
[T-SQL syntax matrix](../../end2end/MssqlSyntaxMatrix/REPORT.md).
