using MySqlConnectorDapperExampleGen;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace EndToEndTests;

public partial class MySqlConnectorDapperTester
{
    private QuerySql QuerySql { get; } = new(
        Environment.GetEnvironmentVariable(EndToEndCommon.MySqlConnectionStringEnv)!);

    [TearDown]
    public async Task EmptyTestsTable()
    {
        await QuerySql.DeleteAllAuthors();
        await QuerySql.TruncateMysqlTypes();
        await QuerySql.TruncateExtendedBiographies();
    }
}