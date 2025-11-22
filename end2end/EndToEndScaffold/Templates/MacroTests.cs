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
                      public async Task TestNargNullAsync()
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
                     """
        },
        [KnownTestType.NargNotNull] = new TestImpl
        {
            Impl = $$"""
                      [Test]
                      public async Task TestNargNotNullAsync()
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
                     """
        },
        [KnownTestType.JoinEmbed] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestJoinEmbedAsync()
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
                         var actual = await QuerySql.ListAllAuthorsBooksAsync();
                         AssertSequenceEquals(expected, actual);

                         void AssertSingularEquals(QuerySql.ListAllAuthorsBooksRow x, QuerySql.ListAllAuthorsBooksRow y)
                         {
                             Assert.That(x.Author{{Consts.UnknownRecordValuePlaceholder}}.Id, Is.EqualTo(y.Author{{Consts.UnknownRecordValuePlaceholder}}.Id));
                             Assert.That(x.Author{{Consts.UnknownRecordValuePlaceholder}}.Name, Is.EqualTo(y.Author{{Consts.UnknownRecordValuePlaceholder}}.Name));
                             Assert.That(x.Author{{Consts.UnknownRecordValuePlaceholder}}.Bio, Is.EqualTo(y.Author{{Consts.UnknownRecordValuePlaceholder}}.Bio));
                             Assert.That(x.Book{{Consts.UnknownRecordValuePlaceholder}}.Id, Is.EqualTo(y.Book{{Consts.UnknownRecordValuePlaceholder}}.Id));
                             Assert.That(x.Book{{Consts.UnknownRecordValuePlaceholder}}.AuthorId, Is.EqualTo(y.Book{{Consts.UnknownRecordValuePlaceholder}}.AuthorId));
                             Assert.That(x.Book{{Consts.UnknownRecordValuePlaceholder}}.Name, Is.EqualTo(y.Book{{Consts.UnknownRecordValuePlaceholder}}.Name));
                         }

                         void AssertSequenceEquals(List<QuerySql.ListAllAuthorsBooksRow> x, List<QuerySql.ListAllAuthorsBooksRow> y)
                         {
                             Assert.That(x.Count, Is.EqualTo(y.Count));
                             for (int i = 0; i < x.Count; i++)
                                 AssertSingularEquals(x[i], y[i]);
                         }
                     }
                     """
        },
        [KnownTestType.SelfJoinEmbed] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestSelfJoinEmbedAsync()
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
                         var actual = await QuerySql.GetDuplicateAuthorsAsync();
                         AssertSequenceEquals(expected, actual);

                        void AssertSingularEquals(QuerySql.GetDuplicateAuthorsRow x, QuerySql.GetDuplicateAuthorsRow y)
                        {
                            Assert.That(x.Author{{Consts.UnknownRecordValuePlaceholder}}.Id, Is.EqualTo(y.Author{{Consts.UnknownRecordValuePlaceholder}}.Id));
                            Assert.That(x.Author{{Consts.UnknownRecordValuePlaceholder}}.Name, Is.EqualTo(y.Author{{Consts.UnknownRecordValuePlaceholder}}.Name));
                            Assert.That(x.Author{{Consts.UnknownRecordValuePlaceholder}}.Bio, Is.EqualTo(y.Author{{Consts.UnknownRecordValuePlaceholder}}.Bio));
                            Assert.That(x.Author2{{Consts.UnknownRecordValuePlaceholder}}.Id, Is.EqualTo(y.Author2{{Consts.UnknownRecordValuePlaceholder}}.Id));
                            Assert.That(x.Author2{{Consts.UnknownRecordValuePlaceholder}}.Name, Is.EqualTo(y.Author2{{Consts.UnknownRecordValuePlaceholder}}.Name));
                            Assert.That(x.Author2{{Consts.UnknownRecordValuePlaceholder}}.Bio, Is.EqualTo(y.Author2{{Consts.UnknownRecordValuePlaceholder}}.Bio));
                        }

                        void AssertSequenceEquals(List<QuerySql.GetDuplicateAuthorsRow> x, List<QuerySql.GetDuplicateAuthorsRow> y)
                        {
                            Assert.That(x.Count, Is.EqualTo(y.Count));
                            for (int i = 0; i < x.Count; i++)
                                AssertSingularEquals(x[i], y[i]);
                        }
                     }
                     """
        },
        [KnownTestType.PartialEmbed] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestPartialEmbedAsync()
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
                         var actual = await QuerySql.GetAuthorsByBookNameAsync(new QuerySql.GetAuthorsByBookNameArgs 
                         { 
                             Name = {{Consts.BojackBookTitle}} 
                         });
                         AssertSequenceEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                        void AssertSingularEquals(QuerySql.GetAuthorsByBookNameRow x, QuerySql.GetAuthorsByBookNameRow y)
                        {
                            Assert.That(x.Id, Is.EqualTo(y.Id));
                            Assert.That(x.Name, Is.EqualTo(y.Name));
                            Assert.That(x.Bio, Is.EqualTo(y.Bio));
                            Assert.That(x.Book{{Consts.UnknownRecordValuePlaceholder}}.Id, Is.EqualTo(y.Book{{Consts.UnknownRecordValuePlaceholder}}.Id));
                            Assert.That(x.Book{{Consts.UnknownRecordValuePlaceholder}}.AuthorId, Is.EqualTo(y.Book{{Consts.UnknownRecordValuePlaceholder}}.AuthorId));
                            Assert.That(x.Book{{Consts.UnknownRecordValuePlaceholder}}.Name, Is.EqualTo(y.Book{{Consts.UnknownRecordValuePlaceholder}}.Name));
                        }

                        void AssertSequenceEquals(List<QuerySql.GetAuthorsByBookNameRow> x, List<QuerySql.GetAuthorsByBookNameRow> y)
                        {
                            Assert.That(x.Count, Is.EqualTo(y.Count));
                            for (int i = 0; i < x.Count; i++)
                                AssertSingularEquals(x[i], y[i]);
                        }
                     }
                     """
        },
        [KnownTestType.Slice] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestSliceAsync()
                     {
                         {{Consts.CreateFirstGenericAuthor}}
                         {{Consts.CreateBojackAuthorWithId}}
                         var actual = await QuerySql.GetAuthorsByIdsAsync(new QuerySql.GetAuthorsByIdsArgs 
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
                     public async Task TestMultipleSlicesAsync()
                     {
                         {{Consts.CreateFirstGenericAuthor}}
                         {{Consts.CreateBojackAuthorWithId}}
                         var actual = await QuerySql.GetAuthorsByIdsAndNamesAsync(new QuerySql.GetAuthorsByIdsAndNamesArgs 
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