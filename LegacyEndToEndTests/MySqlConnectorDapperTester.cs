using MySqlConnectorDapperLegacyExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    public class MySqlConnectorDapperTests
    {
        private static string ConnectionStringEnv => "MYSQL_CONNECTION_STRING";

        private QuerySql QuerySql { get; } = new QuerySql(Environment.GetEnvironmentVariable(ConnectionStringEnv));

        [TearDown]
        public async Task EmptyTestsTable()
        {
            await QuerySql.TruncateAuthors();
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
            ClassicAssert.AreEqual(2, actual.Count);
            var expected = new List<QuerySql.ListAuthorsRow>
            {
                new QuerySql.ListAuthorsRow { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme },
                new QuerySql.ListAuthorsRow { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote }
            };
            Assert.That(actual.SequenceEqual(expected));
            foreach (var a in actual)
            {
                Assert.That(a.Created >= DateTime.Now.Subtract(TimeSpan.FromSeconds(30)) && a.Created < DateTime.Now);
            }
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
            ClassicAssert.IsNotNull(actual);
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

            await QuerySql.DeleteAuthor(new QuerySql.DeleteAuthorArgs { Name = DataGenerator.BojackAuthor });
            var actual = await QuerySql.ListAuthors();
            ClassicAssert.AreEqual(1, actual.Count);
            Assert.That(actual[0].Equals(new QuerySql.ListAuthorsRow
            {
                Name = DataGenerator.DrSeussAuthor,
                Bio = DataGenerator.DrSeussQuote
            }));
        }
    }
}