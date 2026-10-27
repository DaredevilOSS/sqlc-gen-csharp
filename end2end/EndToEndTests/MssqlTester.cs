using MssqlExampleGen;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace EndToEndTests;

public partial class MssqlTester
{
    private QuerySql QuerySql { get; } = new(
        Environment.GetEnvironmentVariable(EndToEndCommon.MssqlConnectionStringEnv)!);

    [TearDown]
    public async Task EmptyTestsTables()
    {
        await QuerySql.DeleteAllAuthorsAsync();
    }
}
