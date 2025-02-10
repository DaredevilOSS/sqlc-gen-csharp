using NpgsqlDapperExampleGen;
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
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote });
            var expected = new QuerySql.GetAuthorRow
            {
                Name = DataGenerator.BojackAuthor,
                Bio = DataGenerator.BojackTheme
            };
            var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = DataGenerator.BojackAuthor });
            Assert.That(SingularEquals(expected, actual));
        }

        private static bool SingularEquals(QuerySql.GetAuthorRow x, QuerySql.GetAuthorRow y)
        {
            return x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
        }

        [Test]
        public async Task TestMany()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote });
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
            var actual = await this.QuerySql.ListAuthors();
            Assert.That(SequenceEquals(expected, actual));
        }

        private static bool SingularEquals(QuerySql.ListAuthorsRow x, QuerySql.ListAuthorsRow y)
        {
            return x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
        }

        private static bool SequenceEquals(List<QuerySql.ListAuthorsRow> x, List<QuerySql.ListAuthorsRow> y)
        {
            if (x.Count != y.Count)
                return false;
            x = x.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Name + o.Bio).ToList();
            y = y.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Name + o.Bio).ToList();
            return !x.Where((t, i) => !SingularEquals(t, y[i])).Any();
        }

        [Test]
        public async Task TestExec()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote });
            await this.QuerySql.DeleteAuthor(new QuerySql.DeleteAuthorArgs { Name = DataGenerator.BojackAuthor });
            var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = DataGenerator.BojackAuthor });
            ClassicAssert.IsNull(actual);
        }

        [Test]
        public async Task TestExecRows()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.GenericAuthor, Bio = DataGenerator.GenericQuote1 });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.GenericAuthor, Bio = DataGenerator.GenericQuote1 });
            var affectedRows = await this.QuerySql.UpdateAuthors(new QuerySql.UpdateAuthorsArgs { Bio = DataGenerator.GenericQuote2 });
            ClassicAssert.AreEqual(2, affectedRows);
            var expected = new List<QuerySql.ListAuthorsRow>
            {
                new QuerySql.ListAuthorsRow
                {
                    Name = DataGenerator.GenericAuthor,
                    Bio = DataGenerator.GenericQuote2
                },
                new QuerySql.ListAuthorsRow
                {
                    Name = DataGenerator.GenericAuthor,
                    Bio = DataGenerator.GenericQuote2
                }
            };
            var actual = await this.QuerySql.ListAuthors();
            Assert.That(SequenceEquals(expected, actual));
        }

        [Test]
        public async Task TestExecLastId()
        {
            var genericId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = DataGenerator.GenericAuthor, Bio = DataGenerator.GenericQuote1 });
            var expected = new QuerySql.GetAuthorByIdRow
            {
                Id = genericId,
                Name = DataGenerator.GenericAuthor,
                Bio = DataGenerator.GenericQuote1
            };
            var actual = await QuerySql.GetAuthorById(new QuerySql.GetAuthorByIdArgs { Id = genericId });
            Assert.That(SingularEquals(expected, actual));
        }

        private static bool SingularEquals(QuerySql.GetAuthorByIdRow x, QuerySql.GetAuthorByIdRow y)
        {
            return x.Id.Equals(y.Id) && x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
        }

        [Test]
        public async Task TestJoinEmbed()
        {
            var bojackId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            await QuerySql.CreateBook(new QuerySql.CreateBookArgs { Name = DataGenerator.BojackBookTitle, AuthorId = bojackId });
            var drSeussId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote });
            await QuerySql.CreateBook(new QuerySql.CreateBookArgs { Name = DataGenerator.DrSeussBookTitle, AuthorId = drSeussId });
            var expected = new List<QuerySql.ListAllAuthorsBooksRow>()
            {
                new QuerySql.ListAllAuthorsBooksRow
                {
                    Author = new Author
                    {
                        Id = bojackId,
                        Name = DataGenerator.BojackAuthor,
                        Bio = DataGenerator.BojackTheme
                    },
                    Book = new Book
                    {
                        AuthorId = bojackId,
                        Name = DataGenerator.BojackBookTitle
                    }
                },
                new QuerySql.ListAllAuthorsBooksRow
                {
                    Author = new Author
                    {
                        Id = drSeussId,
                        Name = DataGenerator.DrSeussAuthor,
                        Bio = DataGenerator.DrSeussQuote
                    },
                    Book = new Book
                    {
                        AuthorId = drSeussId,
                        Name = DataGenerator.DrSeussBookTitle
                    }
                }
            };
            var actual = await QuerySql.ListAllAuthorsBooks();
            Assert.That(SequenceEquals(expected, actual));
        }

        private static bool SingularEquals(QuerySql.ListAllAuthorsBooksRow x, QuerySql.ListAllAuthorsBooksRow y)
        {
            return x.Author.Id.Equals(y.Author.Id) && x.Author.Name.Equals(y.Author.Name) && x.Author.Bio.Equals(y.Author.Bio) && x.Book.AuthorId.Equals(y.Book.AuthorId) && x.Book.Name.Equals(y.Book.Name);
        }

        private static bool SequenceEquals(List<QuerySql.ListAllAuthorsBooksRow> x, List<QuerySql.ListAllAuthorsBooksRow> y)
        {
            if (x.Count != y.Count)
                return false;
            x = x.OrderBy<QuerySql.ListAllAuthorsBooksRow, object>(o => o.Author.Name + o.Book.Name).ToList();
            y = y.OrderBy<QuerySql.ListAllAuthorsBooksRow, object>(o => o.Author.Name + o.Book.Name).ToList();
            return !x.Where((t, i) => !SingularEquals(t, y[i])).Any();
        }

        [Test]
        public async Task TestSelfJoinEmbed()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            var expected = new List<QuerySql.GetDuplicateAuthorsRow>()
            {
                new QuerySql.GetDuplicateAuthorsRow
                {
                    Author = new Author
                    {
                        Name = DataGenerator.BojackAuthor,
                        Bio = DataGenerator.BojackTheme
                    },
                    Author2 = new Author
                    {
                        Name = DataGenerator.BojackAuthor,
                        Bio = DataGenerator.BojackTheme
                    }
                }
            };
            var actual = await QuerySql.GetDuplicateAuthors();
            Assert.That(SequenceEquals(expected, actual));
            Assert.That(actual[0].Author.Id != actual[0].Author2.Id);
        }

        private static bool SingularEquals(QuerySql.GetDuplicateAuthorsRow x, QuerySql.GetDuplicateAuthorsRow y)
        {
            return x.Author.Name.Equals(y.Author.Name) && x.Author.Bio.Equals(y.Author.Bio) && x.Author2.Name.Equals(y.Author2.Name) && x.Author2.Bio.Equals(y.Author2.Bio);
        }

        private static bool SequenceEquals(List<QuerySql.GetDuplicateAuthorsRow> x, List<QuerySql.GetDuplicateAuthorsRow> y)
        {
            if (x.Count != y.Count)
                return false;
            x = x.OrderBy<QuerySql.GetDuplicateAuthorsRow, object>(o => o.Author.Name + o.Author2.Name).ToList();
            y = y.OrderBy<QuerySql.GetDuplicateAuthorsRow, object>(o => o.Author.Name + o.Author2.Name).ToList();
            return !x.Where((t, i) => !SingularEquals(t, y[i])).Any();
        }

        [Test]
        public async Task TestArray()
        {
            var genericId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = DataGenerator.GenericAuthor, Bio = DataGenerator.GenericQuote1 });
            var bojackId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            var actual = await QuerySql.GetAuthorsByIds(new QuerySql.GetAuthorsByIdsArgs { LongArr1 = new[] { genericId, bojackId } });
            ClassicAssert.AreEqual(2, actual.Count);
        }

        [Test]
        public async Task TestMultipleArrays()
        {
            var genericId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = DataGenerator.GenericAuthor, Bio = DataGenerator.GenericQuote1 });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.GenericAuthor, Bio = DataGenerator.GenericQuote1 });
            var bojackId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            var actual = await QuerySql.GetAuthorsByIdsAndNames(new QuerySql.GetAuthorsByIdsAndNamesArgs { LongArr1 = new[] { genericId, bojackId }, StringArr2 = new[] { DataGenerator.GenericAuthor } });
            ClassicAssert.AreEqual(1, actual.Count);
        }

        [Test]
        public async Task TestCopyFrom()
        {
            const int batchSize = 100;
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresTypesBatchArgs { CBoolean = true, CSmallint = 3, CInteger = 1, CBigint = 14214231, CDecimal = 1.2f, CNumeric = 8.4f, CReal = 1.432423f, CVarchar = "abc", CDate = new DateTime(2020, 7, 22, 11, 7, 45, 35), CTimestamp = new DateTime(2020, 7, 22, 11, 7, 45, 35) }).ToList();
            await QuerySql.InsertPostgresTypesBatch(batchArgs);
            var expected = new QuerySql.GetPostgresTypesAggRow
            {
                Cnt = batchSize,
                CBoolean = true,
                CSmallint = 3,
                CInteger = 1,
                CBigint = 14214231,
                CDecimal = 1.2f,
                CNumeric = 8.4f,
                CReal = 1.432423f,
                CVarchar = "abc",
                CDate = new DateTime(2020, 7, 22),
                CTimestamp = new DateTime(2020, 7, 22, 11, 7, 45, 35)
            };
            var actual = await QuerySql.GetPostgresTypesAgg();
            Assert.That(SingularEquals(expected, actual));
        }

        private static bool SingularEquals(QuerySql.GetPostgresTypesAggRow x, QuerySql.GetPostgresTypesAggRow y)
        {
            return x.Cnt.Equals(y.Cnt) && x.CSmallint.Equals(y.CSmallint) && x.CBoolean.Equals(y.CBoolean) && x.CInteger.Equals(y.CInteger) && x.CBigint.Equals(y.CBigint) && x.CDecimal.Equals(y.CDecimal) && x.CNumeric.Equals(y.CNumeric) && x.CReal.Equals(y.CReal) && x.CVarchar.Equals(y.CVarchar) && x.CDate.Equals(y.CDate) && x.CTimestamp.Equals(y.CTimestamp);
        }

        [Test]
        public async Task TestPostgresTypes()
        {
            var insertedId = await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs { CBigint = 1, CReal = 1.0f, CNumeric = 1, CSerial = 1, CSmallint = 1, CDecimal = 1, CDate = DateTime.Now, CTimestamp = DateTime.Now, CBoolean = true, CChar = "a", CInteger = 1, CText = "ab", CVarchar = "abc", CCharacterVarying = "abcd", CTextArray = new string[] { "a", "b" }, CIntegerArray = new int[] { 1, 2 } });
            var expected = new QuerySql.GetPostgresTypesRow
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
                CTextArray = new string[]
                {
                    "a",
                    "b"
                },
                CIntegerArray = new int[]
                {
                    1,
                    2
                }
            };
            var actual = await QuerySql.GetPostgresTypes(new QuerySql.GetPostgresTypesArgs { Id = insertedId });
            Assert.That(SingularEquals(expected, actual));
        }

        private static bool SingularEquals(QuerySql.GetPostgresTypesRow x, QuerySql.GetPostgresTypesRow y)
        {
            return x.CSmallint.Equals(y.CSmallint) && x.CBoolean.Equals(y.CBoolean) && x.CInteger.Equals(y.CInteger) && x.CBigint.Equals(y.CBigint) && x.CSerial.Equals(y.CSerial) && x.CDecimal.Equals(y.CDecimal) && x.CNumeric.Equals(y.CNumeric) && x.CReal.Equals(y.CReal) && x.CChar.Equals(y.CChar) && x.CVarchar.Equals(y.CVarchar) && x.CCharacterVarying.Equals(y.CCharacterVarying) && x.CText.Equals(y.CText) && x.CTextArray.SequenceEqual(y.CTextArray) && x.CIntegerArray.SequenceEqual(y.CIntegerArray);
        }
    }
}
