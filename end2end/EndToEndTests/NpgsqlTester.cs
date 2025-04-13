using NpgsqlExampleGen;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace EndToEndTests;

public partial class NpgsqlTester
{
    private QuerySql QuerySql { get; } = new(
        Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv)!);

    [TearDown]
    public async Task EmptyTestsTables()
    {
        await QuerySql.TruncateAuthors();
        await QuerySql.TruncatePostgresTypes();
        await QuerySql.TruncatePostgresGeoTypes();
    }
}