using NpgsqlDapperLegacyExampleGen;
using NpgsqlTypes;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Xml;
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
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetAuthorRow x, QuerySql.GetAuthorRow y)
            {
                Assert.That(x.Id, Is.EqualTo(y.Id));
                Assert.That(x.Name, Is.EqualTo(y.Name));
                Assert.That(x.Bio, Is.EqualTo(y.Bio));
            }
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
            var actual = await this.QuerySql.ListAuthors(new QuerySql.ListAuthorsArgs { Limit = 2, Offset = 0 });
            AssertSequenceEquals(expected, actual);
            void AssertSingularEquals(QuerySql.ListAuthorsRow x, QuerySql.ListAuthorsRow y)
            {
                Assert.That(x.Id, Is.EqualTo(y.Id));
                Assert.That(x.Name, Is.EqualTo(y.Name));
                Assert.That(x.Bio, Is.EqualTo(y.Bio));
            }

            void AssertSequenceEquals(List<QuerySql.ListAuthorsRow> x, List<QuerySql.ListAuthorsRow> y)
            {
                Assert.That(x.Count, Is.EqualTo(y.Count));
                for (int i = 0; i < x.Count; i++)
                    AssertSingularEquals(x[i], y[i]);
            }
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
            var actual = await this.QuerySql.ListAuthors(new QuerySql.ListAuthorsArgs { Limit = 2, Offset = 0 });
            AssertSequenceEquals(expected, actual);
            void AssertSingularEquals(QuerySql.ListAuthorsRow x, QuerySql.ListAuthorsRow y)
            {
                Assert.That(x.Id, Is.EqualTo(y.Id));
                Assert.That(x.Name, Is.EqualTo(y.Name));
                Assert.That(x.Bio, Is.EqualTo(y.Bio));
            }

            void AssertSequenceEquals(List<QuerySql.ListAuthorsRow> x, List<QuerySql.ListAuthorsRow> y)
            {
                Assert.That(x.Count, Is.EqualTo(y.Count));
                for (int i = 0; i < x.Count; i++)
                    AssertSingularEquals(x[i], y[i]);
            }
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
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetAuthorByIdRow x, QuerySql.GetAuthorByIdRow y)
            {
                Assert.That(x.Id, Is.EqualTo(y.Id));
                Assert.That(x.Name, Is.EqualTo(y.Name));
                Assert.That(x.Bio, Is.EqualTo(y.Bio));
            }
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
            AssertSequenceEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetDuplicateAuthorsRow x, QuerySql.GetDuplicateAuthorsRow y)
            {
                Assert.That(x.Author.Id, Is.EqualTo(y.Author.Id));
                Assert.That(x.Author.Name, Is.EqualTo(y.Author.Name));
                Assert.That(x.Author.Bio, Is.EqualTo(y.Author.Bio));
                Assert.That(x.Author2.Id, Is.EqualTo(y.Author2.Id));
                Assert.That(x.Author2.Name, Is.EqualTo(y.Author2.Name));
                Assert.That(x.Author2.Bio, Is.EqualTo(y.Author2.Bio));
            }

            void AssertSequenceEquals(List<QuerySql.GetDuplicateAuthorsRow> x, List<QuerySql.GetDuplicateAuthorsRow> y)
            {
                Assert.That(x.Count, Is.EqualTo(y.Count));
                for (int i = 0; i < x.Count; i++)
                    AssertSingularEquals(x[i], y[i]);
            }
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
            AssertSequenceEquals(expected, actual);
            void AssertSingularEquals(QuerySql.ListAllAuthorsBooksRow x, QuerySql.ListAllAuthorsBooksRow y)
            {
                Assert.That(x.Author.Id, Is.EqualTo(y.Author.Id));
                Assert.That(x.Author.Name, Is.EqualTo(y.Author.Name));
                Assert.That(x.Author.Bio, Is.EqualTo(y.Author.Bio));
                Assert.That(x.Book.Id, Is.EqualTo(y.Book.Id));
                Assert.That(x.Book.AuthorId, Is.EqualTo(y.Book.AuthorId));
                Assert.That(x.Book.Name, Is.EqualTo(y.Book.Name));
            }

            void AssertSequenceEquals(List<QuerySql.ListAllAuthorsBooksRow> x, List<QuerySql.ListAllAuthorsBooksRow> y)
            {
                Assert.That(x.Count, Is.EqualTo(y.Count));
                for (int i = 0; i < x.Count; i++)
                    AssertSingularEquals(x[i], y[i]);
            }
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
            AssertSequenceEquals(expected, actual);
            void AssertSequenceEquals(List<QuerySql.GetAuthorByNamePatternRow> x, List<QuerySql.GetAuthorByNamePatternRow> y)
            {
                Assert.That(x.Count, Is.EqualTo(y.Count));
                for (int i = 0; i < x.Count; i++)
                    AssertSingularEquals(x[i], y[i]);
            }

            void AssertSingularEquals(QuerySql.GetAuthorByNamePatternRow x, QuerySql.GetAuthorByNamePatternRow y)
            {
                Assert.That(x.Id, Is.EqualTo(y.Id));
                Assert.That(x.Name, Is.EqualTo(y.Name));
                Assert.That(x.Bio, Is.EqualTo(y.Bio));
            }
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
            AssertSequenceEquals(expected, actual);
            void AssertSequenceEquals(List<QuerySql.GetAuthorByNamePatternRow> x, List<QuerySql.GetAuthorByNamePatternRow> y)
            {
                Assert.That(x.Count, Is.EqualTo(y.Count));
                for (int i = 0; i < x.Count; i++)
                    AssertSingularEquals(x[i], y[i]);
            }

            void AssertSingularEquals(QuerySql.GetAuthorByNamePatternRow x, QuerySql.GetAuthorByNamePatternRow y)
            {
                Assert.That(x.Id, Is.EqualTo(y.Id));
                Assert.That(x.Name, Is.EqualTo(y.Name));
                Assert.That(x.Bio, Is.EqualTo(y.Bio));
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
            var actual = await QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            ClassicAssert.IsNull(actual);
            await transaction.CommitAsync();
            var expected = new QuerySql.GetAuthorRow
            {
                Id = 1111,
                Name = "Bojack Horseman",
                Bio = "Back in the 90s he was in a very famous TV show"
            };
            actual = await QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetAuthorRow x, QuerySql.GetAuthorRow y)
            {
                Assert.That(x.Id, Is.EqualTo(y.Id));
                Assert.That(x.Name, Is.EqualTo(y.Name));
                Assert.That(x.Bio, Is.EqualTo(y.Bio));
            }
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
            ClassicAssert.IsNull(actual);
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

        [Test]
        [TestCase(-54355, "White Light from the Mouth of Infinity", "2022-10-2 15:44:01+09:00")]
        [TestCase(null, null, "1970-01-01 00:00:00")]
        public async Task TestPostgresDataTypesOverride(int? cInteger, string cVarchar, DateTime cTimestamp)
        {
            await QuerySql.InsertPostgresNumericTypes(new QuerySql.InsertPostgresNumericTypesArgs { CInteger = cInteger });
            await QuerySql.InsertPostgresDateTimeTypes(new QuerySql.InsertPostgresDateTimeTypesArgs { CTimestamp = cTimestamp });
            await QuerySql.InsertPostgresStringTypes(new QuerySql.InsertPostgresStringTypesArgs { CVarchar = cVarchar });
            var expected = new QuerySql.GetPostgresFunctionsRow
            {
                MaxInteger = cInteger,
                MaxVarchar = cVarchar,
                MaxTimestamp = cTimestamp
            };
            var actual = await QuerySql.GetPostgresFunctions();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresFunctionsRow x, QuerySql.GetPostgresFunctionsRow y)
            {
                Assert.That(x.MaxInteger, Is.EqualTo(y.MaxInteger));
                Assert.That(x.MaxVarchar, Is.EqualTo(y.MaxVarchar));
                Assert.That(x.MaxTimestamp, Is.EqualTo(y.MaxTimestamp));
            }
        }

        [Test]
        public void TestPostgresInvalidJson()
        {
            Assert.ThrowsAsync<Npgsql.PostgresException>(async () => await QuerySql.InsertPostgresSpecialTypes(new QuerySql.InsertPostgresSpecialTypesArgs { CJsonStringOverride = "SOME INVALID JSON", CEnumNotNull = CEnum.Small }));
            Assert.ThrowsAsync<Npgsql.PostgresException>(async () => await QuerySql.InsertPostgresSpecialTypes(new QuerySql.InsertPostgresSpecialTypesArgs { CJsonpath = "SOME INVALID JSONPATH", CEnumNotNull = CEnum.Small }));
        }

        [Test]
        public void TestPostgresInvalidXml()
        {
            Assert.ThrowsAsync<Npgsql.PostgresException>(async () => await QuerySql.InsertPostgresSpecialTypes(new QuerySql.InsertPostgresSpecialTypesArgs { CXmlStringOverride = "<root>SOME INVALID XML", CEnumNotNull = CEnum.Small }));
        }

        [Test]
        [TestCase("E", "It takes a nation of millions to hold us back", "Rebel Without a Pause", "Master of Puppets", "Prophets of Rage")]
        [TestCase(null, null, null, null, null)]
        public async Task TestPostgresStringTypes(string cChar, string cVarchar, string cCharacterVarying, string cBpchar, string cText)
        {
            await QuerySql.InsertPostgresStringTypes(new QuerySql.InsertPostgresStringTypesArgs { CChar = cChar, CVarchar = cVarchar, CCharacterVarying = cCharacterVarying, CBpchar = cBpchar, CText = cText, });
            var expected = new QuerySql.GetPostgresStringTypesRow
            {
                CChar = cChar,
                CVarchar = cVarchar,
                CCharacterVarying = cCharacterVarying,
                CBpchar = cBpchar,
                CText = cText,
            };
            var actual = await QuerySql.GetPostgresStringTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresStringTypesRow x, QuerySql.GetPostgresStringTypesRow y)
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
            await QuerySql.InsertPostgresNumericTypes(new QuerySql.InsertPostgresNumericTypesArgs { CBoolean = cBoolean, CSmallint = cSmallint, CInteger = cInteger, CBigint = cBigint });
            var expected = new QuerySql.GetPostgresNumericTypesRow
            {
                CBoolean = cBoolean,
                CSmallint = cSmallint,
                CInteger = cInteger,
                CBigint = cBigint
            };
            var actual = await QuerySql.GetPostgresNumericTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresNumericTypesRow x, QuerySql.GetPostgresNumericTypesRow y)
            {
                Assert.That(x.CBoolean, Is.EqualTo(y.CBoolean));
                Assert.That(x.CSmallint, Is.EqualTo(y.CSmallint));
                Assert.That(x.CInteger, Is.EqualTo(y.CInteger));
                Assert.That(x.CBigint, Is.EqualTo(y.CBigint));
            }
        }

        [Test]
        [TestCase(3.83f, 4.5534, 998.432, -8403284.321435, 42332.53)]
        [TestCase(null, null, null, null, null)]
        public async Task TestPostgresFloatingPointTypes(float? cReal, decimal? cNumeric, decimal? cDecimal, double? cDoublePrecision, decimal? cMoney)
        {
            await QuerySql.InsertPostgresNumericTypes(new QuerySql.InsertPostgresNumericTypesArgs { CReal = cReal, CNumeric = cNumeric, CDecimal = cDecimal, CDoublePrecision = cDoublePrecision, CMoney = cMoney });
            var expected = new QuerySql.GetPostgresNumericTypesRow
            {
                CReal = cReal,
                CNumeric = cNumeric,
                CDecimal = cDecimal,
                CDoublePrecision = cDoublePrecision,
                CMoney = cMoney
            };
            var actual = await QuerySql.GetPostgresNumericTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresNumericTypesRow x, QuerySql.GetPostgresNumericTypesRow y)
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
            await QuerySql.InsertPostgresDateTimeTypes(new QuerySql.InsertPostgresDateTimeTypesArgs { CDate = cDate, CTime = cTime, CTimestamp = cTimestamp, CTimestampWithTz = cTimestampWithTz, CInterval = cInterval });
            var expected = new QuerySql.GetPostgresDateTimeTypesRow
            {
                CDate = cDate,
                CTime = cTime,
                CTimestamp = cTimestamp,
                CTimestampWithTz = cTimestampWithTz,
                CInterval = cInterval
            };
            var actual = await QuerySql.GetPostgresDateTimeTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresDateTimeTypesRow x, QuerySql.GetPostgresDateTimeTypesRow y)
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
            await QuerySql.InsertPostgresArrayTypes(new QuerySql.InsertPostgresArrayTypesArgs { CBytea = cBytea, CBooleanArray = cBooleanArray, CTextArray = cTextArray, CIntegerArray = cIntegerArray, CDecimalArray = cDecimalArray, CDateArray = cDateArray, CTimestampArray = cTimestampArray });
            var expected = new QuerySql.GetPostgresArrayTypesRow
            {
                CBytea = cBytea,
                CBooleanArray = cBooleanArray,
                CTextArray = cTextArray,
                CIntegerArray = cIntegerArray,
                CDecimalArray = cDecimalArray,
                CDateArray = cDateArray,
                CTimestampArray = cTimestampArray
            };
            var actual = await QuerySql.GetPostgresArrayTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresArrayTypesRow x, QuerySql.GetPostgresArrayTypesRow y)
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
            await QuerySql.InsertPostgresSpecialTypes(new QuerySql.InsertPostgresSpecialTypesArgs { CUuid = cUuid, CEnumNotNull = CEnum.Small });
            var expected = new QuerySql.GetPostgresSpecialTypesRow
            {
                CUuid = cUuid,
                CEnumNotNull = CEnum.Small
            };
            var actual = await QuerySql.GetPostgresSpecialTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresSpecialTypesRow x, QuerySql.GetPostgresSpecialTypesRow y)
            {
                Assert.That(x.CUuid, Is.EqualTo(y.CUuid));
                Assert.That(x.CEnumNotNull, Is.EqualTo(y.CEnumNotNull));
            }
        }

        [Test]
        [Obsolete] // due to NpgsqlTsVector.Parse usage
        public async Task TestPostgresFullTextSearchDataTypes()
        {
            await QuerySql.InsertPostgresStringTypes(new QuerySql.InsertPostgresStringTypesArgs { CText = "Hello world" });
            var actual = await QuerySql.GetPostgresStringTypesTextSearch(new QuerySql.GetPostgresStringTypesTextSearchArgs { ToTsquery = "Hello" });
            var expected = new QuerySql.GetPostgresStringTypesTextSearchRow
            {
                CText = "Hello world",
                Query = new NpgsqlTsQueryLexeme("hello"),
                Tsv = NpgsqlTsVector.Parse("hello:1 world:2"),
                Rnk = 0.07f
            };
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresStringTypesTextSearchRow x, QuerySql.GetPostgresStringTypesTextSearchRow y)
            {
                Assert.That(y.CText, Is.EqualTo(x.CText));
                Assert.That(y.Query.ToString(), Is.EqualTo(x.Query.ToString()));
                Assert.That(y.Tsv.ToString(), Is.EqualTo(x.Tsv.ToString()));
                Assert.That(y.Rnk, Is.AtMost(x.Rnk));
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
            await QuerySql.InsertPostgresNetworkTypes(new QuerySql.InsertPostgresNetworkTypesArgs { CCidr = cCidr, CInet = cInet, CMacaddr = cMacaddr, CMacaddr8 = cMacaddr8 });
            var expected = new QuerySql.GetPostgresNetworkTypesRow
            {
                CCidr = cCidr,
                CInet = cInet,
                CMacaddr = cMacaddr,
                CMacaddr8 = cMacaddr8
            };
            var actual = await QuerySql.GetPostgresNetworkTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresNetworkTypesRow x, QuerySql.GetPostgresNetworkTypesRow y)
            {
                Assert.That(x.CCidr, Is.EqualTo(y.CCidr));
                Assert.That(x.CInet, Is.EqualTo(y.CInet));
                Assert.That(x.CMacaddr, Is.EqualTo(y.CMacaddr));
                Assert.That(x.CMacaddr8, Is.EqualTo(y.CMacaddr8));
            }
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

        [Test]
        [TestCase("{\"name\": \"Swordfishtrombones\", \"year\": 1983}", "$.\"name\"")]
        [TestCase(null, null)]
        public async Task TestPostgresJsonDataTypes(string cJson, string cJsonpath)
        {
            JsonElement? cParsedJson = null;
            if (cJson != null)
                cParsedJson = JsonDocument.Parse(cJson).RootElement;
            await QuerySql.InsertPostgresSpecialTypes(new QuerySql.InsertPostgresSpecialTypesArgs { CJson = cParsedJson, CJsonb = cParsedJson, CJsonStringOverride = cJson, CJsonpath = cJsonpath, CEnumNotNull = CEnum.Small });
            var expected = new QuerySql.GetPostgresSpecialTypesRow
            {
                CJson = cParsedJson,
                CJsonb = cParsedJson,
                CJsonStringOverride = cJson,
                CJsonpath = cJsonpath,
                CEnumNotNull = CEnum.Small
            };
            var actual = await QuerySql.GetPostgresSpecialTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresSpecialTypesRow x, QuerySql.GetPostgresSpecialTypesRow y)
            {
                Assert.That(x.CJson.HasValue, Is.EqualTo(y.CJson.HasValue));
                if (x.CJson.HasValue)
                    Assert.That(x.CJson.Value.GetRawText(), Is.EqualTo(y.CJson.Value.GetRawText()));
                Assert.That(x.CJsonb.HasValue, Is.EqualTo(y.CJsonb.HasValue));
                if (x.CJsonb.HasValue)
                    Assert.That(x.CJsonb.Value.GetRawText(), Is.EqualTo(y.CJsonb.Value.GetRawText()));
                Assert.That(x.CJsonStringOverride, Is.EqualTo(y.CJsonStringOverride));
                Assert.That(x.CJsonpath, Is.EqualTo(y.CJsonpath));
                Assert.That(x.CEnumNotNull, Is.EqualTo(y.CEnumNotNull));
            }
        }

        [Test]
        [TestCase("<root><child>Good morning xml, the world says hello</child></root>")]
        [TestCase(null)]
        public async Task TestPostgresXmlDataTypes(string cXml)
        {
            XmlDocument parsedXml = null;
            if (cXml != null)
            {
                parsedXml = new XmlDocument();
                parsedXml.LoadXml(cXml);
            }

            await QuerySql.InsertPostgresSpecialTypes(new QuerySql.InsertPostgresSpecialTypesArgs { CXml = parsedXml, CEnumNotNull = CEnum.Small });
            var expected = new QuerySql.GetPostgresSpecialTypesRow
            {
                CXml = parsedXml,
                CEnumNotNull = CEnum.Small
            };
            var actual = await QuerySql.GetPostgresSpecialTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresSpecialTypesRow x, QuerySql.GetPostgresSpecialTypesRow y)
            {
                Assert.That(x.CXml == null, Is.EqualTo(y.CXml == null));
                if (x.CXml != null)
                    Assert.That(x.CXml.OuterXml, Is.EqualTo(y.CXml.OuterXml));
                Assert.That(x.CEnumNotNull, Is.EqualTo(y.CEnumNotNull));
            }
        }

        [Test]
        [TestCase(CEnum.Medium, CEnum.Big)]
        [TestCase(null, CEnum.Small)]
        public async Task TestPostgresEnumTypes(CEnum? cEnum, CEnum cEnumNotNull)
        {
            await QuerySql.InsertPostgresSpecialTypes(new QuerySql.InsertPostgresSpecialTypesArgs { CEnum = cEnum, CEnumNotNull = cEnumNotNull });
            var expected = new QuerySql.GetPostgresSpecialTypesRow
            {
                CEnum = cEnum,
                CEnumNotNull = cEnumNotNull
            };
            var actual = await QuerySql.GetPostgresSpecialTypes();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresSpecialTypesRow x, QuerySql.GetPostgresSpecialTypesRow y)
            {
                Assert.That(x.CEnum, Is.EqualTo(y.CEnum));
                Assert.That(x.CEnumNotNull, Is.EqualTo(y.CEnumNotNull));
            }
        }

        [Test]
        [TestCase(100, "z", "Sex Pistols", "Anarchy in the U.K", "Yoshimi Battles the Pink Robots", "Never Mind the Bollocks...")]
        [TestCase(10, null, null, null, null, null)]
        public async Task TestStringCopyFrom(int batchSize, string cChar, string cVarchar, string cCharacterVarying, string cBpchar, string cText)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresStringTypesBatchArgs { CChar = cChar, CVarchar = cVarchar, CCharacterVarying = cCharacterVarying, CBpchar = cBpchar, CText = cText }).ToList();
            await QuerySql.InsertPostgresStringTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresStringTypesCntRow
            {
                Cnt = batchSize,
                CChar = cChar,
                CVarchar = cVarchar,
                CCharacterVarying = cCharacterVarying,
                CBpchar = cBpchar,
                CText = cText
            };
            var actual = await QuerySql.GetPostgresStringTypesCnt();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresStringTypesCntRow x, QuerySql.GetPostgresStringTypesCntRow y)
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
        [TestCase(100, true, 3, 453, -1445214231L)]
        [TestCase(10, null, null, null, null)]
        public async Task TestIntegerCopyFrom(int batchSize, bool? cBoolean, short? cSmallint, int? cInteger, long? cBigint)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresNumericTypesBatchArgs { CBoolean = cBoolean, CSmallint = cSmallint, CInteger = cInteger, CBigint = cBigint }).ToList();
            await QuerySql.InsertPostgresNumericTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresNumericTypesCntRow
            {
                Cnt = batchSize,
                CBoolean = cBoolean,
                CSmallint = cSmallint,
                CInteger = cInteger,
                CBigint = cBigint
            };
            var actual = await QuerySql.GetPostgresNumericTypesCnt();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresNumericTypesCntRow x, QuerySql.GetPostgresNumericTypesCntRow y)
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
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresNumericTypesBatchArgs { CReal = cReal, CDecimal = cDecimal, CNumeric = cNumeric, CDoublePrecision = cDoublePrecision, CMoney = cMoney }).ToList();
            await QuerySql.InsertPostgresNumericTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresNumericTypesCntRow
            {
                Cnt = batchSize,
                CReal = cReal,
                CDecimal = cDecimal,
                CNumeric = cNumeric,
                CDoublePrecision = cDoublePrecision,
                CMoney = cMoney
            };
            var actual = await QuerySql.GetPostgresNumericTypesCnt();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresNumericTypesCntRow x, QuerySql.GetPostgresNumericTypesCntRow y)
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
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresDateTimeTypesBatchArgs { CDate = cDate, CTime = cTime, CTimestamp = cTimestamp, CTimestampWithTz = cTimestampWithTzAsUtc, CInterval = cInterval }).ToList();
            await QuerySql.InsertPostgresDateTimeTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresDateTimeTypesCntRow
            {
                Cnt = batchSize,
                CDate = cDate,
                CTime = cTime,
                CTimestamp = cTimestamp,
                CTimestampWithTz = cTimestampWithTz,
                CInterval = cInterval
            };
            var actual = await QuerySql.GetPostgresDateTimeTypesCnt();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresDateTimeTypesCntRow x, QuerySql.GetPostgresDateTimeTypesCntRow y)
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
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresSpecialTypesBatchArgs { CUuid = cUuid }).ToList();
            await QuerySql.InsertPostgresSpecialTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresSpecialTypesCntRow
            {
                Cnt = batchSize,
                CUuid = cUuid
            };
            var actual = await QuerySql.GetPostgresSpecialTypesCnt();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresSpecialTypesCntRow x, QuerySql.GetPostgresSpecialTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CUuid, Is.EqualTo(y.CUuid));
            }
        }

        [Test]
        [TestCase(100, "{\"song\": \"Pinball Wizard\", \"album\": \"Tommy\", \"artist\": \"The Who\"}")]
        [TestCase(10, null)]
        public async Task TestPostgresJsonCopyFrom(int batchSize, string cJson)
        {
            JsonElement? cParsedJson = null;
            if (cJson != null)
                cParsedJson = JsonDocument.Parse(cJson).RootElement;
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresSpecialTypesBatchArgs { CJson = cParsedJson, CJsonb = cParsedJson }).ToList();
            await QuerySql.InsertPostgresSpecialTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresSpecialTypesCntRow
            {
                Cnt = batchSize,
                CJson = cParsedJson,
                CJsonb = cParsedJson
            };
            var actual = await QuerySql.GetPostgresSpecialTypesCnt();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresSpecialTypesCntRow x, QuerySql.GetPostgresSpecialTypesCntRow y)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = false
                };
                Assert.That(y.Cnt, Is.EqualTo(x.Cnt));
                Assert.That(y.CJson.HasValue, Is.EqualTo(x.CJson.HasValue));
                if (y.CJson.HasValue)
                    Assert.That(JsonSerializer.Serialize(y.CJson.Value, options), Is.EqualTo(JsonSerializer.Serialize(x.CJson.Value, options)));
                Assert.That(y.CJsonb.HasValue, Is.EqualTo(x.CJsonb.HasValue));
                if (y.CJsonb.HasValue)
                    Assert.That(JsonSerializer.Serialize(y.CJsonb.Value, options), Is.EqualTo(JsonSerializer.Serialize(x.CJsonb.Value, options)));
            }
        }

        private static IEnumerable<TestCaseData> PostgresNetworkCopyFromTestCases
        {
            get
            {
                yield return new TestCaseData(200, new NpgsqlCidr("192.168.1.0/24"), new IPAddress(new byte[] { 192, 168, 1, 1 }), new PhysicalAddress(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 })).SetName("Valid Network Copy From");
                yield return new TestCaseData(10, null, null, null).SetName("Null Network Copy From");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresNetworkCopyFromTestCases))]
        public async Task TestPostgresNetworkCopyFrom(int batchSize, NpgsqlCidr? cCidr, IPAddress cInet, PhysicalAddress cMacaddr)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresNetworkTypesBatchArgs { CCidr = cCidr, CInet = cInet, CMacaddr = cMacaddr }).ToList();
            await QuerySql.InsertPostgresNetworkTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresNetworkTypesCntRow
            {
                Cnt = batchSize,
                CCidr = cCidr,
                CInet = cInet,
                CMacaddr = cMacaddr
            };
            var actual = await QuerySql.GetPostgresNetworkTypesCnt();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresNetworkTypesCntRow x, QuerySql.GetPostgresNetworkTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CCidr, Is.EqualTo(y.CCidr));
                Assert.That(x.CInet, Is.EqualTo(y.CInet));
                Assert.That(x.CMacaddr, Is.EqualTo(y.CMacaddr));
            }
        }

        private static IEnumerable<TestCaseData> PostgresArrayCopyFromTestCases
        {
            get
            {
                yield return new TestCaseData(100, new byte[] { 0x53, 0x56 }, new bool[] { true, false }, new string[] { "Sister Ray", "Venus in Furs" }, new int[] { 1, 2 }, new decimal[] { 132.13m, 23.22m }, new DateTime[] { new DateTime(1984, 8, 26), new DateTime(2000, 1, 2) }).SetName("Valid Array Copy From");
                yield return new TestCaseData(10, new byte[] { }, new bool[] { }, new string[] { }, new int[] { }, new decimal[] { }, new DateTime[] { }).SetName("Empty Array Copy From");
                yield return new TestCaseData(10, null, null, null, null, null, null).SetName("Null Array Copy From");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresArrayCopyFromTestCases))]
        public async Task TestArrayCopyFrom(int batchSize, byte[] cBytea, bool[] cBooleanArray, string[] cTextArray, int[] cIntegerArray, decimal[] cDecimalArray, DateTime[] cTimestampArray)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresArrayTypesBatchArgs { CBytea = cBytea, CBooleanArray = cBooleanArray, CTextArray = cTextArray, CIntegerArray = cIntegerArray, CDecimalArray = cDecimalArray, CTimestampArray = cTimestampArray }).ToList();
            await QuerySql.InsertPostgresArrayTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresArrayTypesCntRow
            {
                Cnt = batchSize,
                CBytea = cBytea,
                CBooleanArray = cBooleanArray,
                CTextArray = cTextArray,
                CIntegerArray = cIntegerArray,
                CDecimalArray = cDecimalArray,
                CTimestampArray = cTimestampArray
            };
            var actual = await QuerySql.GetPostgresArrayTypesCnt();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresArrayTypesCntRow x, QuerySql.GetPostgresArrayTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CBytea, Is.EqualTo(y.CBytea));
                Assert.That(x.CBooleanArray, Is.EqualTo(y.CBooleanArray));
                Assert.That(x.CTextArray, Is.EqualTo(y.CTextArray));
                Assert.That(x.CIntegerArray, Is.EqualTo(y.CIntegerArray));
                Assert.That(x.CDecimalArray, Is.EqualTo(y.CDecimalArray));
                Assert.That(x.CTimestampArray, Is.EqualTo(y.CTimestampArray));
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
    }
}
