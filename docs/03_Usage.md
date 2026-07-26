# Usage
## Options

| Option                      | Possible values                                                                                                                                                                   | Optional | Info                                                                                                                                                                                                                                                                                                                                      |
|-----------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| overrideDriverVersion       | default:<br/> `2.3.6` for MySqlConnector  (mysql)<br/>`8.0.3` for Npgsql (postgresql)<br/>`8.0.10` for Microsoft.Data.Sqlite (sqlite)<br/><br/>values: The desired driver version | Yes      | Allows you to override the version of DB driver to be used.                                                                                                                                                                                                                                                                               |
| targetFramework             | default: `net8.0`<br/>values: `netstandard2.0`, `netstandard2.1`, `net8.0`                                                                                                        | Yes      | Determines the target framework for your generated code, meaning the generated code will be compiled to the specified runtime.<br/>For more information and help deciding on the right value, refer to the [Microsoft .NET Standard documentation](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-1-0). |
| generateCsproj              | default: `true`<br/>values: `false`,`true`                                                                                                                                        | Yes      | Assists you with the integration of SQLC and csharp by generating a `.csproj` file. This converts the generated output to a .dll, a project that you can easily incorporate into your build process.                                                                                                                                      |
| namespaceName               | default: the generated project name                                                                                                                                               | Yes      | Allows you to override the namespace name to be different than the project name                                                                                                                                                                                                                                                           |
| useDapper                   | default: `false`<br/>values: `false`,`true`                                                                                                                                       | Yes      | Enables Dapper as a thin wrapper for the generated code. For more information, please refer to the [Dapper documentation](https://github.com/DapperLib/Dapper).                                                                                                                                                                           |
| overrideDapperVersion       | default:<br/> `2.1.35`<br/>values: The desired Dapper version                                                                                                                     | Yes      | If `useDapper` is set to `true`, this option allows you to override the version of Dapper to be used.                                                                                                                                                                                                                                     |
| Override                    | values: A nested override value like [this](#override-option).                                                                                                                    | Yes      | Allows you to override the generated C# data types for specific columns in specific queries. This option accepts a `query_name:column_name` mapping and the overriden data type.                                                                                                                                                          |                                                                                     |
| useCentralPackageManagement | default: `false`<br/>values: `false`,`true`                                                                                                                                       | Yes      | If true, the code is generated to support central package management.                                                                                                                                                                                                                                                                     |
| useProperSingularization    | default: `false`<br/>values: `false`,`true`                                                                                                                                       | Yes      | When true, model names are singularized using English grammar rules (`policies` -> `Policy`, `statuses` -> `Status`). Defaults to the legacy behavior of trimming trailing `s` characters, which mangles some names (`policies` -> `Policie`). Opt-in because enabling it renames existing generated classes.                          |
| withAsyncSuffix             | default: `true`<br/>values: `false`,`true`                                                                                                                                       | Yes      | When true, async methods will have the "Async" suffix appended to their names (e.g., `GetAuthorAsync`). When false, async methods will not have the suffix (e.g., `GetAuthor`).                                                                                                                                                        |
| withCancellationToken       | default: `false`<br/>values: `false`,`true`                                                                                                                                      | Yes      | When true, every generated method takes an optional trailing `CancellationToken cancellationToken = default`, threaded into every async DB call so in-flight queries can be cancelled. See [Cancellation](#cancellation). When false (default), generated output is unchanged.                                                          |

### Override option
Override for a specific query:
```yaml
overrides:
  - column: "<query-name>:<field-name>"
    csharp_type:
      type: "<csharp-datatype>"
      notNull: true|false
```

Override for all queries:
```yaml
overrides:
  - column: "*:<field-name>"
    csharp_type:
      type: "<csharp-datatype>"
      notNull: true|false
```

## Supported Features
- ✅ means the feature is fully supported.
- ⚠️ means SQLC does not yet support this feature, so it cannot be supported by the plugin yet.
- 🚫 means the database does not support the feature.
- ❌ means the feature is not supported by the plugin (but could be supported by the database).

### Query Annotations
Basic functionality - same for all databases:
- `:one`          - returns 0...1 records
- `:many`         - returns 0...n records
- `:exec`         - DML / DDL that does not return anything
- `:execrows`     - returns number of affected rows by DML

Advanced functionality - varies between databases:
- `:execlastid`   - INSERT with returned last inserted id
- `:copyfrom`     - batch insert, implementation varies greatly
<br/>

| Annotation  | PostgresSQL | MySQL | SQLite  |
|-------------|-------------|-------|---------|
| :one        | ✅          | ✅    | ✅      |
| :many       | ✅          | ✅    | ✅      |
| :exec       | ✅          | ✅    | ✅      |
| :execrows   | ✅          | ✅    | ✅      |
| :execlastid | ✅          | ✅    | ✅      |
| :copyfrom   | ✅          | ✅    | ✅      |

More info can be found in [here](https://docs.sqlc.dev/en/stable/reference/query-annotations.html).

### Macro Annotations
- `sqlc.arg`       - Attach a name to a parameter in a SQL query
- `sqlc.narg`      - The same as `sqlc.arg`, but always marks the parameter as nullable
- `sqlc.slice`     - For databases that do not support passing arrays to the `IN` operator, generates a dynamic query at runtime with the correct number of parameters
- `sqlc.embed`     - Embedding allows you to reuse existing model structs in more queries
<br/>

| Annotation  | PostgresSQL | MySQL | SQLite  |
|-------------|-------------|-------|---------|
| sqlc.arg    | ✅          | ✅    | ✅       |
| sqlc.narg   | ✅          | ✅    | ✅       |
| sqlc.slice  | 🚫          | ✅    | ✅       |
| sqlc.embed  | ✅          | ✅    | ✅       |

More info can be found in [here](https://docs.sqlc.dev/en/stable/reference/macros.html#macros).

### Transactions
Transactions are supported by the plugin.
<br/>

#### Example using a transaction
```C#
public async Task ExampleTransaction(IDbConnection connection)
{
    // Begin a transaction
    using (var transaction = connection.BeginTransaction())
    {
        try
        {
            // Create a new Queries object with the transaction instead of the connection
            var queries = QuerySql.WithTransaction(transaction);

            // Example: Insert a new author
            var newAuthor = await queries.CreateAuthor(new CreateAuthorParams { Name = "Jane Doe", Bio = "Another author" });

            // Example: Get the author by ID within the same transaction
            var author = await queries.GetAuthor(newAuthor.AuthorID);

            // Example: Update the author's bio
            await queries.UpdateAuthorBio(new UpdateAuthorBioParams { AuthorID = author.AuthorID, Bio = "Updated bio for Jane Doe" });

            // Commit the transaction if all operations are successful
            transaction.Commit();
            Console.WriteLine("Transaction committed successfully.");
        }
        catch (Exception ex)
        {
            // Rollback the transaction if any error occurs
            transaction.Rollback();
            Console.WriteLine($"Transaction rolled back due to error: {ex.Message}");
            throw;
        }
    }
}
```

More info can be found in [here](https://docs.sqlc.dev/en/stable/howto/transactions.html).

### Cancellation
Set `withCancellationToken: true` to make every generated method accept an optional
`CancellationToken`. The token is threaded into every async database call, so a cancelled
token interrupts the in-flight query and releases the locks that statement holds. The
option is off by default; when off, generated output is identical to before.

```C#
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
try
{
    var author = await queries.GetAuthor(authorId, cts.Token);
}
catch (OperationCanceledException)
{
    // the query was cancelled; its statement-level locks are released
}
```

Notes:
- **PostgreSQL & MySQL** interrupt the running statement server-side (Npgsql sends a cancel
  request; MySqlConnector issues `KILL QUERY`), releasing that statement's locks immediately.
- **SQLite** runs synchronously and only honours the token *before* the statement starts;
  an already-running SQLite statement is not interrupted.
- **Transactions:** cancelling a query inside a `WithTransaction(...)` flow releases only that
  statement's locks. Locks held by earlier statements in the transaction (and, on PostgreSQL,
  the aborted-transaction state) are released when **you** roll back — the generated code never
  rolls back for you.
