using MySqlConnectorLegacyExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    [TestFixture]
    public partial class MySqlConnectorTester
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