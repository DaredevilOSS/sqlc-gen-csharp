using NpgsqlDapperLegacyExampleGen;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    public partial class NpgsqlDapperTester
    {
        private QuerySql QuerySql { get; } = new QuerySql(
            Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv));

        [TearDown]
        public async Task EmptyTestsTable()
        {
            await QuerySql.TruncateAuthors();
            await QuerySql.TruncatePostgresTypes();
            await QuerySql.TruncatePostgresGeoTypes();
        }
    }
}