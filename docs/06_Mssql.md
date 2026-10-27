# SQL Server (experimental)
> **Status: proof of concept.** sqlc's SQL Server engine (`engine: mssql`) is new and lives on sqlc's `main` branch,
> so a sqlc build from `main` is required. The `sqlc.arg` / `sqlc.narg` / `sqlc.slice` / `sqlc.embed` macros,
> `TRUNCATE TABLE` and `:copyfrom` additionally need the fixes tracked in the sqlc repository (see [Contributing](#contributing)).

```yaml
sql:
  - engine: mssql
    schema: schema.sql
    queries: query.sql
    codegen:
      - plugin: csharp
        out: Generated
```

Parameters are written as `@name` (or with the sqlc macros), and the generated code uses `Microsoft.Data.SqlClient`.

<details>
<summary>:execlastid - Implementation</summary>

## :execlastid - Implementation
Implemented via an `OUTPUT` clause, allowing the `INSERT` command to return the newly created id.
Use a `SEQUENCE` default (or `IDENTITY`) for the id column.

```sql
INSERT INTO tab1 (field1, field2) OUTPUT INSERTED.id_field VALUES (@field1, @field2);
```
</details>

<details>
<summary>:copyfrom - Implementation</summary>

## :copyfrom - Implementation
Implemented via `SqlBulkCopy`: the batch is copied into a `DataTable` and written to the destination table.
</details>

<details open>
<summary>Supported Data Types</summary>

| DB Type                            | C# Type          | Supported? |
|------------------------------------|------------------|------------|
| bit                                | bool             | ✅         |
| tinyint                            | byte             | ✅         |
| smallint                           | short            | ✅         |
| int                                | int              | ✅         |
| bigint                             | long             | ✅         |
| real                               | float            | ✅         |
| float                              | double           | ✅         |
| decimal, numeric, money, smallmoney | decimal         | ✅         |
| char, nchar, varchar, nvarchar, text, ntext, xml | string | ✅       |
| binary, varbinary, image, rowversion | byte[]         | ✅         |
| date, datetime, datetime2, smalldatetime | DateTime   | ✅         |
| datetimeoffset                     | DateTimeOffset   | ✅         |
| time                               | TimeSpan         | ✅         |
| uniqueidentifier                   | Guid             | ✅         |
| sql_variant                        | object           | ✅         |

Not supported yet: `geography`, `geometry`, `hierarchyid`, user defined types, netstandard (legacy) targets.

</details>
