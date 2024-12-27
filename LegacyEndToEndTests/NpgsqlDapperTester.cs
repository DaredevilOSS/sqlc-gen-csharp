using NpgsqlDapperLegacyExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    public class NpgsqlDapperTests
    {
        private QuerySql QuerySql { get; } = new QuerySql(Environment.GetEnvironmentVariable(GlobalSetup.PostgresConnectionStringEnv));

        [TearDown]
        public async Task EmptyTestsTable()
        {
            await QuerySql.TruncateAuthors();
        }

        [Test]
        public async Task TestCreateAndListAuthors()
        {
            await QuerySql.CreateAuthor(
                new QuerySql.CreateAuthorArgs
                {
                    Name = DataGenerator.BojackAuthor,
                    Bio = DataGenerator.BojackTheme
                }
            );
            await QuerySql.CreateAuthor(
                new QuerySql.CreateAuthorArgs
                {
                    Name = DataGenerator.DrSeussAuthor,
                    Bio = DataGenerator.DrSeussQuote
                }
            );

            var actual = await QuerySql.ListAuthors();
            var expected = new List<QuerySql.ListAuthorsRow>
            {
                new QuerySql.ListAuthorsRow
                {
                    Name = DataGenerator.BojackAuthor,
                    Bio = DataGenerator.BojackTheme
                },
                new QuerySql.ListAuthorsRow
                {
                    Name = DataGenerator.DrSeussAuthor,
                    Bio = DataGenerator.DrSeussQuote
                }
            };
            Assert.That(SequenceEquals(expected, actual));
        }

        [Test]
        public async Task TestGetAuthor()
        {
            await QuerySql.CreateAuthor(
                new QuerySql.CreateAuthorArgs
                {
                    Name = DataGenerator.BojackAuthor,
                    Bio = DataGenerator.BojackTheme
                }
            );
            await QuerySql.CreateAuthor(
                new QuerySql.CreateAuthorArgs
                {
                    Name = DataGenerator.DrSeussAuthor,
                    Bio = DataGenerator.DrSeussQuote
                }
            );

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
        public async Task TestDeleteAuthor()
        {
            await QuerySql.CreateAuthor(
                new QuerySql.CreateAuthorArgs
                {
                    Name = DataGenerator.BojackAuthor,
                    Bio = DataGenerator.BojackTheme
                }
            );
            await QuerySql.CreateAuthor(
                new QuerySql.CreateAuthorArgs
                {
                    Name = DataGenerator.DrSeussAuthor,
                    Bio = DataGenerator.DrSeussQuote
                }
            );

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
        public async Task TestExecRowsFlow()
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
            var affected = await QuerySql.UpdateAuthors(updateAuthorsArgs);
            Assert.That(affected is 2);
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