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
        await QuerySql.TruncatePostgresStringTypes();
        await QuerySql.TruncatePostgresDateTimeTypes();
        await QuerySql.TruncatePostgresGeoTypes();
        await QuerySql.TruncatePostgresNetworkTypes();
        await QuerySql.TruncatePostgresArrayTypes();
        await QuerySql.TruncatePostgresUnstructuredTypes();
    }
}