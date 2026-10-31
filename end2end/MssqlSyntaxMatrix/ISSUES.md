# SQL Server support: what is missing upstream

Issue drafts derived from the [syntax matrix](REPORT.md) (sqlc `main` at `3ad8b20` and teesql `v1.1.0`, SQL Server 2022
as the reference). Each one names the component, links the matrix cases that reproduce it and says what SQL Server
answers. Parser attribution was checked by feeding the statements straight to `teesql/parser`: everything not listed
under teesql parses fine there and is lost in sqlc.

| # | Component | Issue | Cases |
|---|-----------|-------|-------|
| P1 | sqlc (preprocess, compiler) | mssql macros, `sqlc.embed` on the core path, `:copyfrom` panic, `TRUNCATE`, case-insensitive `@name` | patches in [scripts/sqlc-patches](../../scripts/sqlc-patches) |
| S1 | sqlc (engine/mssql) | statements the converter does not know are silently dropped; multi-statement queries | 9 |
| S2 | sqlc (engine/mssql, analyzer) | FROM items: APPLY, table-valued functions, `VALUES`, `PIVOT`, TVPs | 9 |
| S3 | sqlc (engine/mssql) | date parts (`DAY`, `WEEK`, ...) are read as column references | 2 |
| S4 | sqlc (engine/mssql, catalog) | DDL: views, synonyms, computed columns, alias types, `sp_rename`, `dbo` schema | 5 |
| S5 | sqlc (core analyzer, mssql dialect) | functions and expressions without a result type; `N''` and arithmetic types | 22 |
| S6 | sqlc (core analyzer) | outer joins and `ROLLUP`/`CUBE` columns reported NOT NULL | 2 |
| S7 | sqlc (core analyzer) | parameters missing or untyped in some positions | 9 |
| S8 | sqlc (core analyzer) | result shape of `FOR JSON` / `FOR XML` and multi-statement `:execlastid` | 3 |
| S9 | sqlc (engine/mssql) | statements must end with `;` (optional in T-SQL) | 1 |
| S10 | sqlc (engine/mssql) | identifiers are lowercased (`[Author Id]` -> `author id`) | 1 |
| S11 | sqlc (engine/mssql) | recursive CTEs | 1 |
| T1 | teesql | `MERGE ... USING (VALUES ...) AS s (cols)` | 1 |
| T2 | teesql | `MESSAGE` is treated as a reserved word | 2 |
| T3 | teesql | nested block comments `/* /* */ */` | 1 |
| T4 | teesql | the xml `.query()` method | 1 |

---

## P1. Upstream the patches this plugin depends on (sqlc)
The four commits in [scripts/sqlc-patches](../../scripts/sqlc-patches), each a candidate PR with sqlc endtoend tests:
1. mssql dialect for `internal/sql/preprocess` (`sqlc.arg/narg/slice/embed` -> `@name`, `/*SLICE:x*/@x`, `t.*`),
   and `sqlc.embed` in `parseQueryCore` (affects every engine on the core analysis path). Cases: `macros/*`.
2. `INSERT ... VALUES` without a FROM clause makes `validate.Cmd` panic for `:copyfrom`. Case: `annotations/copyfrom_identity_table`.
3. `TRUNCATE TABLE` is dropped. Case: `delete/truncate`.
4. `@Name` / `@name` are one parameter in SQL Server. Case: `select/repeated_parameter`.

## S1. Unknown statements are silently dropped; multi-statement queries (sqlc)
`internal/engine/mssql/parse.go` skips any statement that converts to `ast.TODO`. With one statement the query file
fails with `no queries contained in paths`; with more, the query silently loses statements. Common T-SQL that
hits it: `SET NOCOUNT ON;` before a query, `DECLARE`, `SET IDENTITY_INSERT`, `EXEC proc`, `MERGE`,
`BEGIN TRAN ... COMMIT`, `TRY ... CATCH`, `IF @@ROWCOUNT = 0 INSERT ...`.
- Expected: an error for an annotated query sqlc cannot handle, and support for `MERGE` (with `OUTPUT`), `EXEC`
  (result set unknown, so `:exec`/`:execrows` at least) and multi-statement queries whose last statement produces
  the result (`SET NOCOUNT ON; SELECT ...`, `INSERT ...; SELECT SCOPE_IDENTITY()`).
- Cases: [batch/set_nocount_on](cases/batch/set_nocount_on.sql), [batch/declare_variables](cases/batch/declare_variables.sql),
  [batch/transaction_in_query](cases/batch/transaction_in_query.sql), [batch/try_catch_throw](cases/batch/try_catch_throw.sql),
  [insert/identity_insert](cases/insert/identity_insert.sql), [insert/output_into_table_variable](cases/insert/output_into_table_variable.sql),
  [merge/merge_delete](cases/merge/merge_delete.sql), [merge/upsert_without_merge](cases/merge/upsert_without_merge.sql),
  [programmability/exec_procedure](cases/programmability/exec_procedure.sql)

