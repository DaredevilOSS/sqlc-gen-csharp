using MySqlConnectorDapperLegacyExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    public class MySqlConnectorDapperTests : IOneTester, IManyTester, IExecTester, IExecRowsTester
    {
        private QuerySql QuerySql { get; } = new QuerySql(
            Environment.GetEnvironmentVariable(EndToEndCommon.MySqlConnectionStringEnv));

        [TearDown]
        public async Task EmptyTestsTable()
        {
            await QuerySql.TruncateAuthors();
        }

        [Test]
        public async Task TestOne()
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
            Assert.That(Equals(expected, actual));
        }

        [Test]
        public async Task TestMany()
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
            Assert.That(SequenceEquals(expected, actual));
        }

        [Test]
        public async Task TestExec()
        {
            await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.BojackAuthor,
                Bio = DataGenerator.BojackTheme
            });
            await QuerySql.DeleteAuthor(new QuerySql.DeleteAuthorArgs
            {
                Name = DataGenerator.BojackAuthor
            });
            var actual = await QuerySql.GetAuthor(new QuerySql.GetAuthorArgs
            {
                Name = DataGenerator.BojackAuthor
            });
            ClassicAssert.IsNull(actual);
        }

        [Test]
        public async Task TestExecRows()
        {
            var bojackCreateAuthorArgs = new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.GenericAuthor,
                Bio = DataGenerator.GenericQuote1
            };
            await QuerySql.CreateAuthor(bojackCreateAuthorArgs);
            await QuerySql.CreateAuthor(bojackCreateAuthorArgs);

            var updateAuthorsArgs = new QuerySql.UpdateAuthorsArgs
            {
                Bio = DataGenerator.GenericQuote2
            };
            var affectedRows = await QuerySql.UpdateAuthors(updateAuthorsArgs);
            ClassicAssert.AreEqual(2, affectedRows);
        }

        private static bool Equals(QuerySql.GetAuthorRow x, QuerySql.GetAuthorRow y)
        {
            return x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
        }

        private static bool Equals(QuerySql.ListAuthorsRow x, QuerySql.ListAuthorsRow y)
        {
            return x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
        }

        private static bool SequenceEquals(List<QuerySql.ListAuthorsRow> x, List<QuerySql.ListAuthorsRow> y)
        {
            if (x.Count != y.Count) return false;
            x = x.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Name + o.Bio).ToList();
            y = y.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Name + o.Bio).ToList();
            return !x.Where((t, i) => !Equals(t, y[i])).Any();
        }
    }
}