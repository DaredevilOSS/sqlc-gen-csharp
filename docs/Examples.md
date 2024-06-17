# Examples
## Engine `postgresql`: [NpgsqlExample](../NpgsqlExample)

### [Schema](../examples/authors/postgresql/schema.sql) | [Queries](../examples/authors/postgresql/query.sql) | [End2End Test](../EndToEndTests/NpgsqlTester.cs)

### Config
```yaml
driver: Npgsql
filePerQuery: false
targetFramework: net8.0
generateCsproj: true
```

## Engine `mysql`: [MySqlConnectorExample](../MySqlConnectorExample)

### [Schema](../examples/authors/mysql/schema.sql) | [Queries](../examples/authors/mysql/query.sql) | [End2End Test](../EndToEndTests/MySqlConnectorTester.cs)

### Config
```yaml
driver: MySqlConnector
filePerQuery: false
targetFramework: net8.0
generateCsproj: true
```

