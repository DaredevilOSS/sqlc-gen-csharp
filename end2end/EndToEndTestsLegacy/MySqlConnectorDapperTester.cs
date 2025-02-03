using MySqlConnectorDapperLegacyExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    [TestFixture]
    public partial class MySqlConnectorDapperTester
    {
        private QuerySql QuerySql { get; } = new QuerySql(
            Environment.GetEnvironmentVariable(EndToEndCommon.MySqlConnectionStringEnv));

        [TearDown]
        public async Task EmptyTestsTable()
        {
            await QuerySql.TruncateAuthors();
        }

        [Test]
        public async Task TestSliceIds()
        {
            var sqlArgs = new QuerySql.CreateAuthorReturnIdArgs
            {
                Name = DataGenerator.GenericAuthor,
                Bio = DataGenerator.GenericQuote1
            };
            var insertedId1 = await QuerySql.CreateAuthorReturnId(sqlArgs);
            var insertedId2 = await QuerySql.CreateAuthorReturnId(sqlArgs);
            await QuerySql.CreateAuthorReturnId(sqlArgs);

            var actual = await QuerySql.SelectAuthorsWithSlice(new QuerySql.SelectAuthorsWithSliceArgs { Ids = new[] { insertedId1, insertedId2 } });

            ClassicAssert.AreEqual(2, actual.Count);
        }

        [Test]
        public async Task TestTwoSlices()
        {
            var sqlArgs = new QuerySql.CreateAuthorReturnIdArgs
            {
                Name = DataGenerator.GenericAuthor,
                Bio = DataGenerator.GenericQuote1
            };
            var insertedId1 = await QuerySql.CreateAuthorReturnId(sqlArgs);
            var insertedId2 = await QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs
            {
                Name = DataGenerator.BojackAuthor,
                Bio = DataGenerator.GenericQuote1
            });
            await QuerySql.CreateAuthorReturnId(sqlArgs);

            var actual = await QuerySql.SelectAuthorsWithTwoSlices(new QuerySql.SelectAuthorsWithTwoSlicesArgs { Ids = new[] { insertedId1, insertedId2 }, Names = new[] { DataGenerator.GenericAuthor } });

            ClassicAssert.AreEqual(1, actual.Count);
        }
    }
}