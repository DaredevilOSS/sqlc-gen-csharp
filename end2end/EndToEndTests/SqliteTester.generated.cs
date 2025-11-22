using SqliteExampleGen;
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
    public partial class SqliteTester
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
            AssertSingularEquals(expected, actual.Value);
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
            AssertSingularEquals(expected, actual.Value);
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
                Assert.That(x.Author.Value.Id, Is.EqualTo(y.Author.Value.Id));
                Assert.That(x.Author.Value.Name, Is.EqualTo(y.Author.Value.Name));
                Assert.That(x.Author.Value.Bio, Is.EqualTo(y.Author.Value.Bio));
                Assert.That(x.Author2.Value.Id, Is.EqualTo(y.Author2.Value.Id));
                Assert.That(x.Author2.Value.Name, Is.EqualTo(y.Author2.Value.Name));
                Assert.That(x.Author2.Value.Bio, Is.EqualTo(y.Author2.Value.Bio));
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
                Assert.That(x.Author.Value.Id, Is.EqualTo(y.Author.Value.Id));
                Assert.That(x.Author.Value.Name, Is.EqualTo(y.Author.Value.Name));
                Assert.That(x.Author.Value.Bio, Is.EqualTo(y.Author.Value.Bio));
                Assert.That(x.Book.Value.Id, Is.EqualTo(y.Book.Value.Id));
                Assert.That(x.Book.Value.AuthorId, Is.EqualTo(y.Book.Value.AuthorId));
                Assert.That(x.Book.Value.Name, Is.EqualTo(y.Book.Value.Name));
            }

            void AssertSequenceEquals(List<QuerySql.ListAllAuthorsBooksRow> x, List<QuerySql.ListAllAuthorsBooksRow> y)
            {
                Assert.That(x.Count, Is.EqualTo(y.Count));
                for (int i = 0; i < x.Count; i++)
                    AssertSingularEquals(x[i], y[i]);
            }
        }

        [Test]
        public async Task TestSliceAsync()
        {
            var id1 = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Quote that everyone always attribute to Einstein" });
            var bojackId = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            var actual = await QuerySql.GetAuthorsByIdsAsync(new QuerySql.GetAuthorsByIdsArgs { Ids = new[] { id1, bojackId } });
            ClassicAssert.AreEqual(2, actual.Count);
        }

        [Test]
        public async Task TestMultipleSlicesAsync()
        {
            var id1 = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Albert Einstein", Bio = "Quote that everyone always attribute to Einstein" });
            var bojackId = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs { Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            var actual = await QuerySql.GetAuthorsByIdsAndNamesAsync(new QuerySql.GetAuthorsByIdsAndNamesArgs { Ids = new[] { id1, bojackId }, Names = new[] { "Albert Einstein" } });
            ClassicAssert.AreEqual(1, actual.Count);
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

        private static IEnumerable<TestCaseData> SqliteTypesTestCases
        {
            get
            {
                yield return new TestCaseData(-54355, 9787.66m, "Songs of Love and Hate", new byte[] { 0x15, 0x20, 0x33 }, true, false, DateTime.SpecifyKind(DateTime.Parse("2020-01-01 14:15:16"), DateTimeKind.Utc), DateTime.SpecifyKind(DateTime.Parse("2025-01-01 17:18:19"), DateTimeKind.Utc), Instant.FromUtc(2025, 10, 15, 19, 55, 2), Instant.FromUtc(1993, 9, 27, 03, 55, 2)).SetName("SqliteTypes with values");
                yield return new TestCaseData(null, null, null, new byte[] { }, null, null, null, null, null, null).SetName("SqliteTypes with empty values");
                yield return new TestCaseData(null, null, null, null, null, null, null, null, null, null).SetName("SqliteTypes with null values");
            }
        }

        [Test]
        [TestCaseSource(nameof(SqliteTypesTestCases))]
        public async Task TestSqliteTypesAsync(int? cInteger, decimal? cReal, string cText, byte[] cBlob, bool? cTextBoolOverride, bool? cIntegerBoolOverride, DateTime? cTextDatetimeOverride, DateTime? cIntegerDatetimeOverride, Instant? cTextNodaInstantOverride, Instant? cIntegerNodaInstantOverride)
        {
            await QuerySql.InsertSqliteTypesAsync(new QuerySql.InsertSqliteTypesArgs { CInteger = cInteger, CReal = cReal, CText = cText, CBlob = cBlob, CTextBoolOverride = cTextBoolOverride, CIntegerBoolOverride = cIntegerBoolOverride, CTextDatetimeOverride = cTextDatetimeOverride, CIntegerDatetimeOverride = cIntegerDatetimeOverride, CTextNodaInstantOverride = cTextNodaInstantOverride, CIntegerNodaInstantOverride = cIntegerNodaInstantOverride });
            var expected = new QuerySql.GetSqliteTypesRow
            {
                CInteger = cInteger,
                CReal = cReal,
                CText = cText,
                CBlob = cBlob,
                CTextBoolOverride = cTextBoolOverride,
                CIntegerBoolOverride = cIntegerBoolOverride,
                CTextDatetimeOverride = cTextDatetimeOverride,
                CIntegerDatetimeOverride = cIntegerDatetimeOverride,
                CTextNodaInstantOverride = cTextNodaInstantOverride,
                CIntegerNodaInstantOverride = cIntegerNodaInstantOverride
            };
            var actual = await QuerySql.GetSqliteTypesAsync();
            AssertSingularEquals(expected, actual.Value);
            void AssertSingularEquals(QuerySql.GetSqliteTypesRow x, QuerySql.GetSqliteTypesRow y)
            {
                Assert.That(x.CInteger, Is.EqualTo(y.CInteger));
                Assert.That(x.CReal, Is.EqualTo(y.CReal));
                Assert.That(x.CText, Is.EqualTo(y.CText));
                Assert.That(x.CBlob, Is.EqualTo(y.CBlob));
                Assert.That(x.CTextBoolOverride, Is.EqualTo(y.CTextBoolOverride));
                Assert.That(x.CIntegerBoolOverride, Is.EqualTo(y.CIntegerBoolOverride));
                Assert.That(x.CTextDatetimeOverride, Is.EqualTo(y.CTextDatetimeOverride));
                Assert.That(x.CIntegerDatetimeOverride, Is.EqualTo(y.CIntegerDatetimeOverride));
                Assert.That(x.CTextNodaInstantOverride, Is.EqualTo(y.CTextNodaInstantOverride));
                Assert.That(x.CIntegerNodaInstantOverride, Is.EqualTo(y.CIntegerNodaInstantOverride));
            }
        }

        [Test]
        public async Task TestGetAuthorByIdWithMultipleNamedParamAsync()
        {
            await this.QuerySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            var expected = new QuerySql.GetAuthorByIdWithMultipleNamedParamRow
            {
                Id = 1111,
                Name = "Bojack Horseman",
                Bio = "Back in the 90s he was in a very famous TV show"
            };
            var actual = await this.QuerySql.GetAuthorByIdWithMultipleNamedParamAsync(new QuerySql.GetAuthorByIdWithMultipleNamedParamArgs { IdArg = 1111, Take = 1 });
            AssertSingularEquals(expected, actual.Value);
            void AssertSingularEquals(QuerySql.GetAuthorByIdWithMultipleNamedParamRow x, QuerySql.GetAuthorByIdWithMultipleNamedParamRow y)
            {
                Assert.That(x.Id, Is.EqualTo(y.Id));
                Assert.That(x.Name, Is.EqualTo(y.Name));
                Assert.That(x.Bio, Is.EqualTo(y.Bio));
            }
        }

        [Test]
        [TestCase(-54355, 9787.66, "Have One On Me")]
        [TestCase(null, 0.0, null)]
        public async Task TestSqliteDataTypesOverrideAsync(int? cInteger, decimal cReal, string cText)
        {
            await QuerySql.InsertSqliteTypesAsync(new QuerySql.InsertSqliteTypesArgs { CInteger = cInteger, CReal = cReal, CText = cText });
            var expected = new QuerySql.GetSqliteFunctionsRow
            {
                MaxInteger = cInteger,
                MaxReal = cReal,
                MaxText = cText
            };
            var actual = await QuerySql.GetSqliteFunctionsAsync();
            AssertSingularEquals(expected, actual.Value);
            void AssertSingularEquals(QuerySql.GetSqliteFunctionsRow x, QuerySql.GetSqliteFunctionsRow y)
            {
                Assert.That(x.MaxInteger, Is.EqualTo(y.MaxInteger));
                Assert.That(x.MaxReal, Is.EqualTo(y.MaxReal));
                Assert.That(x.MaxText, Is.EqualTo(y.MaxText));
            }
        }

        [Test]
        [TestCase(100, 312, -7541.3309, "Johnny B. Good")]
        [TestCase(500, -768, 8453.5678, "Bad to the Bone")]
        [TestCase(10, null, null, null)]
        public async Task TestCopyFromAsync(int batchSize, int? cInteger, decimal? cReal, string cText)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertSqliteTypesBatchArgs { CInteger = cInteger, CReal = cReal, CText = cText }).ToList();
            await QuerySql.InsertSqliteTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetSqliteTypesCntRow
            {
                Cnt = batchSize,
                CInteger = cInteger,
                CReal = cReal,
                CText = cText
            };
            var actual = await QuerySql.GetSqliteTypesCntAsync();
            AssertSingularEquals(expected, actual.Value);
            void AssertSingularEquals(QuerySql.GetSqliteTypesCntRow x, QuerySql.GetSqliteTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CInteger, Is.EqualTo(y.CInteger));
                Assert.That(x.CReal, Is.EqualTo(y.CReal));
                Assert.That(x.CText, Is.EqualTo(y.CText));
            }
        }

        [Test]
        public async Task TestSqliteTransactionAsync()
        {
            var connection = new Microsoft.Data.Sqlite.SqliteConnection(Environment.GetEnvironmentVariable(EndToEndCommon.SqliteConnectionStringEnv));
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();
            var querySqlWithTx = QuerySql.WithTransaction(transaction);
            await querySqlWithTx.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            var actual = await QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            ClassicAssert.IsNull(actual);
            transaction.Commit();
            var expected = new QuerySql.GetAuthorRow
            {
                Id = 1111,
                Name = "Bojack Horseman",
                Bio = "Back in the 90s he was in a very famous TV show"
            };
            actual = await QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            AssertSingularEquals(expected, actual.Value);
            void AssertSingularEquals(QuerySql.GetAuthorRow x, QuerySql.GetAuthorRow y)
            {
                Assert.That(x.Id, Is.EqualTo(y.Id));
                Assert.That(x.Name, Is.EqualTo(y.Name));
                Assert.That(x.Bio, Is.EqualTo(y.Bio));
            }
        }

        [Test]
        public async Task TestSqliteTransactionRollbackAsync()
        {
            var connection = new Microsoft.Data.Sqlite.SqliteConnection(Environment.GetEnvironmentVariable(EndToEndCommon.SqliteConnectionStringEnv));
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();
            var sqlQueryWithTx = QuerySql.WithTransaction(transaction);
            await sqlQueryWithTx.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            transaction.Rollback();
            var actual = await this.QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            ClassicAssert.IsNull(actual);
        }
    }
}
