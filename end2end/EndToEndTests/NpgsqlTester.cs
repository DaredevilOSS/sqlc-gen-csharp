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
        await QuerySql.TruncateAuthorsAsync();
        await QuerySql.TruncatePostgresNumericTypesAsync();
        await QuerySql.TruncatePostgresStringTypesAsync();
        await QuerySql.TruncatePostgresDateTimeTypesAsync();
        await QuerySql.TruncatePostgresGeoTypesAsync();
        await QuerySql.TruncatePostgresNetworkTypesAsync();
        await QuerySql.TruncatePostgresArrayTypesAsync();
        await QuerySql.TruncatePostgresSpecialTypesAsync();
        await QuerySql.TruncatePostgresNotNullTypesAsync();
    }
}