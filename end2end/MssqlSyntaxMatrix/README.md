# SQL Server syntax matrix

What does sqlc need to support for a C# developer to use it with SQL Server? This project answers it with
evidence. Every file in [`cases/`](cases) is a small schema plus one or more sqlc queries, covering one T-SQL
feature, and every case is run through:

1. **each sqlc build** given on the command line (typically sqlc `main`, and sqlc `main` with the patches in
   [`scripts/sqlc-patches`](../../scripts/sqlc-patches)), with the `json` codegen, to get the parameters and
   result columns sqlc infers;
2. **SQL Server itself**: the schema is created in a fresh database and the query is described with
   `sp_describe_undeclared_parameters` and `sp_describe_first_result_set`. This is the oracle: what SQL Server
   says the parameter and column types are;
3. **this plugin** (driver and Dapper flavours) on the output of the last sqlc build;
4. optionally, **the C# compiler** on all the generated code (`--compile`).

The result is [REPORT.md](REPORT.md) (and [results.json](results.json) for tooling).

| Level | Meaning |
|-------|---------|
| ✅ | sqlc agrees with SQL Server |
| 🟡 | minor: sqlc is more conservative, or picks a compatible type of the same family (`int` vs `bigint`) |
| 🟠 | wrong or missing type, nullability, parameter, name or column count |
| 🕳️ | the query is silently dropped from sqlc's output |
| ❌ | sqlc fails |
| 💥 | sqlc panics |

## Running it

Needs SQL Server (`docker compose up -d mssqldb`), the published process plugin (`make dotnet-publish-process`)
and one or more sqlc binaries:

```bash
make mssql-syntax-matrix SQLC_UPSTREAM=/path/to/sqlc-main SQLC_PATCHED=/path/to/sqlc-patched
# or directly
dotnet run --project end2end/MssqlSyntaxMatrix -- \
  --sqlc "upstream=/path/to/sqlc@sqlc main (3ad8b20)" \
  --sqlc "patched=/path/to/sqlc@sqlc main + patches" \
  --compile [--filter select/]
```

`--sqlc name=path[@label]` is repeatable and sets the report columns; the plugin and compile columns use the last one.

## Adding a case

```sql
-- @description What the case covers (shown in the report)
-- @schema
CREATE TABLE things (id INT NOT NULL);   -- optional; cases/base.sql (authors, books) is always created first
-- @queries
-- name: ListThings :many
SELECT id FROM things WHERE id > @min_id;
```

Keep a case to one feature, write it the way a SQL Server developer would, and make sure SQL Server accepts it
(the report flags a schema SQL Server rejects, and the details show when SQL Server cannot describe a query).

## Known limits of the oracle

- SQL Server reports most expressions (`COUNT(*)`, `CASE`, ...) as nullable, so a NOT NULL expression from sqlc is
  only 🟡; for table columns (joins, `ROLLUP`) the difference is 🟠.
- `sp_describe_undeclared_parameters` refuses a parameter used twice; every use is then described separately and
  the types are merged (`@p IS NULL` makes SQL Server guess `int`, so a concrete type wins over it).
- Some parameters cannot be typed by SQL Server either (`COALESCE(@p, '%')`, `STRING_SPLIT(@p, ',')`,
  `OPENJSON(@p)`) and temp tables cannot be described; the details say so.
- SQL Server 2025 types (`json`, `vector`) are not covered because the CI image is SQL Server 2022.
