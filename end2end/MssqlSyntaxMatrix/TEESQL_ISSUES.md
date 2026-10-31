<!-- 4 issues for https://github.com/sqlc-dev/teesql/issues/new . Each starts at a TITLE: line; the body follows. -->

TITLE: MERGE ... USING (VALUES ...) AS alias (columns) fails to parse

**teesql version:** v1.1.0 (`5cd4b20`)

A row constructor (`VALUES`) as the `MERGE` source fails to parse. This is the standard T-SQL upsert pattern, where the incoming row comes from parameters:

```sql
MERGE INTO authors AS target
USING (VALUES (N'a', N'b')) AS source (name, bio)
ON target.name = source.name
WHEN MATCHED THEN UPDATE SET bio = source.bio
WHEN NOT MATCHED THEN INSERT (name, bio) VALUES (source.name, source.bio);
```

```
expected identifier for schema object name
```

The other source forms parse:

| `USING ...` | Result |
|---|---|
| `staging AS source` (table) | ✅ `*ast.MergeStatement` |
| `(SELECT N'a' AS name) AS source` (derived table) | ✅ `*ast.MergeStatement` |
| `(VALUES (...)) AS source (name, bio)` | ❌ parse error |

**Expected:** a `MergeStatement` whose source is a derived table built from a row constructor (`InlineDerivedTable` in SqlScriptDOM), with the column alias list. SQL Server accepts the statement. `(VALUES ...) AS t (cols)` in a `FROM` clause already parses, so presumably only the `MERGE ... USING` path is missing it.

