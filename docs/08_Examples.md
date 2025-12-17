# Examples
<details>
<summary>QuickStartPostgresDalGen</summary>

## Engine `postgresql`: [QuickStartPostgresDalGen](examples/QuickStartPostgresDalGen)
### [Schema](examples/config/postgresql/authors/schema.sql) | [Queries](examples/config/postgresql/authors/query.sql) | [End2End Test](end2end/EndToEndTests/QuickStartPostgresDalGen.cs)
### Config
```yaml
```

</details>
<details>
<summary>QuickStartMySqlDalGen</summary>

## Engine `mysql`: [QuickStartMySqlDalGen](examples/QuickStartMySqlDalGen)
### [Schema](examples/config/mysql/authors/schema.sql) | [Queries](examples/config/mysql/authors/query.sql) | [End2End Test](end2end/EndToEndTests/QuickStartMySqlDalGen.cs)
### Config
```yaml
```

</details>
<details>
<summary>QuickStartSqliteDalGen</summary>

## Engine `sqlite`: [QuickStartSqliteDalGen](examples/QuickStartSqliteDalGen)
### [Schema](examples/config/sqlite/authors/schema.sql) | [Queries](examples/config/sqlite/authors/query.sql) | [End2End Test](end2end/EndToEndTests/QuickStartSqliteDalGen.cs)
### Config
```yaml
```

</details>
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
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
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
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
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
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
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
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
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
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
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
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
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
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
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
- column: "*:c_timestamp_noda_instant_override"
  csharp_type:
    type: "Instant"
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
- column: "GetSqliteFunctions:max_varchar"
  csharp_type:
    type: "string"
- column: "GetSqliteFunctions:max_real"
  csharp_type:
    type: "decimal"
- column: "*:c_text_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_integer_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_text_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_integer_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_text_noda_instant_override"
  csharp_type:
    type: "Instant"
- column: "*:c_integer_noda_instant_override"
  csharp_type:
    type: "Instant"
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
- column: "GetSqliteFunctions:max_varchar"
  csharp_type:
    type: "string"
- column: "GetSqliteFunctions:max_real"
  csharp_type:
    type: "decimal"
- column: "*:c_text_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_integer_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_text_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_integer_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_text_noda_instant_override"
  csharp_type:
    type: "Instant"
- column: "*:c_integer_noda_instant_override"
  csharp_type:
    type: "Instant"
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
- column: "GetSqliteFunctions:max_varchar"
  csharp_type:
    type: "string"
- column: "GetSqliteFunctions:max_real"
  csharp_type:
    type: "decimal"
- column: "*:c_text_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_integer_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_text_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_integer_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_text_noda_instant_override"
  csharp_type:
    type: "Instant"
- column: "*:c_integer_noda_instant_override"
  csharp_type:
    type: "Instant"
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
- column: "GetSqliteFunctions:max_varchar"
  csharp_type:
    type: "string"
- column: "GetSqliteFunctions:max_real"
  csharp_type:
    type: "decimal"
- column: "*:c_text_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_integer_datetime_override"
  csharp_type:
    type: "DateTime"
- column: "*:c_text_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_integer_bool_override"
  csharp_type:
    type: "bool"
- column: "*:c_text_noda_instant_override"
  csharp_type:
    type: "Instant"
- column: "*:c_integer_noda_instant_override"
  csharp_type:
    type: "Instant"
```

</details>
<details>
<summary>benchmark/PostgresqlSqlcImpl</summary>

## Engine `postgresql`: [benchmark/PostgresqlSqlcImpl](benchmark/PostgresqlSqlcImpl)
### [Schema](examples/config/postgresql/benchmark/schema.sql) | [Queries](examples/config/postgresql/benchmark/query.sql) | [End2End Test](end2end/EndToEndTests/benchmark/PostgresqlSqlcImpl.cs)
### Config
```yaml
namespaceName: PostgresSqlcImpl
```

</details>
<details>
<summary>benchmark/SqliteSqlcImpl</summary>

## Engine `sqlite`: [benchmark/SqliteSqlcImpl](benchmark/SqliteSqlcImpl)
### [Schema](examples/config/sqlite/benchmark/schema.sql) | [Queries](examples/config/sqlite/benchmark/query.sql) | [End2End Test](end2end/EndToEndTests/benchmark/SqliteSqlcImpl.cs)
### Config
```yaml
namespaceName: SqliteSqlcImpl
```

</details>
<details>
<summary>benchmark/MysqlSqlcImpl</summary>

## Engine `mysql`: [benchmark/MysqlSqlcImpl](benchmark/MysqlSqlcImpl)
### [Schema](examples/config/mysql/benchmark/schema.sql) | [Queries](examples/config/mysql/benchmark/query.sql) | [End2End Test](end2end/EndToEndTests/benchmark/MysqlSqlcImpl.cs)
### Config
```yaml
namespaceName: MysqlSqlcImpl
```

</details>