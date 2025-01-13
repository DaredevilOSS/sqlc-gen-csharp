using NpgsqlLegacyExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    public partial class NpgsqlTester
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
        public async Task TestSlice()
        {
            await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.BojackAuthor,
                Bio = DataGenerator.BojackTheme
            });
            var author2 = await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.DrSeussAuthor,
                Bio = DataGenerator.DrSeussQuote
            });
            var author3 = await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.GenericAuthor,
                Bio = DataGenerator.GenericQuote1
            });

            var actual = await QuerySql.SelectAuthorsWithSlice(new QuerySql.SelectAuthorsWithSliceArgs
            {
                LongArr1 = new long[] { author2.Id, author3.Id }
            });
            ClassicAssert.AreEqual(2, actual.Count);
            var expected = new List<QuerySql.SelectAuthorsWithSliceRow>
            {
                new QuerySql.SelectAuthorsWithSliceRow { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote },
                new QuerySql.SelectAuthorsWithSliceRow { Name = DataGenerator.GenericAuthor, Bio = DataGenerator.GenericQuote1 }
            };
            Assert.That(SequenceEquals(expected, actual));
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
                CTextArray = new string[] { "a", "b" },
                CIntegerArray = new int[] { 1, 2 }
            };
            var insertedId = await QuerySql.InsertNodePostgresType(nodePostgresTypeArgs);

            var actual = await QuerySql.GetNodePostgresType(new QuerySql.GetNodePostgresTypeArgs
            {
                Id = insertedId
            });

            ClassicAssert.IsNotNull(actual);

            Assert.That(Equals(actual, new QuerySql.GetNodePostgresTypeRow
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
                CTextArray = new string[] { "a", "b" },
                CIntegerArray = new int[] { 1, 2 }
            }));
        }

        private static bool Equals(QuerySql.GetNodePostgresTypeRow x, QuerySql.GetNodePostgresTypeRow y)
        {
            return x.CSmallint.Equals(y.CSmallint) &&
                x.CBoolean.Equals(y.CBoolean) &&
                x.CInteger.Equals(y.CInteger) &&
                x.CBigint.Equals(y.CBigint) &&
                x.CSerial.Equals(y.CSerial) &&
                x.CDecimal.Equals(y.CDecimal) &&
                x.CNumeric.Equals(y.CNumeric) &&
                x.CReal.Equals(y.CReal) &&
                x.CChar.Equals(y.CChar) &&
                x.CVarchar.Equals(y.CVarchar) &&
                x.CCharacterVarying.Equals(y.CCharacterVarying) &&
                x.CText.Equals(y.CText) &&
                x.CTextArray.SequenceEqual(y.CTextArray) &&
                x.CIntegerArray.SequenceEqual(y.CIntegerArray);
        }

        private static bool Equals(QuerySql.SelectAuthorsWithSliceRow x, QuerySql.SelectAuthorsWithSliceRow y)
        {
            return x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
        }

        private static bool SequenceEquals(List<QuerySql.SelectAuthorsWithSliceRow> x, List<QuerySql.SelectAuthorsWithSliceRow> y)
        {
            if (x.Count != y.Count) return false;
            x = x.OrderBy<QuerySql.SelectAuthorsWithSliceRow, object>(o => o.Name + o.Bio).ToList();
            y = y.OrderBy<QuerySql.SelectAuthorsWithSliceRow, object>(o => o.Name + o.Bio).ToList();
            return !x.Where((t, i) => !Equals(t, y[i])).Any();
        }
    }
}