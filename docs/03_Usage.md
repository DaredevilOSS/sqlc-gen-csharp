# Usage
## Options
| Option                 | Possible values                                                                                                                                                                    | Optional | Info                                                                                                                                                                                                                                                                                                                                      |
|------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| overrideDriverVersion  | default:<br/> `2.3.6` for MySqlConnector  (mysql)<br/>`8.0.3` for Npgsql (postgresql)<br/>`8.0.10` for Microsoft.Data.Sqlite (sqlite)<br/><br/>values: The desired driver version  | Yes      | Determines the version of the driver to be used. |
| targetFramework        | default: `net8.0`<br/>values: `netstandard2.0`, `netstandard2.1`, `net8.0`                                                                                                         | Yes      | Determines the target framework for your generated code, meaning the generated code will be compiled to the specified runtime.<br/>For more information and help deciding on the right value, refer to the [Microsoft .NET Standard documentation](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-1-0). |
| generateCsproj         | default: `true`<br/>values: `false`,`true`                                                                                                                                         | Yes      | Assists you with the integration of SQLC and csharp by generating a `.csproj` file. This converts the generated output to a .dll, a project that you can easily incorporate into your build process.                                                                                                                                      |
| namespaceName          | default: the generated project name                                                                                                                                                | Yes      | Allows you to override the namespace name to be different than the project name                                                                                                                                                                                                                                                           |
| useDapper              | default: `false`<br/>values: `false`,`true`                                                                                                                                        | Yes      | Enables Dapper as a thin wrapper for the generated code. For more information, please refer to the [Dapper documentation](https://github.com/DapperLib/Dapper). |
| overrideDapperVersion  | default:<br/> `2.1.35`<br/>values: The desired Dapper version                                                                                                                      | Yes      | If `useDapper` is set to `true`, this option allows you to override the version of Dapper to be used. |

## Query Annotations
Basic functionality - same for all databases:
`:one`          - returns 0...1 records
`:many`         - returns 0...n records
`:exec`         - DML / DDL that does not return anything
`:execrows`     - returns number of affected rows by DML

Advanced functionality - varies between databases:
`:execlastid`   - INSERT with returned last inserted id
`:copyfrom`     - batch insert, implementation varies greatly

| Annotation  | PostgresSQL | MySQL | SQLite | Dapper  |
|-------------|-------------|-------|--------|---------|
| :one        | ✅          | ✅    | ✅     | ✅     |
| :many       | ✅          | ✅    | ✅     | ✅     |
| :exec       | ✅          | ✅    | ✅     | ✅     |
| :execrows   | ✅          | ✅    | ✅     | ✅     |
| :execlastid | ❌          | ✅    | ❌     | ❌     |
| :copyfrom   | ✅          | ❌    | ❌     | ❌     |

More info can be found in [here](https://docs.sqlc.dev/en/latest/reference/query-annotations.html).