## S2. FROM items the converter does not support (sqlc)
`scope: unsupported FROM item *ast.TODO` for `CROSS APPLY` / `OUTER APPLY`, table-valued functions (user-defined,
`OPENJSON ... WITH (...)`, `STRING_SPLIT`, `GENERATE_SERIES`), `VALUES` as a derived table, `PIVOT` / `UNPIVOT`,
and table-valued parameters (`JOIN @ids`, needs `CREATE TYPE ... AS TABLE` in the catalog and a parameter type the
plugin can bind as `SqlDbType.Structured`).
- Cases: [joins/cross_apply](cases/joins/cross_apply.sql), [json/openjson](cases/json/openjson.sql),
  [programmability/string_split](cases/programmability/string_split.sql), [programmability/inline_table_function](cases/programmability/inline_table_function.sql),
  [programmability/multi_statement_table_function](cases/programmability/multi_statement_table_function.sql),
  [programmability/table_valued_parameter](cases/programmability/table_valued_parameter.sql),
  [select/table_value_constructor](cases/select/table_value_constructor.sql), [setops/pivot_unpivot](cases/setops/pivot_unpivot.sql),
  [expressions/sql2022_functions](cases/expressions/sql2022_functions.sql) (`GENERATE_SERIES`)

## S3. Date parts are read as column references (sqlc)
`DATEADD(DAY, @days, published_at)` fails with `unknown column "day"`; the same for `DATEDIFF`, `DATEDIFF_BIG`,
`DATEPART`, `DATENAME`, `DATETRUNC`, `DATE_BUCKET`. The first argument is a keyword, not an expression.
- Cases: [expressions/datetime_functions](cases/expressions/datetime_functions.sql), [expressions/sql2022_functions](cases/expressions/sql2022_functions.sql)

## S4. DDL the catalog does not understand (sqlc)
- `CREATE VIEW` and `CREATE SYNONYM`: querying them fails (`relation "public"."author_books" ... no rows`).
  Cases: [ddl/view](cases/ddl/view.sql), [ddl/synonym](cases/ddl/synonym.sql)
- Computed columns `full_name AS (...)` [PERSISTED]: `missing type name`; SQL Server derives the type from the
  expression. Case: [ddl/computed_columns](cases/ddl/computed_columns.sql)
- Alias types `CREATE TYPE dbo.email FROM NVARCHAR(320) NOT NULL`: columns and parameters come through as
  `email_address` instead of `nvarchar(320)` (and NOT NULL). Case: [ddl/alias_type](cases/ddl/alias_type.sql)
- `EXEC sp_rename 'authors.bio', 'biography', 'COLUMN'` (how T-SQL renames a column) is ignored. Case: [ddl/sp_rename_column](cases/ddl/sp_rename_column.sql)
- The default schema is reported as `public` (and listed twice in the catalog); SQL Server calls it `dbo`.

## S5. Expressions without a result type (sqlc)
These come through with an empty type, so a typed client can only use `object`. SQL Server's types are in the report details.
- String: `SUBSTRING`, `LEFT`, `RIGHT`, `REPLACE`, `CHARINDEX`, `PATINDEX`, `REVERSE`, `REPLICATE`, `STUFF`, `CONCAT`,
  `CONCAT_WS`, `FORMAT`, `QUOTENAME`, `TRANSLATE`, `STRING_AGG`, `STRING_ESCAPE`
- Math: `ABS`, `ROUND`, `CEILING`, `FLOOR`, `POWER`, `SQRT`, `SIGN`, `PI`, `RAND`; aggregates `STDEV`, `VAR`, `APPROX_COUNT_DISTINCT`, `GROUPING`
- Window: `ROW_NUMBER`, `RANK`, `DENSE_RANK`, `NTILE`, `LAG`, `LEAD`, `FIRST_VALUE`, `LAST_VALUE`, `PERCENTILE_CONT`, `PERCENTILE_DISC`
- Conditional / conversion: `ISNULL`, `NULLIF`, `IIF`, `CHOOSE`, `PARSE`, `TRY_PARSE`, `GREATEST`, `LEAST`, `AT TIME ZONE`, `SWITCHOFFSET`
- JSON: `JSON_VALUE`, `JSON_QUERY`, `ISJSON`, `JSON_MODIFY`, `JSON_OBJECT`, `JSON_ARRAY`
- System: `@@ROWCOUNT`, `IDENT_CURRENT`, `DB_NAME`, `SUSER_SNAME`, `HASHBYTES`, `CHECKSUM`, `NEXT VALUE FOR`, scalar user-defined functions
- Wrong types: `N'...'` literals are typed `varchar` (SQL Server: `nvarchar`), `decimal * int` is typed `bigint` (SQL Server: `decimal`).
- Cases: see every 🟠 "has no type" in the report, e.g. [expressions/string_functions](cases/expressions/string_functions.sql),
  [window/ranking](cases/window/ranking.sql), [expressions/math_functions](cases/expressions/math_functions.sql),
  [json/json_value_query](cases/json/json_value_query.sql), [select/unicode_literals](cases/select/unicode_literals.sql),
  [expressions/arithmetic](cases/expressions/arithmetic.sql)

