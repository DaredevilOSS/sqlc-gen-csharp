using MySqlConnectorDapperExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public partial class MySqlConnectorDapperTester
{
    private QuerySql QuerySql { get; } = new(
        Environment.GetEnvironmentVariable(EndToEndCommon.MySqlConnectionStringEnv)!);

    [TearDown]
    public async Task EmptyTestsTable()
    {
        await QuerySql.DeleteAllAuthors();
        await QuerySql.TruncateMysqlTypes();
    }
}