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
| bit(1..8)                 | ✅ | ✅          |
| bit(9..64)                | ✅ | 🚫          |
| tinyint                   | ✅ | ✅          |
| tinyint unsigned          | ✅ | ✅ (1)      |
| smallint                  | ✅ | ✅          |
| smallint unsigned         | ✅ | ✅          |
| mediumint                 | ✅ | ✅          |
| integer, int              | ✅ | ✅          |
| integer unsigned, int unsigned | ✅ | ✅     |
| bigint                    | ✅ | ✅          |
| bigint unsigned           | ✅ | ✅          |
| float                     | ✅ (2) | ✅      |
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

Notes on integer, `bit` and `float` columns:
- Unsigned integers (sqlc reports them via the column's `unsigned` flag) are mapped to the unsigned C# types so that large values do not overflow:

  | DB Type                    | C# type  |
  |----------------------------|----------|
  | tinyint unsigned           | `byte`   |
  | smallint unsigned          | `ushort` |
  | int, integer unsigned      | `uint`   |
  | bigint unsigned            | `ulong`  |

  `mediumint unsigned` (max 16,777,215) fits in a signed `int` and stays `int`. `tinyint(1)`, `bool` and `boolean` are always `bool`.
- `bit(n)` with `n <= 8` is read as `byte`; wider `bit(n)` (up to 64 bits) is read as `ulong`, which is how MySqlConnector returns `BIT` columns.
- (1) A `:copyfrom` query cannot mix a `bit(n <= 8)` column with a `tinyint unsigned` column (both are `byte`, but `bit` is written to the CSV as a raw character). `bit(n > 8)` is not supported in `:copyfrom`; the plugin fails with a clear error instead of generating wrong code.
- (2) `float` is mapped to `double` by default to keep the generated code backward compatible. To get single precision, add an override with the type `float`, e.g. `column: "*:c_float"` with `csharp_type: { type: "float", notNull: false }`.

</details>