## S6. Nullability of outer joins and grouping extensions (sqlc)
Columns from the nullable side of `LEFT` / `RIGHT` / `FULL OUTER JOIN`, and the grouping columns of `ROLLUP` /
`CUBE` / `GROUPING SETS`, are reported NOT NULL; a generated reader then fails at runtime on a NULL.
- Cases: [joins/inner_left_right_full](cases/joins/inner_left_right_full.sql), [aggregate/rollup_cube_grouping_sets](cases/aggregate/rollup_cube_grouping_sets.sql)

## S7. Parameters missing or untyped (sqlc)
Missing (the generated code would not bind them): `UPDATE TOP (@n)`, `DELETE TOP (@n)`, `FOR SYSTEM_TIME AS OF @at`,
`AT TIME ZONE @tz`, `TRY_PARSE(@text AS ...)`, `IS DISTINCT FROM @bio`.
Untyped although SQL Server knows the type: a parameter in an `INSERT ... SELECT` list, `ISNULL(@p, '%')`,
`GREATEST(price, @floor)`, `HASHBYTES('SHA2_256', @payload)`, a scalar UDF argument, `JSON_VALUE(...) = @kind`.
- Cases: [update/top](cases/update/top.sql), [delete/top](cases/delete/top.sql), [ddl/temporal_table](cases/ddl/temporal_table.sql),
  [expressions/at_time_zone](cases/expressions/at_time_zone.sql), [expressions/cast_convert](cases/expressions/cast_convert.sql),
  [expressions/is_distinct_from](cases/expressions/is_distinct_from.sql), [insert/insert_select](cases/insert/insert_select.sql),
  [expressions/coalesce_isnull_nullif](cases/expressions/coalesce_isnull_nullif.sql), [expressions/greatest_least](cases/expressions/greatest_least.sql)

## S8. Result shape (sqlc)
- `FOR JSON` / `FOR XML` return one `nvarchar(max)` / `xml` column, not the selected columns.
  Cases: [json/for_json](cases/json/for_json.sql), [xml/for_xml](cases/xml/for_xml.sql)
- `INSERT ...; SELECT SCOPE_IDENTITY()` reports no result column. Case: [annotations/execlastid_scope_identity](cases/annotations/execlastid_scope_identity.sql)

## S9. A terminating semicolon is required (sqlc)
`missing semicolon at end of file`; statement splitting in `engine/mssql/parse.go` relies on `;`, but teesql already
returns statement boundaries. Case: [select/no_semicolon_terminator](cases/select/no_semicolon_terminator.sql)

## S10. Identifiers are lowercased (sqlc)
`SELECT id AS [Author Id]` comes back as `author id`; SQL Server keeps `Author Id` (Minor in the report, but names
matter to clients that map by name). Case: [select/quoted_aliases](cases/select/quoted_aliases.sql)

## S11. Recursive CTEs (sqlc)
`WITH numbers (n) AS (SELECT 1 UNION ALL SELECT n + 1 FROM numbers ...)` fails: the CTE is looked up as a table.
Case: [cte/recursive_cte](cases/cte/recursive_cte.sql)

The teesql tickets below are written out in full, ready to open, in [TEESQL_ISSUES.md](TEESQL_ISSUES.md).

## T1. `MERGE ... USING (VALUES ...) AS source (cols)` (teesql)
`expected identifier for schema object name`; `USING table AS source` parses.
Case: [merge/merge_upsert](cases/merge/merge_upsert.sql)

## T2. `MESSAGE` is not a reserved word (teesql)
`INSERT INTO t (message) ...` -> `expected ), got message`, `SELECT message FROM t` -> `unexpected token in expression`.
Cases: [insert/schema_scoped](cases/insert/schema_scoped.sql), [annotations/copyfrom_schema_scoped](cases/annotations/copyfrom_schema_scoped.sql)

## T3. Nested block comments (teesql)
T-SQL nests block comments: `SELECT 1 /* a /* b */ c */;` -> `unexpected token: *`. Case: [select/comments](cases/select/comments.sql)

## T4. The xml `.query()` method (teesql)
`doc.query('/order/item')` -> `unexpected token: query` (`doc.value(...)` parses). Case: [xml/xml_methods](cases/xml/xml_methods.sql)
