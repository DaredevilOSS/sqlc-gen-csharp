# Examples
## Engine `postgresql`: [NpgsqlExample](../NpgsqlExample)

### [Schema](../examples/authors/postgresql/schema.sql) | [Queries](../examples/authors/postgresql/query.sql) | [End2End Test](../EndToEndTests/NpgsqlTester.cs)

### Config
```yaml
driver: Npgsql
targetFramework: net8.0
generateCsproj: true
namespaceName: NpgsqlExampleGen
```

## Engine `postgresql`: [NpgsqlDapperExample](../NpgsqlDapperExample)

### [Schema](../examples/authors/postgresql/schema.sql) | [Queries](../examples/authors/postgresql/query.sql) | [End2End Test](../EndToEndTests/NpgsqlDapperTester.cs)

### Config
```yaml
driver: Npgsql
useDapper: true
targetFramework: net8.0
generateCsproj: true
namespaceName: NpgsqlDapperExampleGen
```

## Engine `mysql`: [MySqlConnectorExample](../MySqlConnectorExample)

### [Schema](../examples/authors/mysql/schema.sql) | [Queries](../examples/authors/mysql/query.sql) | [End2End Test](../EndToEndTests/MySqlConnectorTester.cs)

### Config
```yaml
driver: MySqlConnector
targetFramework: net8.0
generateCsproj: true
namespaceName: MySqlConnectorExampleGen
```

## Engine `mysql`: [MySqlConnectorDapperExample](../MySqlConnectorDapperExample)

### [Schema](../examples/authors/mysql/schema.sql) | [Queries](../examples/authors/mysql/query.sql) | [End2End Test](../EndToEndTests/MySqlConnectorDapperTester.cs)

### Config
```yaml
driver: MySqlConnector
useDapper: true
targetFramework: net8.0
generateCsproj: true
namespaceName: MySqlConnectorDapperExampleGen
```

## Engine `sqlite`: [SqliteExample](../SqliteExample)

### [Schema](../examples/authors/sqlite/schema.sql) | [Queries](../examples/authors/sqlite/query.sql) | [End2End Test](../EndToEndTests/SqliteTester.cs)

### Config
```yaml
driver: Sqlite
targetFramework: net8.0
generateCsproj: true
namespaceName: SqliteExampleGen
```

## Engine `sqlite`: [SqliteDapperExample](../SqliteDapperExample)

### [Schema](../examples/authors/sqlite/schema.sql) | [Queries](../examples/authors/sqlite/query.sql) | [End2End Test](../EndToEndTests/SqliteDapperTester.cs)

### Config
```yaml
driver: Sqlite
useDapper: true
targetFramework: net8.0
generateCsproj: true
namespaceName: SqliteDapperExampleGen
```

