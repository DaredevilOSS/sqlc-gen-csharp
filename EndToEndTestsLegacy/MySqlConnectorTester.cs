using MySqlConnectorLegacyExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    [TestFixture]
    public partial class MySqlConnectorTester : IExecLastIdTester, ICopyFromTester
    {
        private static readonly Random Randomizer = new Random();

        private QuerySql QuerySql { get; } = new QuerySql(
            Environment.GetEnvironmentVariable(EndToEndCommon.MySqlConnectionStringEnv));

        [TearDown]
        public async Task EmptyTestsTable()
        {
            await QuerySql.TruncateAuthors();
        }

        [Test]
        public async Task TestExecLastId()
        {
            var bojackCreateAuthorArgs = new QuerySql.CreateAuthorReturnIdArgs
            {
                Name = DataGenerator.BojackAuthor,
                Bio = DataGenerator.BojackTheme
            };
            var insertedId = await QuerySql.CreateAuthorReturnId(bojackCreateAuthorArgs);

            var expected = new QuerySql.GetAuthorByIdRow
            {
                Id = insertedId,
                Name = DataGenerator.BojackAuthor,
                Bio = DataGenerator.BojackTheme
            };
            var actual = await QuerySql.GetAuthorById(new QuerySql.GetAuthorByIdArgs
            {
                Id = insertedId
            });
            ClassicAssert.IsNotNull(actual);
            Assert.That(Equals(expected, actual));
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

        private static bool Equals(QuerySql.GetAuthorByIdRow x, QuerySql.GetAuthorByIdRow y)
        {
            return x.Id.Equals(y.Id) && x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
        }

    }
}