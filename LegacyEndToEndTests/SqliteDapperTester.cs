using NUnit.Framework;
using SqliteDapperLegacyExampleGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    public class SqliteDapperTests
    {
        private static string ConnectionStringEnv => "SQLITE_CONNECTION_STRING";

        private QuerySql QuerySql { get; } = new QuerySql(Environment.GetEnvironmentVariable(ConnectionStringEnv));


        [TearDown]
        public async Task EmptyTestsTable()
        {
            await QuerySql.DeleteAllAuthors();
        }

        [Test]
        public async Task TestCreateAndListAuthors()
        {
            await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.BojackAuthor,
                Bio = DataGenerator.BojackTheme
            });
            await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.DrSeussAuthor,
                Bio = DataGenerator.DrSeussQuote
            });

            var actual = await QuerySql.ListAuthors();
            var expected = new List<QuerySql.ListAuthorsRow>
            {
                new QuerySql.ListAuthorsRow { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme },
                new QuerySql.ListAuthorsRow { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote }
            };
            Assert.That(actual.SequenceEqual(expected));
        }

        [Test]
        public async Task TestGetAuthor()
        {
            await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.BojackAuthor,
                Bio = DataGenerator.BojackTheme
            });
            await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.DrSeussAuthor,
                Bio = DataGenerator.DrSeussQuote
            });

            var actual = await QuerySql.GetAuthor(new QuerySql.GetAuthorArgs
            {
                Name = DataGenerator.BojackAuthor
            });
            var expected = new QuerySql.GetAuthorRow
            {
                Name = DataGenerator.BojackAuthor,
                Bio = DataGenerator.BojackTheme
            };
            Assert.Equals(expected, actual);
        }

        [Test]
        public async Task TestDeleteAuthor()
        {
            await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.BojackAuthor,
                Bio = DataGenerator.BojackTheme
            });
            await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.DrSeussAuthor,
                Bio = DataGenerator.DrSeussQuote
            });

            var deleteAuthorArgs = new QuerySql.DeleteAuthorArgs { Name = DataGenerator.BojackAuthor };
            await QuerySql.DeleteAuthor(deleteAuthorArgs);

            var actual = await QuerySql.ListAuthors();
            var expected = new QuerySql.GetAuthorRow
            {
                Name = DataGenerator.DrSeussAuthor,
                Bio = DataGenerator.DrSeussQuote
            };
            Assert.Equals(expected, actual);
        }
    }
}