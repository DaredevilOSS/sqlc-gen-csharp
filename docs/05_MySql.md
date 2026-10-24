# MySQL
<details>
<summary>:execlastid - Implementation</summary>

The `LAST_INSERT_ID` query is appended to the original query, both with and without `Dapper`:

```sql
INSERT INTO tab1 (field1, field2) VALUES ('a', 1); 
SELECT LAST_INSERT_ID();
```
The result is read as a scalar and converted to the C# type of the table's id column
(`int` for `int`/`integer`/`mediumint`, `long` for `bigint`/`serial`).

</details>

<details>
<summary>:copyfrom - Implementation</summary>
Implemented via the `LOAD DATA` command which can load data from a `CSV` file to a table.
Requires us to first save the input batch as a CSV, and then load it via the driver.

</details>

<details open>
<summary>Supported Data Types</summary>

Since in batch insert the data is not validated by the SQL itself but written and read from a CSV,
we consider support for the different data types separately for batch inserts and everything else.

| DB Type                   | Supported? | Supported in Batch? |
|---------------------------|----|-------------|
| bool, boolean, tinyint(1) | ✅ | ✅          |
| bit                       | ✅ | ✅          |
| tinyint                   | ✅ | ✅          |
| smallint                  | ✅ | ✅          |
| mediumint                 | ✅ | ✅          |
| integer, int              | ✅ | ✅          |
| bigint                    | ✅ | ✅          |
| real                      | ✅ | ✅          |
| numeric                   | ✅ | ✅          |
| decimal                   | ✅ | ✅          |
| double precision          | ✅ | ✅          |
| year                      | ✅ | ✅          |
| date                      | ✅ | ✅          |
| timestamp                 | ✅ | ✅          |
| char                      | ✅ | ✅          |
| nchar, national char      | ✅ | ✅          |
| varchar                   | ✅ | ✅          |
| tinytext                  | ✅ | ✅          |
| mediumtext                | ✅ | ✅          |
| text                      | ✅ | ✅          |
| longtext                  | ✅ | ✅          |
| binary                    | ✅ | ✅          |
| varbinary                 | ✅ | ✅          |
| tinyblob                  | ✅ | ✅          |
| blob                      | ✅ | ✅          |
| mediumblob                | ✅ | ✅          |
| longblob                  | ✅ | ✅          |
| enum                      | ✅ | ✅          |
| set                       | ✅ | ✅          |
| json                      | ✅ | ✅          |
| geometry                  | ⚠️  | ⚠️         |
| point                     | ⚠️  | ⚠️         |
| linestring                | ⚠️  | ⚠️         |
| polygon                   | ⚠️  | ⚠️         |
| multipoint                | ⚠️  | ⚠️         |
| multilinestring           | ⚠️  | ⚠️         |
| multipolygon              | ⚠️  | ⚠️         |
| geometrycollection        | ⚠️  | ⚠️         |

</details>

