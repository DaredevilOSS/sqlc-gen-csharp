using MySqlConnectorDapperLegacyExampleGen;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    public partial class MySqlConnectorDapperTester
    {

        private static readonly Random Randomizer = new Random();

        private QuerySql QuerySql { get; } = new QuerySql(
            Environment.GetEnvironmentVariable(EndToEndCommon.MySqlConnectionStringEnv));

        [TearDown]
        public async Task EmptyTestsTable()
        {
            await QuerySql.TruncateBooks();
            await QuerySql.DeleteAllAuthors();
            await QuerySql.TruncateCopyToTests();
        }
    }
}