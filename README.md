# <p align="center">sqlc-gen-csharp</p>
<p align="center">
    <img src="https://github.com/DaredevilOSS/sqlc-gen-csharp/actions/workflows/build.yml/badge.svg?branch=main" alt="Build">
    <img src="https://github.com/DaredevilOSS/sqlc-gen-csharp/actions/workflows/tests.yml/badge.svg?branch=main" alt=".Net Core Tests">
    <img src="https://github.com/DaredevilOSS/sqlc-gen-csharp/actions/workflows/legacy-tests.yml/badge.svg?branch=main" alt=".Net Framework Tests (Legacy)">
</p>

sqlc-gen-csharp is a .Net plugin for [sqlc](https://github.com/sqlc-dev/sqlc).<br/> It leverages the SQLC plugin system to generate type-safe C# code for SQL queries, supporting  PostgresSQL, MySQL & SQLite via the corresponding driver or suitable Dapper abstraction.
## Quickstart
```yaml
version: "2"
plugins:
- name: csharp
  wasm:
    url: https://github.com/DaredevilOSS/sqlc-gen-csharp/releases/download/v0.21.2/sqlc-gen-csharp.wasm
    sha256: 98990c2a46b0e315c7f062f5643e3404ed4150a5ef2808c47a4e10084443506e
sql:
  # For PostgresSQL
  - schema: schema.sql
    queries: queries.sql
    engine: postgresql
    codegen:
      - plugin: csharp
        out: PostgresDalGen
  # For MySQL
  - schema: schema.sql
    queries: queries.sql
    engine: mysql
    codegen:
      - plugin: csharp
        out: MySqlDalGen
  # For SQLite
  - schema: schema.sql
    queries: queries.sql
    engine: sqlite
    codegen:
      - plugin: csharp
        out: SqliteDalGen
```
# Usage
## Options
| Option                | Possible values                                                                                | Optional | Info |
|--------------------- |----------------------------------------------------------------------------------------------|----------|------|
| overrideDriverVersion| default:<br/> `2.3.6` for MySqlConnector  (mysql)<br/>`8.0.3` for Npgsql (postgresql)<br/>`8.0.10` for Microsoft.Data.Sqlite (sqlite)<br/><br/>values: The desired driver version | Yes | Allows you to override the version of DB driver to be used. |
| targetFramework      | default: `net8.0`<br/>values: `netstandard2.0`, `netstandard2.1`, `net8.0` | Yes | Determines the target framework for your generated code, meaning the generated code will be compiled to the specified runtime.<br/>For more information and help deciding on the right value, refer to the [Microsoft .NET Standard documentation](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-1-0). |
| generateCsproj       | default: `true`<br/>values: `false`,`true` | Yes | Assists you with the integration of SQLC and csharp by generating a `.csproj` file. This converts the generated output to a .dll, a project that you can easily incorporate into your build process. |
| namespaceName        | default: the generated project name | Yes | Allows you to override the namespace name to be different than the project name |
| useDapper           | default: `false`<br/>values: `false`,`true` | Yes | Enables Dapper as a thin wrapper for the generated code. For more information, please refer to the [Dapper documentation](https://github.com/DapperLib/Dapper). |
| overrideDapperVersion| default:<br/> `2.1.35`<br/>values: The desired Dapper version | Yes | If `useDapper` is set to `true`, this option allows you to override the version of Dapper to be used. |
| Override            | values: A nested override value like [this](#override-option). | Yes | Allows you to override the generated C# data types for specific columns in specific queries. This option accepts a `query_name:column_name` mapping and the overriden data type. |                                                                                     |

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

</details>

# MySQL
<details>
<summary>:execlastid - Implementation</summary>

The implementation differs if we're using `Dapper` or not.

### Driver - MySqlConnector
The driver provides a `LastInsertedId` property to get the latest inserted id in the DB. 
When accessing the property, it automatically performs the below query: 

```sql
SELECT LAST_INSERT_ID();
```

That will work only when the id column is defined as `serial` or `bigserial`, and the generated method will always return
a `long` value.

### Dapper
Since the `LastInsertedId` is DB specific and hence not available in Dapper, the `LAST_INSERT_ID` query is simply 
appended to the original query like this:

```sql
INSERT INTO tab1 (field1, field2) VALUES ('a', 1); 
SELECT LAST_INSERT_ID();
```
The generated method will return `int` & `long` for `serial` & `bigserial` respectively.

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

# Contributing
## Local plugin development
### Prerequisites
Make sure that the following applications are installed and added to your path.

Follow the instructions in each of these:
- Dotnet CLI - [Dotnet Installation](https://github.com/dotnet/sdk) - use version `.NET 8.0 (latest)` <br/>
- Buf build - [Buf Build](https://buf.build/docs/installation) <br/>
- WASM (follow this guide) - [WASM libs](https://www.strathweb.com/2023/09/dotnet-wasi-applications-in-net-8-0/)

## Pre-commit Setup
This repository uses [pre-commit](https://pre-commit.com/). To set up pre-commit hooks, run:

```bash
pip install pre-commit
pre-commit install
```

### Protobuf
SQLC protobuf are defined in sqlc-dev/sqlc repository.
Generating C# code from protocol buffer files:
```
make protobuf-generate
```

### Generating code
SQLC utilizes our process / WASM plugin to generate code:
```
make sqlc-generate-process
make sqlc-generate-wasm
```

### Testing generated code
Testing the SQLC generated code via a predefined flow:
```
make test-process-plugin
make test-wasm-plugin
```

## Release flow
The release flow in this repo follows the semver conventions, building tag as `v[major].[minor].[patch]`.
In order to create a release you need to add `[release]` somewhere in your commit message when merging to master.

### Version bumping (built on tags)
By default, the release script will bump the patch version. Adding `[release]` to your commit message results in a new tag with `v[major].[minor].[patch]+1`. 
- Bump `minor` version by adding `[minor]` to your commit message resulting in a new tag with `v[major].[minor]+1.0` <br/>
- Bump `major` version by adding `[major]` to your commit message resulting in a new tag with `v[major]+1.0.0` <br/>

### Release structure
The new created tag will create a draft release with it, in the release there will be the wasm plugin embedded in the release. <br/>
# Examples
<details>
<summary>Npgsql</summary>

## Engine `postgresql`: [NpgsqlExample](examples/NpgsqlExample)
### [Schema](examples/config/postgresql/authors/schema.sql) | [Queries](examples/config/postgresql/authors/query.sql) | [End2End Test](end2end/EndToEndTests/NpgsqlTester.cs)
### Config
```yaml
useDapper: false
targetFramework: net8.0
generateCsproj: true
namespaceName: NpgsqlExampleGen
overrides:
- column: "GetPostgresFunctions:max_integer"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetPostgresFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetPostgresFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "GetPostgresSpecialTypesCnt:c_json"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "GetPostgresSpecialTypesCnt:c_jsonb"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_xml_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_macaddr8"
  csharp_type:
    type: "string"
    notNull: false
```

</details>
<details>
<summary>NpgsqlDapper</summary>

## Engine `postgresql`: [NpgsqlDapperExample](examples/NpgsqlDapperExample)
### [Schema](examples/config/postgresql/authors/schema.sql) | [Queries](examples/config/postgresql/authors/query.sql) | [End2End Test](end2end/EndToEndTests/NpgsqlDapperTester.cs)
### Config
```yaml
useDapper: true
targetFramework: net8.0
generateCsproj: true
namespaceName: NpgsqlDapperExampleGen
overrides:
- column: "GetPostgresFunctions:max_integer"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetPostgresFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetPostgresFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "GetPostgresSpecialTypesCnt:c_json"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "GetPostgresSpecialTypesCnt:c_jsonb"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_xml_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_macaddr8"
  csharp_type:
    type: "string"
    notNull: false
```

</details>
<details>
<summary>NpgsqlLegacy</summary>

## Engine `postgresql`: [NpgsqlLegacyExample](examples/NpgsqlLegacyExample)
### [Schema](examples/config/postgresql/authors/schema.sql) | [Queries](examples/config/postgresql/authors/query.sql) | [End2End Test](end2end/EndToEndTestsLegacy/NpgsqlTester.cs)
### Config
```yaml
useDapper: false
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: NpgsqlLegacyExampleGen
overrides:
- column: "GetPostgresFunctions:max_integer"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetPostgresFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetPostgresFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "GetPostgresSpecialTypesCnt:c_json"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "GetPostgresSpecialTypesCnt:c_jsonb"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_xml_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_macaddr8"
  csharp_type:
    type: "string"
    notNull: false
```

</details>
<details>
<summary>NpgsqlDapperLegacy</summary>

## Engine `postgresql`: [NpgsqlDapperLegacyExample](examples/NpgsqlDapperLegacyExample)
### [Schema](examples/config/postgresql/authors/schema.sql) | [Queries](examples/config/postgresql/authors/query.sql) | [End2End Test](end2end/EndToEndTestsLegacy/NpgsqlDapperTester.cs)
### Config
```yaml
useDapper: true
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: NpgsqlDapperLegacyExampleGen
overrides:
- column: "GetPostgresFunctions:max_integer"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetPostgresFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetPostgresFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "GetPostgresSpecialTypesCnt:c_json"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "GetPostgresSpecialTypesCnt:c_jsonb"
  csharp_type:
    type: "JsonElement"
    notNull: false
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_xml_string_override"
  csharp_type:
    type: "string"
    notNull: false
- column: "*:c_macaddr8"
  csharp_type:
    type: "string"
    notNull: false
```

</details>
<details>
<summary>MySqlConnector</summary>

## Engine `mysql`: [MySqlConnectorExample](examples/MySqlConnectorExample)
### [Schema](examples/config/mysql/authors/schema.sql) | [Queries](examples/config/mysql/authors/query.sql) | [End2End Test](end2end/EndToEndTests/MySqlConnectorTester.cs)
### Config
```yaml
useDapper: false
targetFramework: net8.0
generateCsproj: true
namespaceName: MySqlConnectorExampleGen
overrides:
- column: "GetMysqlFunctions:max_int"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetMysqlFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetMysqlFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
```

</details>
<details>
<summary>MySqlConnectorDapper</summary>

## Engine `mysql`: [MySqlConnectorDapperExample](examples/MySqlConnectorDapperExample)
### [Schema](examples/config/mysql/authors/schema.sql) | [Queries](examples/config/mysql/authors/query.sql) | [End2End Test](end2end/EndToEndTests/MySqlConnectorDapperTester.cs)
### Config
```yaml
useDapper: true
targetFramework: net8.0
generateCsproj: true
namespaceName: MySqlConnectorDapperExampleGen
overrides:
- column: "GetMysqlFunctions:max_int"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetMysqlFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetMysqlFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
```

</details>
<details>
<summary>MySqlConnectorLegacy</summary>

## Engine `mysql`: [MySqlConnectorLegacyExample](examples/MySqlConnectorLegacyExample)
### [Schema](examples/config/mysql/authors/schema.sql) | [Queries](examples/config/mysql/authors/query.sql) | [End2End Test](end2end/EndToEndTestsLegacy/MySqlConnectorTester.cs)
### Config
```yaml
useDapper: false
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: MySqlConnectorLegacyExampleGen
overrides:
- column: "GetMysqlFunctions:max_int"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetMysqlFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetMysqlFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
```

</details>
<details>
<summary>MySqlConnectorDapperLegacy</summary>

## Engine `mysql`: [MySqlConnectorDapperLegacyExample](examples/MySqlConnectorDapperLegacyExample)
### [Schema](examples/config/mysql/authors/schema.sql) | [Queries](examples/config/mysql/authors/query.sql) | [End2End Test](end2end/EndToEndTestsLegacy/MySqlConnectorDapperTester.cs)
### Config
```yaml
useDapper: true
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: MySqlConnectorDapperLegacyExampleGen
overrides:
- column: "GetMysqlFunctions:max_int"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetMysqlFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetMysqlFunctions:max_timestamp"
  csharp_type:
    type: "DateTime"
    notNull: true
- column: "*:c_json_string_override"
  csharp_type:
    type: "string"
    notNull: false
```

</details>
<details>
<summary>Sqlite</summary>

## Engine `sqlite`: [SqliteExample](examples/SqliteExample)
### [Schema](examples/config/sqlite/authors/schema.sql) | [Queries](examples/config/sqlite/authors/query.sql) | [End2End Test](end2end/EndToEndTests/SqliteTester.cs)
### Config
```yaml
useDapper: false
targetFramework: net8.0
generateCsproj: true
namespaceName: SqliteExampleGen
overrides:
- column: "GetSqliteFunctions:max_integer"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetSqliteFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetSqliteFunctions:max_real"
  csharp_type:
    type: "decimal"
    notNull: true
```

</details>
<details>
<summary>SqliteDapper</summary>

## Engine `sqlite`: [SqliteDapperExample](examples/SqliteDapperExample)
### [Schema](examples/config/sqlite/authors/schema.sql) | [Queries](examples/config/sqlite/authors/query.sql) | [End2End Test](end2end/EndToEndTests/SqliteDapperTester.cs)
### Config
```yaml
useDapper: true
targetFramework: net8.0
generateCsproj: true
namespaceName: SqliteDapperExampleGen
overrides:
- column: "GetSqliteFunctions:max_integer"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetSqliteFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetSqliteFunctions:max_real"
  csharp_type:
    type: "decimal"
    notNull: true
```

</details>
<details>
<summary>SqliteLegacy</summary>

## Engine `sqlite`: [SqliteLegacyExample](examples/SqliteLegacyExample)
### [Schema](examples/config/sqlite/authors/schema.sql) | [Queries](examples/config/sqlite/authors/query.sql) | [End2End Test](end2end/EndToEndTestsLegacy/SqliteTester.cs)
### Config
```yaml
useDapper: false
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: SqliteLegacyExampleGen
overrides:
- column: "GetSqliteFunctions:max_integer"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetSqliteFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetSqliteFunctions:max_real"
  csharp_type:
    type: "decimal"
    notNull: true
```

</details>
<details>
<summary>SqliteDapperLegacy</summary>

## Engine `sqlite`: [SqliteDapperLegacyExample](examples/SqliteDapperLegacyExample)
### [Schema](examples/config/sqlite/authors/schema.sql) | [Queries](examples/config/sqlite/authors/query.sql) | [End2End Test](end2end/EndToEndTestsLegacy/SqliteDapperTester.cs)
### Config
```yaml
useDapper: true
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: SqliteDapperLegacyExampleGen
overrides:
- column: "GetSqliteFunctions:max_integer"
  csharp_type:
    type: "int"
    notNull: false
- column: "GetSqliteFunctions:max_varchar"
  csharp_type:
    type: "string"
    notNull: false
- column: "GetSqliteFunctions:max_real"
  csharp_type:
    type: "decimal"
    notNull: true
```

</details>