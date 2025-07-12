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
    SqliteDataTypesOverride,
    SqliteCopyFrom,
    SqliteTransaction,
    SqliteTransactionRollback,

    // Postgres
    PostgresStringDataTypes,
    PostgresIntegerDataTypes,
    PostgresFloatingPointDataTypes,
    PostgresDateTimeDataTypes,
    PostgresArrayDataTypes,
    PostgresDataTypesOverride,

    PostgresStringCopyFrom,
    PostgresTransaction,
    PostgresTransactionRollback,
    PostgresIntegerCopyFrom,
    PostgresFloatingPointCopyFrom,
    PostgresDateTimeCopyFrom,
    PostgresArrayCopyFrom,
    PostgresGeoDataTypes,

    ArrayAsParam,
    MultipleArraysAsParams,

    // MySql
    MySqlStringDataTypes,
    MySqlIntegerDataTypes,
    MySqlTransaction,
    MySqlTransactionRollback,
    MySqlFloatingPointDataTypes,
    MySqlDateTimeDataTypes,
    MySqlBinaryDataTypes,
    MySqlEnumDataType,
    MySqlDataTypesOverride,
    MySqlScopedSchemaEnum,
    MySqlJsonDataTypes,
    MySqlJsonCopyFrom,

    MySqlStringCopyFrom,
    MySqlIntegerCopyFrom,
    MySqlFloatingPointCopyFrom,
    MySqlDateTimeCopyFrom,
    MySqlBinaryCopyFrom,
    MySqlEnumCopyFrom
}

