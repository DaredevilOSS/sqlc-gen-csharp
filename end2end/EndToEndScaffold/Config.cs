using System.Collections.Generic;

namespace EndToEndScaffold;

public readonly record struct ClassGenConfig(
    string TestNamespace,
    string LegacyTestNamespace,
    SortedSet<KnownTestType> TestTypes
);

public enum KnownTestType
{
    // query annotations
    One,
    Many,
    Exec,
    ExecRows,
    ExecLastId,

    // macros
    SelfJoinEmbed,
    JoinEmbed,
    PartialEmbed,
    Slice,
    MultipleSlices,
    NargNull,
    NargNotNull,

    // Sqlite
    SqliteDataTypes,
    SqliteMultipleNamedParam,
    SqliteDataTypesOverride,
    SqliteCopyFrom,
    SqliteTransaction,
    SqliteTransactionRollback,

    // Postgres
    PostgresTransaction,
    PostgresTransactionRollback,
    ArrayAsParam,
    MultipleArraysAsParams,
    PostgresDataTypesOverride,
    PostgresInvalidJson,
    PostgresInvalidXml,

    // Data types
    PostgresStringDataTypes,
    PostgresIntegerDataTypes,
    PostgresFloatingPointDataTypes,
    PostgresDateTimeDataTypes,
    PostgresArrayDataTypes,
    PostgresGuidDataTypes,
    PostgresFullTextSearchDataTypes,
    PostgresNetworkDataTypes,
    PostgresGeoDataTypes,
    PostgresJsonDataTypes,
    PostgresXmlDataTypes,
    PostgresEnumDataType,

    // :copyfrom (Batch)
    PostgresStringCopyFrom,
    PostgresIntegerCopyFrom,
    PostgresFloatingPointCopyFrom,
    PostgresDateTimeCopyFrom,
    PostgresGuidCopyFrom,
    PostgresNetworkCopyFrom,
    PostgresArrayCopyFrom,
    PostgresGeoCopyFrom,

    // MySql
    MySqlTransaction,
    MySqlTransactionRollback,
    MySqlDataTypesOverride,
    MySqlScopedSchemaEnum,
    MySqlInvalidJson,

    // Data types
    MySqlStringDataTypes,
    MySqlIntegerDataTypes,
    MySqlFloatingPointDataTypes,
    MySqlDateTimeDataTypes,
    MySqlBinaryDataTypes,
    MySqlEnumDataType,
    MySqlJsonDataTypes,

    // :copyfrom (Batch)
    MySqlStringCopyFrom,
    MySqlIntegerCopyFrom,
    MySqlFloatingPointCopyFrom,
    MySqlDateTimeCopyFrom,
    MySqlBinaryCopyFrom,
    MySqlEnumCopyFrom,
    MySqlJsonCopyFrom
}

internal static class Config
{
    private static readonly SortedSet<KnownTestType> _mysqlTestTypes = [
        KnownTestType.MySqlTransaction,
        KnownTestType.MySqlTransactionRollback,
        KnownTestType.MySqlDataTypesOverride,
        KnownTestType.MySqlScopedSchemaEnum,
        KnownTestType.MySqlInvalidJson,

        // query annotations
        KnownTestType.One,
        KnownTestType.Many,
        KnownTestType.Exec,
        KnownTestType.ExecRows,
        KnownTestType.ExecLastId,

        // macros
        KnownTestType.JoinEmbed,
        KnownTestType.SelfJoinEmbed,
        KnownTestType.Slice,
        KnownTestType.MultipleSlices,
        KnownTestType.NargNull,
        KnownTestType.NargNotNull,

        // Data types
        KnownTestType.MySqlStringDataTypes,
        KnownTestType.MySqlIntegerDataTypes,
        KnownTestType.MySqlFloatingPointDataTypes,
        KnownTestType.MySqlDateTimeDataTypes,
        KnownTestType.MySqlBinaryDataTypes,
        KnownTestType.MySqlEnumDataType,
        KnownTestType.MySqlJsonDataTypes,

        // :copyfrom (Batch)
        KnownTestType.MySqlJsonCopyFrom,
        KnownTestType.MySqlStringCopyFrom,
        KnownTestType.MySqlIntegerCopyFrom,
        KnownTestType.MySqlFloatingPointCopyFrom,
        KnownTestType.MySqlDateTimeCopyFrom,
        KnownTestType.MySqlBinaryCopyFrom,
        KnownTestType.MySqlEnumCopyFrom
    ];

