using NUnit.Framework;
using SqliteExampleGen;
using System;
using System.Threading.Tasks;

namespace EndToEndTests;

public partial class SqliteTester
{
    private QuerySql QuerySql { get; } = new(
        Environment.GetEnvironmentVariable(EndToEndCommon.SqliteConnectionStringEnv)!);

    [TearDown]
    public async Task EmptyTestsTable()
    {
        await QuerySql.DeleteAllAuthorsAsync();
        await QuerySql.DeleteAllSqliteTypesAsync();
    }
}