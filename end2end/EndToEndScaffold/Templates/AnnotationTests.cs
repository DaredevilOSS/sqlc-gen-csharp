using System.Collections.Generic;

namespace EndToEndScaffold.Templates;

public readonly record struct TestImpl(string Impl);

public static class AnnotationTests
{
    public static Dictionary<KnownTestType, TestImpl> TestImplementations { get; } = new()
    {
        [KnownTestType.One] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestOneAsync()
                     {
                         {{Consts.CreateBojackAuthor}}
                         {{Consts.CreateDrSeussAuthor}}
                         var expected = new QuerySql.GetAuthorRow
                         {
                             Id = {{Consts.BojackId}},
                             Name = {{Consts.BojackAuthor}},
                             Bio = {{Consts.BojackTheme}}
                         };
                         var actual = await this.QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs
                         {
                             Name = {{Consts.BojackAuthor}}
                         });
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetAuthorRow x, QuerySql.GetAuthorRow y)
                         {
                            Assert.That(x.Id, Is.EqualTo(y.Id));
                            Assert.That(x.Name, Is.EqualTo(y.Name));
                            Assert.That(x.Bio, Is.EqualTo(y.Bio));
                         }
                     }
                     """
        },
        [KnownTestType.Many] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestManyAsync()
                     {
                         {{Consts.CreateBojackAuthor}}
                         {{Consts.CreateDrSeussAuthor}}
                         var expected = new List<QuerySql.ListAuthorsRow>
                         {
                             new QuerySql.ListAuthorsRow 
                             { 
                                 Id = {{Consts.BojackId}},
                                 Name = {{Consts.BojackAuthor}}, 
                                 Bio = {{Consts.BojackTheme}}
                             },
                             new QuerySql.ListAuthorsRow 
                             { 
                                 Id = {{Consts.DrSeussId}},
                                 Name = {{Consts.DrSeussAuthor}}, 
                                 Bio = {{Consts.DrSeussQuote}}
                             }
                         };
                         var actual = await this.QuerySql.ListAuthorsAsync(new QuerySql.ListAuthorsArgs
                         {
                             Limit = 2,
                             Offset = 0
                         });
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
                     """
        },
        [KnownTestType.Exec] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestExecAsync()
                     {
                         {{Consts.CreateBojackAuthor}}
                         {{Consts.CreateDrSeussAuthor}}
                         await this.QuerySql.DeleteAuthorAsync(new QuerySql.DeleteAuthorArgs 
                         { 
                             Name = {{Consts.BojackAuthor}} 
                         });
                         var actual = await this.QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs 
                         {
                             Name = {{Consts.BojackAuthor}}
                         });
                         ClassicAssert.IsNull(actual);
                     }
                     """,
        },
        [KnownTestType.ExecRows] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestExecRowsAsync()
                     {
                         {{Consts.CreateBojackAuthor}}
                         {{Consts.CreateDrSeussAuthor}}
                         var affectedRows = await this.QuerySql.UpdateAuthorsAsync(new QuerySql.UpdateAuthorsArgs
                         {
                             Bio = {{Consts.GenericQuote1}}
                         });
                         ClassicAssert.AreEqual(2, affectedRows);
                         var expected = new List<QuerySql.ListAuthorsRow>
                         {
                             new QuerySql.ListAuthorsRow 
                             { 
                                 Id = {{Consts.BojackId}},
                                 Name = {{Consts.BojackAuthor}}, 
                                 Bio = {{Consts.GenericQuote1}}
                             },
                             new QuerySql.ListAuthorsRow 
                             { 
                                 Id = {{Consts.DrSeussId}},
                                 Name = {{Consts.DrSeussAuthor}}, 
                                 Bio = {{Consts.GenericQuote1}}
                             }
                         };
                         var actual = await this.QuerySql.ListAuthorsAsync(new QuerySql.ListAuthorsArgs
                         {
                             Limit = 2,
                             Offset = 0
                         });
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
                     """,
        },
        [KnownTestType.ExecLastId] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestExecLastIdAsync()
                     {
                         {{Consts.CreateFirstGenericAuthor}}
                         var expected = new QuerySql.GetAuthorByIdRow 
                         {
                             Id = id1,
                             Name = {{Consts.GenericAuthor}},
                             Bio = {{Consts.GenericQuote1}}
                         };
                         var actual = await QuerySql.GetAuthorByIdAsync(new QuerySql.GetAuthorByIdArgs 
                         {
                             Id = id1
                         });
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetAuthorByIdRow x, QuerySql.GetAuthorByIdRow y)
                         {
                             Assert.That(x.Id, Is.EqualTo(y.Id));
                             Assert.That(x.Name, Is.EqualTo(y.Name));
                             Assert.That(x.Bio, Is.EqualTo(y.Bio));
                         }
                     }
                     """
        },
    };
}