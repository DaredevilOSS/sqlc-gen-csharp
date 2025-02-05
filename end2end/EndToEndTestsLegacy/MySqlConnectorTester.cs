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
        }

        [Test]
        public async Task TestCopyFrom()
        {
            const int batchSize = 100;
            var createAuthorBatchArgs = Enumerable.Range(0, batchSize)
                .Select(_ => GenerateRandom())
                .ToList();
            await QuerySql.CopyToTests(createAuthorBatchArgs);
            var countRows = QuerySql.CountCopyRows().Result.Cnt;
            ClassicAssert.AreEqual(batchSize, countRows);
            return;

            QuerySql.CopyToTestsArgs GenerateRandom()
            {
                return new QuerySql.CopyToTestsArgs
                {
                    CVarchar = Randomizer.Next().ToString(),
                    CInt = Randomizer.Next(),
                    CDate = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(Randomizer.Next())),
                    CTimestamp = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(Randomizer.Next()))
                };
            }
        }
    }
}