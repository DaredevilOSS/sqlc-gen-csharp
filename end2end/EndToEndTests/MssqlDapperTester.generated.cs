using MssqlDapperExampleGen;
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
    public partial class MssqlDapperTester
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

        [Test]
        public async Task TestMssqlIntegerTypes()
        {
            await AssertRoundTrip(true);
            await QuerySql.TruncateMssqlNumericTypesAsync();
            await AssertRoundTrip(false);
            async Task AssertRoundTrip(bool withValues)
            {
                await QuerySql.InsertMssqlNumericTypesAsync(new QuerySql.InsertMssqlNumericTypesArgs { CBit = withValues ? (bool? )true : null, CTinyint = withValues ? (byte? )(byte)13 : null, CSmallint = withValues ? (short? )(short)2084 : null, CInt = withValues ? (int? )-54355 : null, CBigint = withValues ? (long? )9787668656L : null, CReal = withValues ? (float? )3.4f : null, CFloat = withValues ? (double? )21.214312452534 : null, CDecimal = withValues ? (decimal? )11.0986m : null, CNumeric = withValues ? (decimal? )-31.5556m : null, CMoney = withValues ? (decimal? )922.5000m : null, CSmallmoney = withValues ? (decimal? )12.3400m : null });
                var expected = new QuerySql.GetMssqlNumericTypesRow
                {
                    CBit = withValues ? (bool? )true : null,
                    CTinyint = withValues ? (byte? )(byte)13 : null,
                    CSmallint = withValues ? (short? )(short)2084 : null,
                    CInt = withValues ? (int? )-54355 : null,
                    CBigint = withValues ? (long? )9787668656L : null,
                    CReal = withValues ? (float? )3.4f : null,
                    CFloat = withValues ? (double? )21.214312452534 : null,
                    CDecimal = withValues ? (decimal? )11.0986m : null,
                    CNumeric = withValues ? (decimal? )-31.5556m : null,
                    CMoney = withValues ? (decimal? )922.5000m : null,
                    CSmallmoney = withValues ? (decimal? )12.3400m : null
                };
                var actual = await QuerySql.GetMssqlNumericTypesAsync();
                AssertSingularEquals(expected, actual);
            }

            void AssertSingularEquals(QuerySql.GetMssqlNumericTypesRow x, QuerySql.GetMssqlNumericTypesRow y)
            {
                Assert.That(x.CBit, Is.EqualTo(y.CBit));
                Assert.That(x.CTinyint, Is.EqualTo(y.CTinyint));
                Assert.That(x.CSmallint, Is.EqualTo(y.CSmallint));
                Assert.That(x.CInt, Is.EqualTo(y.CInt));
                Assert.That(x.CBigint, Is.EqualTo(y.CBigint));
                Assert.That(x.CReal, Is.EqualTo(y.CReal));
                Assert.That(x.CFloat, Is.EqualTo(y.CFloat));
                Assert.That(x.CDecimal, Is.EqualTo(y.CDecimal));
                Assert.That(x.CNumeric, Is.EqualTo(y.CNumeric));
                Assert.That(x.CMoney, Is.EqualTo(y.CMoney));
                Assert.That(x.CSmallmoney, Is.EqualTo(y.CSmallmoney));
            }
        }

        [Test]
        [TestCase(100, true)]
        [TestCase(10, false)]
        public async Task TestMssqlIntegerCopyFrom(int batchSize, bool withValues)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertMssqlNumericTypesBatchArgs { CBit = withValues ? (bool? )true : null, CTinyint = withValues ? (byte? )(byte)13 : null, CSmallint = withValues ? (short? )(short)2084 : null, CInt = withValues ? (int? )-54355 : null, CBigint = withValues ? (long? )9787668656L : null, CReal = withValues ? (float? )3.4f : null, CFloat = withValues ? (double? )21.214312452534 : null, CDecimal = withValues ? (decimal? )11.0986m : null, CNumeric = withValues ? (decimal? )-31.5556m : null, CMoney = withValues ? (decimal? )922.5000m : null, CSmallmoney = withValues ? (decimal? )12.3400m : null }).ToList();
            await QuerySql.InsertMssqlNumericTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetMssqlNumericTypesCntRow
            {
                Cnt = batchSize,
                CBit = withValues ? (bool? )true : null,
                CTinyint = withValues ? (byte? )(byte)13 : null,
                CSmallint = withValues ? (short? )(short)2084 : null,
                CInt = withValues ? (int? )-54355 : null,
                CBigint = withValues ? (long? )9787668656L : null,
                CReal = withValues ? (float? )3.4f : null,
                CFloat = withValues ? (double? )21.214312452534 : null,
                CDecimal = withValues ? (decimal? )11.0986m : null,
                CNumeric = withValues ? (decimal? )-31.5556m : null,
                CMoney = withValues ? (decimal? )922.5000m : null,
                CSmallmoney = withValues ? (decimal? )12.3400m : null
            };
            var actual = await QuerySql.GetMssqlNumericTypesCntAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetMssqlNumericTypesCntRow x, QuerySql.GetMssqlNumericTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CBit, Is.EqualTo(y.CBit));
                Assert.That(x.CTinyint, Is.EqualTo(y.CTinyint));
                Assert.That(x.CSmallint, Is.EqualTo(y.CSmallint));
                Assert.That(x.CInt, Is.EqualTo(y.CInt));
                Assert.That(x.CBigint, Is.EqualTo(y.CBigint));
                Assert.That(x.CReal, Is.EqualTo(y.CReal));
                Assert.That(x.CFloat, Is.EqualTo(y.CFloat));
                Assert.That(x.CDecimal, Is.EqualTo(y.CDecimal));
                Assert.That(x.CNumeric, Is.EqualTo(y.CNumeric));
                Assert.That(x.CMoney, Is.EqualTo(y.CMoney));
                Assert.That(x.CSmallmoney, Is.EqualTo(y.CSmallmoney));
            }
        }

        [Test]
        public async Task TestMssqlStringTypes()
        {
            await AssertRoundTrip(true);
            await QuerySql.TruncateMssqlStringTypesAsync();
            await AssertRoundTrip(false);
            async Task AssertRoundTrip(bool withValues)
            {
                await QuerySql.InsertMssqlStringTypesAsync(new QuerySql.InsertMssqlStringTypesArgs { CChar = withValues ? (string? )"abcdefghij" : null, CNchar = withValues ? (string? )"\u1857\u2649abcdefgh" : null, CVarchar = withValues ? (string? )"Holiday in Cambodia" : null, CNvarchar = withValues ? (string? )"Sheena \u2649 is a Punk Rocker" : null, CVarcharMax = withValues ? (string? )"London's Calling" : null, CNvarcharMax = withValues ? (string? )"Police & Thieves \u1857" : null });
                var expected = new QuerySql.GetMssqlStringTypesRow
                {
                    CChar = withValues ? (string? )"abcdefghij" : null,
                    CNchar = withValues ? (string? )"\u1857\u2649abcdefgh" : null,
                    CVarchar = withValues ? (string? )"Holiday in Cambodia" : null,
                    CNvarchar = withValues ? (string? )"Sheena \u2649 is a Punk Rocker" : null,
                    CVarcharMax = withValues ? (string? )"London's Calling" : null,
                    CNvarcharMax = withValues ? (string? )"Police & Thieves \u1857" : null
                };
                var actual = await QuerySql.GetMssqlStringTypesAsync();
                AssertSingularEquals(expected, actual);
            }

            void AssertSingularEquals(QuerySql.GetMssqlStringTypesRow x, QuerySql.GetMssqlStringTypesRow y)
            {
                Assert.That(x.CChar, Is.EqualTo(y.CChar));
                Assert.That(x.CNchar, Is.EqualTo(y.CNchar));
                Assert.That(x.CVarchar, Is.EqualTo(y.CVarchar));
                Assert.That(x.CNvarchar, Is.EqualTo(y.CNvarchar));
                Assert.That(x.CVarcharMax, Is.EqualTo(y.CVarcharMax));
                Assert.That(x.CNvarcharMax, Is.EqualTo(y.CNvarcharMax));
            }
        }

        [Test]
        [TestCase(100, true)]
        [TestCase(10, false)]
        public async Task TestMssqlStringCopyFrom(int batchSize, bool withValues)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertMssqlStringTypesBatchArgs { CChar = withValues ? (string? )"abcdefghij" : null, CNchar = withValues ? (string? )"\u1857\u2649abcdefgh" : null, CVarchar = withValues ? (string? )"Holiday in Cambodia" : null, CNvarchar = withValues ? (string? )"Sheena \u2649 is a Punk Rocker" : null, CVarcharMax = withValues ? (string? )"London's Calling" : null, CNvarcharMax = withValues ? (string? )"Police & Thieves \u1857" : null }).ToList();
            await QuerySql.InsertMssqlStringTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetMssqlStringTypesCntRow
            {
                Cnt = batchSize,
                CChar = withValues ? (string? )"abcdefghij" : null,
                CNchar = withValues ? (string? )"\u1857\u2649abcdefgh" : null,
                CVarchar = withValues ? (string? )"Holiday in Cambodia" : null,
                CNvarchar = withValues ? (string? )"Sheena \u2649 is a Punk Rocker" : null,
                CVarcharMax = withValues ? (string? )"London's Calling" : null,
                CNvarcharMax = withValues ? (string? )"Police & Thieves \u1857" : null
            };
            var actual = await QuerySql.GetMssqlStringTypesCntAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetMssqlStringTypesCntRow x, QuerySql.GetMssqlStringTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CChar, Is.EqualTo(y.CChar));
                Assert.That(x.CNchar, Is.EqualTo(y.CNchar));
                Assert.That(x.CVarchar, Is.EqualTo(y.CVarchar));
                Assert.That(x.CNvarchar, Is.EqualTo(y.CNvarchar));
                Assert.That(x.CVarcharMax, Is.EqualTo(y.CVarcharMax));
                Assert.That(x.CNvarcharMax, Is.EqualTo(y.CNvarcharMax));
            }
        }

        [Test]
        public async Task TestMssqlDateTimeTypes()
        {
            await AssertRoundTrip(true);
            await QuerySql.TruncateMssqlDatetimeTypesAsync();
            await AssertRoundTrip(false);
            async Task AssertRoundTrip(bool withValues)
            {
                await QuerySql.InsertMssqlDatetimeTypesAsync(new QuerySql.InsertMssqlDatetimeTypesArgs { CDate = withValues ? (DateTime? )new DateTime(2020, 1, 2) : null, CDatetime = withValues ? (DateTime? )new DateTime(2020, 5, 6, 7, 8, 9) : null, CDatetime2 = withValues ? (DateTime? )new DateTime(2020, 5, 6, 7, 8, 9, 123) : null, CSmalldatetime = withValues ? (DateTime? )new DateTime(2020, 5, 6, 7, 8, 0) : null, CDatetimeoffset = withValues ? (DateTimeOffset? )new DateTimeOffset(2020, 5, 6, 7, 8, 9, TimeSpan.FromHours(2)) : null, CTime = withValues ? (TimeSpan? )new TimeSpan(7, 8, 9) : null });
                var expected = new QuerySql.GetMssqlDatetimeTypesRow
                {
                    CDate = withValues ? (DateTime? )new DateTime(2020, 1, 2) : null,
                    CDatetime = withValues ? (DateTime? )new DateTime(2020, 5, 6, 7, 8, 9) : null,
                    CDatetime2 = withValues ? (DateTime? )new DateTime(2020, 5, 6, 7, 8, 9, 123) : null,
                    CSmalldatetime = withValues ? (DateTime? )new DateTime(2020, 5, 6, 7, 8, 0) : null,
                    CDatetimeoffset = withValues ? (DateTimeOffset? )new DateTimeOffset(2020, 5, 6, 7, 8, 9, TimeSpan.FromHours(2)) : null,
                    CTime = withValues ? (TimeSpan? )new TimeSpan(7, 8, 9) : null
                };
                var actual = await QuerySql.GetMssqlDatetimeTypesAsync();
                AssertSingularEquals(expected, actual);
            }

            void AssertSingularEquals(QuerySql.GetMssqlDatetimeTypesRow x, QuerySql.GetMssqlDatetimeTypesRow y)
            {
                Assert.That(x.CDate, Is.EqualTo(y.CDate));
                Assert.That(x.CDatetime, Is.EqualTo(y.CDatetime));
                Assert.That(x.CDatetime2, Is.EqualTo(y.CDatetime2));
                Assert.That(x.CSmalldatetime, Is.EqualTo(y.CSmalldatetime));
                Assert.That(x.CDatetimeoffset, Is.EqualTo(y.CDatetimeoffset));
                Assert.That(x.CTime, Is.EqualTo(y.CTime));
            }
        }

        [Test]
        [TestCase(100, true)]
        [TestCase(10, false)]
        public async Task TestMssqlDateTimeCopyFrom(int batchSize, bool withValues)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertMssqlDatetimeTypesBatchArgs { CDate = withValues ? (DateTime? )new DateTime(2020, 1, 2) : null, CDatetime = withValues ? (DateTime? )new DateTime(2020, 5, 6, 7, 8, 9) : null, CDatetime2 = withValues ? (DateTime? )new DateTime(2020, 5, 6, 7, 8, 9, 123) : null, CSmalldatetime = withValues ? (DateTime? )new DateTime(2020, 5, 6, 7, 8, 0) : null, CDatetimeoffset = withValues ? (DateTimeOffset? )new DateTimeOffset(2020, 5, 6, 7, 8, 9, TimeSpan.FromHours(2)) : null, CTime = withValues ? (TimeSpan? )new TimeSpan(7, 8, 9) : null }).ToList();
            await QuerySql.InsertMssqlDatetimeTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetMssqlDatetimeTypesCntRow
            {
                Cnt = batchSize,
                CDate = withValues ? (DateTime? )new DateTime(2020, 1, 2) : null,
                CDatetime = withValues ? (DateTime? )new DateTime(2020, 5, 6, 7, 8, 9) : null,
                CDatetime2 = withValues ? (DateTime? )new DateTime(2020, 5, 6, 7, 8, 9, 123) : null,
                CSmalldatetime = withValues ? (DateTime? )new DateTime(2020, 5, 6, 7, 8, 0) : null,
                CDatetimeoffset = withValues ? (DateTimeOffset? )new DateTimeOffset(2020, 5, 6, 7, 8, 9, TimeSpan.FromHours(2)) : null,
                CTime = withValues ? (TimeSpan? )new TimeSpan(7, 8, 9) : null
            };
            var actual = await QuerySql.GetMssqlDatetimeTypesCntAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetMssqlDatetimeTypesCntRow x, QuerySql.GetMssqlDatetimeTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CDate, Is.EqualTo(y.CDate));
                Assert.That(x.CDatetime, Is.EqualTo(y.CDatetime));
                Assert.That(x.CDatetime2, Is.EqualTo(y.CDatetime2));
                Assert.That(x.CSmalldatetime, Is.EqualTo(y.CSmalldatetime));
                Assert.That(x.CDatetimeoffset, Is.EqualTo(y.CDatetimeoffset));
                Assert.That(x.CTime, Is.EqualTo(y.CTime));
            }
        }

        [Test]
        public async Task TestMssqlBinaryTypes()
        {
            await AssertRoundTrip(true);
            await QuerySql.TruncateMssqlBinaryTypesAsync();
            await AssertRoundTrip(false);
            async Task AssertRoundTrip(bool withValues)
            {
                await QuerySql.InsertMssqlBinaryTypesAsync(new QuerySql.InsertMssqlBinaryTypesArgs { CBinary = withValues ? (byte[]? )new byte[] { 1, 2, 3 } : null, CVarbinary = withValues ? (byte[]? )new byte[] { 4, 5, 6, 7 } : null, CVarbinaryMax = withValues ? (byte[]? )new byte[] { 8, 9, 10, 11, 12 } : null, CUniqueidentifier = withValues ? (Guid? )Guid.Parse("a3b1c5d7-1234-4abc-9def-0123456789ab") : null });
                var expected = new QuerySql.GetMssqlBinaryTypesRow
                {
                    CBinary = withValues ? (byte[]? )new byte[]
                    {
                        1,
                        2,
                        3
                    }

                    : null,
                    CVarbinary = withValues ? (byte[]? )new byte[]
                    {
                        4,
                        5,
                        6,
                        7
                    }

                    : null,
                    CVarbinaryMax = withValues ? (byte[]? )new byte[]
                    {
                        8,
                        9,
                        10,
                        11,
                        12
                    }

                    : null,
                    CUniqueidentifier = withValues ? (Guid? )Guid.Parse("a3b1c5d7-1234-4abc-9def-0123456789ab") : null
                };
                var actual = await QuerySql.GetMssqlBinaryTypesAsync();
                AssertSingularEquals(expected, actual);
            }

            void AssertSingularEquals(QuerySql.GetMssqlBinaryTypesRow x, QuerySql.GetMssqlBinaryTypesRow y)
            {
                Assert.That(x.CBinary, Is.EqualTo(y.CBinary));
                Assert.That(x.CVarbinary, Is.EqualTo(y.CVarbinary));
                Assert.That(x.CVarbinaryMax, Is.EqualTo(y.CVarbinaryMax));
                Assert.That(x.CUniqueidentifier, Is.EqualTo(y.CUniqueidentifier));
            }
        }

        [Test]
        [TestCase(100, true)]
        [TestCase(10, false)]
        public async Task TestMssqlBinaryCopyFrom(int batchSize, bool withValues)
        {
            var batchArgs = Enumerable.Range(0, batchSize).Select(_ => new QuerySql.InsertMssqlBinaryTypesBatchArgs { CBinary = withValues ? (byte[]? )new byte[] { 1, 2, 3 } : null, CVarbinary = withValues ? (byte[]? )new byte[] { 4, 5, 6, 7 } : null, CVarbinaryMax = withValues ? (byte[]? )new byte[] { 8, 9, 10, 11, 12 } : null, CUniqueidentifier = withValues ? (Guid? )Guid.Parse("a3b1c5d7-1234-4abc-9def-0123456789ab") : null }).ToList();
            await QuerySql.InsertMssqlBinaryTypesBatchAsync(batchArgs);
            var expected = new QuerySql.GetMssqlBinaryTypesCntRow
            {
                Cnt = batchSize,
                CBinary = withValues ? (byte[]? )new byte[]
                {
                    1,
                    2,
                    3
                }

                : null,
                CVarbinary = withValues ? (byte[]? )new byte[]
                {
                    4,
                    5,
                    6,
                    7
                }

                : null,
                CVarbinaryMax = withValues ? (byte[]? )new byte[]
                {
                    8,
                    9,
                    10,
                    11,
                    12
                }

                : null,
                CUniqueidentifier = withValues ? (Guid? )Guid.Parse("a3b1c5d7-1234-4abc-9def-0123456789ab") : null
            };
            var actual = await QuerySql.GetMssqlBinaryTypesCntAsync();
            AssertSingularEquals(expected, actual);
            void AssertSingularEquals(QuerySql.GetMssqlBinaryTypesCntRow x, QuerySql.GetMssqlBinaryTypesCntRow y)
            {
                Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                Assert.That(x.CBinary, Is.EqualTo(y.CBinary));
                Assert.That(x.CVarbinary, Is.EqualTo(y.CVarbinary));
                Assert.That(x.CVarbinaryMax, Is.EqualTo(y.CVarbinaryMax));
                Assert.That(x.CUniqueidentifier, Is.EqualTo(y.CUniqueidentifier));
            }
        }

        [Test]
        public async Task TestMssqlTransaction()
        {
            var connection = new Microsoft.Data.SqlClient.SqlConnection(Environment.GetEnvironmentVariable(EndToEndCommon.MssqlConnectionStringEnv));
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();
            var querySqlWithTx = QuerySql.WithTransaction(transaction);
            await querySqlWithTx.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            // the uncommitted row is locked by the transaction, so it is read from within it
            var actual = await querySqlWithTx.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            ClassicAssert.IsNotNull(actual);
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
        public async Task TestMssqlTransactionRollback()
        {
            var connection = new Microsoft.Data.SqlClient.SqlConnection(Environment.GetEnvironmentVariable(EndToEndCommon.MssqlConnectionStringEnv));
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();
            var querySqlWithTx = QuerySql.WithTransaction(transaction);
            await querySqlWithTx.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });
            await transaction.RollbackAsync();
            var actual = await QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
            ClassicAssert.IsNull(actual);
        }
    }
}
