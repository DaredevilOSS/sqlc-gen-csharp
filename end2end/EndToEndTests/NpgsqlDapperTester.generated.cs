using NpgsqlDapperExampleGen;
using NpgsqlTypes;
using System.Net;
using System.Net.NetworkInformation;
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
        [TestCase("E", "It takes a nation of millions to hold us back", "Rebel Without a Pause", "Master of Puppets", "Prophets of Rage")]
        [TestCase(null, null, null, null, null)]
        public async Task TestPostgresStringTypes(string cChar, string cVarchar, string cCharacterVarying, string cBpchar, string cText)
        {
            await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CChar = cChar, CVarchar = cVarchar, CCharacterVarying = cCharacterVarying, CBpchar = cBpchar, CText = cText, });
            var expected = new QuerySql.GetPostgresTypesRow
            {
                CChar = cChar,
                CVarchar = cVarchar,
                CCharacterVarying = cCharacterVarying,
                CBpchar = cBpchar,
                CText = cText,
            };
            var actual = await QuerySql.GetPostgresTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresTypesRow x, QuerySql.GetPostgresTypesRow y)
            {
                Assert.That(x.CChar, Is.EqualTo(y.CChar));
                Assert.That(x.CVarchar, Is.EqualTo(y.CVarchar));
                Assert.That(x.CCharacterVarying, Is.EqualTo(y.CCharacterVarying));
                Assert.That(x.CBpchar?.Trim(), Is.EqualTo(y.CBpchar?.Trim()));
                Assert.That(x.CText, Is.EqualTo(y.CText));
            }
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
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresTypesRow x, QuerySql.GetPostgresTypesRow y)
            {
                Assert.That(x.CReal, Is.EqualTo(y.CReal));
                Assert.That(x.CNumeric, Is.EqualTo(y.CNumeric));
                Assert.That(x.CDecimal, Is.EqualTo(y.CDecimal));
                Assert.That(x.CDoublePrecision, Is.EqualTo(y.CDoublePrecision));
                Assert.That(x.CMoney, Is.EqualTo(y.CMoney));
            }
        }

        [Test]
        [TestCase("2000-1-30", "12:13:14", "1983-11-3 02:01:22", "2022-10-2 15:44:01+09:00", "02:03:04")]
        [TestCase(null, null, null, null, null)]
        public async Task TestPostgresDateTimeTypes(DateTime? cDate, TimeSpan? cTime, DateTime? cTimestamp, DateTime? cTimestampWithTz, TimeSpan? cInterval)
        {
            await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CDate = cDate, CTime = cTime, CTimestamp = cTimestamp, CTimestampWithTz = cTimestampWithTz, CInterval = cInterval });
            var expected = new QuerySql.GetPostgresTypesRow
            {
                CDate = cDate,
                CTime = cTime,
                CTimestamp = cTimestamp,
                CTimestampWithTz = cTimestampWithTz,
                CInterval = cInterval
            };
            var actual = await QuerySql.GetPostgresTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresTypesRow x, QuerySql.GetPostgresTypesRow y)
            {
                Assert.That(x.CDate, Is.EqualTo(y.CDate));
                Assert.That(x.CTime, Is.EqualTo(y.CTime));
                Assert.That(x.CTimestamp, Is.EqualTo(y.CTimestamp));
                Assert.That(x.CTimestampWithTz, Is.EqualTo(y.CTimestampWithTz));
                Assert.That(x.CInterval, Is.EqualTo(y.CInterval));
            }
        }

        private static IEnumerable<TestCaseData> PostgresArrayTypesTestCases
        {
            get
            {
                yield return new TestCaseData(new byte[] { 0x45, 0x42 }, new bool[] { true, false }, new string[] { "Makeshift", "Swahili" }, new int[] { 543, -4234 }, new decimal[] { 1.2345678m, 2.3456789m }, new DateTime[] { new DateTime(2021, 1, 1), new DateTime(2022, 2, 2) }, new DateTime[] { new DateTime(2023, 3, 3), new DateTime(2024, 4, 4) }).SetName("Arrays with values");
                yield return new TestCaseData(new byte[] { }, new bool[] { }, new string[] { }, new int[] { }, new decimal[] { }, new DateTime[] { }, new DateTime[] { }).SetName("Arrays with null values");
                yield return new TestCaseData(null, null, null, null, null, null, null).SetName("Null Array Types");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresArrayTypesTestCases))]
        public async Task TestPostgresArrayTypes(byte[] cBytea, bool[] cBooleanArray, string[] cTextArray, int[] cIntegerArray, decimal[] cDecimalArray, DateTime[] cDateArray, DateTime[] cTimestampArray)
        {
            await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CBytea = cBytea, CBooleanArray = cBooleanArray, CTextArray = cTextArray, CIntegerArray = cIntegerArray, CDecimalArray = cDecimalArray, CDateArray = cDateArray, CTimestampArray = cTimestampArray });
            var expected = new QuerySql.GetPostgresTypesRow
            {
                CBytea = cBytea,
                CBooleanArray = cBooleanArray,
                CTextArray = cTextArray,
                CIntegerArray = cIntegerArray,
                CDecimalArray = cDecimalArray,
                CDateArray = cDateArray,
                CTimestampArray = cTimestampArray
            };
            var actual = await QuerySql.GetPostgresTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresTypesRow x, QuerySql.GetPostgresTypesRow y)
            {
                Assert.That(x.CBytea, Is.EqualTo(y.CBytea));
                Assert.That(x.CTextArray, Is.EqualTo(y.CTextArray));
                Assert.That(x.CIntegerArray, Is.EqualTo(y.CIntegerArray));
                Assert.That(x.CBooleanArray, Is.EqualTo(y.CBooleanArray));
                Assert.That(x.CDecimalArray, Is.EqualTo(y.CDecimalArray));
                Assert.That(x.CDateArray, Is.EqualTo(y.CDateArray));
                Assert.That(x.CTimestampArray, Is.EqualTo(y.CTimestampArray));
            }
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

        private static IEnumerable<TestCaseData> PostgresGuidDataTypesTestCases
        {
            get
            {
                yield return new TestCaseData(Guid.NewGuid()).SetName("Valid Guid");
                yield return new TestCaseData(null).SetName("Null Guid");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresGuidDataTypesTestCases))]
        public async Task TestPostgresGuidDataTypes(Guid? cUuid)
        {
            await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CUuid = cUuid });
            var expected = new QuerySql.GetPostgresTypesRow
            {
                CUuid = cUuid
            };
            var actual = await QuerySql.GetPostgresTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresTypesRow x, QuerySql.GetPostgresTypesRow y)
            {
                Assert.That(x.CUuid, Is.EqualTo(y.CUuid));
            }
        }

        [Test]
        [TestCase(100, "z", "Sex Pistols", "Anarchy in the U.K", "Yoshimi Battles the Pink Robots", "Never Mind the Bollocks...")]
        [TestCase(10, null, null, null, null, null)]
        public async Task TestStringCopyFrom(int batchSize, string cChar, string cVarchar, string cCharacterVarying, string cBpchar, string cText)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresTypesBatchArgs { CChar = cChar, CVarchar = cVarchar, CCharacterVarying = cCharacterVarying, CBpchar = cBpchar, CText = cText }).ToList();
            await QuerySql.InsertPostgresTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresTypesCntRow
            {
                Cnt = batchSize,
                CChar = cChar,
                CVarchar = cVarchar,
                CCharacterVarying = cCharacterVarying,
                CBpchar = cBpchar,
                CText = cText
            };
            var actual = await QuerySql.GetPostgresTypesCnt();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresTypesCntRow x, QuerySql.GetPostgresTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CChar, Is.EqualTo(y.CChar));
                Assert.That(x.CVarchar, Is.EqualTo(y.CVarchar));
                Assert.That(x.CCharacterVarying, Is.EqualTo(y.CCharacterVarying));
                Assert.That(x.CBpchar?.Trim(), Is.EqualTo(y.CBpchar?.Trim()));
                Assert.That(x.CText, Is.EqualTo(y.CText));
            }
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
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresTypesCntRow x, QuerySql.GetPostgresTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CReal, Is.EqualTo(y.CReal));
                Assert.That(x.CDecimal, Is.EqualTo(y.CDecimal));
                Assert.That(x.CNumeric, Is.EqualTo(y.CNumeric));
                Assert.That(x.CDoublePrecision, Is.EqualTo(y.CDoublePrecision));
                Assert.That(x.CMoney, Is.EqualTo(y.CMoney));
            }
        }

        [Test]
        [TestCase(100, "1973-12-3", "00:34:00", "1960-11-3 02:01:22", "2030-07-20 15:44:01+09:00", "02:03:04")]
        [TestCase(10, null, null, null, null, null)]
        public async Task TestDateTimeCopyFrom(int batchSize, DateTime? cDate, TimeSpan? cTime, DateTime? cTimestamp, DateTime? cTimestampWithTz, TimeSpan? cInterval)
        {
            DateTime? cTimestampWithTzAsUtc = null;
            if (cTimestampWithTz != null)
                cTimestampWithTzAsUtc = DateTime.SpecifyKind(cTimestampWithTz.Value, DateTimeKind.Utc);
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresTypesBatchArgs { CDate = cDate, CTime = cTime, CTimestamp = cTimestamp, CTimestampWithTz = cTimestampWithTzAsUtc, CInterval = cInterval }).ToList();
            await QuerySql.InsertPostgresTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresTypesCntRow
            {
                Cnt = batchSize,
                CDate = cDate,
                CTime = cTime,
                CTimestamp = cTimestamp,
                CTimestampWithTz = cTimestampWithTz,
                CInterval = cInterval
            };
            var actual = await QuerySql.GetPostgresTypesCnt();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresTypesCntRow x, QuerySql.GetPostgresTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CDate, Is.EqualTo(y.CDate));
                Assert.That(x.CTime, Is.EqualTo(y.CTime));
                Assert.That(x.CTimestamp, Is.EqualTo(y.CTimestamp));
                Assert.That(x.CTimestampWithTz, Is.EqualTo(y.CTimestampWithTz));
                Assert.That(x.CInterval, Is.EqualTo(y.CInterval));
            }
        }

        private static IEnumerable<TestCaseData> PostgresGuidCopyFromTestCases
        {
            get
            {
                yield return new TestCaseData(200, Guid.NewGuid()).SetName("Valid Guid Copy From");
                yield return new TestCaseData(10, null).SetName("Null Guid Copy From");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresGuidCopyFromTestCases))]
        public async Task TestPostgresGuidCopyFrom(int batchSize, Guid? cUuid)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresTypesBatchArgs { CUuid = cUuid }).ToList();
            await QuerySql.InsertPostgresTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresTypesCntRow
            {
                Cnt = batchSize,
                CUuid = cUuid
            };
            var actual = await QuerySql.GetPostgresTypesCnt();
            Assert.That(actual.Cnt, Is.EqualTo(expected.Cnt));
            void AssertSingularEquals(QuerySql.GetPostgresTypesCntRow x, QuerySql.GetPostgresTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CUuid, Is.EqualTo(y.CUuid));
            }
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

        private static IEnumerable<TestCaseData> PostgresGeoTypesTestCases
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

        private static IEnumerable<TestCaseData> PostgresGeoCopyFromTestCases
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

        private static IEnumerable<TestCaseData> PostgresNetworkDataTypesTestCases
        {
            get
            {
                yield return new TestCaseData(new NpgsqlCidr("192.168.1.0/24"), new IPAddress(new byte[] { 192, 168, 1, 1 }), new PhysicalAddress(new byte[] { 0x00, 0x1A, 0x2B, 0x3C, 0x4D, 0x5E }), "00:1a:2b:ff:fe:3c:4d:5e").SetName("Valid Network Data Types");
                yield return new TestCaseData(null, null, null, null).SetName("Null Network Data Types");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresNetworkDataTypesTestCases))]
        public async Task TestPostgresNetworkDataTypes(NpgsqlCidr? cCidr, IPAddress cInet, PhysicalAddress cMacaddr, string cMacaddr8)
        {
            await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CCidr = cCidr, CInet = cInet, CMacaddr = cMacaddr, CMacaddr8 = cMacaddr8 });
            var expected = new QuerySql.GetPostgresTypesRow
            {
                CCidr = cCidr,
                CInet = cInet,
                CMacaddr = cMacaddr,
                CMacaddr8 = cMacaddr8
            };
            var actual = await QuerySql.GetPostgresTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresTypesRow x, QuerySql.GetPostgresTypesRow y)
            {
                Assert.That(x.CCidr, Is.EqualTo(y.CCidr));
                Assert.That(x.CInet, Is.EqualTo(y.CInet));
                Assert.That(x.CMacaddr, Is.EqualTo(y.CMacaddr));
                Assert.That(x.CMacaddr8, Is.EqualTo(y.CMacaddr8));
            }
        }

        [Test]
        [TestCase("{\"name\": \"Swordfishtrombones\", \"year\": 1983}", "$.\"name\"")]
        [TestCase(null, null)]
        public async Task TestPostgresJsonDataTypes(string cJson, string cJsonpath)
        {
            JsonElement? cParsedJson = null;
            if (cJson != null)
                cParsedJson = JsonDocument.Parse(cJson).RootElement;
            await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CJson = cParsedJson, CJsonb = cParsedJson, CJsonStringOverride = cJson, CJsonpath = cJsonpath });
            var expected = new QuerySql.GetPostgresTypesRow
            {
                CJson = cParsedJson,
                CJsonb = cParsedJson,
                CJsonStringOverride = cJson,
                CJsonpath = cJsonpath
            };
            var actual = await QuerySql.GetPostgresTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresTypesRow x, QuerySql.GetPostgresTypesRow y)
            {
                Assert.That(x.CJson.HasValue, Is.EqualTo(y.CJson.HasValue));
                if (x.CJson.HasValue)
                    Assert.That(x.CJson.Value.GetRawText(), Is.EqualTo(y.CJson.Value.GetRawText()));
                Assert.That(x.CJsonb.HasValue, Is.EqualTo(y.CJsonb.HasValue));
                if (x.CJsonb.HasValue)
                    Assert.That(x.CJsonb.Value.GetRawText(), Is.EqualTo(y.CJsonb.Value.GetRawText()));
                Assert.That(x.CJsonStringOverride, Is.EqualTo(y.CJsonStringOverride));
                Assert.That(x.CJsonpath, Is.EqualTo(y.CJsonpath));
            }
        }

        [Test]
        public void TestPostgresInvalidJson()
        {
            Assert.ThrowsAsync<Npgsql.PostgresException>(async () => await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CJsonStringOverride = "SOME INVALID JSON" }));
            Assert.ThrowsAsync<Npgsql.PostgresException>(async () => await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CJsonpath = "SOME INVALID JSONPATH" }));
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
