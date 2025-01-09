using NUnit.Framework;
using NUnit.Framework.Legacy;
using SqliteLegacyExampleGen;
using System;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    public partial class SqliteTester : IExecLastIdTester
    {
        private QuerySql QuerySql { get; } = new QuerySql(
            Environment.GetEnvironmentVariable(EndToEndCommon.SqliteConnectionStringEnv));

        [TearDown]
        public async Task EmptyTestsTable()
        {
            await QuerySql.DeleteAllAuthors();
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

        private static bool Equals(QuerySql.GetAuthorByIdRow x, QuerySql.GetAuthorByIdRow y)
        {
            return x.Id.Equals(y.Id) && x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
        }
    }
}