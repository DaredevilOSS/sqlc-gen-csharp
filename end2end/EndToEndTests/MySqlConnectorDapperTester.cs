using MySqlConnectorDapperExampleGen;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public partial class MySqlConnectorDapperTester
{
    private QuerySql QuerySql { get; } = new(
        Environment.GetEnvironmentVariable(EndToEndCommon.MySqlConnectionStringEnv)!);

    [TearDown]
    public async Task EmptyTestsTable()
    {
        await QuerySql.TruncateAuthors();
    }
}