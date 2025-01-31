# Examples
## Engine `postgresql`: [NpgsqlExample](examples/NpgsqlExample)

### [Schema](examples/config/postgresql/schema.sql) | [Queries](examples/config/postgresql/query.sql) | [End2End Test](EndToEndTests/NpgsqlTester.cs)

### Config
```yaml
useDapper: false
targetFramework: net8.0
generateCsproj: true
namespaceName: NpgsqlExampleGen
```
## Engine `postgresql`: [NpgsqlDapperExample](examples/NpgsqlDapperExample)

### [Schema](examples/config/postgresql/schema.sql) | [Queries](examples/config/postgresql/query.sql) | [End2End Test](EndToEndTests/NpgsqlDapperTester.cs)

### Config
```yaml
useDapper: true
targetFramework: net8.0
generateCsproj: true
namespaceName: NpgsqlDapperExampleGen
```
## Engine `postgresql`: [NpgsqlLegacyExample](examples/NpgsqlLegacyExample)

### [Schema](examples/config/postgresql/schema.sql) | [Queries](examples/config/postgresql/query.sql) | [End2End Test](LegacyEndToEndTests/NpgsqlTester.cs)

### Config
```yaml
useDapper: false
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: NpgsqlLegacyExampleGen
```
## Engine `postgresql`: [NpgsqlDapperLegacyExample](examples/NpgsqlDapperLegacyExample)

### [Schema](examples/config/postgresql/schema.sql) | [Queries](examples/config/postgresql/query.sql) | [End2End Test](LegacyEndToEndTests/NpgsqlDapperTester.cs)

### Config
```yaml
useDapper: true
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: NpgsqlDapperLegacyExampleGen
```
## Engine `mysql`: [MySqlConnectorExample](examples/MySqlConnectorExample)

### [Schema](examples/config/mysql/schema.sql) | [Queries](examples/config/mysql/query.sql) | [End2End Test](EndToEndTests/MySqlConnectorTester.cs)

### Config
```yaml
useDapper: false
targetFramework: net8.0
generateCsproj: true
namespaceName: MySqlConnectorExampleGen
```
## Engine `mysql`: [MySqlConnectorDapperExample](examples/MySqlConnectorDapperExample)

### [Schema](examples/config/mysql/schema.sql) | [Queries](examples/config/mysql/query.sql) | [End2End Test](EndToEndTests/MySqlConnectorDapperTester.cs)

### Config
```yaml
useDapper: true
targetFramework: net8.0
generateCsproj: true
namespaceName: MySqlConnectorDapperExampleGen
```
## Engine `mysql`: [MySqlConnectorLegacyExample](examples/MySqlConnectorLegacyExample)

### [Schema](examples/config/mysql/schema.sql) | [Queries](examples/config/mysql/query.sql) | [End2End Test](LegacyEndToEndTests/MySqlConnectorTester.cs)

### Config
```yaml
useDapper: false
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: MySqlConnectorLegacyExampleGen
```
## Engine `mysql`: [MySqlConnectorDapperLegacyExample](examples/MySqlConnectorDapperLegacyExample)

### [Schema](examples/config/mysql/schema.sql) | [Queries](examples/config/mysql/query.sql) | [End2End Test](LegacyEndToEndTests/MySqlConnectorDapperTester.cs)

### Config
```yaml
useDapper: true
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: MySqlConnectorDapperLegacyExampleGen
```
## Engine `sqlite`: [SqliteExample](examples/SqliteExample)

### [Schema](examples/config/sqlite/schema.sql) | [Queries](examples/config/sqlite/query.sql) | [End2End Test](EndToEndTests/SqliteTester.cs)

### Config
```yaml
useDapper: false
targetFramework: net8.0
generateCsproj: true
namespaceName: SqliteExampleGen
```
## Engine `sqlite`: [SqliteDapperExample](examples/SqliteDapperExample)

### [Schema](examples/config/sqlite/schema.sql) | [Queries](examples/config/sqlite/query.sql) | [End2End Test](EndToEndTests/SqliteDapperTester.cs)

### Config
```yaml
useDapper: true
targetFramework: net8.0
generateCsproj: true
namespaceName: SqliteDapperExampleGen
```
## Engine `sqlite`: [SqliteLegacyExample](examples/SqliteLegacyExample)

### [Schema](examples/config/sqlite/schema.sql) | [Queries](examples/config/sqlite/query.sql) | [End2End Test](LegacyEndToEndTests/SqliteTester.cs)

### Config
```yaml
useDapper: false
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: SqliteLegacyExampleGen
```
## Engine `sqlite`: [SqliteDapperLegacyExample](examples/SqliteDapperLegacyExample)

### [Schema](examples/config/sqlite/schema.sql) | [Queries](examples/config/sqlite/query.sql) | [End2End Test](LegacyEndToEndTests/SqliteDapperTester.cs)

### Config
```yaml
useDapper: true
targetFramework: netstandard2.0
generateCsproj: true
namespaceName: SqliteDapperLegacyExampleGen
```