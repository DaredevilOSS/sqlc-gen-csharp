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
                TestTypes = [KnownTestType.One, KnownTestType.Many, KnownTestType.Exec, KnownTestType.ExecRows]
            }
        },
        {
            "MySqlConnectorDapperTester", new ClassGenConfig
                {
                    TestNamespace = "MySqlConnectorDapperExampleGen",
                    LegacyTestNamespace = "MySqlConnectorDapperLegacyExampleGen",
                    TestTypes = [KnownTestType.One, KnownTestType.Many, KnownTestType.Exec, KnownTestType.ExecRows]
                }
        },
        {
            "NpgsqlTester", new ClassGenConfig
            {
                TestNamespace = "NpgsqlExampleGen",
                LegacyTestNamespace = "NpgsqlLegacyExampleGen",
                TestTypes = [KnownTestType.One, KnownTestType.Many, KnownTestType.Exec, KnownTestType.ExecRows]
            }
        },
        {
            "NpgsqlDapperTester", new ClassGenConfig
            {
                TestNamespace = "NpgsqlDapperExampleGen",
                LegacyTestNamespace = "NpgsqlDapperLegacyExampleGen",
                TestTypes = [KnownTestType.One, KnownTestType.Many, KnownTestType.Exec, KnownTestType.ExecRows]
            }
        },
        {
            "SqliteTester", new ClassGenConfig
            {
                TestNamespace = "SqliteExampleGen",
                LegacyTestNamespace = "SqliteLegacyExampleGen",
                TestTypes = [KnownTestType.One, KnownTestType.Many, KnownTestType.Exec, KnownTestType.ExecRows]
            }
        },
        {
            "SqliteDapperTester", new ClassGenConfig
            {
                TestNamespace = "SqliteDapperExampleGen",
                LegacyTestNamespace = "SqliteDapperLegacyExampleGen",
                TestTypes = [KnownTestType.One, KnownTestType.Many, KnownTestType.Exec, KnownTestType.ExecRows]
            }
        },
    };
}