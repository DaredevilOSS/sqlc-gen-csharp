using NpgsqlDapperExampleGen;
using NpgsqlTypes;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Xml;
using NodaTime;
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
        public async Task TestOneAsync()
        {
            await this.QuerySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            await this.QuerySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 2222, Name = "Dr. Seuss", Bio = "You'll miss the best things if you keep your eyes shut" });
            var expected = new QuerySql.GetAuthorRow
            {
                Id = 1111,
                Name = "Bojack Horseman",
                Bio = "Back in the 90s he was in a very famous TV show"
            };
            var actual = await this.QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetAuthorRow x, QuerySql.GetAuthorRow y)
            {
                Assert.That(x.Id, Is.EqualTo(y.Id));
                Assert.That(x.Name, Is.EqualTo(y.Name));
                Assert.That(x.Bio, Is.EqualTo(y.Bio));
            }
        }

        [Test]
        public async Task TestManyAsync()
        {
            await this.QuerySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            await this.QuerySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 2222, Name = "Dr. Seuss", Bio = "You'll miss the best things if you keep your eyes shut" });
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
            var actual = await this.QuerySql.ListAuthorsAsync(new QuerySql.ListAuthorsArgs { Limit = 2, Offset = 0 });
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
        public async Task TestExecAsync()
        {
            await this.QuerySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            await this.QuerySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 2222, Name = "Dr. Seuss", Bio = "You'll miss the best things if you keep your eyes shut" });
            await this.QuerySql.DeleteAuthorAsync(new QuerySql.DeleteAuthorArgs { Name = "Bojack Horseman" });
            var actual = await this.QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            ClassicAssert.IsNull(actual);
        }

        [Test]
        public async Task TestExecRowsAsync()
        {
            await this.QuerySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            await this.QuerySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 2222, Name = "Dr. Seuss", Bio = "You'll miss the best things if you keep your eyes shut" });
            var affectedRows = await this.QuerySql.UpdateAuthorsAsync(new QuerySql.UpdateAuthorsArgs { Bio = "Quote that everyone always attribute to Einstein" });
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
            var actual = await this.QuerySql.ListAuthorsAsync(new QuerySql.ListAuthorsArgs { Limit = 2, Offset = 0 });
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
        public async Task TestExecLastIdAsync()
        {
            var id1 = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Quote that everyone always attribute to Einstein" });
            var expected = new QuerySql.GetAuthorByIdRow
            {
                Id = id1,
                Name = "Albert Einstein",
                Bio = "Quote that everyone always attribute to Einstein"
            };
            var actual = await QuerySql.GetAuthorByIdAsync(new QuerySql.GetAuthorByIdArgs { Id = id1 });
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetAuthorByIdRow x, QuerySql.GetAuthorByIdRow y)
            {
                Assert.That(x.Id, Is.EqualTo(y.Id));
                Assert.That(x.Name, Is.EqualTo(y.Name));
                Assert.That(x.Bio, Is.EqualTo(y.Bio));
            }
        }

        [Test]
        public async Task TestSelfJoinEmbedAsync()
        {
            var id1 = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Quote that everyone always attribute to Einstein" });
            var id2 = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Only 2 things are infinite, the universe and human stupidity" });
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
            var actual = await QuerySql.GetDuplicateAuthorsAsync();
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
        public async Task TestJoinEmbedAsync()
        {
            var bojackId = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            var bojackBookId = await QuerySql.CreateBookAsync(new QuerySql.CreateBookArgs { Name = "One Trick Pony", AuthorId = bojackId });
            var drSeussId = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Dr. Seuss", Bio = "You'll miss the best things if you keep your eyes shut" });
            var drSeussBookId = await QuerySql.CreateBookAsync(new QuerySql.CreateBookArgs { AuthorId = drSeussId, Name = "How the Grinch Stole Christmas!" });
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
            var actual = await QuerySql.ListAllAuthorsBooksAsync();
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
        public async Task TestPartialEmbedAsync()
        {
            var bojackId = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            var bojackBookId = await QuerySql.CreateBookAsync(new QuerySql.CreateBookArgs { Name = "One Trick Pony", AuthorId = bojackId });
            var drSeussId = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Dr. Seuss", Bio = "You'll miss the best things if you keep your eyes shut" });
            var drSeussBookId = await QuerySql.CreateBookAsync(new QuerySql.CreateBookArgs { AuthorId = drSeussId, Name = "How the Grinch Stole Christmas!" });
            var expected = new List<QuerySql.GetAuthorsByBookNameRow>
            {
                new QuerySql.GetAuthorsByBookNameRow
                {
                    Id = bojackId,
                    Name = "Bojack Horseman",
                    Bio = "Back in the 90s he was in a very famous TV show",
                    Book = new Book
                    {
                        Id = bojackBookId,
                        AuthorId = bojackId,
                        Name = "One Trick Pony"
                    }
                }
            };
            var actual = await QuerySql.GetAuthorsByBookNameAsync(new QuerySql.GetAuthorsByBookNameArgs { Name = "One Trick Pony" });
            AssertSequenceEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetAuthorsByBookNameRow x, QuerySql.GetAuthorsByBookNameRow y)
            {
                Assert.That(x.Id, Is.EqualTo(y.Id));
                Assert.That(x.Name, Is.EqualTo(y.Name));
                Assert.That(x.Bio, Is.EqualTo(y.Bio));
                Assert.That(x.Book.Id, Is.EqualTo(y.Book.Id));
                Assert.That(x.Book.AuthorId, Is.EqualTo(y.Book.AuthorId));
                Assert.That(x.Book.Name, Is.EqualTo(y.Book.Name));
            }

            void AssertSequenceEquals(List<QuerySql.GetAuthorsByBookNameRow> x, List<QuerySql.GetAuthorsByBookNameRow> y)
            {
                Assert.That(x.Count, Is.EqualTo(y.Count));
                for (int i = 0; i < x.Count; i++)
                    AssertSingularEquals(x[i], y[i]);
            }
        }

        [Test]
        public async Task TestNargNullAsync()
        {
            await this.QuerySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            await this.QuerySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 2222, Name = "Dr. Seuss", Bio = "You'll miss the best things if you keep your eyes shut" });
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
            var actual = await this.QuerySql.GetAuthorByNamePatternAsync(new QuerySql.GetAuthorByNamePatternArgs());
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
        public async Task TestNargNotNullAsync()
        {
            await this.QuerySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            await this.QuerySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 2222, Name = "Dr. Seuss", Bio = "You'll miss the best things if you keep your eyes shut" });
            var expected = new List<QuerySql.GetAuthorByNamePatternRow>
            {
                new QuerySql.GetAuthorByNamePatternRow
                {
                    Id = 1111,
                    Name = "Bojack Horseman",
                    Bio = "Back in the 90s he was in a very famous TV show"
                }
            };
            var actual = await this.QuerySql.GetAuthorByNamePatternAsync(new QuerySql.GetAuthorByNamePatternArgs { NamePattern = "Bojack%" });
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
        public async Task TestPostgresTransactionAsync()
        {
            var connection = new Npgsql.NpgsqlConnection(Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv));
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();
            var querySqlWithTx = QuerySql.WithTransaction(transaction);
            await querySqlWithTx.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            var actual = await QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            ClassicAssert.IsNull(actual);
            await transaction.CommitAsync();
            var expected = new QuerySql.GetAuthorRow
            {
                Id = 1111,
                Name = "Bojack Horseman",
                Bio = "Back in the 90s he was in a very famous TV show"
            };
            actual = await QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetAuthorRow x, QuerySql.GetAuthorRow y)
            {
                Assert.That(x.Id, Is.EqualTo(y.Id));
                Assert.That(x.Name, Is.EqualTo(y.Name));
                Assert.That(x.Bio, Is.EqualTo(y.Bio));
            }
        }

        [Test]
        public async Task TestPostgresTransactionRollbackAsync()
        {
            var connection = new Npgsql.NpgsqlConnection(Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv));
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();
            var sqlQueryWithTx = QuerySql.WithTransaction(transaction);
            await sqlQueryWithTx.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            await transaction.RollbackAsync();
            var actual = await QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            ClassicAssert.IsNull(actual);
        }

        [Test]
        public async Task TestArrayAsync()
        {
            var id1 = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Quote that everyone always attribute to Einstein" });
            var bojackId = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            var actual = await QuerySql.GetAuthorsByIdsAsync(new QuerySql.GetAuthorsByIdsArgs { LongArr1 = new[] { id1, bojackId } });
            ClassicAssert.AreEqual(2, actual.Count);
        }

        [Test]
        public async Task TestMultipleArraysAsync()
        {
            var id1 = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Quote that everyone always attribute to Einstein" });
            var id2 = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Only 2 things are infinite, the universe and human stupidity" });
            var bojackId = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            var actual = await QuerySql.GetAuthorsByIdsAndNamesAsync(new QuerySql.GetAuthorsByIdsAndNamesArgs { LongArr1 = new[] { id1, bojackId }, StringArr2 = new[] { "Albert Einstein" } });
            ClassicAssert.AreEqual(1, actual.Count);
        }

        [Test]
        [TestCase(-54355, "White Light from the Mouth of Infinity", "2022-10-2 15:44:01+09:00")]
        [TestCase(null, null, "1970-01-01 00:00:00")]
        public async Task TestPostgresDataTypesOverrideAsync(int? cInteger, string cVarchar, DateTime cTimestamp)
        {
            await QuerySql.InsertPostgresNumericTypesAsync(new QuerySql.InsertPostgresNumericTypesArgs { CInteger = cInteger });
            await QuerySql.InsertPostgresDateTimeTypesAsync(new QuerySql.InsertPostgresDateTimeTypesArgs { CTimestamp = cTimestamp });
            await QuerySql.InsertPostgresStringTypesAsync(new QuerySql.InsertPostgresStringTypesArgs { CVarchar = cVarchar });
            var expected = new QuerySql.GetPostgresFunctionsRow
            {
                MaxInteger = cInteger,
                MaxVarchar = cVarchar,
                MaxTimestamp = cTimestamp
            };
            var actual = await QuerySql.GetPostgresFunctionsAsync();
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
            Assert.ThrowsAsync<Npgsql.PostgresException>(async () => await QuerySql.InsertPostgresSpecialTypesAsync(new QuerySql.InsertPostgresSpecialTypesArgs { CJsonStringOverride = "SOME INVALID JSON" }));
            Assert.ThrowsAsync<Npgsql.PostgresException>(async () => await QuerySql.InsertPostgresSpecialTypesAsync(new QuerySql.InsertPostgresSpecialTypesArgs { CJsonpath = "SOME INVALID JSONPATH" }));
        }

        [Test]
        public void TestPostgresInvalidXml()
        {
            Assert.ThrowsAsync<Npgsql.PostgresException>(async () => await QuerySql.InsertPostgresSpecialTypesAsync(new QuerySql.InsertPostgresSpecialTypesArgs { CXmlStringOverride = "<root>SOME INVALID XML" }));
        }

        [Test]
        [TestCase("E", "It takes a nation of millions to hold us back", "Rebel Without a Pause", "Master of Puppets", "Prophets of Rage")]
        [TestCase(null, null, null, null, null)]
        public async Task TestPostgresStringTypesAsync(string cChar, string cVarchar, string cCharacterVarying, string cBpchar, string cText)
        {
            await QuerySql.InsertPostgresStringTypesAsync(new QuerySql.InsertPostgresStringTypesArgs { CChar = cChar, CVarchar = cVarchar, CCharacterVarying = cCharacterVarying, CBpchar = cBpchar, CText = cText, });
            var expected = new QuerySql.GetPostgresStringTypesRow
            {
                CChar = cChar,
                CVarchar = cVarchar,
                CCharacterVarying = cCharacterVarying,
                CBpchar = cBpchar,
                CText = cText,
            };
            var actual = await QuerySql.GetPostgresStringTypesAsync();
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
        public async Task TestPostgresIntegerTypesAsync(bool cBoolean, short cSmallint, int cInteger, long cBigint)
        {
            await QuerySql.InsertPostgresNumericTypesAsync(new QuerySql.InsertPostgresNumericTypesArgs { CBoolean = cBoolean, CSmallint = cSmallint, CInteger = cInteger, CBigint = cBigint });
            var expected = new QuerySql.GetPostgresNumericTypesRow
            {
                CBoolean = cBoolean,
                CSmallint = cSmallint,
                CInteger = cInteger,
                CBigint = cBigint
            };
            var actual = await QuerySql.GetPostgresNumericTypesAsync();
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
        public async Task TestPostgresFloatingPointTypesAsync(float? cReal, decimal? cNumeric, decimal? cDecimal, double? cDoublePrecision, decimal? cMoney)
        {
            await QuerySql.InsertPostgresNumericTypesAsync(new QuerySql.InsertPostgresNumericTypesArgs { CReal = cReal, CNumeric = cNumeric, CDecimal = cDecimal, CDoublePrecision = cDoublePrecision, CMoney = cMoney });
            var expected = new QuerySql.GetPostgresNumericTypesRow
            {
                CReal = cReal,
                CNumeric = cNumeric,
                CDecimal = cDecimal,
                CDoublePrecision = cDoublePrecision,
                CMoney = cMoney
            };
            var actual = await QuerySql.GetPostgresNumericTypesAsync();
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

        private static IEnumerable<TestCaseData> PostgresDateTimeTypesTestCases
        {
            get
            {
                yield return new TestCaseData(DateTime.Parse("2000-1-30"), TimeSpan.Parse("12:13:14"), DateTime.Parse("1983-11-3 02:01:22"), DateTime.Parse("2022-10-2 15:44:01+09:00").ToUniversalTime(), TimeSpan.Parse("02:03:04"), Instant.FromUtc(2022, 10, 2, 15, 44, 1)).SetName("DateTimeTypes with values");
                yield return new TestCaseData(null, null, null, null, null, null).SetName("DateTimeTypes with null values");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresDateTimeTypesTestCases))]
        public async Task TestPostgresDateTimeTypesAsync(DateTime? cDate, TimeSpan? cTime, DateTime? cTimestamp, DateTime? cTimestampWithTz, TimeSpan? cInterval, Instant? cTimestampNodaInstantOverride)
        {
            await QuerySql.InsertPostgresDateTimeTypesAsync(new QuerySql.InsertPostgresDateTimeTypesArgs { CDate = cDate, CTime = cTime, CTimestamp = cTimestamp, CTimestampWithTz = cTimestampWithTz, CInterval = cInterval, CTimestampNodaInstantOverride = cTimestampNodaInstantOverride });
            var expected = new QuerySql.GetPostgresDateTimeTypesRow
            {
                CDate = cDate,
                CTime = cTime,
                CTimestamp = cTimestamp,
                CTimestampWithTz = cTimestampWithTz,
                CInterval = cInterval,
                CTimestampNodaInstantOverride = cTimestampNodaInstantOverride
            };
            var actual = await QuerySql.GetPostgresDateTimeTypesAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresDateTimeTypesRow x, QuerySql.GetPostgresDateTimeTypesRow y)
            {
                Assert.That(x.CDate, Is.EqualTo(y.CDate));
                Assert.That(x.CTime, Is.EqualTo(y.CTime));
                Assert.That(x.CTimestamp, Is.EqualTo(y.CTimestamp));
                Assert.That(x.CTimestampWithTz, Is.EqualTo(y.CTimestampWithTz));
                Assert.That(x.CInterval, Is.EqualTo(y.CInterval));
                Assert.That(x.CTimestampNodaInstantOverride, Is.EqualTo(y.CTimestampNodaInstantOverride));
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
        public async Task TestPostgresArrayTypesAsync(byte[] cBytea, bool[] cBooleanArray, string[] cTextArray, int[] cIntegerArray, decimal[] cDecimalArray, DateTime[] cDateArray, DateTime[] cTimestampArray)
        {
            await QuerySql.InsertPostgresArrayTypesAsync(new QuerySql.InsertPostgresArrayTypesArgs { CBytea = cBytea, CBooleanArray = cBooleanArray, CTextArray = cTextArray, CIntegerArray = cIntegerArray, CDecimalArray = cDecimalArray, CDateArray = cDateArray, CTimestampArray = cTimestampArray });
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
            var actual = await QuerySql.GetPostgresArrayTypesAsync();
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
        public async Task TestPostgresGuidDataTypesAsync(Guid? cUuid)
        {
            await QuerySql.InsertPostgresSpecialTypesAsync(new QuerySql.InsertPostgresSpecialTypesArgs { CUuid = cUuid });
            var expected = new QuerySql.GetPostgresSpecialTypesRow
            {
                CUuid = cUuid
            };
            var actual = await QuerySql.GetPostgresSpecialTypesAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresSpecialTypesRow x, QuerySql.GetPostgresSpecialTypesRow y)
            {
                Assert.That(x.CUuid, Is.EqualTo(y.CUuid));
            }
        }

        [Test]
        [Obsolete] // due to NpgsqlTsVector.Parse usage
        public async Task TestPostgresFullTextSearchDataTypesAsync()
        {
            await QuerySql.InsertPostgresStringTypesAsync(new QuerySql.InsertPostgresStringTypesArgs { CText = "Hello world" });
            var actual = await QuerySql.GetPostgresStringTypesTextSearchAsync(new QuerySql.GetPostgresStringTypesTextSearchArgs { ToTsquery = "Hello" });
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
        public async Task TestPostgresNetworkDataTypesAsync(NpgsqlCidr? cCidr, IPAddress cInet, PhysicalAddress cMacaddr, string cMacaddr8)
        {
            await QuerySql.InsertPostgresNetworkTypesAsync(new QuerySql.InsertPostgresNetworkTypesArgs { CCidr = cCidr, CInet = cInet, CMacaddr = cMacaddr, CMacaddr8 = cMacaddr8 });
            var expected = new QuerySql.GetPostgresNetworkTypesRow
            {
                CCidr = cCidr,
                CInet = cInet,
                CMacaddr = cMacaddr,
                CMacaddr8 = cMacaddr8
            };
            var actual = await QuerySql.GetPostgresNetworkTypesAsync();
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
        public async Task TestPostgresGeoTypesAsync(NpgsqlPoint? cPoint, NpgsqlLine? cLine, NpgsqlLSeg? cLSeg, NpgsqlBox? cBox, NpgsqlPath? cPath, NpgsqlPolygon? cPolygon, NpgsqlCircle? cCircle)
        {
            await QuerySql.InsertPostgresGeoTypesAsync(new QuerySql.InsertPostgresGeoTypesArgs { CPoint = cPoint, CLine = cLine, CLseg = cLSeg, CBox = cBox, CPath = cPath, CPolygon = cPolygon, CCircle = cCircle });
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
            var actual = await QuerySql.GetPostgresGeoTypesAsync();
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
        [TestCase("{\"name\": \"Swordfishtrombones\", \"year\" :1983}", "$.\"name\"")]
        [TestCase(null, null)]
        public async Task TestPostgresJsonDataTypesAsync(string cJson, string cJsonpath)
        {
            JsonElement? cParsedJson = null;
            if (cJson != null)
                cParsedJson = JsonDocument.Parse(cJson).RootElement;
            await QuerySql.InsertPostgresSpecialTypesAsync(new QuerySql.InsertPostgresSpecialTypesArgs { CJson = cParsedJson, CJsonb = cParsedJson, CJsonStringOverride = cJson, CJsonpath = cJsonpath });
            var expected = new QuerySql.GetPostgresSpecialTypesRow
            {
                CJson = cParsedJson,
                CJsonb = cParsedJson,
                CJsonStringOverride = cJson,
                CJsonpath = cJsonpath
            };
            var actual = await QuerySql.GetPostgresSpecialTypesAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresSpecialTypesRow x, QuerySql.GetPostgresSpecialTypesRow y)
            {
                AssertJsonElementEquals(y.CJson, x.CJson);
                AssertJsonElementEquals(y.CJsonb, x.CJsonb);
                Assert.That(y.CJsonStringOverride, Is.EqualTo(x.CJsonStringOverride));
                Assert.That(y.CJsonpath, Is.EqualTo(x.CJsonpath));
            }

            void AssertJsonElementEquals(JsonElement? x, JsonElement? y)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = false
                };
                Assert.That(y.HasValue, Is.EqualTo(x.HasValue));
                if (y.HasValue)
                    Assert.That(JsonSerializer.Serialize(y.Value, options), Is.EqualTo(JsonSerializer.Serialize(x.Value, options)));
            }
        }

        [Test]
        [TestCase("<root><child>Good morning xml, the world says hello</child></root>")]
        [TestCase(null)]
        public async Task TestPostgresXmlDataTypesAsync(string cXml)
        {
            XmlDocument parsedXml = null;
            if (cXml != null)
            {
                parsedXml = new XmlDocument();
                parsedXml.LoadXml(cXml);
            }

            await QuerySql.InsertPostgresSpecialTypesAsync(new QuerySql.InsertPostgresSpecialTypesArgs { CXml = parsedXml });
            var expected = new QuerySql.GetPostgresSpecialTypesRow
            {
                CXml = parsedXml
            };
            var actual = await QuerySql.GetPostgresSpecialTypesAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresSpecialTypesRow x, QuerySql.GetPostgresSpecialTypesRow y)
            {
                Assert.That(x.CXml == null, Is.EqualTo(y.CXml == null));
                if (x.CXml != null)
                    Assert.That(x.CXml.OuterXml, Is.EqualTo(y.CXml.OuterXml));
            }
        }

        [Test]
        [TestCase(CEnum.Small)]
        [TestCase(CEnum.Medium)]
        [TestCase(CEnum.Big)]
        [TestCase(null)]
        public async Task TestPostgresEnumTypesAsync(CEnum? cEnum)
        {
            await QuerySql.InsertPostgresSpecialTypesAsync(new QuerySql.InsertPostgresSpecialTypesArgs { CEnum = cEnum });
            var expected = new QuerySql.GetPostgresSpecialTypesRow
            {
                CEnum = cEnum
            };
            var actual = await QuerySql.GetPostgresSpecialTypesAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresSpecialTypesRow x, QuerySql.GetPostgresSpecialTypesRow y)
            {
                Assert.That(x.CEnum, Is.EqualTo(y.CEnum));
            }
        }

        [Test]
        [TestCase(CEnum.Small)]
        public async Task TestPostgresNotNullTypesAsync(CEnum cEnumNotNull)
        {
            await QuerySql.InsertPostgresNotNullTypesAsync(new QuerySql.InsertPostgresNotNullTypesArgs { CEnumNotNull = cEnumNotNull });
            var expected = new QuerySql.GetPostgresNotNullTypesRow
            {
                CEnumNotNull = cEnumNotNull
            };
            var actual = await QuerySql.GetPostgresNotNullTypesAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresNotNullTypesRow x, QuerySql.GetPostgresNotNullTypesRow y)
            {
                Assert.That(x.CEnumNotNull, Is.EqualTo(y.CEnumNotNull));
            }
        }

        private static IEnumerable<TestCaseData> PostgresBitDataTypesTestCases
        {
            get
            {
                yield return new TestCaseData(new BitArray(new[] { true, false, true, true, false, false, true, false, true, true }), new BitArray(new[] { true }), new BitArray(new[] { false, true, true, false, true }), new BitArray(new[] { true, false, false }), new BitArray[] { new BitArray(new[] { true, true, false, false, true, true, false, false, true, true }), new BitArray(new[] { false, false, false, false, false, false, false, false, false, true }) }, new BitArray[] { new BitArray(new[] { true, false, true }), new BitArray(new[] { false, true, true, false, false, true, true }) }).SetName("Valid Bit Data Types");
                yield return new TestCaseData(null, null, null, null, null, null).SetName("Null Bit Data Types");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresBitDataTypesTestCases))]
        public async Task TestPostgresBitDataTypesAsync(BitArray cBit, BitArray cBitSingle, BitArray cVarbit, BitArray cBitVarying, BitArray[] cBitArray, BitArray[] cVarbitArray)
        {
            await QuerySql.InsertPostgresBitTypesAsync(new QuerySql.InsertPostgresBitTypesArgs { CBit = cBit, CBitSingle = cBitSingle, CVarbit = cVarbit, CBitVarying = cBitVarying, CBitArray = cBitArray, CVarbitArray = cVarbitArray });
            var expected = new QuerySql.GetPostgresBitTypesRow
            {
                CBit = cBit,
                CBitSingle = cBitSingle,
                CVarbit = cVarbit,
                CBitVarying = cBitVarying,
                CBitArray = cBitArray,
                CVarbitArray = cVarbitArray
            };
            var actual = await QuerySql.GetPostgresBitTypesAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresBitTypesRow x, QuerySql.GetPostgresBitTypesRow y)
            {
                Assert.That(x.CBit, Is.EqualTo(y.CBit));
                Assert.That(x.CBitSingle, Is.EqualTo(y.CBitSingle));
                Assert.That(x.CVarbit, Is.EqualTo(y.CVarbit));
                Assert.That(x.CBitVarying, Is.EqualTo(y.CBitVarying));
                Assert.That(x.CBitArray, Is.EqualTo(y.CBitArray));
                Assert.That(x.CVarbitArray, Is.EqualTo(y.CVarbitArray));
            }
        }

        [Test]
        [TestCase("Hello World", "Top.Science.Astronomy", "Top.Science.*", "Europe & Russia@* & !Transportation", "some_name")]
        [TestCase(null, null, null, null, null)]
        public async Task TestPostgresExtendedStringTypesAsync(string cCitext, string cLtree, string cLquery, string cLtxtquery, string cName)
        {
            await QuerySql.InsertPostgresExtendedStringTypesAsync(new QuerySql.InsertPostgresExtendedStringTypesArgs { CCitext = cCitext, CLtree = cLtree, CLquery = cLquery, CLtxtquery = cLtxtquery, CName = cName });
            var expected = new QuerySql.GetPostgresExtendedStringTypesRow
            {
                CCitext = cCitext,
                CLtree = cLtree,
                CLquery = cLquery,
                CLtxtquery = cLtxtquery,
                CName = cName
            };
            var actual = await QuerySql.GetPostgresExtendedStringTypesAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresExtendedStringTypesRow x, QuerySql.GetPostgresExtendedStringTypesRow y)
            {
                Assert.That(x.CCitext, Is.EqualTo(y.CCitext));
                Assert.That(x.CLtree, Is.EqualTo(y.CLtree));
                Assert.That(x.CLquery, Is.EqualTo(y.CLquery));
                Assert.That(x.CLtxtquery, Is.EqualTo(y.CLtxtquery));
                Assert.That(x.CName, Is.EqualTo(y.CName));
            }
        }

        private static IEnumerable<TestCaseData> PostgresRangeDataTypesTestCases
        {
            get
            {
                yield return new TestCaseData(new NpgsqlRange<int>(1, true, 11, false), new NpgsqlRange<long>(-5000000000L, true, 5000000000L, false), new NpgsqlRange<decimal>(1.5m, true, 9.25m, true), new NpgsqlRange<DateTime>(new DateTime(2024, 1, 1), true, new DateTime(2024, 2, 1), false), new NpgsqlRange<DateTime>(new DateTime(2024, 1, 1, 10, 30, 0), true, new DateTime(2024, 1, 2, 12, 0, 0), false), new NpgsqlRange<DateTime>(new DateTime(2024, 1, 1, 10, 30, 0, DateTimeKind.Utc), true, new DateTime(2024, 1, 2, 12, 0, 0, DateTimeKind.Utc), false), new NpgsqlRange<int>[] { new NpgsqlRange<int>(1, true, 5, false), new NpgsqlRange<int>(10, true, 20, false) }).SetName("Valid Range Data Types");
                yield return new TestCaseData(null, null, null, null, null, null, null).SetName("Null Range Data Types");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresRangeDataTypesTestCases))]
        public async Task TestPostgresRangeDataTypesAsync(NpgsqlRange<int>? cInt4range, NpgsqlRange<long>? cInt8range, NpgsqlRange<decimal>? cNumrange, NpgsqlRange<DateTime>? cDaterange, NpgsqlRange<DateTime>? cTsrange, NpgsqlRange<DateTime>? cTstzrange, NpgsqlRange<int>[] cInt4rangeArray)
        {
            await QuerySql.InsertPostgresRangeTypesAsync(new QuerySql.InsertPostgresRangeTypesArgs { CInt4range = cInt4range, CInt8range = cInt8range, CNumrange = cNumrange, CDaterange = cDaterange, CTsrange = cTsrange, CTstzrange = cTstzrange, CInt4rangeArray = cInt4rangeArray });
            var expected = new QuerySql.GetPostgresRangeTypesRow
            {
                CInt4range = cInt4range,
                CInt8range = cInt8range,
                CNumrange = cNumrange,
                CDaterange = cDaterange,
                CTsrange = cTsrange,
                CTstzrange = cTstzrange,
                CInt4rangeArray = cInt4rangeArray
            };
            var actual = await QuerySql.GetPostgresRangeTypesAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresRangeTypesRow x, QuerySql.GetPostgresRangeTypesRow y)
            {
                Assert.That(x.CInt4range, Is.EqualTo(y.CInt4range));
                Assert.That(x.CInt8range, Is.EqualTo(y.CInt8range));
                Assert.That(x.CNumrange, Is.EqualTo(y.CNumrange));
                Assert.That(x.CDaterange, Is.EqualTo(y.CDaterange));
                Assert.That(x.CTsrange, Is.EqualTo(y.CTsrange));
                Assert.That(x.CTstzrange, Is.EqualTo(y.CTstzrange));
                Assert.That(x.CInt4rangeArray, Is.EqualTo(y.CInt4rangeArray));
            }
        }

        private static IEnumerable<TestCaseData> PostgresMultirangeDataTypesTestCases
        {
            get
            {
                yield return new TestCaseData(new[] { new NpgsqlRange<int>(1, true, 5, false), new NpgsqlRange<int>(10, true, 20, false) }, new[] { new NpgsqlRange<long>(-5000000000L, true, 5L, false), new NpgsqlRange<long>(10L, true, 5000000000L, false) }, new[] { new NpgsqlRange<decimal>(1.5m, true, 2.5m, true), new NpgsqlRange<decimal>(9.25m, true, 10.75m, true) }, new[] { new NpgsqlRange<DateTime>(new DateTime(2024, 1, 1), true, new DateTime(2024, 2, 1), false), new NpgsqlRange<DateTime>(new DateTime(2024, 3, 1), true, new DateTime(2024, 4, 1), false) }, new[] { new NpgsqlRange<DateTime>(new DateTime(2024, 1, 1, 10, 30, 0), true, new DateTime(2024, 1, 2, 12, 0, 0), false), new NpgsqlRange<DateTime>(new DateTime(2024, 3, 1, 8, 0, 0), true, new DateTime(2024, 3, 2, 8, 0, 0), false) }, new[] { new NpgsqlRange<DateTime>(new DateTime(2024, 1, 1, 10, 30, 0, DateTimeKind.Utc), true, new DateTime(2024, 1, 2, 12, 0, 0, DateTimeKind.Utc), false), new NpgsqlRange<DateTime>(new DateTime(2024, 3, 1, 8, 0, 0, DateTimeKind.Utc), true, new DateTime(2024, 3, 2, 8, 0, 0, DateTimeKind.Utc), false) }).SetName("Valid Multirange Data Types");
                yield return new TestCaseData(null, null, null, null, null, null).SetName("Null Multirange Data Types");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresMultirangeDataTypesTestCases))]
        public async Task TestPostgresMultirangeDataTypesAsync(NpgsqlRange<int>[] cInt4multirange, NpgsqlRange<long>[] cInt8multirange, NpgsqlRange<decimal>[] cNummultirange, NpgsqlRange<DateTime>[] cDatemultirange, NpgsqlRange<DateTime>[] cTsmultirange, NpgsqlRange<DateTime>[] cTstzmultirange)
        {
            await QuerySql.InsertPostgresMultirangeTypesAsync(new QuerySql.InsertPostgresMultirangeTypesArgs { CInt4multirange = cInt4multirange, CInt8multirange = cInt8multirange, CNummultirange = cNummultirange, CDatemultirange = cDatemultirange, CTsmultirange = cTsmultirange, CTstzmultirange = cTstzmultirange });
            var expected = new QuerySql.GetPostgresMultirangeTypesRow
            {
                CInt4multirange = cInt4multirange,
                CInt8multirange = cInt8multirange,
                CNummultirange = cNummultirange,
                CDatemultirange = cDatemultirange,
                CTsmultirange = cTsmultirange,
                CTstzmultirange = cTstzmultirange
            };
            var actual = await QuerySql.GetPostgresMultirangeTypesAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresMultirangeTypesRow x, QuerySql.GetPostgresMultirangeTypesRow y)
            {
                Assert.That(x.CInt4multirange, Is.EqualTo(y.CInt4multirange));
                Assert.That(x.CInt8multirange, Is.EqualTo(y.CInt8multirange));
                Assert.That(x.CNummultirange, Is.EqualTo(y.CNummultirange));
                Assert.That(x.CDatemultirange, Is.EqualTo(y.CDatemultirange));
                Assert.That(x.CTsmultirange, Is.EqualTo(y.CTsmultirange));
                Assert.That(x.CTstzmultirange, Is.EqualTo(y.CTstzmultirange));
            }
        }

        private static IEnumerable<TestCaseData> PostgresSystemDataTypesTestCases
        {
            get
            {
                yield return new TestCaseData(4000000000u, 4242u, 7u, 9999999999UL, new NpgsqlTid(5, 10), new NpgsqlLogSequenceNumber(0x16B374D848UL)).SetName("Valid System Data Types");
                yield return new TestCaseData(null, null, null, null, null, null).SetName("Null System Data Types");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresSystemDataTypesTestCases))]
        public async Task TestPostgresSystemDataTypesAsync(uint? cOid, uint? cXid, uint? cCid, ulong? cXid8, NpgsqlTid? cTid, NpgsqlLogSequenceNumber? cPgLsn)
        {
            await QuerySql.InsertPostgresSystemTypesAsync(new QuerySql.InsertPostgresSystemTypesArgs { COid = cOid, CXid = cXid, CCid = cCid, CXid8 = cXid8, CTid = cTid, CPgLsn = cPgLsn });
            var expected = new QuerySql.GetPostgresSystemTypesRow
            {
                COid = cOid,
                CXid = cXid,
                CCid = cCid,
                CXid8 = cXid8,
                CTid = cTid,
                CPgLsn = cPgLsn
            };
            var actual = await QuerySql.GetPostgresSystemTypesAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresSystemTypesRow x, QuerySql.GetPostgresSystemTypesRow y)
            {
                Assert.That(x.COid, Is.EqualTo(y.COid));
                Assert.That(x.CXid, Is.EqualTo(y.CXid));
                Assert.That(x.CCid, Is.EqualTo(y.CCid));
                Assert.That(x.CXid8, Is.EqualTo(y.CXid8));
                Assert.That(x.CTid, Is.EqualTo(y.CTid));
                Assert.That(x.CPgLsn, Is.EqualTo(y.CPgLsn));
            }
        }

        private static IEnumerable<TestCaseData> PostgresHstoreDataTypesTestCases
        {
            get
            {
                yield return new TestCaseData(new Dictionary<string, string> { { "a", "1" }, { "key with space", "some value" }, { "empty", null } }, new[] { new Dictionary<string, string> { { "x", "y" } }, new Dictionary<string, string> { { "p", "q" }, { "r", null } } }).SetName("Valid Hstore Data Types");
                yield return new TestCaseData(null, null).SetName("Null Hstore Data Types");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresHstoreDataTypesTestCases))]
        public async Task TestPostgresHstoreDataTypesAsync(Dictionary<string, string> cHstore, Dictionary<string, string>[] cHstoreArray)
        {
            await QuerySql.InsertPostgresHstoreTypesAsync(new QuerySql.InsertPostgresHstoreTypesArgs { CHstore = cHstore, CHstoreArray = cHstoreArray });
            var expected = new QuerySql.GetPostgresHstoreTypesRow
            {
                CHstore = cHstore,
                CHstoreArray = cHstoreArray
            };
            var actual = await QuerySql.GetPostgresHstoreTypesAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresHstoreTypesRow x, QuerySql.GetPostgresHstoreTypesRow y)
            {
                Assert.That(x.CHstore, Is.EqualTo(y.CHstore));
                Assert.That(x.CHstoreArray, Is.EqualTo(y.CHstoreArray));
            }
        }

        [Test]
        [TestCase(1, 2, 3, 4, 5L, 6L)]
        public async Task TestPostgresSerialTypesAsync(short cSmallserial, short cSerial2, int cSerial, int cSerial4, long cBigserial, long cSerial8)
        {
            await QuerySql.InsertPostgresSerialTypesAsync(new QuerySql.InsertPostgresSerialTypesArgs { CSmallserial = cSmallserial, CSerial2 = cSerial2, CSerial = cSerial, CSerial4 = cSerial4, CBigserial = cBigserial, CSerial8 = cSerial8 });
            var expected = new QuerySql.GetPostgresSerialTypesRow
            {
                CSmallserial = cSmallserial,
                CSerial2 = cSerial2,
                CSerial = cSerial,
                CSerial4 = cSerial4,
                CBigserial = cBigserial,
                CSerial8 = cSerial8
            };
            var actual = await QuerySql.GetPostgresSerialTypesAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresSerialTypesRow x, QuerySql.GetPostgresSerialTypesRow y)
            {
                Assert.That(x.CSmallserial, Is.EqualTo(y.CSmallserial));
                Assert.That(x.CSerial2, Is.EqualTo(y.CSerial2));
                Assert.That(x.CSerial, Is.EqualTo(y.CSerial));
                Assert.That(x.CSerial4, Is.EqualTo(y.CSerial4));
                Assert.That(x.CBigserial, Is.EqualTo(y.CBigserial));
                Assert.That(x.CSerial8, Is.EqualTo(y.CSerial8));
            }
        }

        [Test]
        [TestCase(100, "z", "Sex Pistols", "Anarchy in the U.K", "Yoshimi Battles the Pink Robots", "Never Mind the Bollocks...")]
        [TestCase(10, null, null, null, null, null)]
        public async Task TestStringCopyFromAsync(int batchSize, string cChar, string cVarchar, string cCharacterVarying, string cBpchar, string cText)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresStringTypesBatchArgs { CChar = cChar, CVarchar = cVarchar, CCharacterVarying = cCharacterVarying, CBpchar = cBpchar, CText = cText }).ToList();
            await QuerySql.InsertPostgresStringTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetPostgresStringTypesCntRow
            {
                Cnt = batchSize,
                CChar = cChar,
                CVarchar = cVarchar,
                CCharacterVarying = cCharacterVarying,
                CBpchar = cBpchar,
                CText = cText
            };
            var actual = await QuerySql.GetPostgresStringTypesCntAsync();
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
        public async Task TestIntegerCopyFromAsync(int batchSize, bool? cBoolean, short? cSmallint, int? cInteger, long? cBigint)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresNumericTypesBatchArgs { CBoolean = cBoolean, CSmallint = cSmallint, CInteger = cInteger, CBigint = cBigint }).ToList();
            await QuerySql.InsertPostgresNumericTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetPostgresNumericTypesCntRow
            {
                Cnt = batchSize,
                CBoolean = cBoolean,
                CSmallint = cSmallint,
                CInteger = cInteger,
                CBigint = cBigint
            };
            var actual = await QuerySql.GetPostgresNumericTypesCntAsync();
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
        public async Task TestFloatingPointCopyFromAsync(int batchSize, float? cReal, decimal? cDecimal, decimal? cNumeric, double? cDoublePrecision, decimal? cMoney)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresNumericTypesBatchArgs { CReal = cReal, CDecimal = cDecimal, CNumeric = cNumeric, CDoublePrecision = cDoublePrecision, CMoney = cMoney }).ToList();
            await QuerySql.InsertPostgresNumericTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetPostgresNumericTypesCntRow
            {
                Cnt = batchSize,
                CReal = cReal,
                CDecimal = cDecimal,
                CNumeric = cNumeric,
                CDoublePrecision = cDoublePrecision,
                CMoney = cMoney
            };
            var actual = await QuerySql.GetPostgresNumericTypesCntAsync();
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
        public async Task TestDateTimeCopyFromAsync(int batchSize, DateTime? cDate, TimeSpan? cTime, DateTime? cTimestamp, DateTime? cTimestampWithTz, TimeSpan? cInterval)
        {
            DateTime? cTimestampWithTzAsUtc = null;
            if (cTimestampWithTz != null)
                cTimestampWithTzAsUtc = DateTime.SpecifyKind(cTimestampWithTz.Value, DateTimeKind.Utc);
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresDateTimeTypesBatchArgs { CDate = cDate, CTime = cTime, CTimestamp = cTimestamp, CTimestampWithTz = cTimestampWithTzAsUtc, CInterval = cInterval }).ToList();
            await QuerySql.InsertPostgresDateTimeTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetPostgresDateTimeTypesCntRow
            {
                Cnt = batchSize,
                CDate = cDate,
                CTime = cTime,
                CTimestamp = cTimestamp,
                CTimestampWithTz = cTimestampWithTz,
                CInterval = cInterval
            };
            var actual = await QuerySql.GetPostgresDateTimeTypesCntAsync();
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
        public async Task TestPostgresGuidCopyFromAsync(int batchSize, Guid? cUuid)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresSpecialTypesBatchArgs { CUuid = cUuid }).ToList();
            await QuerySql.InsertPostgresSpecialTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetPostgresSpecialTypesCntRow
            {
                Cnt = batchSize,
                CUuid = cUuid
            };
            var actual = await QuerySql.GetPostgresSpecialTypesCntAsync();
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
        public async Task TestPostgresJsonCopyFromAsync(int batchSize, string cJson)
        {
            JsonElement? cParsedJson = null;
            if (cJson != null)
                cParsedJson = JsonDocument.Parse(cJson).RootElement;
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresSpecialTypesBatchArgs { CJson = cParsedJson, CJsonb = cParsedJson }).ToList();
            await QuerySql.InsertPostgresSpecialTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetPostgresSpecialTypesCntRow
            {
                Cnt = batchSize,
                CJson = cParsedJson,
                CJsonb = cParsedJson
            };
            var actual = await QuerySql.GetPostgresSpecialTypesCntAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresSpecialTypesCntRow x, QuerySql.GetPostgresSpecialTypesCntRow y)
            {
                Assert.That(y.Cnt, Is.EqualTo(x.Cnt));
                AssertJsonElementEquals(y.CJson, x.CJson);
                AssertJsonElementEquals(y.CJsonb, x.CJsonb);
            }

            void AssertJsonElementEquals(JsonElement? x, JsonElement? y)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = false
                };
                Assert.That(y.HasValue, Is.EqualTo(x.HasValue));
                if (y.HasValue)
                    Assert.That(JsonSerializer.Serialize(y.Value, options), Is.EqualTo(JsonSerializer.Serialize(x.Value, options)));
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
        public async Task TestPostgresNetworkCopyFromAsync(int batchSize, NpgsqlCidr? cCidr, IPAddress cInet, PhysicalAddress cMacaddr)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresNetworkTypesBatchArgs { CCidr = cCidr, CInet = cInet, CMacaddr = cMacaddr }).ToList();
            await QuerySql.InsertPostgresNetworkTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetPostgresNetworkTypesCntRow
            {
                Cnt = batchSize,
                CCidr = cCidr,
                CInet = cInet,
                CMacaddr = cMacaddr
            };
            var actual = await QuerySql.GetPostgresNetworkTypesCntAsync();
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
        public async Task TestArrayCopyFromAsync(int batchSize, byte[] cBytea, bool[] cBooleanArray, string[] cTextArray, int[] cIntegerArray, decimal[] cDecimalArray, DateTime[] cTimestampArray)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresArrayTypesBatchArgs { CBytea = cBytea, CBooleanArray = cBooleanArray, CTextArray = cTextArray, CIntegerArray = cIntegerArray, CDecimalArray = cDecimalArray, CTimestampArray = cTimestampArray }).ToList();
            await QuerySql.InsertPostgresArrayTypesBatchAsync(batchArgs);
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
            var actual = await QuerySql.GetPostgresArrayTypesCntAsync();
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
        public async Task TestPostgresGeoCopyFromAsync(int batchSize, NpgsqlPoint? cPoint, NpgsqlLine? cLine, NpgsqlLSeg? cLSeg, NpgsqlBox? cBox, NpgsqlPath? cPath, NpgsqlPolygon? cPolygon, NpgsqlCircle? cCircle)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresGeoTypesBatchArgs { CPoint = cPoint, CLine = cLine, CLseg = cLSeg, CBox = cBox, CPath = cPath, CPolygon = cPolygon, CCircle = cCircle }).ToList();
            await QuerySql.InsertPostgresGeoTypesBatchAsync(batchArgs);
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
            var actual = await QuerySql.GetPostgresGeoTypesAsync();
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

        private static IEnumerable<TestCaseData> PostgresBitCopyFromTestCases
        {
            get
            {
                yield return new TestCaseData(100, new BitArray(new[] { true, false, true, true, false, false, true, false, true, true }), new BitArray(new[] { false }), new BitArray(new[] { false, true, true, false, true }), new BitArray(new[] { true, false, false })).SetName("Valid Bit Copy From");
                yield return new TestCaseData(10, null, null, null, null).SetName("Null Bit Copy From");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresBitCopyFromTestCases))]
        public async Task TestPostgresBitCopyFromAsync(int batchSize, BitArray cBit, BitArray cBitSingle, BitArray cVarbit, BitArray cBitVarying)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresBitTypesBatchArgs { CBit = cBit, CBitSingle = cBitSingle, CVarbit = cVarbit, CBitVarying = cBitVarying }).ToList();
            await QuerySql.InsertPostgresBitTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetPostgresBitTypesCntRow
            {
                Cnt = batchSize,
                CBit = cBit,
                CBitSingle = cBitSingle,
                CVarbit = cVarbit,
                CBitVarying = cBitVarying
            };
            var actual = await QuerySql.GetPostgresBitTypesCntAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresBitTypesCntRow x, QuerySql.GetPostgresBitTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CBit, Is.EqualTo(y.CBit));
                Assert.That(x.CBitSingle, Is.EqualTo(y.CBitSingle));
                Assert.That(x.CVarbit, Is.EqualTo(y.CVarbit));
                Assert.That(x.CBitVarying, Is.EqualTo(y.CBitVarying));
            }
        }

        [Test]
        [TestCase(100, "Hello World", "Top.Science.Astronomy", "Top.Science.*", "Europe & Russia@* & !Transportation", "some_name")]
        [TestCase(10, null, null, null, null, null)]
        public async Task TestPostgresExtendedStringCopyFromAsync(int batchSize, string cCitext, string cLtree, string cLquery, string cLtxtquery, string cName)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresExtendedStringTypesBatchArgs { CCitext = cCitext, CLtree = cLtree, CLquery = cLquery, CLtxtquery = cLtxtquery, CName = cName }).ToList();
            await QuerySql.InsertPostgresExtendedStringTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetPostgresExtendedStringTypesCntRow
            {
                Cnt = batchSize,
                CCitext = cCitext,
                CLtree = cLtree,
                CName = cName
            };
            var actual = await QuerySql.GetPostgresExtendedStringTypesCntAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresExtendedStringTypesCntRow x, QuerySql.GetPostgresExtendedStringTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CCitext, Is.EqualTo(y.CCitext));
                Assert.That(x.CLtree, Is.EqualTo(y.CLtree));
                Assert.That(x.CName, Is.EqualTo(y.CName));
            }

            // lquery and ltxtquery have no equality operator so they cannot be grouped by
            var actualQueries = await QuerySql.GetPostgresExtendedStringTypesAsync();
            Assert.That(actualQueries.CLquery, Is.EqualTo(cLquery));
            Assert.That(actualQueries.CLtxtquery, Is.EqualTo(cLtxtquery));
        }

        private static IEnumerable<TestCaseData> PostgresRangeCopyFromTestCases
        {
            get
            {
                yield return new TestCaseData(50, new NpgsqlRange<int>(1, true, 11, false), new NpgsqlRange<long>(-5000000000L, true, 5000000000L, false), new NpgsqlRange<decimal>(1.5m, true, 9.25m, true), new NpgsqlRange<DateTime>(new DateTime(2024, 1, 1), true, new DateTime(2024, 2, 1), false), new NpgsqlRange<DateTime>(new DateTime(2024, 1, 1, 10, 30, 0), true, new DateTime(2024, 1, 2, 12, 0, 0), false), new NpgsqlRange<DateTime>(new DateTime(2024, 1, 1, 10, 30, 0, DateTimeKind.Utc), true, new DateTime(2024, 1, 2, 12, 0, 0, DateTimeKind.Utc), false), new NpgsqlRange<int>[] { new NpgsqlRange<int>(1, true, 5, false), new NpgsqlRange<int>(10, true, 20, false) }).SetName("Valid Range Copy From");
                yield return new TestCaseData(10, null, null, null, null, null, null, null).SetName("Null Range Copy From");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresRangeCopyFromTestCases))]
        public async Task TestPostgresRangeCopyFromAsync(int batchSize, NpgsqlRange<int>? cInt4range, NpgsqlRange<long>? cInt8range, NpgsqlRange<decimal>? cNumrange, NpgsqlRange<DateTime>? cDaterange, NpgsqlRange<DateTime>? cTsrange, NpgsqlRange<DateTime>? cTstzrange, NpgsqlRange<int>[] cInt4rangeArray)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresRangeTypesBatchArgs { CInt4range = cInt4range, CInt8range = cInt8range, CNumrange = cNumrange, CDaterange = cDaterange, CTsrange = cTsrange, CTstzrange = cTstzrange, CInt4rangeArray = cInt4rangeArray }).ToList();
            await QuerySql.InsertPostgresRangeTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetPostgresRangeTypesCntRow
            {
                Cnt = batchSize,
                CInt4range = cInt4range,
                CInt8range = cInt8range,
                CNumrange = cNumrange,
                CDaterange = cDaterange,
                CTsrange = cTsrange,
                CTstzrange = cTstzrange,
                CInt4rangeArray = cInt4rangeArray
            };
            var actual = await QuerySql.GetPostgresRangeTypesCntAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresRangeTypesCntRow x, QuerySql.GetPostgresRangeTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CInt4range, Is.EqualTo(y.CInt4range));
                Assert.That(x.CInt8range, Is.EqualTo(y.CInt8range));
                Assert.That(x.CNumrange, Is.EqualTo(y.CNumrange));
                Assert.That(x.CDaterange, Is.EqualTo(y.CDaterange));
                Assert.That(x.CTsrange, Is.EqualTo(y.CTsrange));
                Assert.That(x.CTstzrange, Is.EqualTo(y.CTstzrange));
                Assert.That(x.CInt4rangeArray, Is.EqualTo(y.CInt4rangeArray));
            }
        }

        private static IEnumerable<TestCaseData> PostgresMultirangeCopyFromTestCases
        {
            get
            {
                yield return new TestCaseData(50, new[] { new NpgsqlRange<int>(1, true, 5, false), new NpgsqlRange<int>(10, true, 20, false) }, new[] { new NpgsqlRange<long>(-5000000000L, true, 5L, false), new NpgsqlRange<long>(10L, true, 5000000000L, false) }, new[] { new NpgsqlRange<decimal>(1.5m, true, 2.5m, true), new NpgsqlRange<decimal>(9.25m, true, 10.75m, true) }, new[] { new NpgsqlRange<DateTime>(new DateTime(2024, 1, 1), true, new DateTime(2024, 2, 1), false), new NpgsqlRange<DateTime>(new DateTime(2024, 3, 1), true, new DateTime(2024, 4, 1), false) }, new[] { new NpgsqlRange<DateTime>(new DateTime(2024, 1, 1, 10, 30, 0), true, new DateTime(2024, 1, 2, 12, 0, 0), false), new NpgsqlRange<DateTime>(new DateTime(2024, 3, 1, 8, 0, 0), true, new DateTime(2024, 3, 2, 8, 0, 0), false) }, new[] { new NpgsqlRange<DateTime>(new DateTime(2024, 1, 1, 10, 30, 0, DateTimeKind.Utc), true, new DateTime(2024, 1, 2, 12, 0, 0, DateTimeKind.Utc), false), new NpgsqlRange<DateTime>(new DateTime(2024, 3, 1, 8, 0, 0, DateTimeKind.Utc), true, new DateTime(2024, 3, 2, 8, 0, 0, DateTimeKind.Utc), false) }).SetName("Valid Multirange Copy From");
                yield return new TestCaseData(10, null, null, null, null, null, null).SetName("Null Multirange Copy From");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresMultirangeCopyFromTestCases))]
        public async Task TestPostgresMultirangeCopyFromAsync(int batchSize, NpgsqlRange<int>[] cInt4multirange, NpgsqlRange<long>[] cInt8multirange, NpgsqlRange<decimal>[] cNummultirange, NpgsqlRange<DateTime>[] cDatemultirange, NpgsqlRange<DateTime>[] cTsmultirange, NpgsqlRange<DateTime>[] cTstzmultirange)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresMultirangeTypesBatchArgs { CInt4multirange = cInt4multirange, CInt8multirange = cInt8multirange, CNummultirange = cNummultirange, CDatemultirange = cDatemultirange, CTsmultirange = cTsmultirange, CTstzmultirange = cTstzmultirange }).ToList();
            await QuerySql.InsertPostgresMultirangeTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetPostgresMultirangeTypesCntRow
            {
                Cnt = batchSize,
                CInt4multirange = cInt4multirange,
                CInt8multirange = cInt8multirange,
                CNummultirange = cNummultirange,
                CDatemultirange = cDatemultirange,
                CTsmultirange = cTsmultirange,
                CTstzmultirange = cTstzmultirange
            };
            var actual = await QuerySql.GetPostgresMultirangeTypesCntAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresMultirangeTypesCntRow x, QuerySql.GetPostgresMultirangeTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CInt4multirange, Is.EqualTo(y.CInt4multirange));
                Assert.That(x.CInt8multirange, Is.EqualTo(y.CInt8multirange));
                Assert.That(x.CNummultirange, Is.EqualTo(y.CNummultirange));
                Assert.That(x.CDatemultirange, Is.EqualTo(y.CDatemultirange));
                Assert.That(x.CTsmultirange, Is.EqualTo(y.CTsmultirange));
                Assert.That(x.CTstzmultirange, Is.EqualTo(y.CTstzmultirange));
            }
        }

        private static IEnumerable<TestCaseData> PostgresSystemCopyFromTestCases
        {
            get
            {
                yield return new TestCaseData(50, 4000000000u, 4242u, 7u, 9999999999UL, new NpgsqlTid(5, 10), new NpgsqlLogSequenceNumber(0x16B374D848UL)).SetName("Valid System Copy From");
                yield return new TestCaseData(10, null, null, null, null, null, null).SetName("Null System Copy From");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresSystemCopyFromTestCases))]
        public async Task TestPostgresSystemCopyFromAsync(int batchSize, uint? cOid, uint? cXid, uint? cCid, ulong? cXid8, NpgsqlTid? cTid, NpgsqlLogSequenceNumber? cPgLsn)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresSystemTypesBatchArgs { COid = cOid, CXid = cXid, CCid = cCid, CXid8 = cXid8, CTid = cTid, CPgLsn = cPgLsn }).ToList();
            await QuerySql.InsertPostgresSystemTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetPostgresSystemTypesCntRow
            {
                Cnt = batchSize,
                COid = cOid,
                CXid = cXid,
                CCid = cCid,
                CXid8 = cXid8,
                CTid = cTid,
                CPgLsn = cPgLsn
            };
            var actual = await QuerySql.GetPostgresSystemTypesCntAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresSystemTypesCntRow x, QuerySql.GetPostgresSystemTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.COid, Is.EqualTo(y.COid));
                Assert.That(x.CXid, Is.EqualTo(y.CXid));
                Assert.That(x.CCid, Is.EqualTo(y.CCid));
                Assert.That(x.CXid8, Is.EqualTo(y.CXid8));
                Assert.That(x.CTid, Is.EqualTo(y.CTid));
                Assert.That(x.CPgLsn, Is.EqualTo(y.CPgLsn));
            }
        }

        private static IEnumerable<TestCaseData> PostgresHstoreCopyFromTestCases
        {
            get
            {
                yield return new TestCaseData(50, new Dictionary<string, string> { { "a", "1" }, { "key with space", "some value" }, { "empty", null } }, new[] { new Dictionary<string, string> { { "x", "y" } }, new Dictionary<string, string> { { "p", "q" }, { "r", null } } }).SetName("Valid Hstore Copy From");
                yield return new TestCaseData(10, null, null).SetName("Null Hstore Copy From");
            }
        }

        [Test]
        [TestCaseSource(nameof(PostgresHstoreCopyFromTestCases))]
        public async Task TestPostgresHstoreCopyFromAsync(int batchSize, Dictionary<string, string> cHstore, Dictionary<string, string>[] cHstoreArray)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertPostgresHstoreTypesBatchArgs { CHstore = cHstore, CHstoreArray = cHstoreArray }).ToList();
            await QuerySql.InsertPostgresHstoreTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetPostgresHstoreTypesCntRow
            {
                Cnt = batchSize,
                CHstore = cHstore,
                CHstoreArray = cHstoreArray
            };
            var actual = await QuerySql.GetPostgresHstoreTypesCntAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetPostgresHstoreTypesCntRow x, QuerySql.GetPostgresHstoreTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CHstore, Is.EqualTo(y.CHstore));
                Assert.That(x.CHstoreArray, Is.EqualTo(y.CHstoreArray));
            }
        }
    }
}
