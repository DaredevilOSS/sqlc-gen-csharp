using MySqlConnectorExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EndToEndTests
{
    [TestFixture]
    public partial class MySqlConnectorTester
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
            Assert.That(SingularEquals(expected, actual.Value));
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
            Assert.That(SingularEquals(expected, actual.Value));
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
            return SingularEquals(x.Author.Value, y.Author.Value) && SingularEquals(x.Author2.Value, y.Author2.Value);
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
            return SingularEquals(x.Author.Value, y.Author.Value) && SingularEquals(x.Book.Value, y.Book.Value);
        }

        private static bool SequenceEquals(List<QuerySql.ListAllAuthorsBooksRow> x, List<QuerySql.ListAllAuthorsBooksRow> y)
        {
            if (x.Count != y.Count)
                return false;
            x = x.OrderBy<QuerySql.ListAllAuthorsBooksRow, object>(o => o.Author.Value.Name + o.Book.Value.Name).ToList();
            y = y.OrderBy<QuerySql.ListAllAuthorsBooksRow, object>(o => o.Author.Value.Name + o.Book.Value.Name).ToList();
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
        public async Task TestSlice()
        {
            var id1 = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Quote that everyone always attribute to Einstein" });
            var bojackId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            var actual = await QuerySql.GetAuthorsByIds(new QuerySql.GetAuthorsByIdsArgs { Ids = new[] { id1, bojackId } });
            ClassicAssert.AreEqual(2, actual.Count);
        }

        [Test]
        public async Task TestMultipleSlices()
        {
            var id1 = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Quote that everyone always attribute to Einstein" });
            var bojackId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            var actual = await QuerySql.GetAuthorsByIdsAndNames(new QuerySql.GetAuthorsByIdsAndNamesArgs { Ids = new[] { id1, bojackId }, Names = new[] { "Albert Einstein" } });
            ClassicAssert.AreEqual(1, actual.Count);
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
        [TestCase("&", "\u1857", "\u2649", "Sheena is a Punk Rocker", "Holiday in Cambodia", "London's Calling", "London's Burning", "Police & Thieves")]
        [TestCase(null, null, null, null, null, null, null, null)]
        public async Task TestMySqlStringTypes(string cChar, string cNchar, string cNationalChar, string cVarchar, string cTinytext, string cMediumtext, string cText, string cLongtext)
        {
            await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs { CChar = cChar, CNchar = cNchar, CNationalChar = cNationalChar, CVarchar = cVarchar, CTinytext = cTinytext, CMediumtext = cMediumtext, CText = cText, CLongtext = cLongtext });
            var expected = new QuerySql.GetMysqlTypesRow
            {
                CChar = cChar,
                CNchar = cNchar,
                CNationalChar = cNationalChar,
                CVarchar = cVarchar,
                CTinytext = cTinytext,
                CMediumtext = cMediumtext,
                CText = cText,
                CLongtext = cLongtext
            };
            var actual = await QuerySql.GetMysqlTypes();
            Assert.That(actual.Value.CChar, Is.EqualTo(expected.CChar));
            Assert.That(actual.Value.CNchar, Is.EqualTo(expected.CNchar));
            Assert.That(actual.Value.CNationalChar, Is.EqualTo(expected.CNationalChar));
            Assert.That(actual.Value.CVarchar, Is.EqualTo(expected.CVarchar));
            Assert.That(actual.Value.CTinytext, Is.EqualTo(expected.CTinytext));
            Assert.That(actual.Value.CMediumtext, Is.EqualTo(expected.CMediumtext));
            Assert.That(actual.Value.CText, Is.EqualTo(expected.CText));
            Assert.That(actual.Value.CLongtext, Is.EqualTo(expected.CLongtext));
        }

        [Test]
        [TestCase(false, true, 13, 2084, 3124, -54355, 324245, -67865, 9787668656L)]
        [TestCase(null, null, null, null, null, null, null, null, null)]
        public async Task TestMySqlIntegerTypes(bool? cBool, bool? cBoolean, short? cTinyint, short? cYear, short? cSmallint, int? cMediumint, int? cInt, int? cInteger, long? cBigint)
        {
            await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs { CBool = cBool, CBoolean = cBoolean, CTinyint = cTinyint, CSmallint = cSmallint, CMediumint = cMediumint, CInt = cInt, CInteger = cInteger, CBigint = cBigint });
            var expected = new QuerySql.GetMysqlTypesRow
            {
                CBool = cBool,
                CBoolean = cBoolean,
                CTinyint = cTinyint,
                CSmallint = cSmallint,
                CMediumint = cMediumint,
                CInt = cInt,
                CInteger = cInteger,
                CBigint = cBigint
            };
            var actual = await QuerySql.GetMysqlTypes();
            Assert.That(actual.Value.CBool, Is.EqualTo(expected.CBool));
            Assert.That(actual.Value.CBoolean, Is.EqualTo(expected.CBoolean));
            Assert.That(actual.Value.CTinyint, Is.EqualTo(expected.CTinyint));
            Assert.That(actual.Value.CSmallint, Is.EqualTo(expected.CSmallint));
            Assert.That(actual.Value.CMediumint, Is.EqualTo(expected.CMediumint));
            Assert.That(actual.Value.CInt, Is.EqualTo(expected.CInt));
            Assert.That(actual.Value.CInteger, Is.EqualTo(expected.CInteger));
            Assert.That(actual.Value.CBigint, Is.EqualTo(expected.CBigint));
        }

        [Test]
        public async Task TestMySqlTransaction()
        {
            var connection = new MySqlConnector.MySqlConnection(Environment.GetEnvironmentVariable(EndToEndCommon.MySqlConnectionStringEnv));
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();
            var sqlQueryWithTx = QuerySql.WithTransaction(transaction);
            await sqlQueryWithTx.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            var actualNull = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            Assert.That(actualNull == null, "there is author"); // This is correct for nullable types
            await transaction.CommitAsync();
            var expected = new QuerySql.GetAuthorRow
            {
                Id = 1111,
                Name = "Bojack Horseman",
                Bio = "Back in the 90s he was in a very famous TV show"
            };
            var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            Assert.That(SingularEquals(expected, actual.Value)); // Apply placeholder here
        }

        [Test]
        [TestCase(3.4f, -31.555666, 11.098643, 34.4424, 423.2445, 998.9994542, 21.214312452534)]
        [TestCase(null, null, null, null, null, null, null)]
        public async Task TestMySqlFloatingPointTypes(float? cFloat, decimal? cNumeric, decimal? cDecimal, decimal? cDec, decimal? cFixed, double? cDouble, double? cDoublePrecision)
        {
            await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs { CFloat = cFloat, CNumeric = cNumeric, CDecimal = cDecimal, CDec = cDec, CFixed = cFixed, CDouble = cDouble, CDoublePrecision = cDoublePrecision });
            var expected = new QuerySql.GetMysqlTypesRow
            {
                CFloat = cFloat,
                CNumeric = cNumeric,
                CDecimal = cDecimal,
                CDec = cDec,
                CFixed = cFixed,
                CDouble = cDouble,
                CDoublePrecision = cDoublePrecision
            };
            var actual = await QuerySql.GetMysqlTypes();
            Assert.That(actual.Value.CFloat, Is.EqualTo(expected.CFloat));
            Assert.That(actual.Value.CNumeric, Is.EqualTo(expected.CNumeric));
            Assert.That(actual.Value.CDecimal, Is.EqualTo(expected.CDecimal));
            Assert.That(actual.Value.CDec, Is.EqualTo(expected.CDec));
            Assert.That(actual.Value.CFixed, Is.EqualTo(expected.CFixed));
            Assert.That(actual.Value.CDouble, Is.EqualTo(expected.CDouble));
            Assert.That(actual.Value.CDoublePrecision, Is.EqualTo(expected.CDoublePrecision));
        }

        [Test]
        [TestCase(1999, "2000-1-30", "1983-11-3 02:01:22")]
        [TestCase(null, null, "1970-1-1 00:00:01")]
        public async Task TestMySqlDateTimeTypes(short? cYear, DateTime? cDate, DateTime? cTimestamp)
        {
            await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs { CYear = cYear, CDate = cDate, CTimestamp = cTimestamp });
            var expected = new QuerySql.GetMysqlTypesRow
            {
                CYear = cYear,
                CDate = cDate,
                CTimestamp = cTimestamp
            };
            var actual = await QuerySql.GetMysqlTypes();
            Assert.That(actual.Value.CYear, Is.EqualTo(expected.CYear));
            Assert.That(actual.Value.CDate, Is.EqualTo(expected.CDate));
            Assert.That(actual.Value.CTimestamp, Is.EqualTo(expected.CTimestamp));
        }

        [Test]
        [TestCase(0x32, new byte[] { 0x15, 0x16, 0x17 }, new byte[] { 0x15, 0x24 }, new byte[] { 0x23 }, new byte[] { 0x33, 0x13 }, new byte[] { 0x11, 0x62, 0x10 }, new byte[] { 0x38, 0x45, 0x06 })]
        [TestCase(null, new byte[] { 0x0, 0x0, 0x0 }, new byte[] { }, new byte[] { }, new byte[] { }, new byte[] { }, new byte[] { })]
        [TestCase(null, null, null, null, null, null, null)]
        public async Task TestMySqlBinaryTypes(byte? cBit, byte[] cBinary, byte[] cVarbinary, byte[] cTinyblob, byte[] cBlob, byte[] cMediumblob, byte[] cLongblob)
        {
            await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs { CBit = cBit, CBinary = cBinary, CVarbinary = cVarbinary, CTinyblob = cTinyblob, CBlob = cBlob, CMediumblob = cMediumblob, CLongblob = cLongblob });
            var expected = new QuerySql.GetMysqlTypesRow
            {
                CBit = cBit,
                CBinary = cBinary,
                CVarbinary = cVarbinary,
                CTinyblob = cTinyblob,
                CBlob = cBlob,
                CMediumblob = cMediumblob,
                CLongblob = cLongblob
            };
            var actual = await QuerySql.GetMysqlTypes();
            Assert.That(actual.Value.CBit, Is.EqualTo(expected.CBit));
            Assert.That(actual.Value.CBinary, Is.EqualTo(expected.CBinary));
            Assert.That(actual.Value.CVarbinary, Is.EqualTo(expected.CVarbinary));
            Assert.That(actual.Value.CTinyblob, Is.EqualTo(expected.CTinyblob));
            Assert.That(actual.Value.CBlob, Is.EqualTo(expected.CBlob));
            Assert.That(actual.Value.CMediumblob, Is.EqualTo(expected.CMediumblob));
            Assert.That(actual.Value.CLongblob, Is.EqualTo(expected.CLongblob));
        }

        [Test]
        [TestCase(MysqlTypesCEnum.Medium)]
        [TestCase(null)]
        public async Task TestMySqlStringTypes(MysqlTypesCEnum? cEnum)
        {
            await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs { CEnum = cEnum });
            var expected = new QuerySql.GetMysqlTypesRow
            {
                CEnum = cEnum
            };
            var actual = await QuerySql.GetMysqlTypes();
            Assert.That(actual.Value.CEnum, Is.EqualTo(expected.CEnum));
        }

        [Test]
        [TestCase(-54355, 9787876578, "Scream of the Butterfly", "2025-06-29 12:00:00")]
        [TestCase(null, 0, null, "1971-01-01 00:00:00")]
        public async Task TestMySqlDataTypesOverride(int? cInt, long cBigint, string cVarchar, DateTime cTimestamp)
        {
            await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs { CInt = cInt, CBigint = cBigint, CVarchar = cVarchar, CTimestamp = cTimestamp });
            var expected = new QuerySql.GetMysqlFunctionsRow
            {
                MaxInt = cInt,
                MaxBigint = cBigint,
                MaxVarchar = cVarchar,
                MaxTimestamp = cTimestamp
            };
            var actual = await QuerySql.GetMysqlFunctions();
            AssertSingularEquals(expected, actual.Value);
        }

        private static void AssertSingularEquals(QuerySql.GetMysqlFunctionsRow expected, QuerySql.GetMysqlFunctionsRow actual)
        {
            Assert.That(actual.MaxInt, Is.EqualTo(expected.MaxInt));
            Assert.That(actual.MaxBigint, Is.EqualTo(expected.MaxBigint));
            Assert.That(actual.MaxVarchar, Is.EqualTo(expected.MaxVarchar));
            Assert.That(actual.MaxTimestamp, Is.EqualTo(expected.MaxTimestamp));
        }

        [Test]
        public async Task TestMySqlScopedSchemaEnum()
        {
            await this.QuerySql.CreateExtendedBio(new QuerySql.CreateExtendedBioArgs { AuthorName = "Bojack Horseman", Name = "One Trick Pony", BioType = ExtendedBiosBioType.Memoir });
            var expected = new QuerySql.GetFirstExtendedBioByTypeRow
            {
                AuthorName = "Bojack Horseman",
                Name = "One Trick Pony",
                BioType = ExtendedBiosBioType.Memoir
            };
            var actual = await this.QuerySql.GetFirstExtendedBioByType(new QuerySql.GetFirstExtendedBioByTypeArgs { BioType = ExtendedBiosBioType.Memoir });
            Assert.That(SingularEquals(expected, actual.Value));
        }

        private static bool SingularEquals(QuerySql.GetFirstExtendedBioByTypeRow x, QuerySql.GetFirstExtendedBioByTypeRow y)
        {
            return x.AuthorName.Equals(y.AuthorName) && x.Name.Equals(y.Name) && x.BioType.Equals(y.BioType);
        }

        [Test]
        [TestCase(100, "D", "\u4321", "\u2345", "Parasite", "Clockwork Orange", "Dr. Strangelove", "Interview with a Vampire", "Memento")]
        [TestCase(10, null, null, null, null, null, null, null, null)]
        public async Task TestStringCopyFrom(int batchSize, string cChar, string cNchar, string cNationalChar, string cVarchar, string cTinytext, string cMediumtext, string cText, string cLongtext)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertMysqlTypesBatchArgs { CChar = cChar, CNchar = cNchar, CNationalChar = cNationalChar, CVarchar = cVarchar, CTinytext = cTinytext, CMediumtext = cMediumtext, CText = cText, CLongtext = cLongtext }).ToList();
            await QuerySql.InsertMysqlTypesBatch(batchArgs);
            var expected = new QuerySql.GetMysqlTypesCntRow
            {
                Cnt = batchSize,
                CChar = cChar,
                CNchar = cNchar,
                CNationalChar = cNationalChar,
                CVarchar = cVarchar,
                CTinytext = cTinytext,
                CMediumtext = cMediumtext,
                CText = cText,
                CLongtext = cLongtext,
            };
            var actual = await QuerySql.GetMysqlTypesCnt();
            Assert.That(actual.Value.Cnt, Is.EqualTo(expected.Cnt));
            Assert.That(actual.Value.CChar, Is.EqualTo(expected.CChar));
            Assert.That(actual.Value.CNchar, Is.EqualTo(expected.CNchar));
            Assert.That(actual.Value.CNationalChar, Is.EqualTo(expected.CNationalChar));
            Assert.That(actual.Value.CVarchar, Is.EqualTo(expected.CVarchar));
            Assert.That(actual.Value.CTinytext, Is.EqualTo(expected.CTinytext));
            Assert.That(actual.Value.CMediumtext, Is.EqualTo(expected.CMediumtext));
            Assert.That(actual.Value.CText, Is.EqualTo(expected.CText));
            Assert.That(actual.Value.CLongtext, Is.EqualTo(expected.CLongtext));
        }

        [Test]
        [TestCase(100, true, false, -13, 324, -98760, 987965, 3132423, -7785442L)]
        [TestCase(10, null, null, null, null, null, null, null, null)]
        public async Task TestIntegerCopyFrom(int batchSize, bool? cBool, bool? cBoolean, short? cTinyint, short? cSmallint, int? cMediumint, int? cInt, int? cInteger, long? cBigint)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertMysqlTypesBatchArgs { CBool = cBool, CBoolean = cBoolean, CTinyint = cTinyint, CSmallint = cSmallint, CMediumint = cMediumint, CInt = cInt, CInteger = cInteger, CBigint = cBigint }).ToList();
            await QuerySql.InsertMysqlTypesBatch(batchArgs);
            var expected = new QuerySql.GetMysqlTypesCntRow
            {
                Cnt = batchSize,
                CBool = cBool,
                CBoolean = cBoolean,
                CTinyint = cTinyint,
                CSmallint = cSmallint,
                CMediumint = cMediumint,
                CInt = cInt,
                CInteger = cInteger,
                CBigint = cBigint
            };
            var actual = await QuerySql.GetMysqlTypesCnt();
            Assert.That(actual.Value.Cnt, Is.EqualTo(expected.Cnt));
            Assert.That(actual.Value.CBool, Is.EqualTo(expected.CBool));
            Assert.That(actual.Value.CBoolean, Is.EqualTo(expected.CBoolean));
            Assert.That(actual.Value.CTinyint, Is.EqualTo(expected.CTinyint));
            Assert.That(actual.Value.CSmallint, Is.EqualTo(expected.CSmallint));
            Assert.That(actual.Value.CMediumint, Is.EqualTo(expected.CMediumint));
            Assert.That(actual.Value.CInt, Is.EqualTo(expected.CInt));
            Assert.That(actual.Value.CInteger, Is.EqualTo(expected.CInteger));
            Assert.That(actual.Value.CBigint, Is.EqualTo(expected.CBigint));
        }

        [Test]
        [TestCase(100, 3.4f, -31.55566, 11.09643, 34.4424, 423.2445, 998.999442, 21.214314)]
        [TestCase(10, null, null, null, null, null, null, null)]
        public async Task TestFloatingPointCopyFrom(int batchSize, float? cFloat, decimal? cNumeric, decimal? cDecimal, decimal? cDec, decimal? cFixed, double? cDouble, double? cDoublePrecision)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertMysqlTypesBatchArgs { CFloat = cFloat, CNumeric = cNumeric, CDecimal = cDecimal, CDec = cDec, CFixed = cFixed, CDouble = cDouble, CDoublePrecision = cDoublePrecision }).ToList();
            await QuerySql.InsertMysqlTypesBatch(batchArgs);
            var expected = new QuerySql.GetMysqlTypesCntRow
            {
                Cnt = batchSize,
                CFloat = cFloat,
                CNumeric = cNumeric,
                CDecimal = cDecimal,
                CDec = cDec,
                CFixed = cFixed,
                CDouble = cDouble,
                CDoublePrecision = cDoublePrecision
            };
            var actual = await QuerySql.GetMysqlTypesCnt();
            Assert.That(actual.Value.CFloat, Is.EqualTo(expected.CFloat));
            Assert.That(actual.Value.CNumeric, Is.EqualTo(expected.CNumeric));
            Assert.That(actual.Value.CDecimal, Is.EqualTo(expected.CDecimal));
            Assert.That(actual.Value.CDec, Is.EqualTo(expected.CDec));
            Assert.That(actual.Value.CFixed, Is.EqualTo(expected.CFixed));
            Assert.That(actual.Value.CDouble, Is.EqualTo(expected.CDouble));
            Assert.That(actual.Value.CDoublePrecision, Is.EqualTo(expected.CDoublePrecision));
        }

        [Test]
        [TestCase(100, 1993, "2000-1-30", "1983-11-3 02:01:22", "2010-1-30 08:11:00")]
        [TestCase(10, null, null, null, null)]
        public async Task TestDateTimeCopyFrom(int batchSize, short? cYear, DateTime? cDate, DateTime? cDatetime, DateTime? cTimestamp)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertMysqlTypesBatchArgs { CYear = cYear, CDate = cDate, CDatetime = cDatetime, CTimestamp = cTimestamp }).ToList();
            await QuerySql.InsertMysqlTypesBatch(batchArgs);
            var expected = new QuerySql.GetMysqlTypesCntRow
            {
                Cnt = batchSize,
                CYear = cYear,
                CDate = cDate,
                CDatetime = cDatetime,
                CTimestamp = cTimestamp
            };
            var actual = await QuerySql.GetMysqlTypesCnt();
            Assert.That(actual.Value.Cnt, Is.EqualTo(expected.Cnt));
            Assert.That(actual.Value.CYear, Is.EqualTo(expected.CYear));
            Assert.That(actual.Value.CDate, Is.EqualTo(expected.CDate));
            Assert.That(actual.Value.CDatetime, Is.EqualTo(expected.CDatetime));
            Assert.That(actual.Value.CTimestamp, Is.EqualTo(expected.CTimestamp));
        }

        [Test]
        [TestCase(100, 0x05, new byte[] { 0x15, 0x16, 0x17 }, new byte[] { 0x15, 0x20 }, new byte[] { 0x23 }, new byte[] { 0x33, 0x13 }, new byte[] { 0x11, 0x62, 0x10 }, new byte[] { 0x38, 0x45, 0x06, 0x04 })]
        [TestCase(500, null, new byte[] { 0x0, 0x0, 0x0 }, new byte[] { }, new byte[] { }, new byte[] { }, new byte[] { }, new byte[] { })]
        [TestCase(10, null, null, null, null, null, null, null)]
        public async Task TestBinaryCopyFrom(int batchSize, byte? cBit, byte[] cBinary, byte[] cVarbinary, byte[] cTinyblob, byte[] cBlob, byte[] cMediumblob, byte[] cLongblob)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertMysqlTypesBatchArgs { CBit = cBit, CBinary = cBinary, CVarbinary = cVarbinary, CTinyblob = cTinyblob, CBlob = cBlob, CMediumblob = cMediumblob, CLongblob = cLongblob }).ToList();
            await QuerySql.InsertMysqlTypesBatch(batchArgs);
            var expected = new QuerySql.GetMysqlTypesCntRow
            {
                Cnt = batchSize,
                CBit = cBit,
                CBinary = cBinary,
                CVarbinary = cVarbinary,
                CTinyblob = cTinyblob,
                CBlob = cBlob,
                CMediumblob = cMediumblob,
                CLongblob = cLongblob
            };
            var actual = await QuerySql.GetMysqlTypesCnt();
            Assert.That(actual.Value.Cnt, Is.EqualTo(expected.Cnt));
            Assert.That(actual.Value.CBit, Is.EqualTo(expected.CBit));
            Assert.That(actual.Value.CBinary, Is.EqualTo(expected.CBinary));
            Assert.That(actual.Value.CVarbinary, Is.EqualTo(expected.CVarbinary));
            Assert.That(actual.Value.CTinyblob, Is.EqualTo(expected.CTinyblob));
            Assert.That(actual.Value.CBlob, Is.EqualTo(expected.CBlob));
            Assert.That(actual.Value.CMediumblob, Is.EqualTo(expected.CMediumblob));
            Assert.That(actual.Value.CLongblob, Is.EqualTo(expected.CLongblob));
        }

        [Test]
        [TestCase(100, MysqlTypesCEnum.Big)]
        [TestCase(500, MysqlTypesCEnum.Small)]
        [TestCase(10, null)]
        public async Task TestCopyFrom(int batchSize, MysqlTypesCEnum? cEnum)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertMysqlTypesBatchArgs { CEnum = cEnum }).ToList();
            await QuerySql.InsertMysqlTypesBatch(batchArgs);
            var expected = new QuerySql.GetMysqlTypesCntRow
            {
                Cnt = batchSize,
                CEnum = cEnum
            };
            var actual = await QuerySql.GetMysqlTypesCnt();
            Assert.That(actual.Value.CEnum, Is.EqualTo(expected.CEnum));
        }
    }
}
