using System.Collections.Generic;

namespace EndToEndScaffold;

public readonly record struct ClassGenConfig(
    string TestNamespace,
    string LegacyTestNamespace,
    HashSet<KnownTestType> TestTypes
);

public enum KnownTestType
{
    One,
    Many,
    Exec,
    ExecRows,
    ExecLastId,
    SelfJoinEmbed,
    JoinEmbed
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
                TestTypes =
                [
                    KnownTestType.One,
                    KnownTestType.Many,
                    KnownTestType.Exec,
                    KnownTestType.ExecRows,
                    KnownTestType.ExecLastId,
                    KnownTestType.JoinEmbed,
                    KnownTestType.SelfJoinEmbed
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
                        KnownTestType.ExecLastId
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
                    KnownTestType.SelfJoinEmbed
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
                    KnownTestType.ExecLastId
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
                    KnownTestType.SelfJoinEmbed
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
                    KnownTestType.ExecLastId
                ]
            }
        },
    };
}