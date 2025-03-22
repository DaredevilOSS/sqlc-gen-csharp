using System.Collections.Generic;

namespace EndToEndScaffold.Templates;

public static class MacroTests
{
    public static Dictionary<KnownTestType, TestImpl> TestImplementations { get; } = new()
    {
        [KnownTestType.NargNull] = new TestImpl
        {
            Impl = $$"""
                      [Test]
                      public async Task TestNargNull()
                      {
                          {{Consts.CreateBojackAuthor}}
                          {{Consts.CreateDrSeussAuthor}}
                          var expected = new List<QuerySql.GetAuthorByNamePatternRow>
                          {
                              new QuerySql.GetAuthorByNamePatternRow
                              {
                                  Id = {{Consts.BojackId}},
                                  Name = {{Consts.BojackAuthor}},
                                  Bio = {{Consts.BojackTheme}}
                              },
                              new QuerySql.GetAuthorByNamePatternRow
                              {
                                  Id = {{Consts.DrSeussId}},
                                  Name = {{Consts.DrSeussAuthor}},
                                  Bio = {{Consts.DrSeussQuote}}
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
                     """
        },
        [KnownTestType.NargNotNull] = new TestImpl
        {
            Impl = $$"""
                      [Test]
                      public async Task TestNargNotNull()
                      {
                          {{Consts.CreateBojackAuthor}}
                          {{Consts.CreateDrSeussAuthor}}
                     
                          var expected = new List<QuerySql.GetAuthorByNamePatternRow>
                          {
                              new QuerySql.GetAuthorByNamePatternRow
                              {
                                  Id = {{Consts.BojackId}},
                                  Name = {{Consts.BojackAuthor}},
                                  Bio = {{Consts.BojackTheme}}
                              }
                          };
                     
                          var actual = await this.QuerySql.GetAuthorByNamePattern(new QuerySql.GetAuthorByNamePatternArgs { NamePattern = "Bojack%" });
                          Assert.That(SequenceEquals(expected, actual));
                      }
                     """
        },
        [KnownTestType.JoinEmbed] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestJoinEmbed()
                     {
                         {{Consts.CreateBojackAuthorWithId}}
                         {{Consts.CreateBookByBojack}}
                         {{Consts.CreateDrSeussAuthorWithId}}
                         {{Consts.CreateBookByDrSeuss}}
                         var expected = new List<QuerySql.ListAllAuthorsBooksRow>()
                         {
                             new QuerySql.ListAllAuthorsBooksRow
                             {
                                 Author = new Author 
                                 { 
                                     Id = bojackId, 
                                     Name = {{Consts.BojackAuthor}}, 
                                     Bio = {{Consts.BojackTheme}}
                                 },
                                 Book = new Book 
                                 { 
                                     Id = bojackBookId,
                                     AuthorId = bojackId, 
                                     Name = {{Consts.BojackBookTitle}}
                                 }
                             },
                             new QuerySql.ListAllAuthorsBooksRow
                             {
                                 Author = new Author 
                                 { 
                                     Id = drSeussId, 
                                     Name = {{Consts.DrSeussAuthor}}, 
                                     Bio = {{Consts.DrSeussQuote}}
                                 },
                                 Book = new Book 
                                 { 
                                     Id = drSeussBookId,
                                     AuthorId = drSeussId, 
                                     Name = {{Consts.DrSeussBookTitle}} 
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
                         return x.Id.Equals(y.Id) && 
                             x.Name.Equals(y.Name) && 
                             x.Bio.Equals(y.Bio);
                     }

                     private static bool SingularEquals(Book x, Book y)
                     {
                         return x.Id.Equals(y.Id) && 
                             x.AuthorId.Equals(y.AuthorId) && 
                             x.Name.Equals(y.Name);
                     }
                     """
        },
        [KnownTestType.SelfJoinEmbed] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestSelfJoinEmbed()
                     {
                         {{Consts.CreateFirstGenericAuthor}}
                         {{Consts.CreateSecondGenericAuthor}}
                         var expected = new List<QuerySql.GetDuplicateAuthorsRow>()
                         {
                             new QuerySql.GetDuplicateAuthorsRow
                             {
                                 Author = new Author 
                                 {
                                     Id = id1,
                                     Name = {{Consts.GenericAuthor}}, 
                                     Bio = {{Consts.GenericQuote1}}
                                 },
                                 Author2 = new Author 
                                 {
                                     Id = id2,
                                     Name = {{Consts.GenericAuthor}}, 
                                     Bio = {{Consts.GenericQuote2}}
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
                     """
        },
        [KnownTestType.PartialEmbed] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestPartialEmbed()
                     {
                         {{Consts.CreateBojackAuthorWithId}}
                         {{Consts.CreateBookByBojack}}
                         {{Consts.CreateDrSeussAuthorWithId}}
                         {{Consts.CreateBookByDrSeuss}}
                         var expected = new List<QuerySql.GetAuthorsByBookNameRow>
                         {
                             new QuerySql.GetAuthorsByBookNameRow
                             {
                                 Id = bojackId,
                                 Name = {{Consts.BojackBookTitle}},
                                 Bio = {{Consts.DrSeussBookTitle}},
                                 Book = new Book 
                                 { 
                                     Id = bojackBookId,
                                     AuthorId = bojackId,
                                     Name = {{Consts.DrSeussBookTitle}} 
                                 }
                             }
                         };
                         var actual = await QuerySql.GetAuthorsByBookName(new QuerySql.GetAuthorsByBookNameArgs 
                         { 
                             Name = {{Consts.BojackBookTitle}} 
                         });
                         Assert.That(SequenceEquals(expected, actual));
                     }

                     private static bool SingularEquals(QuerySql.GetAuthorsByBookNameRow x, QuerySql.GetAuthorsByBookNameRow y)
                     {
                         return x.Id.Equals(y.Id) && 
                             x.Name.Equals(y.Name) && 
                             x.Bio.Equals(y.Bio) && 
                             SingularEquals(x.Book, y.Book);
                     }

                     private static bool SequenceEquals(List<QuerySql.GetAuthorsByBookNameRow> x, List<QuerySql.GetAuthorsByBookNameRow> y)
                     {
                         if (x.Count != y.Count)
                             return false;
                         return !x.Where((t, i) => !SingularEquals(t, y[i])).Any();
                     }
                     """
        },
        [KnownTestType.Slice] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestSlice()
                     {
                         {{Consts.CreateFirstGenericAuthor}}
                         {{Consts.CreateBojackAuthorWithId}}
                         var actual = await QuerySql.GetAuthorsByIds(new QuerySql.GetAuthorsByIdsArgs 
                         { 
                             Ids = new[] { id1, bojackId } 
                         });
                         ClassicAssert.AreEqual(2, actual.Count);
                     }
                     """
        },
        [KnownTestType.MultipleSlices] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestMultipleSlices()
                     {
                         {{Consts.CreateFirstGenericAuthor}}
                         {{Consts.CreateBojackAuthorWithId}}
                         var actual = await QuerySql.GetAuthorsByIdsAndNames(new QuerySql.GetAuthorsByIdsAndNamesArgs 
                         { 
                             Ids = new[] { id1, bojackId }, 
                             Names = new[] { {{Consts.GenericAuthor}} } 
                         });
                         ClassicAssert.AreEqual(1, actual.Count);
                     }
                     """
        },
    };
}