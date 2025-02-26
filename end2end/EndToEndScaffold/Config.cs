using System.Collections.Generic;

namespace EndToEndScaffold;

public readonly record struct ClassGenConfig(
    string TestNamespace,
    string LegacyTestNamespace,
    SortedSet<KnownTestType> TestTypes
);

public enum KnownTestType
{
    // query annotations aligned tests
    One,
    Many,
    Exec,
    ExecRows,
    ExecLastId,
    PostgresCopyFrom,
    MySqlCopyFrom,

    // macros aligned tests
    SelfJoinEmbed,
    JoinEmbed,
    PartialEmbed,
    Slice,
    MultipleSlices,
    NargNull,
    NargNotNull,

    // data types aligned tests
    ArrayAsParam,
    MultipleArraysAsParams,
    PostgresDataTypes,
    MySqlDataTypes,
    SqliteDataTypes,
    SqliteCopyFrom
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
                    KnownTestType.MySqlDataTypes,
                    KnownTestType.MySqlCopyFrom
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
                        KnownTestType.MySqlDataTypes,
                        KnownTestType.MySqlCopyFrom,
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
                    KnownTestType.PostgresDataTypes,
                    KnownTestType.PostgresCopyFrom
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
                    KnownTestType.PostgresDataTypes,
                    KnownTestType.PostgresCopyFrom
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
                    KnownTestType.SqliteDataTypes,
                    KnownTestType.SqliteCopyFrom
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
                    KnownTestType.SqliteDataTypes,
                    KnownTestType.SqliteCopyFrom
                ]
            }
        },
    };
}