# SQL Server (experimental)
> **Status: experimental.** sqlc's SQL Server engine (`engine: mssql`) is only on sqlc `main`, and this plugin
> currently needs it with the patches in [`scripts/sqlc-patches`](scripts/sqlc-patches) (macros, `:copyfrom`,
> `TRUNCATE`, case-insensitive parameter names). What sqlc does and does not support yet is tracked, with SQL Server
> itself as the reference, in the [T-SQL syntax matrix](end2end/MssqlSyntaxMatrix/REPORT.md).

```yaml
sql:
  - engine: mssql
    schema: schema.sql
    queries: query.sql
    codegen:
      - plugin: csharp
        out: Generated
```

Parameters are written as `@name` (matched case-insensitively, like SQL Server does) or with the sqlc macros
(`sqlc.arg`, `sqlc.narg`, `sqlc.slice`, `sqlc.embed`). The generated code uses `Microsoft.Data.SqlClient`, binds every
parameter with an explicit `SqlDbType` (so a `NULL` reaches a binary column and a `DateTime` keeps `datetime2`
precision), and opens a `SqlConnection` per call unless a transaction is given (`QuerySql.WithTransaction`).
All target frameworks are supported, Dapper included.

<details>
<summary>:execlastid - Implementation</summary>

## :execlastid - Implementation
Implemented via an `OUTPUT` clause on the `INSERT`, for an `IDENTITY` column or a `SEQUENCE` default alike:

```sql
INSERT INTO tab1 (field1, field2) OUTPUT INSERTED.id VALUES (@field1, @field2);
```
</details>

<details>
<summary>:copyfrom - Implementation</summary>

## :copyfrom - Implementation
Implemented via `SqlBulkCopy`: the batch is copied into a `DataTable` and written to the destination table.
</details>

<details open>
<summary>Supported Data Types</summary>

| DB Type                                            | C# Type          |
|----------------------------------------------------|------------------|
| bit                                                | bool             |
| tinyint                                            | byte             |
| smallint                                           | short            |
| int                                                | int              |
| bigint                                             | long             |
| real                                               | float            |
| float                                              | double           |
| decimal, numeric, money, smallmoney                | decimal          |
| char, nchar, varchar, nvarchar, text, ntext, xml   | string           |
| binary, varbinary, image, rowversion               | byte[]           |
| date, datetime, datetime2, smalldatetime           | DateTime         |
| datetimeoffset                                     | DateTimeOffset   |
| time                                               | TimeSpan         |
| uniqueidentifier                                   | Guid             |
| geography, geometry, hierarchyid                   | SqlGeography, SqlGeometry, SqlHierarchyId (`Microsoft.SqlServer.Types`) |
| sql_variant, and expressions sqlc cannot type yet  | object           |

## Useful Overrides
| DB Type                                | C# Type Override | Description                      |
|----------------------------------------|------------------|----------------------------------|
| datetime, datetime2, datetimeoffset    | Instant          | NodaTime instant (UTC)           |

Not supported yet: SQL Server 2025 `json` / `vector`, user-defined alias types (sqlc reports the alias name, not
its base type), table-valued parameters, stored procedure results.
</details>
