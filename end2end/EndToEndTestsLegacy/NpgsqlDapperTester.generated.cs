using NpgsqlDapperLegacyExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
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
        [TestCase(100, true, 3, 453, -1445214231, 666.6f, 336.3431, -99.999, -1377.996, -43242.43, "1973-12-3", "1960-11-3 02:01:22", "2030-07-20 15:44:01+09:00", "z", "Sex Pistols", "Anarchy in the U.K", "Never Mind the Bollocks...", new byte[] { 0x53, 0x56 })]
        [TestCase(500, false, -4, 867, 8768769709, -64.8f, -324.8671, 127.4793, 423.9869, 32143.99, "2024-12-31", "1999-3-1 03:00:10", "1999-9-13 08:30:11-04:00", "1", "Fugazi", "Waiting Room", "13 Songs", new byte[] { 0x03 })]
        [TestCase(10, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, new byte[] { })]
        [TestCase(10, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)]
        public async Task TestCopyFrom(int batchSize, bool cBoolean, short cSmallint, int cInteger, long cBigint, float cReal, decimal cDecimal, decimal cNumeric, double cDoublePrecision, decimal cMoney, DateTime? cDate, DateTime? cTimestamp, DateTime? cTimestampWithTz, string cChar, string cVarchar, string cCharacterVarying, string cText, byte[] cBytea)
        {
            DateTime? cTimestampWithTzAsUtc = null;
            if (cTimestampWithTz != null)
                cTimestampWithTzAsUtc = DateTime.SpecifyKind(cTimestampWithTz.Value, DateTimeKind.Utc);
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresTypesBatchArgs { CBoolean = cBoolean, CSmallint = cSmallint, CInteger = cInteger, CBigint = cBigint, CReal = cReal, CDecimal = cDecimal, CNumeric = cNumeric, CDoublePrecision = cDoublePrecision, CMoney = cMoney, CDate = cDate, CTimestamp = cTimestamp, CTimestampWithTz = cTimestampWithTzAsUtc, CChar = cChar, CVarchar = cVarchar, CCharacterVarying = cCharacterVarying, CText = cText, CBytea = cBytea }).ToList();
            await QuerySql.InsertPostgresTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresTypesAggRow
            {
                Cnt = batchSize,
                CBoolean = cBoolean,
                CSmallint = cSmallint,
                CInteger = cInteger,
                CBigint = cBigint,
                CReal = cReal,
                CDecimal = cDecimal,
                CNumeric = cNumeric,
                CDoublePrecision = cDoublePrecision,
                CMoney = cMoney,
                CDate = cDate,
                CTimestamp = cTimestamp,
                CTimestampWithTz = cTimestampWithTz,
                CChar = cChar,
                CVarchar = cVarchar,
                CCharacterVarying = cCharacterVarying,
                CText = cText,
                CBytea = cBytea
            };
            var actual = await QuerySql.GetPostgresTypesAgg();
            AssertSingularEquals(expected, actual);
        }

        private static void AssertSingularEquals(QuerySql.GetPostgresTypesAggRow expected, QuerySql.GetPostgresTypesAggRow actual)
        {
            Assert.That(actual.Cnt, Is.EqualTo(expected.Cnt));
            Assert.That(actual.CBoolean, Is.EqualTo(expected.CBoolean));
            Assert.That(actual.CSmallint, Is.EqualTo(expected.CSmallint));
            Assert.That(actual.CInteger, Is.EqualTo(expected.CInteger));
            Assert.That(actual.CBigint, Is.EqualTo(expected.CBigint));
            Assert.That(actual.CReal, Is.EqualTo(expected.CReal));
            Assert.That(actual.CDecimal, Is.EqualTo(expected.CDecimal));
            Assert.That(actual.CNumeric, Is.EqualTo(expected.CNumeric));
            Assert.That(actual.CDoublePrecision, Is.EqualTo(expected.CDoublePrecision));
            Assert.That(actual.CMoney, Is.EqualTo(expected.CMoney));
            Assert.That(actual.CDate, Is.EqualTo(expected.CDate));
            Assert.That(actual.CTimestamp, Is.EqualTo(expected.CTimestamp));
            Assert.That(actual.CTimestampWithTz, Is.EqualTo(expected.CTimestampWithTz));
            Assert.That(actual.CChar, Is.EqualTo(expected.CChar));
            Assert.That(actual.CVarchar, Is.EqualTo(expected.CVarchar));
            Assert.That(actual.CCharacterVarying, Is.EqualTo(expected.CCharacterVarying));
            Assert.That(actual.CText, Is.EqualTo(expected.CText));
            Assert.That(actual.CBytea, Is.EqualTo(expected.CBytea));
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
        [TestCase(true, 35, -23423, 4235235263, 3.83f, 4.5534, 998.432, -8403284.321435, 42332.53, "2000-1-30", "1983-11-3 02:01:22", "2022-10-2 15:44:01+09:00", "E", "It takes a nation of millions to hold us back", "Rebel Without a Pause", "Prophets of Rage", new byte[] { 0x45, 0x42 }, new string[] { "Party", "Fight" }, new int[] { 543, -4234 })]
        [TestCase(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, new byte[] { }, new string[] { }, new int[] { })]
        [TestCase(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)]
        public async Task TestPostgresTypes(bool cBoolean, short cSmallint, int cInteger, long cBigint, float cReal, decimal cNumeric, decimal cDecimal, double cDoublePrecision, decimal cMoney, DateTime cDate, DateTime cTimestamp, DateTime cTimestampWithTz, string cChar, string cVarchar, string cCharacterVarying, string cText, byte[] cBytea, string[] cTextArray, int[] cIntegerArray)
        {
            await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CBoolean = cBoolean, CSmallint = cSmallint, CInteger = cInteger, CBigint = cBigint, CReal = cReal, CNumeric = cNumeric, CDecimal = cDecimal, CDoublePrecision = cDoublePrecision, CMoney = cMoney, CDate = cDate, CTimestamp = cTimestamp, CTimestampWithTz = cTimestampWithTz, CChar = cChar, CVarchar = cVarchar, CCharacterVarying = cCharacterVarying, CText = cText, CBytea = cBytea, CTextArray = cTextArray, CIntegerArray = cIntegerArray });
            var expected = new QuerySql.GetPostgresTypesRow
            {
                CBoolean = cBoolean,
                CSmallint = cSmallint,
                CInteger = cInteger,
                CBigint = cBigint,
                CReal = cReal,
                CNumeric = cNumeric,
                CDecimal = cDecimal,
                CDoublePrecision = cDoublePrecision,
                CMoney = cMoney,
                CDate = cDate,
                CTimestamp = cTimestamp,
                CTimestampWithTz = cTimestampWithTz,
                CChar = cChar,
                CVarchar = cVarchar,
                CCharacterVarying = cCharacterVarying,
                CText = cText,
                CBytea = cBytea,
                CTextArray = cTextArray,
                CIntegerArray = cIntegerArray
            };
            var actual = await QuerySql.GetPostgresTypes();
            AssertSingularEquals(expected, actual);
        }

        private static void AssertSingularEquals(QuerySql.GetPostgresTypesRow expected, QuerySql.GetPostgresTypesRow actual)
        {
            Assert.That(actual.CBoolean, Is.EqualTo(expected.CBoolean));
            Assert.That(actual.CSmallint, Is.EqualTo(expected.CSmallint));
            Assert.That(actual.CInteger, Is.EqualTo(expected.CInteger));
            Assert.That(actual.CBigint, Is.EqualTo(expected.CBigint));
            Assert.That(actual.CReal, Is.EqualTo(expected.CReal));
            Assert.That(actual.CNumeric, Is.EqualTo(expected.CNumeric));
            Assert.That(actual.CDecimal, Is.EqualTo(expected.CDecimal));
            Assert.That(actual.CDoublePrecision, Is.EqualTo(expected.CDoublePrecision));
            Assert.That(actual.CMoney, Is.EqualTo(expected.CMoney));
            Assert.That(actual.CDate, Is.EqualTo(expected.CDate));
            Assert.That(actual.CTimestamp, Is.EqualTo(expected.CTimestamp));
            Assert.That(actual.CTimestampWithTz, Is.EqualTo(expected.CTimestampWithTz));
            Assert.That(actual.CChar, Is.EqualTo(expected.CChar));
            Assert.That(actual.CVarchar, Is.EqualTo(expected.CVarchar));
            Assert.That(actual.CCharacterVarying, Is.EqualTo(expected.CCharacterVarying));
            Assert.That(actual.CText, Is.EqualTo(expected.CText));
            Assert.That(actual.CBytea, Is.EqualTo(expected.CBytea));
            Assert.That(actual.CTextArray, Is.EqualTo(expected.CTextArray));
            Assert.That(actual.CIntegerArray, Is.EqualTo(expected.CIntegerArray));
        }
    }
}
