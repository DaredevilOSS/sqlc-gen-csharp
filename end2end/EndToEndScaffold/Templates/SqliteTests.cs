using System.Collections.Generic;

namespace EndToEndScaffold.Templates;

public static class SqliteTests
{
    public static Dictionary<KnownTestType, TestImpl> TestImplementations { get; } = new()
    {
        [KnownTestType.SqliteDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(-54355, 9787.66, "Songs of Love and Hate", new byte[] { 0x15, 0x20, 0x33 })]
                     [TestCase(null, null, null, new byte[] { })]
                     [TestCase(null, null, null, null)]
                     public async Task TestSqliteTypes(
                          int? cInteger,
                          decimal? cReal,
                          string cText,
                          byte[] cBlob)
                     {
                         await QuerySql.InsertSqliteTypes(new QuerySql.InsertSqliteTypesArgs
                         {
                             CInteger = cInteger,
                             CReal = cReal,
                             CText = cText,
                             CBlob = cBlob
                         });
                     
                         var expected = new QuerySql.GetSqliteTypesRow
                         {
                             CInteger = cInteger,
                             CReal = cReal,
                             CText = cText,
                             CBlob = cBlob
                         };
                         var actual = await QuerySql.GetSqliteTypes();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetSqliteTypesRow x, QuerySql.GetSqliteTypesRow y)
                         {
                             Assert.That(x.CInteger, Is.EqualTo(y.CInteger));
                             Assert.That(x.CReal, Is.EqualTo(y.CReal));
                             Assert.That(x.CText, Is.EqualTo(y.CText));
                             Assert.That(x.CBlob, Is.EqualTo(y.CBlob));
                         }
                     }
                     """
        },
        [KnownTestType.SqliteCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, 312, -7541.3309, "Johnny B. Good")]
                     [TestCase(500, -768, 8453.5678, "Bad to the Bone")]
                     [TestCase(10, null, null, null)]
                     public async Task TestCopyFrom(
                        int batchSize, 
                        int? cInteger, 
                        decimal? cReal, 
                        string cText)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertSqliteTypesBatchArgs
                             {
                                 CInteger = cInteger,
                                 CReal = cReal,
                                 CText = cText
                             })
                             .ToList();
                         await QuerySql.InsertSqliteTypesBatch(batchArgs);
                         var expected = new QuerySql.GetSqliteTypesCntRow
                         {
                             Cnt = batchSize,
                             CInteger = cInteger,
                             CReal = cReal,
                             CText = cText
                         };
                         var actual = await QuerySql.GetSqliteTypesCnt();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});
                         
                         void AssertSingularEquals(QuerySql.GetSqliteTypesCntRow x, QuerySql.GetSqliteTypesCntRow y)
                         {
                             Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                             Assert.That(x.CInteger, Is.EqualTo(y.CInteger));
                             Assert.That(x.CReal, Is.EqualTo(y.CReal));
                             Assert.That(x.CText, Is.EqualTo(y.CText));
                         }
                     }
                     """
        },
        [KnownTestType.SqliteTransaction] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestSqliteTransaction()
                     {
                         var connection = new Microsoft.Data.Sqlite.SqliteConnection(Environment.GetEnvironmentVariable(EndToEndCommon.SqliteConnectionStringEnv));
                         await connection.OpenAsync();
                         var transaction = connection.BeginTransaction();

                         var querySqlWithTx = QuerySql.WithTransaction(transaction);
                         await querySqlWithTx.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = {{Consts.BojackId}}, Name = {{Consts.BojackAuthor}}, Bio = {{Consts.BojackTheme}} });

                         var actual = await QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = {{Consts.BojackAuthor}} });
                         ClassicAssert.IsNull(actual);

                         transaction.Commit();

                         var expected = new QuerySql.GetAuthorRow
                         {
                             Id = {{Consts.BojackId}},
                             Name = {{Consts.BojackAuthor}},
                             Bio = {{Consts.BojackTheme}}
                         };
                         actual = await QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = {{Consts.BojackAuthor}} });
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
        [KnownTestType.SqliteTransactionRollback] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestSqliteTransactionRollback()
                     {
                         var connection = new Microsoft.Data.Sqlite.SqliteConnection(Environment.GetEnvironmentVariable(EndToEndCommon.SqliteConnectionStringEnv));
                         await connection.OpenAsync();
                         var transaction = connection.BeginTransaction();

                         var sqlQueryWithTx = QuerySql.WithTransaction(transaction);
                         await sqlQueryWithTx.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = {{Consts.BojackId}}, Name = {{Consts.BojackAuthor}}, Bio = {{Consts.BojackTheme}} });

                         transaction.Rollback();

                         var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = {{Consts.BojackAuthor}} });
                         ClassicAssert.IsNull(actual);
                     }
                     """
        },
        [KnownTestType.SqliteDataTypesOverride] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(-54355, 9787.66, "Have One On Me")]
                     [TestCase(null, 0.0, null)]
                     public async Task TestSqliteDataTypesOverride(
                        int? cInteger,
                        decimal cReal,
                        string cText)
                     {
                         await QuerySql.InsertSqliteTypes(new QuerySql.InsertSqliteTypesArgs
                         {
                             CInteger = cInteger,
                             CReal = cReal,
                             CText = cText
                         });
                     
                         var expected = new QuerySql.GetSqliteFunctionsRow
                         {
                             MaxInteger = cInteger,
                             MaxReal = cReal,
                             MaxText = cText
                         };
                         var actual = await QuerySql.GetSqliteFunctions();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetSqliteFunctionsRow x, QuerySql.GetSqliteFunctionsRow y)
                         {
                             Assert.That(x.MaxInteger, Is.EqualTo(y.MaxInteger));
                             Assert.That(x.MaxReal, Is.EqualTo(y.MaxReal));
                             Assert.That(x.MaxText, Is.EqualTo(y.MaxText));
                         }
                     }
                     """
        },
        [KnownTestType.SqliteMultipleNamedParam] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestGetAuthorByIdWithMultipleNamedParam()
                     {
                         {{Consts.CreateBojackAuthor}}
                         var expected = new QuerySql.GetAuthorByIdWithMultipleNamedParamRow
                         {
                             Id = {{Consts.BojackId}},
                             Name = {{Consts.BojackAuthor}},
                             Bio = {{Consts.BojackTheme}}
                         };
                         var actual = await this.QuerySql.GetAuthorByIdWithMultipleNamedParam(new QuerySql.GetAuthorByIdWithMultipleNamedParamArgs
                         {
                             IdArg = {{Consts.BojackId}},
                             Take = 1
                         });
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetAuthorByIdWithMultipleNamedParamRow x, QuerySql.GetAuthorByIdWithMultipleNamedParamRow y)
                         {
                             Assert.That(x.Id, Is.EqualTo(y.Id));
                             Assert.That(x.Name, Is.EqualTo(y.Name));
                             Assert.That(x.Bio, Is.EqualTo(y.Bio));
                         }
                     }
                     """
        }
    };
}