using NpgsqlLegacyExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static NpgsqlLegacyExampleGen.QuerySql;

namespace SqlcGenCsharpTests
{
    public class NpgsqlTester : IOneTester, IManyTester, IExecTester, IExecRowsTester, IExecLastIdTester, ICopyFromTester
    {
        private static readonly Random Randomizer = new Random();

        private QuerySql QuerySql { get; } = new QuerySql(
            Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv));

        [TearDown]
        public async Task EmptyTestsTable()
        {
            await QuerySql.TruncateAuthors();
            await QuerySql.TruncateNodePostgresTypes();
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
        public async Task TestNodePostgresType()
        {
            var nodePostgresTypeArgs = new QuerySql.InsertNodePostgresTypeArgs
            {
                CBigint = 1,
                CReal = 1.0f,
                CNumeric = 1,
                CSerial = 1,
                CSmallint = 1,
                CDecimal = 1,
                CDate = DateTime.Now,
                CTimestamp = DateTime.Now,
                CBoolean = true,
                CChar = "a",
                CInteger = 1,
                CText = "ab",
                CVarchar = "abc",
                CCharacterVarying = "abcd",
                CTextArray = new string[] { "a", "b" }
            };
            var insertedId = await QuerySql.InsertNodePostgresType(nodePostgresTypeArgs);

            var actual = await QuerySql.GetNodePostgresType(new QuerySql.GetNodePostgresTypeArgs
            {
                Id = insertedId
            });

            ClassicAssert.IsNotNull(actual);

            Assert.That(Equals(actual, new GetNodePostgresTypeRow
            {
                CBigint = 1,
                CReal = 1.0f,
                CSerial = 1,
                CNumeric = 1,
                CDecimal = 1,
                CSmallint = 1,
                CBoolean = true,
                CChar = "a",
                CInteger = 1,
                CText = "ab",
                CVarchar = "abc",
                CCharacterVarying = "abcd",
                CTextArray = new string[] { "a", "b" }
            }));
        }

        private static bool Equals(GetNodePostgresTypeRow x, GetNodePostgresTypeRow y)
        {
            return x.CSmallint.Equals(y.CSmallint) &&
                x.CBoolean.Equals(y.CBoolean) &&
                x.CInteger.Equals(y.CInteger) &&
                x.CBigint.Equals(y.CBigint) &&
                x.CSerial.Equals(y.CSerial) &&
                x.CDecimal.Equals(y.CDecimal) &&
                x.CNumeric.Equals(y.CNumeric) &&
                x.CReal.Equals(y.CReal) &&
                x.CDate.Equals(y.CDate) &&
                x.CChar.Equals(y.CChar) &&
                x.CVarchar.Equals(y.CVarchar) &&
                x.CCharacterVarying.Equals(y.CCharacterVarying) &&
                x.CText.Equals(y.CText) &&
                x.CTextArray.SequenceEqual(y.CTextArray);
        }

        private static bool Equals(QuerySql.GetAuthorRow x, QuerySql.GetAuthorRow y)
        {
            return x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
        }

        private static bool Equals(QuerySql.GetAuthorByIdRow x, QuerySql.GetAuthorByIdRow y)
        {
            return x.Id.Equals(y.Id) && x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
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