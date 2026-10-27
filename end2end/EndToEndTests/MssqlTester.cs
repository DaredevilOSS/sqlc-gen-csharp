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
        await QuerySql.TruncateMssqlNumericTypesAsync();
        await QuerySql.TruncateMssqlStringTypesAsync();
        await QuerySql.TruncateMssqlDatetimeTypesAsync();
        await QuerySql.TruncateMssqlBinaryTypesAsync();
    }
}