**Context:** found by the SQL Server syntax matrix in [sqlc-gen-csharp](https://github.com/DaredevilOSS/sqlc-gen-csharp/blob/9eac436c0842634df09af3d8699f4ab5884273be/end2end/MssqlSyntaxMatrix/README.md), which runs T-SQL cases through sqlc (and so teesql) and checks them against SQL Server 2022. The case ([`merge/merge_upsert.sql`](https://github.com/DaredevilOSS/sqlc-gen-csharp/blob/9eac436c0842634df09af3d8699f4ab5884273be/end2end/MssqlSyntaxMatrix/cases/merge/merge_upsert.sql)):

```sql
MERGE INTO authors AS target
USING (VALUES (@name, @bio)) AS source (name, bio)
ON target.name = source.name
WHEN MATCHED THEN UPDATE SET bio = source.bio
WHEN NOT MATCHED THEN INSERT (name, bio) VALUES (source.name, source.bio)
OUTPUT $action AS action, INSERTED.id;
```

---------------------------------------------------------------------------

TITLE: MESSAGE is treated as a reserved word in expressions and column lists

**teesql version:** v1.1.0 (`5cd4b20`)

A column named `message`, which is common in log and event tables, can be declared but not used:

| Statement | Result |
|---|---|
| `CREATE TABLE logs (id INT, message NVARCHAR(200));` | ✅ |
| `SELECT [message] FROM logs;` | ✅ |
| `SELECT message FROM logs;` | ❌ `unexpected token in expression: message` |
| `INSERT INTO logs (message) VALUES (N'x');` | ❌ `expected ), got message` |

**Expected:** all four parse. `MESSAGE` is not a reserved keyword in T-SQL. It is not in the [reserved keywords list](https://learn.microsoft.com/en-us/sql/t-sql/language-elements/reserved-keywords-transact-sql); it is only a contextual keyword in Service Broker statements (`CREATE MESSAGE TYPE`, `SEND ON CONVERSATION ... MESSAGE TYPE`). SQL Server runs both failing statements unquoted.

Other contextual keywords may be lexed the same way. The ones I'd check are `TYPE`, `STATUS`, `LEVEL`, `NAME`, `ROLE`, `STATE` and `VALUE`.

**Context:** found by the SQL Server syntax matrix in [sqlc-gen-csharp](https://github.com/DaredevilOSS/sqlc-gen-csharp/blob/9eac436c0842634df09af3d8699f4ab5884273be/end2end/MssqlSyntaxMatrix/README.md). The case ([`insert/schema_scoped.sql`](https://github.com/DaredevilOSS/sqlc-gen-csharp/blob/9eac436c0842634df09af3d8699f4ab5884273be/end2end/MssqlSyntaxMatrix/cases/insert/schema_scoped.sql)):

```sql
CREATE SCHEMA logging;
GO
CREATE TABLE logging.entries (id INT IDENTITY PRIMARY KEY, message NVARCHAR(200) NOT NULL);

INSERT INTO logging.entries (message) VALUES (@message);
```

---------------------------------------------------------------------------

TITLE: Nested block comments fail to parse

**teesql version:** v1.1.0 (`5cd4b20`)

T-SQL block comments nest; a `/* ... */` inside another one does not end the outer comment:

```sql
SELECT 1 /* outer /* inner */ still outer */ AS one;
```

```
unexpected token: outer
```

Both the flat comment `SELECT 1 /* flat */ AS one;` and the nested one run on SQL Server.

**Expected:** the whole `/* outer /* inner */ still outer */` is one comment. SQL Server [documents this](https://learn.microsoft.com/en-us/sql/t-sql/language-elements/slash-star-comment-transact-sql): "Nested comments are supported". It matters in practice because commenting out a block that already contains a comment is a common habit.

**Context:** found by the SQL Server syntax matrix in [sqlc-gen-csharp](https://github.com/DaredevilOSS/sqlc-gen-csharp/blob/9eac436c0842634df09af3d8699f4ab5884273be/end2end/MssqlSyntaxMatrix/README.md). The case ([`select/comments.sql`](https://github.com/DaredevilOSS/sqlc-gen-csharp/blob/9eac436c0842634df09af3d8699f4ab5884273be/end2end/MssqlSyntaxMatrix/cases/select/comments.sql)):

```sql
SELECT id, /* the name */ name -- trailing
FROM authors /* outer /* nested */ still a comment */
WHERE id > @min_id;
```

---------------------------------------------------------------------------

TITLE: The xml .query() method fails to parse

**teesql version:** v1.1.0 (`5cd4b20`)

Of the methods on an `xml` column, only `.query()` fails:

| Expression | Result |
|---|---|
| `doc.value('(/order/@id)[1]', 'INT')` | ✅ |
| `doc.exist('/order')` | ✅ |
| `o.doc.nodes('/order/item') AS i(n)` in `CROSS APPLY` | ✅ |
| `doc.query('/order/item')` | ❌ `unexpected token: query` |

```sql
SELECT doc.query('/order/item') FROM orders;
```

**Expected:** a function call on the column (`FunctionCall` with a `CallTarget` in SqlScriptDOM), like `.value()`. `QUERY` is probably lexed as a keyword after the `.`. That position is a method name, so any identifier, keywords included, should be accepted there.

**Context:** found by the SQL Server syntax matrix in [sqlc-gen-csharp](https://github.com/DaredevilOSS/sqlc-gen-csharp/blob/9eac436c0842634df09af3d8699f4ab5884273be/end2end/MssqlSyntaxMatrix/README.md). The case ([`xml/xml_methods.sql`](https://github.com/DaredevilOSS/sqlc-gen-csharp/blob/9eac436c0842634df09af3d8699f4ab5884273be/end2end/MssqlSyntaxMatrix/cases/xml/xml_methods.sql)):

```sql
CREATE TABLE orders_xml (id INT IDENTITY PRIMARY KEY, doc XML NOT NULL);

SELECT id, doc.value('(/order/@id)[1]', 'INT') AS order_id, doc.query('/order/item') AS items
FROM orders_xml WHERE doc.exist('/order[@status=sql:variable("@status")]') = 1;

SELECT o.id, i.n.value('@sku', 'NVARCHAR(20)') AS sku
FROM orders_xml o CROSS APPLY o.doc.nodes('/order/item') AS i(n);
```
