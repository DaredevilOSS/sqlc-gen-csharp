# SQLite3

## :execlastid - Implementation
Implemented via a `RETURNING` clause, allowing the `INSERT` command to return the newly created id, which can be of any
data type that can have a unique constraint.

```sql
INSERT INTO tab1 (field1, field2) VALUES ('a', 1) RETURNING id_field;
```

## :copyfrom - Implementation
Implemented via a multi `VALUES` clause, like this:

```sql
INSERT INTO tab1 (field1, field2) VALUES 
('a', 1),
('b', 2),
('c', 3);
```

## Data Types

| DB Type | Supported? |
|---------|------------|
| integer | ✅         |
| real    | ✅         |
| text    | ✅         |
| blob    | ✅         |