internal static class Config
{
    public static Dictionary<string, ClassGenConfig> FilesToGenerate { get; } =
    new()
    {
        {
            "MySqlConnectorTester", new ClassGenConfig
            {
                TestNamespace = "MySqlConnectorExampleGen",
                LegacyTestNamespace = "MySqlConnectorLegacyExampleGen",
                TestTypes = [
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

                    KnownTestType.MySqlStringDataTypes,
                    KnownTestType.MySqlIntegerDataTypes,
                    KnownTestType.MySqlTransaction,
                    KnownTestType.MySqlTransactionRollback,
                    KnownTestType.MySqlFloatingPointDataTypes,
                    KnownTestType.MySqlDateTimeDataTypes,
                    KnownTestType.MySqlBinaryDataTypes,
                    KnownTestType.MySqlEnumDataType,
                    KnownTestType.MySqlScopedSchemaEnum,
                    KnownTestType.MySqlJsonDataTypes,
                    KnownTestType.MySqlJsonCopyFrom,
                    KnownTestType.MySqlDataTypesOverride,

                    KnownTestType.MySqlStringCopyFrom,
                    KnownTestType.MySqlIntegerCopyFrom,
                    KnownTestType.MySqlFloatingPointCopyFrom,
                    KnownTestType.MySqlDateTimeCopyFrom,
                    KnownTestType.MySqlBinaryCopyFrom,
                    KnownTestType.MySqlEnumCopyFrom
                ]
            }
        },
        {
            "MySqlConnectorDapperTester", new ClassGenConfig
                {
                    TestNamespace = "MySqlConnectorDapperExampleGen",
                    LegacyTestNamespace = "MySqlConnectorDapperLegacyExampleGen",
                    TestTypes = [
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

                        KnownTestType.MySqlStringDataTypes,
                        KnownTestType.MySqlIntegerDataTypes,
                        KnownTestType.MySqlTransaction,
                        KnownTestType.MySqlTransactionRollback,
                        KnownTestType.MySqlFloatingPointDataTypes,
                        KnownTestType.MySqlDateTimeDataTypes,
                        KnownTestType.MySqlBinaryDataTypes,
                        KnownTestType.MySqlEnumDataType,
                        KnownTestType.MySqlScopedSchemaEnum,
                        KnownTestType.MySqlJsonDataTypes,
                        KnownTestType.MySqlJsonCopyFrom,
                        KnownTestType.MySqlDataTypesOverride,

                        KnownTestType.MySqlStringCopyFrom,
                        KnownTestType.MySqlIntegerCopyFrom,
                        KnownTestType.MySqlFloatingPointCopyFrom,
                        KnownTestType.MySqlDateTimeCopyFrom,
                        KnownTestType.MySqlBinaryCopyFrom,
                        KnownTestType.MySqlEnumCopyFrom
                    ]
                }
        },
        {
            "NpgsqlTester", new ClassGenConfig
            {
                TestNamespace = "NpgsqlExampleGen",
                LegacyTestNamespace = "NpgsqlLegacyExampleGen",
                TestTypes = [
                    KnownTestType.One,
                    KnownTestType.Many,
                    KnownTestType.Exec,
                    KnownTestType.ExecRows,
                    KnownTestType.ExecLastId,
                    KnownTestType.JoinEmbed,
                    KnownTestType.SelfJoinEmbed,
                    KnownTestType.ArrayAsParam,
                    KnownTestType.MultipleArraysAsParams,
                    KnownTestType.NargNull,
                    KnownTestType.NargNotNull,

                    KnownTestType.PostgresTransaction,
                    KnownTestType.PostgresTransactionRollback,
                    KnownTestType.PostgresStringDataTypes,
                    KnownTestType.PostgresIntegerDataTypes,
                    KnownTestType.PostgresFloatingPointDataTypes,
                    KnownTestType.PostgresDateTimeDataTypes,
                    KnownTestType.PostgresArrayDataTypes,
                    KnownTestType.PostgresGeoDataTypes,
                    KnownTestType.PostgresDataTypesOverride,

                    KnownTestType.PostgresStringCopyFrom,
                    KnownTestType.PostgresIntegerCopyFrom,
                    KnownTestType.PostgresFloatingPointCopyFrom,
                    KnownTestType.PostgresDateTimeCopyFrom,
                    KnownTestType.PostgresArrayCopyFrom,
                ]
            }
        },
        {
            "NpgsqlDapperTester", new ClassGenConfig
            {
                TestNamespace = "NpgsqlDapperExampleGen",
                LegacyTestNamespace = "NpgsqlDapperLegacyExampleGen",
                TestTypes = [
                    KnownTestType.One,
                    KnownTestType.Many,
                    KnownTestType.Exec,
                    KnownTestType.ExecRows,
                    KnownTestType.ExecLastId,
                    KnownTestType.JoinEmbed,
                    KnownTestType.SelfJoinEmbed,
                    KnownTestType.ArrayAsParam,
                    KnownTestType.MultipleArraysAsParams,
                    KnownTestType.NargNull,
                    KnownTestType.NargNotNull,
                    KnownTestType.PostgresTransaction,
                    KnownTestType.PostgresTransactionRollback,

                    KnownTestType.PostgresStringDataTypes,
                    KnownTestType.PostgresIntegerDataTypes,
                    KnownTestType.PostgresFloatingPointDataTypes,
                    KnownTestType.PostgresDateTimeDataTypes,
                    KnownTestType.PostgresArrayDataTypes,
                    KnownTestType.PostgresGeoDataTypes,
                    KnownTestType.PostgresDataTypesOverride,

                    KnownTestType.PostgresStringCopyFrom,
                    KnownTestType.PostgresIntegerCopyFrom,
                    KnownTestType.PostgresFloatingPointCopyFrom,
                    KnownTestType.PostgresDateTimeCopyFrom,
                    KnownTestType.PostgresArrayCopyFrom,
                ]
            }
        },
        {
            "SqliteTester", new ClassGenConfig
            {
                TestNamespace = "SqliteExampleGen",
                LegacyTestNamespace = "SqliteLegacyExampleGen",
                TestTypes = [
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
                    KnownTestType.SqliteDataTypesOverride
                ]
            }
        },
        {
            "SqliteDapperTester", new ClassGenConfig
            {
                TestNamespace = "SqliteDapperExampleGen",
                LegacyTestNamespace = "SqliteDapperLegacyExampleGen",
                TestTypes = [
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
                    KnownTestType.SqliteDataTypesOverride
                ]
            }
        },
    };
}