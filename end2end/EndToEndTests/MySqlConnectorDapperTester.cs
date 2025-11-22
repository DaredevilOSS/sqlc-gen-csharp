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
        await QuerySql.DeleteAllAuthorsAsync();
        await QuerySql.TruncateExtendedBiosAsync();
        await QuerySql.TruncateMysqlNumericTypesAsync();
        await QuerySql.TruncateMysqlStringTypesAsync();
        await QuerySql.TruncateMysqlDatetimeTypesAsync();
        await QuerySql.TruncateMysqlBinaryTypesAsync();
    }
}