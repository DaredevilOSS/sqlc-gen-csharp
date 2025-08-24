using NpgsqlDapperExampleGen;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace EndToEndTests;

public partial class NpgsqlDapperTester
{
    private QuerySql QuerySql { get; } = new(
        Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv)!);

    [TearDown]
    public async Task EmptyTestsTable()
    {
        await QuerySql.TruncateAuthors();
        await QuerySql.TruncatePostgresNumericTypes();
        await QuerySql.TruncatePostgresStringTypes();
        await QuerySql.TruncatePostgresDateTimeTypes();
        await QuerySql.TruncatePostgresGeoTypes();
        await QuerySql.TruncatePostgresNetworkTypes();
        await QuerySql.TruncatePostgresArrayTypes();
        await QuerySql.TruncatePostgresSpecialTypes();
    }
}