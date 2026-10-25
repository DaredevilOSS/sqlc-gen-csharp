# PostgresSQL
<details>
<summary>:execlastid - Implementation</summary>

Implemented via a `RETURNING` clause, allowing the `INSERT` command to return the newly created id.
The data types that can be used as id data types for this annotation are:
1. uuid
2. bigint
3. integer
4. smallint (less recommended due to small id range, but possible)

```sql
INSERT INTO tab1 (field1, field2) VALUES ('a', 1) RETURNING id_field;
```
</details>

<details>
<summary>:copyfrom - Implementation</summary>

Implemented via the `COPY FROM` command which can load binary data directly from `stdin`.
</details>

<details open>
<summary>Supported Data Types</summary>

Since in batch insert the data is not validated by the SQL itself but written in a binary format, 
we consider support for the different data types separately for batch inserts and everything else.

| DB Type                                 | Supported? | Supported in Batch? |
|-----------------------------------------|------------|-------------------- |
| boolean                                 | ✅         | ✅                  |
| smallint                                | ✅         | ✅                  |
| integer                                 | ✅         | ✅                  |
| bigint                                  | ✅         | ✅                  |
| real                                    | ✅         | ✅                  |
| decimal, numeric                        | ✅         | ✅                  |
| double precision                        | ✅         | ✅                  |
| date                                    | ✅         | ✅                  |
| timestamp, timestamp without time zone  | ✅         | ✅                  |
| timestamp with time zone                | ✅         | ✅                  |
| time, time without time zone            | ✅         | ✅                  |
| time with time zone                     | 🚫         | 🚫                  |
| interval                                | ✅         | ✅                  |
| char                                    | ✅         | ✅                  |
| bpchar                                  | ✅         | ✅                  |
| varchar, character varying              | ✅         | ✅                  |
| text                                    | ✅         | ✅                  |
| name                                    | ✅         | ✅                  |
| citext                                  | ✅         | ✅                  |
| ltree                                   | ✅         | ✅                  |
| lquery                                  | ✅         | ✅                  |
| ltxtquery                               | ✅         | ✅                  |
| oid, xid, cid                           | ✅         | ✅                  |
| xid8                                    | ✅         | ✅                  |
| tid                                     | ✅         | ✅                  |
| pg_lsn                                  | ✅         | ✅                  |
| int4range, int8range, numrange          | ✅         | ✅                  |
| daterange, tsrange, tstzrange           | ✅         | ✅                  |
| int4multirange, int8multirange, nummultirange | ✅   | ✅                  |
| datemultirange, tsmultirange, tstzmultirange  | ✅   | ✅                  |
| bytea                                   | ✅         | ✅                  |
| bit, bit varying (varbit)               | ✅         | ✅                  |
| 2-dimensional arrays (e.g text[],int[]) | ✅         | ✅                  |
| money                                   | ✅         | ✅                  |
| point                                   | ✅         | ✅                  |
| line                                    | ✅         | ✅                  |
| lseg                                    | ✅         | ✅                  |
| box                                     | ✅         | ✅                  |
| path                                    | ✅         | ✅                  |
| polygon                                 | ✅         | ✅                  |
| circle                                  | ✅         | ✅                  |
| cidr                                    | ✅         | ✅                  |
| inet                                    | ✅         | ✅                  |
| macaddr                                 | ✅         | ✅                  |
| macaddr8                                | ✅         | ⚠️                  |
| tsvector                                | ✅         | ❌                   |
| tsquery                                 | ✅         | ❌                   |
| uuid                                    | ✅         | ✅                  |
| json                                    | ✅         | ✅                  |
| jsonb                                   | ✅         | ✅                  |
| jsonpath                                | ✅         | ⚠️                  |
| xml                                     | ✅         | ⚠️                  |
| enum                                    | ✅         | ⚠️                  |
| any                                     | ✅         | ❌                   |
| hstore                                  | ✅         | ❌                   |

*** `time with time zone` is not useful and not recommended to use by Postgres themselves - 
see [here](https://www.postgresql.org/docs/current/datatype-datetime.html#DATATYPE-DATETIME) -
so we decided not to implement support for it.

*** Some data types require conversion in the INSERT statement, and SQLC disallows argument conversion in queries with `:copyfrom` annotation, which are used for batch inserts. 
These are the data types that require this conversion:
1. `macaddr8`
2. `jsonpath`
3. `xml`
4. `enum`

An example of this conversion:
```sql
INSERT INTO tab1 (macaddr8_field) VALUES (sqlc.narg('macaddr8_field')::macaddr8);
```

*** `bit`, `bit(n)`, `varbit` and `bit varying` map to `System.Collections.BitArray` (including `bit(1)`, which Npgsql
would otherwise return as `bool`). Arrays of bit strings are supported for regular queries; in `:copyfrom` a `bit(n)[]` column is not
supported since Npgsql writes `BitArray[]` as `varbit[]`, which does not match the column type in the binary `COPY` protocol.

*** `name`, `citext`, `ltree`, `lquery` and `ltxtquery` map to `string`. The last four come from the `citext` / `ltree` extensions, which
must be created in your schema (`CREATE EXTENSION citext;`) for sqlc to recognize them. When using Dapper, `ltree`, `lquery` and `ltxtquery`
parameters need an explicit cast in the INSERT statement (e.g. `sqlc.narg('c_ltree')::ltree`), since Dapper sends the value as `text`.
`lquery` and `ltxtquery` have no equality operator, so they cannot be used in `GROUP BY` / `DISTINCT`.

*** System types map to `uint` (`oid`, `xid`, `cid`), `ulong` (`xid8`), `NpgsqlTid` (`tid`) and `NpgsqlLogSequenceNumber` (`pg_lsn`).
When using Dapper, these parameters are sent with an explicit `NpgsqlDbType`, since it cannot be inferred from the CLR type (`oid`, `xid` and `cid` are all `uint`).

*** Range types map to `NpgsqlRange<T>` (`int4range` - `NpgsqlRange<int>`, `int8range` - `NpgsqlRange<long>`, `numrange` - `NpgsqlRange<decimal>`,
`daterange` / `tsrange` / `tstzrange` - `NpgsqlRange<DateTime>`) and multirange types (PostgreSQL 14+) to `NpgsqlRange<T>[]`.
Both are also supported as array elements (`int4range[]` is `NpgsqlRange<int>[]`, the same C# type as `int4multirange`).
Note that PostgreSQL normalizes discrete ranges (e.g. `[1,10]` is read back as `[1,11)`), and that a `tstzrange` must be written with `DateTimeKind.Utc` bounds.
When using Dapper, range parameters are sent with an explicit `NpgsqlDbType`, so no cast is needed in the query.

*** `any` is a pseudo-type reported by sqlc when it cannot infer a column's type; it maps to `object` in C#.
*** `hstore` is a key-value store type that maps to `object` in C#; readers use `GetValue()` to retrieve the value.

</details>

