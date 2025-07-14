using NpgsqlDapperLegacyExampleGen;
using NpgsqlTypes;
using System.Text.Json;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EndToEndTests
{
    [TestFixture]
    public partial class NpgsqlDapperTester
    {
        [Test]
        public async Task TestOne()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 2222, Name = "Dr. Seuss", Bio = "You'll miss the best things if you keep your eyes shut" });
            var expected = new QuerySql.GetAuthorRow
            {
                Id = 1111,
                Name = "Bojack Horseman",
                Bio = "Back in the 90s he was in a very famous TV show"
            };
            var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            Assert.That(SingularEquals(expected, actual));
        }

        private static bool SingularEquals(QuerySql.GetAuthorRow x, QuerySql.GetAuthorRow y)
        {
            return x.Id.Equals(y.Id) && x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
        }

        [Test]
        public async Task TestMany()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 2222, Name = "Dr. Seuss", Bio = "You'll miss the best things if you keep your eyes shut" });
            var expected = new List<QuerySql.ListAuthorsRow>
            {
                new QuerySql.ListAuthorsRow
                {
                    Id = 1111,
                    Name = "Bojack Horseman",
                    Bio = "Back in the 90s he was in a very famous TV show"
                },
                new QuerySql.ListAuthorsRow
                {
                    Id = 2222,
                    Name = "Dr. Seuss",
                    Bio = "You'll miss the best things if you keep your eyes shut"
                }
            };
            var actual = await this.QuerySql.ListAuthors();
            Assert.That(SequenceEquals(expected, actual));
        }

        private static bool SingularEquals(QuerySql.ListAuthorsRow x, QuerySql.ListAuthorsRow y)
        {
            return x.Id.Equals(y.Id) && x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
        }

        private static bool SequenceEquals(List<QuerySql.ListAuthorsRow> x, List<QuerySql.ListAuthorsRow> y)
        {
            if (x.Count != y.Count)
                return false;
            x = x.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Id).ToList();
            y = y.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Id).ToList();
            return !x.Where((t, i) => !SingularEquals(t, y[i])).Any();
        }

        [Test]
        public async Task TestExec()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 2222, Name = "Dr. Seuss", Bio = "You'll miss the best things if you keep your eyes shut" });
            await this.QuerySql.DeleteAuthor(new QuerySql.DeleteAuthorArgs { Name = "Bojack Horseman" });
            var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            ClassicAssert.IsNull(actual);
        }

        [Test]
        public async Task TestExecRows()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 2222, Name = "Dr. Seuss", Bio = "You'll miss the best things if you keep your eyes shut" });
            var affectedRows = await this.QuerySql.UpdateAuthors(new QuerySql.UpdateAuthorsArgs { Bio = "Quote that everyone always attribute to Einstein" });
            ClassicAssert.AreEqual(2, affectedRows);
            var expected = new List<QuerySql.ListAuthorsRow>
            {
                new QuerySql.ListAuthorsRow
                {
                    Id = 1111,
                    Name = "Bojack Horseman",
                    Bio = "Quote that everyone always attribute to Einstein"
                },
                new QuerySql.ListAuthorsRow
                {
                    Id = 2222,
                    Name = "Dr. Seuss",
                    Bio = "Quote that everyone always attribute to Einstein"
                }
            };
            var actual = await this.QuerySql.ListAuthors();
            Assert.That(SequenceEquals(expected, actual));
        }

        [Test]
        public async Task TestExecLastId()
        {
            var id1 = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Quote that everyone always attribute to Einstein" });
            var expected = new QuerySql.GetAuthorByIdRow
            {
                Id = id1,
                Name = "Albert Einstein",
                Bio = "Quote that everyone always attribute to Einstein"
            };
            var actual = await QuerySql.GetAuthorById(new QuerySql.GetAuthorByIdArgs { Id = id1 });
            Assert.That(SingularEquals(expected, actual));
        }

        private static bool SingularEquals(QuerySql.GetAuthorByIdRow x, QuerySql.GetAuthorByIdRow y)
        {
            return x.Id.Equals(y.Id) && x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
        }

        [Test]
        public async Task TestSelfJoinEmbed()
        {
            var id1 = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Quote that everyone always attribute to Einstein" });
            var id2 = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Only 2 things are infinite, the universe and human stupidity" });
            var expected = new List<QuerySql.GetDuplicateAuthorsRow>()
            {
                new QuerySql.GetDuplicateAuthorsRow
                {
                    Author = new Author
                    {
                        Id = id1,
                        Name = "Albert Einstein",
                        Bio = "Quote that everyone always attribute to Einstein"
                    },
                    Author2 = new Author
                    {
                        Id = id2,
                        Name = "Albert Einstein",
                        Bio = "Only 2 things are infinite, the universe and human stupidity"
                    }
                }
            };
            var actual = await QuerySql.GetDuplicateAuthors();
            Assert.That(SequenceEquals(expected, actual));
        }

        private static bool SingularEquals(QuerySql.GetDuplicateAuthorsRow x, QuerySql.GetDuplicateAuthorsRow y)
        {
            return SingularEquals(x.Author, y.Author) && SingularEquals(x.Author2, y.Author2);
        }

        private static bool SequenceEquals(List<QuerySql.GetDuplicateAuthorsRow> x, List<QuerySql.GetDuplicateAuthorsRow> y)
        {
            if (x.Count != y.Count)
                return false;
            return !x.Where((t, i) => !SingularEquals(t, y[i])).Any();
        }

        [Test]
        public async Task TestJoinEmbed()
        {
            var bojackId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            var bojackBookId = await QuerySql.CreateBook(new QuerySql.CreateBookArgs { Name = "One Trick Pony", AuthorId = bojackId });
            var drSeussId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = "Dr. Seuss", Bio = "You'll miss the best things if you keep your eyes shut" });
            var drSeussBookId = await QuerySql.CreateBook(new QuerySql.CreateBookArgs { AuthorId = drSeussId, Name = "How the Grinch Stole Christmas!" });
            var expected = new List<QuerySql.ListAllAuthorsBooksRow>()
            {
                new QuerySql.ListAllAuthorsBooksRow
                {
                    Author = new Author
                    {
                        Id = bojackId,
                        Name = "Bojack Horseman",
                        Bio = "Back in the 90s he was in a very famous TV show"
                    },
                    Book = new Book
                    {
                        Id = bojackBookId,
                        AuthorId = bojackId,
                        Name = "One Trick Pony"
                    }
                },
                new QuerySql.ListAllAuthorsBooksRow
                {
                    Author = new Author
                    {
                        Id = drSeussId,
                        Name = "Dr. Seuss",
                        Bio = "You'll miss the best things if you keep your eyes shut"
                    },
                    Book = new Book
                    {
                        Id = drSeussBookId,
                        AuthorId = drSeussId,
                        Name = "How the Grinch Stole Christmas!"
                    }
                }
            };
            var actual = await QuerySql.ListAllAuthorsBooks();
            Assert.That(SequenceEquals(expected, actual));
        }

        private static bool SingularEquals(QuerySql.ListAllAuthorsBooksRow x, QuerySql.ListAllAuthorsBooksRow y)
        {
            return SingularEquals(x.Author, y.Author) && SingularEquals(x.Book, y.Book);
        }

        private static bool SequenceEquals(List<QuerySql.ListAllAuthorsBooksRow> x, List<QuerySql.ListAllAuthorsBooksRow> y)
        {
            if (x.Count != y.Count)
                return false;
            x = x.OrderBy<QuerySql.ListAllAuthorsBooksRow, object>(o => o.Author.Name + o.Book.Name).ToList();
            y = y.OrderBy<QuerySql.ListAllAuthorsBooksRow, object>(o => o.Author.Name + o.Book.Name).ToList();
            return !x.Where((t, i) => !SingularEquals(t, y[i])).Any();
        }

        private static bool SingularEquals(Author x, Author y)
        {
            return x.Id.Equals(y.Id) && x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
        }

        private static bool SingularEquals(Book x, Book y)
        {
            return x.Id.Equals(y.Id) && x.AuthorId.Equals(y.AuthorId) && x.Name.Equals(y.Name);
        }

        [Test]
        public async Task TestNargNull()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 2222, Name = "Dr. Seuss", Bio = "You'll miss the best things if you keep your eyes shut" });
            var expected = new List<QuerySql.GetAuthorByNamePatternRow>
            {
                new QuerySql.GetAuthorByNamePatternRow
                {
                    Id = 1111,
                    Name = "Bojack Horseman",
                    Bio = "Back in the 90s he was in a very famous TV show"
                },
                new QuerySql.GetAuthorByNamePatternRow
                {
                    Id = 2222,
                    Name = "Dr. Seuss",
                    Bio = "You'll miss the best things if you keep your eyes shut"
                }
            };
            var actual = await this.QuerySql.GetAuthorByNamePattern(new QuerySql.GetAuthorByNamePatternArgs());
            Assert.That(SequenceEquals(expected, actual));
        }

        private static bool SequenceEquals(List<QuerySql.GetAuthorByNamePatternRow> x, List<QuerySql.GetAuthorByNamePatternRow> y)
        {
            if (x.Count != y.Count)
                return false;
            x = x.OrderBy<QuerySql.GetAuthorByNamePatternRow, object>(o => o.Id).ToList();
            y = y.OrderBy<QuerySql.GetAuthorByNamePatternRow, object>(o => o.Id).ToList();
            return !x.Where((t, i) => !SingularEquals(t, y[i])).Any();
        }

        private static bool SingularEquals(QuerySql.GetAuthorByNamePatternRow x, QuerySql.GetAuthorByNamePatternRow y)
        {
            return x.Id.Equals(y.Id) && x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
        }

        [Test]
        public async Task TestNargNotNull()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 2222, Name = "Dr. Seuss", Bio = "You'll miss the best things if you keep your eyes shut" });
            var expected = new List<QuerySql.GetAuthorByNamePatternRow>
            {
                new QuerySql.GetAuthorByNamePatternRow
                {
                    Id = 1111,
                    Name = "Bojack Horseman",
                    Bio = "Back in the 90s he was in a very famous TV show"
                }
            };
            var actual = await this.QuerySql.GetAuthorByNamePattern(new QuerySql.GetAuthorByNamePatternArgs { NamePattern = "Bojack%" });
            Assert.That(SequenceEquals(expected, actual));
        }

        [Test]
        [TestCase("E", "It takes a nation of millions to hold us back", "Rebel Without a Pause", "Prophets of Rage")]
        [TestCase(null, null, null, null)]
        public async Task TestPostgresStringTypes(string cChar, string cVarchar, string cCharacterVarying, string cText)
        {
            await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CChar = cChar, CVarchar = cVarchar, CCharacterVarying = cCharacterVarying, CText = cText, });
            var expected = new QuerySql.GetPostgresTypesRow
            {
                CChar = cChar,
                CVarchar = cVarchar,
                CCharacterVarying = cCharacterVarying,
                CText = cText,
            };
            var actual = await QuerySql.GetPostgresTypes();
            Assert.That(actual.CChar, Is.EqualTo(expected.CChar));
            Assert.That(actual.CVarchar, Is.EqualTo(expected.CVarchar));
            Assert.That(actual.CCharacterVarying, Is.EqualTo(expected.CCharacterVarying));
            Assert.That(actual.CText, Is.EqualTo(expected.CText));
        }

        [Test]
        [TestCase(true, 35, -23423, 4235235263L)]
        [TestCase(null, null, null, null)]
        public async Task TestPostgresIntegerTypes(bool cBoolean, short cSmallint, int cInteger, long cBigint)
        {
            await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CBoolean = cBoolean, CSmallint = cSmallint, CInteger = cInteger, CBigint = cBigint });
            var expected = new QuerySql.GetPostgresTypesRow
            {
                CBoolean = cBoolean,
                CSmallint = cSmallint,
                CInteger = cInteger,
                CBigint = cBigint
            };
            var actual = await QuerySql.GetPostgresTypes();
            Assert.That(actual.CBoolean, Is.EqualTo(expected.CBoolean));
            Assert.That(actual.CSmallint, Is.EqualTo(expected.CSmallint));
            Assert.That(actual.CInteger, Is.EqualTo(expected.CInteger));
            Assert.That(actual.CBigint, Is.EqualTo(expected.CBigint));
        }

        [Test]
        [TestCase(3.83f, 4.5534, 998.432, -8403284.321435, 42332.53)]
        [TestCase(null, null, null, null, null)]
        public async Task TestPostgresFloatingPointTypes(float? cReal, decimal? cNumeric, decimal? cDecimal, double? cDoublePrecision, decimal? cMoney)
        {
            await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CReal = cReal, CNumeric = cNumeric, CDecimal = cDecimal, CDoublePrecision = cDoublePrecision, CMoney = cMoney });
            var expected = new QuerySql.GetPostgresTypesRow
            {
                CReal = cReal,
                CNumeric = cNumeric,
                CDecimal = cDecimal,
                CDoublePrecision = cDoublePrecision,
                CMoney = cMoney
            };
            var actual = await QuerySql.GetPostgresTypes();
            Assert.That(actual.CReal, Is.EqualTo(expected.CReal));
            Assert.That(actual.CNumeric, Is.EqualTo(expected.CNumeric));
            Assert.That(actual.CDecimal, Is.EqualTo(expected.CDecimal));
            Assert.That(actual.CDoublePrecision, Is.EqualTo(expected.CDoublePrecision));
            Assert.That(actual.CMoney, Is.EqualTo(expected.CMoney));
        }

        [Test]
        [TestCase("2000-1-30", "12:13:14", "1983-11-3 02:01:22", "2022-10-2 15:44:01+09:00")]
        [TestCase(null, null, null, null)]
        public async Task TestPostgresDateTimeTypes(DateTime? cDate, TimeSpan? cTime, DateTime? cTimestamp, DateTime? cTimestampWithTz)
        {
            await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CDate = cDate, CTime = cTime, CTimestamp = cTimestamp, CTimestampWithTz = cTimestampWithTz });
            var expected = new QuerySql.GetPostgresTypesRow
            {
                CDate = cDate,
                CTime = cTime,
                CTimestamp = cTimestamp,
                CTimestampWithTz = cTimestampWithTz
            };
            var actual = await QuerySql.GetPostgresTypes();
            Assert.That(actual.CDate, Is.EqualTo(expected.CDate));
            Assert.That(actual.CTime, Is.EqualTo(expected.CTime));
            Assert.That(actual.CTimestamp, Is.EqualTo(expected.CTimestamp));
            Assert.That(actual.CTimestampWithTz, Is.EqualTo(expected.CTimestampWithTz));
        }

        [Test]
        [TestCase(new byte[] { 0x45, 0x42 }, new string[] { "Party", "Fight" }, new int[] { 543, -4234 })]
        [TestCase(new byte[] { }, new string[] { }, new int[] { })]
        [TestCase(null, null, null)]
        public async Task TestPostgresArrayTypes(byte[] cBytea, string[] cTextArray, int[] cIntegerArray)
        {
            await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CBytea = cBytea, CTextArray = cTextArray, CIntegerArray = cIntegerArray });
            var expected = new QuerySql.GetPostgresTypesRow
            {
                CBytea = cBytea,
                CTextArray = cTextArray,
                CIntegerArray = cIntegerArray
            };
            var actual = await QuerySql.GetPostgresTypes();
            Assert.That(actual.CBytea, Is.EqualTo(expected.CBytea));
            Assert.That(actual.CTextArray, Is.EqualTo(expected.CTextArray));
            Assert.That(actual.CIntegerArray, Is.EqualTo(expected.CIntegerArray));
        }

        [Test]
        [TestCase(-54355, "White Light from the Mouth of Infinity", "2022-10-2 15:44:01+09:00")]
        [TestCase(null, null, "1970-01-01 00:00:00")]
        public async Task TestPostgresDataTypesOverride(int? cInteger, string cVarchar, DateTime cTimestamp)
        {
            await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CInteger = cInteger, CVarchar = cVarchar, CTimestamp = cTimestamp });
            var expected = new QuerySql.GetPostgresFunctionsRow
            {
                MaxInteger = cInteger,
                MaxVarchar = cVarchar,
                MaxTimestamp = cTimestamp
            };
            var actual = await QuerySql.GetPostgresFunctions();
            AssertSingularEquals(expected, actual);
        }

        private static void AssertSingularEquals(QuerySql.GetPostgresFunctionsRow expected, QuerySql.GetPostgresFunctionsRow actual)
        {
            Assert.That(actual.MaxInteger, Is.EqualTo(expected.MaxInteger));
            Assert.That(actual.MaxVarchar, Is.EqualTo(expected.MaxVarchar));
            Assert.That(actual.MaxTimestamp, Is.EqualTo(expected.MaxTimestamp));
        }

        [Test]
        [TestCase(100, "z", "Sex Pistols", "Anarchy in the U.K", "Never Mind the Bollocks...")]
        [TestCase(10, null, null, null, null)]
        public async Task TestStringCopyFrom(int batchSize, string cChar, string cVarchar, string cCharacterVarying, string cText)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresTypesBatchArgs { CChar = cChar, CVarchar = cVarchar, CCharacterVarying = cCharacterVarying, CText = cText }).ToList();
            await QuerySql.InsertPostgresTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresTypesCntRow
            {
                Cnt = batchSize,
                CChar = cChar,
                CVarchar = cVarchar,
                CCharacterVarying = cCharacterVarying,
                CText = cText
            };
            var actual = await QuerySql.GetPostgresTypesCnt();
            Assert.That(actual.Cnt, Is.EqualTo(expected.Cnt));
            Assert.That(actual.CChar, Is.EqualTo(expected.CChar));
            Assert.That(actual.CVarchar, Is.EqualTo(expected.CVarchar));
            Assert.That(actual.CCharacterVarying, Is.EqualTo(expected.CCharacterVarying));
            Assert.That(actual.CText, Is.EqualTo(expected.CText));
        }

        [Test]
        public async Task TestPostgresTransaction()
        {
            var connection = new Npgsql.NpgsqlConnection(Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv));
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();
            var querySqlWithTx = QuerySql.WithTransaction(transaction);
            await querySqlWithTx.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            // The GetAuthor method in NpgsqlExampleGen returns QuerySql.GetAuthorRow? (nullable record struct)
            var actualNull = await QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            Assert.That(actualNull == null, "there is author"); // This is correct for nullable types
            await transaction.CommitAsync();
            var expected = new QuerySql.GetAuthorRow
            {
                Id = 1111,
                Name = "Bojack Horseman",
                Bio = "Back in the 90s he was in a very famous TV show"
            };
            var actual = await QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            Assert.That(SingularEquals(expected, actual)); // Apply placeholder here
        }

        [Test]
        public async Task TestPostgresTransactionRollback()
        {
            var connection = new Npgsql.NpgsqlConnection(Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv));
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();
            var sqlQueryWithTx = QuerySql.WithTransaction(transaction);
            await sqlQueryWithTx.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            await transaction.RollbackAsync();
            var actual = await QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            Assert.That(actual == null, "author should not exist after rollback");
        }

        [Test]
        [TestCase(100, true, 3, 453, -1445214231L)]
        [TestCase(10, null, null, null, null)]
        public async Task TestIntegerCopyFrom(int batchSize, bool? cBoolean, short? cSmallint, int? cInteger, long? cBigint)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresTypesBatchArgs { CBoolean = cBoolean, CSmallint = cSmallint, CInteger = cInteger, CBigint = cBigint }).ToList();
            await QuerySql.InsertPostgresTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresTypesCntRow
            {
                Cnt = batchSize,
                CBoolean = cBoolean,
                CSmallint = cSmallint,
                CInteger = cInteger,
                CBigint = cBigint
            };
            var actual = await QuerySql.GetPostgresTypesCnt();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresTypesCntRow x, QuerySql.GetPostgresTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CBoolean, Is.EqualTo(y.CBoolean));
                Assert.That(x.CSmallint, Is.EqualTo(y.CSmallint));
                Assert.That(x.CInteger, Is.EqualTo(y.CInteger));
                Assert.That(x.CBigint, Is.EqualTo(y.CBigint));
            }
        }

        [Test]
        [TestCase(100, 666.6f, 336.3431, -99.999, -1377.996, -43242.43)]
        [TestCase(10, null, null, null, null, null)]
        public async Task TestFloatingPointCopyFrom(int batchSize, float? cReal, decimal? cDecimal, decimal? cNumeric, double? cDoublePrecision, decimal? cMoney)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresTypesBatchArgs { CReal = cReal, CDecimal = cDecimal, CNumeric = cNumeric, CDoublePrecision = cDoublePrecision, CMoney = cMoney }).ToList();
            await QuerySql.InsertPostgresTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresTypesCntRow
            {
                Cnt = batchSize,
                CReal = cReal,
                CDecimal = cDecimal,
                CNumeric = cNumeric,
                CDoublePrecision = cDoublePrecision,
                CMoney = cMoney
            };
            var actual = await QuerySql.GetPostgresTypesCnt();
            Assert.That(actual.Cnt, Is.EqualTo(expected.Cnt));
            Assert.That(actual.CReal, Is.EqualTo(expected.CReal));
            Assert.That(actual.CDecimal, Is.EqualTo(expected.CDecimal));
            Assert.That(actual.CNumeric, Is.EqualTo(expected.CNumeric));
            Assert.That(actual.CDoublePrecision, Is.EqualTo(expected.CDoublePrecision));
            Assert.That(actual.CMoney, Is.EqualTo(expected.CMoney));
        }

        [Test]
        [TestCase(100, "1973-12-3", "00:34:00", "1960-11-3 02:01:22", "2030-07-20 15:44:01+09:00")]
        [TestCase(10, null, null, null, null)]
        public async Task TestDateTimeCopyFrom(int batchSize, DateTime? cDate, TimeSpan? cTime, DateTime? cTimestamp, DateTime? cTimestampWithTz)
        {
            DateTime? cTimestampWithTzAsUtc = null;
            if (cTimestampWithTz != null)
                cTimestampWithTzAsUtc = DateTime.SpecifyKind(cTimestampWithTz.Value, DateTimeKind.Utc);
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresTypesBatchArgs { CDate = cDate, CTime = cTime, CTimestamp = cTimestamp, CTimestampWithTz = cTimestampWithTzAsUtc }).ToList();
            await QuerySql.InsertPostgresTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresTypesCntRow
            {
                Cnt = batchSize,
                CDate = cDate,
                CTime = cTime,
                CTimestamp = cTimestamp,
                CTimestampWithTz = cTimestampWithTz,
            };
            var actual = await QuerySql.GetPostgresTypesCnt();
            Assert.That(actual.Cnt, Is.EqualTo(expected.Cnt));
            Assert.That(actual.CDate, Is.EqualTo(expected.CDate));
            Assert.That(actual.CTime, Is.EqualTo(expected.CTime));
            Assert.That(actual.CTimestamp, Is.EqualTo(expected.CTimestamp));
            Assert.That(actual.CTimestampWithTz, Is.EqualTo(expected.CTimestampWithTz));
        }

        [Test]
        [TestCase(100, new byte[] { 0x53, 0x56 })]
        [TestCase(10, new byte[] { })]
        [TestCase(10, null)]
        public async Task TestArrayCopyFrom(int batchSize, byte[] cBytea)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresTypesBatchArgs { CBytea = cBytea }).ToList();
            await QuerySql.InsertPostgresTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresTypesCntRow
            {
                Cnt = batchSize,
                CBytea = cBytea
            };
            var actual = await QuerySql.GetPostgresTypesCnt();
            Assert.That(actual.Cnt, Is.EqualTo(expected.Cnt));
            Assert.That(actual.CBytea, Is.EqualTo(expected.CBytea));
        }

        public static IEnumerable<TestCaseData> PostgresGeoTypesTestCases
        {
            get
            {
                yield return new TestCaseData(new NpgsqlPoint(1, 2), new NpgsqlLine(3, 4, 5), new NpgsqlLSeg(1, 2, 3, 4), new NpgsqlBox(1, 2, 3, 4), new NpgsqlPath(new NpgsqlPoint[] { new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4) }), new NpgsqlPolygon(new NpgsqlPoint[] { new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4) }), new NpgsqlCircle(1, 2, 3)).SetName("Valid Geo Types");
                yield return new TestCaseData(null, null, null, null, null, null, null).SetName("Null Geo Types");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresGeoTypesTestCases))]
        public async Task TestPostgresGeoTypes(NpgsqlPoint? cPoint, NpgsqlLine? cLine, NpgsqlLSeg? cLSeg, NpgsqlBox? cBox, NpgsqlPath? cPath, NpgsqlPolygon? cPolygon, NpgsqlCircle? cCircle)
        {
            await QuerySql.InsertPostgresGeoTypes(new QuerySql.InsertPostgresGeoTypesArgs { CPoint = cPoint, CLine = cLine, CLseg = cLSeg, CBox = cBox, CPath = cPath, CPolygon = cPolygon, CCircle = cCircle });
            var expected = new QuerySql.GetPostgresGeoTypesRow
            {
                CPoint = cPoint,
                CLine = cLine,
                CLseg = cLSeg,
                CBox = cBox,
                CPath = cPath,
                CPolygon = cPolygon,
                CCircle = cCircle
            };
            var actual = await QuerySql.GetPostgresGeoTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresGeoTypesRow x, QuerySql.GetPostgresGeoTypesRow y)
            {
                Assert.That(x.CPoint, Is.EqualTo(y.CPoint));
                Assert.That(x.CLine, Is.EqualTo(y.CLine));
                Assert.That(x.CLseg, Is.EqualTo(y.CLseg));
                Assert.That(x.CBox, Is.EqualTo(y.CBox));
                Assert.That(x.CPath, Is.EqualTo(y.CPath));
                Assert.That(x.CPolygon, Is.EqualTo(y.CPolygon));
                Assert.That(x.CCircle, Is.EqualTo(y.CCircle));
            }
        }

        public static IEnumerable<TestCaseData> PostgresGeoCopyFromTestCases
        {
            get
            {
                yield return new TestCaseData(200, new NpgsqlPoint(1, 2), new NpgsqlLine(3, 4, 5), new NpgsqlLSeg(1, 2, 3, 4), new NpgsqlBox(1, 2, 3, 4), new NpgsqlPath(new NpgsqlPoint[] { new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4) }), new NpgsqlPolygon(new NpgsqlPoint[] { new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4) }), new NpgsqlCircle(1, 2, 3)).SetName("Valid Geo Types Copy From");
                yield return new TestCaseData(10, null, null, null, null, null, null, null).SetName("Null Geo Types Copy From");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresGeoCopyFromTestCases))]
        public async Task TestPostgresGeoCopyFrom(int batchSize, NpgsqlPoint? cPoint, NpgsqlLine? cLine, NpgsqlLSeg? cLSeg, NpgsqlBox? cBox, NpgsqlPath? cPath, NpgsqlPolygon? cPolygon, NpgsqlCircle? cCircle)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresGeoTypesBatchArgs { CPoint = cPoint, CLine = cLine, CLseg = cLSeg, CBox = cBox, CPath = cPath, CPolygon = cPolygon, CCircle = cCircle }).ToList();
            await QuerySql.InsertPostgresGeoTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresGeoTypesRow
            {
                CPoint = cPoint,
                CLine = cLine,
                CLseg = cLSeg,
                CBox = cBox,
                CPath = cPath,
                CPolygon = cPolygon,
                CCircle = cCircle
            };
            var actual = await QuerySql.GetPostgresGeoTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresGeoTypesRow x, QuerySql.GetPostgresGeoTypesRow y)
            {
                Assert.That(x.CPoint, Is.EqualTo(y.CPoint));
                Assert.That(x.CLine, Is.EqualTo(y.CLine));
                Assert.That(x.CLseg, Is.EqualTo(y.CLseg));
                Assert.That(x.CBox, Is.EqualTo(y.CBox));
                Assert.That(x.CPath, Is.EqualTo(y.CPath));
                Assert.That(x.CPolygon, Is.EqualTo(y.CPolygon));
                Assert.That(x.CCircle, Is.EqualTo(y.CCircle));
            }
        }

        [Test]
        [TestCase("{\"name\": \"Swordfishtrombones\", \"year\": 1983}")]
        [TestCase(null)]
        public async Task TestPostgresJsonDataTypes(string cJson)
        {
            JsonElement? cParsedJson = null;
            if (cJson != null)
                cParsedJson = JsonDocument.Parse(cJson).RootElement;
            await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CJson = cParsedJson, CJsonStringOverride = cJson });
            var expected = new QuerySql.GetPostgresTypesRow
            {
                CJson = cParsedJson,
                CJsonStringOverride = cJson
            };
            var actual = await QuerySql.GetPostgresTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresTypesRow x, QuerySql.GetPostgresTypesRow y)
            {
                Assert.That(x.CJson.HasValue, Is.EqualTo(y.CJson.HasValue));
                if (x.CJson.HasValue)
                    Assert.That(x.CJson.Value.GetRawText(), Is.EqualTo(y.CJson.Value.GetRawText()));
                Assert.That(x.CJsonStringOverride, Is.EqualTo(y.CJsonStringOverride));
            }
        }

        [Test]
        public void TestPostgresInvalidJson()
        {
            Assert.ThrowsAsync<Npgsql.PostgresException>(async () => await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CJsonStringOverride = "SOME INVALID JSON" }));
        }

        [Test]
        public async Task TestArray()
        {
            var id1 = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Quote that everyone always attribute to Einstein" });
            var bojackId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            var actual = await QuerySql.GetAuthorsByIds(new QuerySql.GetAuthorsByIdsArgs { LongArr1 = new[] { id1, bojackId } });
            ClassicAssert.AreEqual(2, actual.Count);
        }

        [Test]
        public async Task TestMultipleArrays()
        {
            var id1 = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Quote that everyone always attribute to Einstein" });
            var id2 = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Only 2 things are infinite, the universe and human stupidity" });
            var bojackId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            var actual = await QuerySql.GetAuthorsByIdsAndNames(new QuerySql.GetAuthorsByIdsAndNamesArgs { LongArr1 = new[] { id1, bojackId }, StringArr2 = new[] { "Albert Einstein" } });
            ClassicAssert.AreEqual(1, actual.Count);
        }
    }
}
