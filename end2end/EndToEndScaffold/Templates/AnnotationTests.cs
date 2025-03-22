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
                     public async Task TestOne()
                     {
                         {{Consts.CreateBojackAuthor}}
                         {{Consts.CreateDrSeussAuthor}}
                         var expected = new QuerySql.GetAuthorRow
                         {
                             Id = {{Consts.BojackId}},
                             Name = {{Consts.BojackAuthor}},
                             Bio = {{Consts.BojackTheme}}
                         };
                         var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs
                         {
                             Name = {{Consts.BojackAuthor}}
                         });
                         Assert.That(SingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}}));
                     }

                     private static bool SingularEquals(QuerySql.GetAuthorRow x, QuerySql.GetAuthorRow y)
                     {
                         return x.Id.Equals(y.Id) && 
                             x.Name.Equals(y.Name) && 
                             x.Bio.Equals(y.Bio);
                     }
                     """
        },
        [KnownTestType.Many] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestMany()
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
                         var actual = await this.QuerySql.ListAuthors();
                         Assert.That(SequenceEquals(expected, actual));
                     }

                     private static bool SingularEquals(QuerySql.ListAuthorsRow x, QuerySql.ListAuthorsRow y)
                     {
                         return x.Id.Equals(y.Id) && 
                             x.Name.Equals(y.Name) && 
                             x.Bio.Equals(y.Bio);
                     }

                     private static bool SequenceEquals(List<QuerySql.ListAuthorsRow> x, List<QuerySql.ListAuthorsRow> y)
                     {
                         if (x.Count != y.Count) return false;
                         x = x.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Id).ToList();
                         y = y.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Id).ToList();
                         return !x.Where((t, i) => !SingularEquals(t, y[i])).Any();
                     }
                     """
        },
        [KnownTestType.Exec] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestExec()
                     {
                         {{Consts.CreateBojackAuthor}}
                         {{Consts.CreateDrSeussAuthor}}
                         await this.QuerySql.DeleteAuthor(new QuerySql.DeleteAuthorArgs 
                         { 
                             Name = {{Consts.BojackAuthor}} 
                         });
                         var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs 
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
                     public async Task TestExecRows()
                     {
                         {{Consts.CreateBojackAuthor}}
                         {{Consts.CreateDrSeussAuthor}}
                         var affectedRows = await this.QuerySql.UpdateAuthors(new QuerySql.UpdateAuthorsArgs
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
                         var actual = await this.QuerySql.ListAuthors();
                         Assert.That(SequenceEquals(expected, actual));
                     }
                     """,
        },
        [KnownTestType.ExecLastId] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestExecLastId()
                     {
                         {{Consts.CreateFirstGenericAuthor}}
                         var expected = new QuerySql.GetAuthorByIdRow 
                         {
                             Id = id1,
                             Name = {{Consts.GenericAuthor}},
                             Bio = {{Consts.GenericQuote1}}
                         };
                         var actual = await QuerySql.GetAuthorById(new QuerySql.GetAuthorByIdArgs 
                         {
                             Id = id1
                         });
                         Assert.That(SingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}}));
                     }

                     private static bool SingularEquals(QuerySql.GetAuthorByIdRow x, QuerySql.GetAuthorByIdRow y)
                     {
                         return x.Id.Equals(y.Id) && 
                             x.Name.Equals(y.Name) && 
                             x.Bio.Equals(y.Bio);
                     }
                     """
        },
    };
}