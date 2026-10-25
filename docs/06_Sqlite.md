# SQLite3
<details>
<summary>:execlastid - Implementation</summary>

## :execlastid - Implementation
Implemented via a `RETURNING` clause, allowing the `INSERT` command to return the newly created id.
Only integer data type is supported as id for this annotation.
   
```sql
INSERT INTO tab1 (field1, field2) VALUES ('a', 1) RETURNING id_field;
```
</details>

<details>
<summary>:copyfrom - Implementation</summary>
Implemented via a multi `VALUES` clause, like this:

```sql
INSERT INTO tab1 (field1, field2) VALUES 
('a', 1),
('b', 2),
('c', 3);
```

</details>

<details open>
<summary>Supported Data Types</summary>

The declared type is matched case-insensitively and any `(...)` arguments are ignored (e.g. `VARCHAR(255)`). Like in sqlc,
`character`, `varchar`, `varying character`, `nchar`, `native character`, `nvarchar` and `decimal` match by prefix.

| DB Type                                                              | C# Type       | Supported? |
|----------------------------------------------------------------------|---------------|------------|
| integer, int, tinyint, smallint, mediumint, int2                     | `int`         | ✅         |
| bigint, unsigned big int, int8                                       | `long`        | ✅         |
| real, numeric, decimal                                               | `decimal`     | ✅         |
| double, double precision, float                                      | `double`      | ✅         |
| text, clob, character, varchar, varying character, nchar, native character, nvarchar | `string` | ✅ |
| blob                                                                 | `byte[]`      | ✅         |
| boolean, bool                                                        | `bool`        | ✅         |
| date, datetime, timestamp                                            | `DateTime`    | ✅         |
| json, jsonb                                                          | `JsonElement` | ✅         |
| any                                                                  | `object`      | ✅         |

`boolean`/`bool` are stored as `0`/`1`. `date`, `datetime` and `timestamp` are stored as text in the `yyyy-MM-dd HH:mm:ss` format.
`json`/`jsonb` are stored as text. `:copyfrom` does not apply type conversions, so `bool`, `DateTime` and `JsonElement` columns
are not supported in `:copyfrom` queries.

## Useful Overrides
It's possible to use the override data type functionality of the plugin thus overcoming the limited
amount of data types that are supported by SQLite. The supported overrides are specified below:

| DB Type | C# Type Override | Supported? | Description                                                      |
|---------|------------------|------------|------------------------------------------------------------------|
| integer | DateTime         | ✅         | Unix Epoch - seconds since 1-1-1970                              |
| text    | DateTime         | ✅         | String representation of the datetime in a configurable format   |
| integer | bool             | ✅         | If x equals 0 -> False, otherwise -> True                        |
| text    | bool             | ✅         | Converts string to a boolean value using Convert.ToBoolean rules |

</details>

