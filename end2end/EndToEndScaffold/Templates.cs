using System.Collections.Generic;

namespace EndToEndScaffold;

public readonly record struct TestImpl(string Modern, string Legacy);

public static class Templates
{
    public static Dictionary<KnownTestType, TestImpl> TestImplementations { get; } =
        new()
        {
            [KnownTestType.One] = new TestImpl
            {
                Modern = """
                             [Test]
                             public async Task TestOne()
                             {
                                 await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs 
                                 {
                                     Name = DataGenerator.BojackAuthor,
                                     Bio = DataGenerator.BojackTheme
                                 });
                                 await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs 
                                 {
                                     Name = DataGenerator.DrSeussAuthor,
                                     Bio = DataGenerator.DrSeussQuote
                                 });
                             
                                 var actualAuthor = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs 
                                 {
                                     Name = DataGenerator.BojackAuthor
                                 });
                                 ClassicAssert.IsNotNull(actualAuthor);
                                 Assert.That(actualAuthor is 
                                 {
                                     Name: DataGenerator.BojackAuthor,
                                     Bio: DataGenerator.BojackTheme
                                 });
                             }
                             """,
                Legacy = """
                             [Test]
                             public async Task TestOne()
                             {
                                 await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
                                 {
                                     Name = DataGenerator.BojackAuthor,
                                     Bio = DataGenerator.BojackTheme
                                 });
                                 await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
                                 {
                                     Name = DataGenerator.DrSeussAuthor,
                                     Bio = DataGenerator.DrSeussQuote
                                 });
                             
                                 var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs
                                 {
                                     Name = DataGenerator.BojackAuthor
                                 });
                                 var expected = new QuerySql.GetAuthorRow
                                 {
                                     Name = DataGenerator.BojackAuthor,
                                     Bio = DataGenerator.BojackTheme
                                 };
                                 Assert.That(Equals(expected, actual));
                             }
                             
                             private static bool Equals(QuerySql.GetAuthorRow x, QuerySql.GetAuthorRow y)
                             {
                                 return x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
                             }
                             """
            },
            [KnownTestType.Many] = new TestImpl
            {
                Modern = """
                             [Test]
                             public async Task TestMany()
                             {
                                 await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs 
                                 {
                                     Name = DataGenerator.BojackAuthor,
                                     Bio = DataGenerator.BojackTheme
                                 });
                                 await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs 
                                 {
                                     Name = DataGenerator.DrSeussAuthor,
                                     Bio = DataGenerator.DrSeussQuote
                                 });
                             
                                 var actualAuthors = await this.QuerySql.ListAuthors();
                                 ClassicAssert.AreEqual(2, actualAuthors.Count);
                                 Assert.That(actualAuthors is [
                                     {
                                         Name: DataGenerator.BojackAuthor,
                                         Bio: DataGenerator.BojackTheme
                                     },
                                     {
                                         Name: DataGenerator.DrSeussAuthor,
                                         Bio: DataGenerator.DrSeussQuote
                                     }
                                 ]);
                             }
                             """,
                Legacy = """
                             [Test]
                             public async Task TestMany()
                             {
                                 await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs 
                                 {
                                     Name = DataGenerator.BojackAuthor,
                                     Bio = DataGenerator.BojackTheme
                                 });
                                 await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs 
                                 {
                                     Name = DataGenerator.DrSeussAuthor,
                                     Bio = DataGenerator.DrSeussQuote
                                 });
                             
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
                             
                             private static bool Equals(QuerySql.ListAuthorsRow x, QuerySql.ListAuthorsRow y)
                             {
                                 return x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
                             }
                             
                             private static bool SequenceEquals(List<QuerySql.ListAuthorsRow> x, List<QuerySql.ListAuthorsRow> y)
                             {
                                 if (x.Count != y.Count) return false;
                                 x = x.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Name + o.Bio).ToList();
                                 y = y.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Name + o.Bio).ToList();
                                 return !x.Where((t, i) => !Equals(t, y[i])).Any();
                             }
                             """
            },
            [KnownTestType.Exec] = new TestImpl
            {
                Modern = """
                             [Test]
                             public async Task TestExec()
                             {
                                 await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs 
                                 {
                                     Name = DataGenerator.BojackAuthor,
                                     Bio = DataGenerator.BojackTheme
                                 });
                                 await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs 
                                 {
                                     Name = DataGenerator.DrSeussAuthor,
                                     Bio = DataGenerator.DrSeussQuote
                                 });
                             
                                 await this.QuerySql.DeleteAuthor(new QuerySql.DeleteAuthorArgs 
                                 { 
                                    Name = DataGenerator.BojackAuthor 
                                });
                                 var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs 
                                 {
                                     Name = DataGenerator.BojackAuthor
                                 });
                                 ClassicAssert.IsNull(actual);
                             }
                             """,
                Legacy = """
                             [Test]
                             public async Task TestExec()
                             {
                                 await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs 
                                 {
                                     Name = DataGenerator.BojackAuthor,
                                     Bio = DataGenerator.BojackTheme
                                 });
                                 await this.QuerySql.DeleteAuthor(new QuerySql.DeleteAuthorArgs 
                                 {
                                     Name = DataGenerator.BojackAuthor
                                 });
                                 var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs 
                                 {
                                     Name = DataGenerator.BojackAuthor
                                 });
                                 ClassicAssert.IsNull(actual);
                             }
                             """
            },
            [KnownTestType.ExecRows] = new TestImpl
            {
                Modern = """
                             [Test]
                             public async Task TestExecRows()
                             {
                                 var createAuthorArgs = new QuerySql.CreateAuthorArgs
                                 {
                                     Name = DataGenerator.GenericAuthor,
                                     Bio = DataGenerator.GenericQuote1
                                 };
                                 await this.QuerySql.CreateAuthor(createAuthorArgs);
                                 await this.QuerySql.CreateAuthor(createAuthorArgs);
                             
                                 var updateAuthorsArgs = new QuerySql.UpdateAuthorsArgs
                                 {
                                     Bio = DataGenerator.GenericQuote1
                                 };
                                 var affectedRows = await this.QuerySql.UpdateAuthors(updateAuthorsArgs);
                                 ClassicAssert.AreEqual(2, affectedRows);
                             }
                             """,
                Legacy = """
                             [Test]
                             public async Task TestExecRows()
                             {
                                 var createAuthorArgs = new QuerySql.CreateAuthorArgs
                                 {
                                     Name = DataGenerator.GenericAuthor,
                                     Bio = DataGenerator.GenericQuote1
                                 };
                                 await this.QuerySql.CreateAuthor(createAuthorArgs);
                                 await this.QuerySql.CreateAuthor(createAuthorArgs);
                             
                                 var updateAuthorsArgs = new QuerySql.UpdateAuthorsArgs
                                 {
                                     Bio = DataGenerator.GenericQuote1
                                 };
                                 var affectedRows = await this.QuerySql.UpdateAuthors(updateAuthorsArgs);
                                 ClassicAssert.AreEqual(2, affectedRows);
                             }
                             """
            },
            [KnownTestType.ExecLastId] = new TestImpl
            {
                Modern = """
                             [Test]
                             public async Task TestExecLastId()
                             {
                                 var createAuthorArgs = new QuerySql.CreateAuthorReturnIdArgs 
                                 {
                                     Name = DataGenerator.GenericAuthor,
                                     Bio = DataGenerator.GenericQuote1
                                 };
                                 var insertedId = await QuerySql.CreateAuthorReturnId(createAuthorArgs);
                             
                                 var getAuthorByIdArgs = new QuerySql.GetAuthorByIdArgs 
                                 {
                                     Id = insertedId
                                 };
                                 var actual = await QuerySql.GetAuthorById(getAuthorByIdArgs);
                                 Assert.That(actual is 
                                 {
                                     Name: DataGenerator.GenericAuthor,
                                     Bio: DataGenerator.GenericQuote1
                                 });
                             }
                             """,
                Legacy = """
                             [Test]
                             public async Task TestExecLastId()
                             {
                                 var createAuthorArgs = new QuerySql.CreateAuthorReturnIdArgs 
                                 {
                                     Name = DataGenerator.GenericAuthor,
                                     Bio = DataGenerator.GenericQuote1
                                 };
                                 var insertedId = await QuerySql.CreateAuthorReturnId(createAuthorArgs);
                             
                                 var expected = new QuerySql.GetAuthorByIdRow 
                                 {
                                     Id = insertedId,
                                     Name = DataGenerator.GenericAuthor,
                                     Bio = DataGenerator.GenericQuote1
                                 };
                                 var actual = await QuerySql.GetAuthorById(new QuerySql.GetAuthorByIdArgs 
                                 {
                                     Id = insertedId
                                 });
                                 ClassicAssert.IsNotNull(actual);
                                 Assert.That(Equals(expected, actual));
                             }
                             
                             private static bool Equals(QuerySql.GetAuthorByIdRow x, QuerySql.GetAuthorByIdRow y)
                             {
                                 return x.Id.Equals(y.Id) && x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
                             }
                             """
            },
            [KnownTestType.SqlcEmbed] = new TestImpl
            {
                Modern = """
                         [Test]
                         public async Task TestJoinEmbed()
                         {
                            var createAuthorArgs = new QuerySql.CreateAuthorReturnIdArgs
                            {
                                Name = DataGenerator.BojackAuthor,
                                Bio = DataGenerator.BojackTheme
                            };
                            var bojackAuthorId = await QuerySql.CreateAuthorReturnId(createAuthorArgs);
                            var createBookArgs = new QuerySql.CreateBookArgs
                            {
                                Name = DataGenerator.BojackBookTitle,
                                AuthorId = bojackAuthorId
                            };
                            await QuerySql.CreateBook(createBookArgs);
                         
                            createAuthorArgs = new QuerySql.CreateAuthorReturnIdArgs
                            {
                                Name = DataGenerator.DrSeussAuthor,
                                Bio = DataGenerator.DrSeussQuote
                            };
                            var drSeussAuthorId = await QuerySql.CreateAuthorReturnId(createAuthorArgs);
                            createBookArgs = new QuerySql.CreateBookArgs
                            {
                                Name = DataGenerator.DrSeussBookTitle,
                                AuthorId = drSeussAuthorId
                            };
                            await QuerySql.CreateBook(createBookArgs);
                         
                            var actual = await QuerySql.ListAllAuthorsBooks();
                            Assert.That(actual is
                            [
                            {
                               Author:
                               {
                                   Name: DataGenerator.BojackAuthor,
                                   Bio: DataGenerator.BojackTheme,
                               },
                               Book.Name: DataGenerator.BojackBookTitle,
                            },
                            {
                               Author:
                               {
                                   Name: DataGenerator.DrSeussAuthor,
                                   Bio: DataGenerator.DrSeussQuote,
                               },
                               Book.Name: DataGenerator.DrSeussBookTitle,
                            }
                            ]);
                         }
                         """,
                Legacy = """
                         [Test]
                         public async Task TestJoinEmbed()
                         {
                            var createAuthorArgs = new QuerySql.CreateAuthorReturnIdArgs
                            {
                                Name = DataGenerator.BojackAuthor,
                                Bio = DataGenerator.BojackTheme
                            };
                            var bojackAuthorId = await QuerySql.CreateAuthorReturnId(createAuthorArgs);
                            var createBookArgs = new QuerySql.CreateBookArgs
                            {
                                Name = DataGenerator.BojackBookTitle,
                                AuthorId = bojackAuthorId
                            };
                            await QuerySql.CreateBook(createBookArgs);
                         
                            createAuthorArgs = new QuerySql.CreateAuthorReturnIdArgs
                            {
                                Name = DataGenerator.DrSeussAuthor,
                                Bio = DataGenerator.DrSeussQuote
                            };
                            var drSeussAuthorId = await QuerySql.CreateAuthorReturnId(createAuthorArgs);
                            createBookArgs = new QuerySql.CreateBookArgs
                            {
                                Name = DataGenerator.DrSeussBookTitle,
                                AuthorId = drSeussAuthorId
                            };
                            await QuerySql.CreateBook(createBookArgs);
                         
                            var expected = new List<QuerySql.ListAllAuthorsBooksRow>()
                            {
                                new QuerySql.ListAllAuthorsBooksRow
                                {
                                    Author = new Author { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme, },
                                    Book = new Book { Name = DataGenerator.BojackBookTitle, }
                                },
                                new QuerySql.ListAllAuthorsBooksRow
                                {
                                    Author = new Author { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote, },
                                    Book = new Book { Name = DataGenerator.DrSeussBookTitle, }
                                }
                            };
                            var actual = await QuerySql.ListAllAuthorsBooks();
                            Assert.That(SequenceEquals(expected, actual));
                         }
                         
                         private static bool Equals(QuerySql.ListAllAuthorsBooksRow x, QuerySql.ListAllAuthorsBooksRow y)
                         {
                             return x.Author.Name.Equals(y.Author.Name) && x.Author.Bio.Equals(y.Author.Bio) && x.Book.Name.Equals(y.Book.Name);
                         }
                         
                         private static bool SequenceEquals(List<QuerySql.ListAllAuthorsBooksRow> x, List<QuerySql.ListAllAuthorsBooksRow> y)
                         {
                             if (x.Count != y.Count)
                                 return false;
                             x = x.OrderBy<QuerySql.ListAllAuthorsBooksRow, object>(o => o.Author.Name + o.Book.Name).ToList();
                             y = y.OrderBy<QuerySql.ListAllAuthorsBooksRow, object>(o => o.Author.Name + o.Book.Name).ToList();
                             return !x.Where((t, i) => !Equals(t, y[i])).Any();
                         }
                         """
            }
        };
}