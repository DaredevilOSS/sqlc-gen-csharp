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
| bytea                                   | ✅         | ✅                  |
| 2-dimensional arrays (e.g text[],int[]) | ✅         | ❌                  |
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
| macaddr8                                | ✅         | ❌                  |
| tsvector                                | ✅         | ❌                  |
| tsquery                                 | ✅         | ❌                  |
| uuid                                    | ✅         | ✅                  |
| json                                    | ✅         | ❌                  |
| jsonb                                   | ✅         | ❌                  |
| jsonpath                                | ✅         | ❌                  |
| xml                                     | ✅         | ❌                  |
| enum                                    | ✅         | ❌                  |

*** `time with time zone` is not useful and not recommended to use by Postgres themselves - 
see [here](https://www.postgresql.org/docs/current/datatype-datetime.html#DATATYPE-DATETIME) -
so we decided not to implement support for it.

</details>

