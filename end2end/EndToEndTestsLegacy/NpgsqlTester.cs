using NpgsqlLegacyExampleGen;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace EndToEndTests
{
    public partial class NpgsqlTester
    {
        private QuerySql QuerySql { get; } = new QuerySql(
            Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv));

        [TearDown]
        public async Task EmptyTestsTable()
        {
            await QuerySql.TruncateAuthors();
            await QuerySql.TruncatePostgresTypes();
            await QuerySql.TruncatePostgresStringTypes();
            await QuerySql.TruncatePostgresGeoTypes();
            await QuerySql.TruncatePostgresArrayTypes();
            await QuerySql.TruncatePostgresUnstructuredTypes();
        }
    }
}