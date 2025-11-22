using NpgsqlDapperLegacyExampleGen;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace EndToEndTests
{
    public partial class NpgsqlDapperTester
    {
        private QuerySql QuerySql { get; } = new QuerySql(
            Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv));

        [TearDown]
        public async Task EmptyTestsTable()
        {
            await QuerySql.TruncateAuthorsAsync();
            await QuerySql.TruncatePostgresNumericTypesAsync();
            await QuerySql.TruncatePostgresStringTypesAsync();
            await QuerySql.TruncatePostgresDateTimeTypesAsync();
            await QuerySql.TruncatePostgresGeoTypesAsync();
            await QuerySql.TruncatePostgresNetworkTypesAsync();
            await QuerySql.TruncatePostgresArrayTypesAsync();
            await QuerySql.TruncatePostgresSpecialTypesAsync();
        }
    }
}