    private static readonly SortedSet<KnownTestType> _postgresTestTypes = [
        KnownTestType.PostgresTransaction,
        KnownTestType.PostgresTransactionRollback,
        KnownTestType.ArrayAsParam,
        KnownTestType.MultipleArraysAsParams,
        KnownTestType.PostgresInvalidJson,
        KnownTestType.PostgresInvalidXml,

        // query annotations
        KnownTestType.One,
        KnownTestType.Many,
        KnownTestType.Exec,
        KnownTestType.ExecRows,
        KnownTestType.ExecLastId,

        // macros
        KnownTestType.JoinEmbed,
        KnownTestType.SelfJoinEmbed,
        KnownTestType.NargNull,
        KnownTestType.NargNotNull,

        // Data types
        KnownTestType.PostgresStringDataTypes,
        KnownTestType.PostgresIntegerDataTypes,
        KnownTestType.PostgresFloatingPointDataTypes,
        KnownTestType.PostgresDateTimeDataTypes,
        KnownTestType.PostgresGuidDataTypes,
        KnownTestType.PostgresArrayDataTypes,
        KnownTestType.PostgresGeoDataTypes,
        KnownTestType.PostgresDataTypesOverride,
        KnownTestType.PostgresJsonDataTypes,
        KnownTestType.PostgresNetworkDataTypes,
        KnownTestType.PostgresXmlDataTypes,
        KnownTestType.PostgresEnumDataType,
        KnownTestType.PostgresFullTextSearchDataTypes,

        // :copyfrom (Batch)
        KnownTestType.PostgresGeoCopyFrom,
        KnownTestType.PostgresStringCopyFrom,
        KnownTestType.PostgresIntegerCopyFrom,
        KnownTestType.PostgresFloatingPointCopyFrom,
        KnownTestType.PostgresDateTimeCopyFrom,
        KnownTestType.PostgresGuidCopyFrom,
        KnownTestType.PostgresArrayCopyFrom,
        KnownTestType.PostgresNetworkCopyFrom
    ];

    private static readonly SortedSet<KnownTestType> _sqliteTestTypes = [
        KnownTestType.One,
        KnownTestType.Many,
        KnownTestType.Exec,
        KnownTestType.ExecRows,
        KnownTestType.ExecLastId,
        KnownTestType.JoinEmbed,
        KnownTestType.SelfJoinEmbed,
        KnownTestType.Slice,
        KnownTestType.MultipleSlices,
        KnownTestType.NargNull,
        KnownTestType.NargNotNull,
        KnownTestType.SqliteTransaction,
        KnownTestType.SqliteTransactionRollback,
        KnownTestType.SqliteDataTypes,
        KnownTestType.SqliteCopyFrom,
        KnownTestType.SqliteDataTypesOverride,
        KnownTestType.SqliteMultipleNamedParam
    ];

    public static Dictionary<string, ClassGenConfig> FilesToGenerate { get; } =
    new()
    {
        {
            "MySqlConnectorTester", new ClassGenConfig
            {
                TestNamespace = "MySqlConnectorExampleGen",
                LegacyTestNamespace = "MySqlConnectorLegacyExampleGen",
                TestTypes = _mysqlTestTypes
            }
        },
        {
            "MySqlConnectorDapperTester", new ClassGenConfig
            {
                TestNamespace = "MySqlConnectorDapperExampleGen",
                LegacyTestNamespace = "MySqlConnectorDapperLegacyExampleGen",
                TestTypes = _mysqlTestTypes
            }
        },
        {
            "NpgsqlTester", new ClassGenConfig
            {
                TestNamespace = "NpgsqlExampleGen",
                LegacyTestNamespace = "NpgsqlLegacyExampleGen",
                TestTypes = _postgresTestTypes
            }
        },
        {
            "NpgsqlDapperTester", new ClassGenConfig
            {
                TestNamespace = "NpgsqlDapperExampleGen",
                LegacyTestNamespace = "NpgsqlDapperLegacyExampleGen",
                TestTypes = _postgresTestTypes
            }
        },
        {
            "SqliteTester", new ClassGenConfig
            {
                TestNamespace = "SqliteExampleGen",
                LegacyTestNamespace = "SqliteLegacyExampleGen",
                TestTypes = _sqliteTestTypes
            }
        },
        {
            "SqliteDapperTester", new ClassGenConfig
            {
                TestNamespace = "SqliteDapperExampleGen",
                LegacyTestNamespace = "SqliteDapperLegacyExampleGen",
                TestTypes = _sqliteTestTypes
            }
        },
    };
}