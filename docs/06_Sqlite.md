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

| DB Type | Supported? |
|---------|------------|
| integer | ✅         |
| real    | ✅         |
| text    | ✅         |
| blob    | ✅         |

</details>

