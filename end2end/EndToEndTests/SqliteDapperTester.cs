using NUnit.Framework;
using NUnit.Framework.Legacy;
using SqliteDapperExampleGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public partial class SqliteDapperTester
{
    private QuerySql QuerySql { get; } = new(
        Environment.GetEnvironmentVariable(EndToEndCommon.SqliteConnectionStringEnv)!);

    [TearDown]
    public async Task EmptyTestsTable()
    {
        await QuerySql.DeleteAllAuthors();
    }
}