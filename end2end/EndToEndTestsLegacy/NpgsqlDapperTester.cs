using NpgsqlDapperLegacyExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    public partial class NpgsqlDapperTester
    {
        private static readonly Random Randomizer = new Random();

        private QuerySql QuerySql { get; } = new QuerySql(
            Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv));

        [TearDown]
        public async Task EmptyTestsTable()
        {
            await QuerySql.TruncateAuthors();
            await QuerySql.TruncatePostgresTypes();
            await QuerySql.TruncateCopyToTests();
        }
    }
}