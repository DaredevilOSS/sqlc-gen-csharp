using System.Collections.Generic;

namespace EndToEndScaffold.Templates;

public static class SqliteTests
{
    public static Dictionary<KnownTestType, TestImpl> TestImplementations { get; } = new()
    {
        [KnownTestType.SqliteDataTypes] = new TestImpl
        {
            Impl = $$"""
                      private static IEnumerable<TestCaseData> SqliteTypesTestCases
                      {
                          get
                          {
                              yield return new TestCaseData(
                                   -54355, 
                                   9787.66m, 
                                   "Songs of Love and Hate", 
                                   new byte[] { 0x15, 0x20, 0x33 }, 
                                   true, 
                                   false, 
                                   DateTime.SpecifyKind(DateTime.Parse("2020-01-01 14:15:16"), DateTimeKind.Utc), 
                                   DateTime.SpecifyKind(DateTime.Parse("2025-01-01 17:18:19"), DateTimeKind.Utc), 
                                   Instant.FromUtc(2025, 10, 15, 19, 55, 2), 
                                   Instant.FromUtc(1993, 9, 27, 03, 55, 2)
                              ).SetName("SqliteTypes with values");

                              yield return new TestCaseData(
                                  null, null, null, new byte[] { }, null, null, null, null, null, null
                              ).SetName("SqliteTypes with empty values");

                              yield return new TestCaseData(
                                  null, null, null, null, null, null, null, null, null, null
                              ).SetName("SqliteTypes with null values");
                          }
                      }
            
                     [Test]
                     [TestCaseSource(nameof(SqliteTypesTestCases))]
                     public async Task TestSqliteTypesAsync(
                          int? cInteger,
                          decimal? cReal,
                          string cText,
                          byte[] cBlob,
                          bool? cTextBoolOverride,
                          bool? cIntegerBoolOverride,
                          DateTime? cTextDatetimeOverride,
                          DateTime? cIntegerDatetimeOverride,
                          Instant? cTextNodaInstantOverride,
                          Instant? cIntegerNodaInstantOverride)
                     {
                         await QuerySql.InsertSqliteTypesAsync(new QuerySql.InsertSqliteTypesArgs
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
                         });
                     
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
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

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
                     """
        },
        [KnownTestType.SqliteCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, 312, -7541.3309, "Johnny B. Good")]
                     [TestCase(500, -768, 8453.5678, "Bad to the Bone")]
                     [TestCase(10, null, null, null)]
                     public async Task TestCopyFromAsync(
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
                         await QuerySql.InsertSqliteTypesBatchAsync(batchArgs);
                         var expected = new QuerySql.GetSqliteTypesCntRow
                         {
                             Cnt = batchSize,
                             CInteger = cInteger,
                             CReal = cReal,
                             CText = cText
                         };
                         var actual = await QuerySql.GetSqliteTypesCntAsync();
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
                     public async Task TestSqliteTransactionAsync()
                     {
                         var connection = new Microsoft.Data.Sqlite.SqliteConnection(Environment.GetEnvironmentVariable(EndToEndCommon.SqliteConnectionStringEnv));
                         await connection.OpenAsync();
                         var transaction = connection.BeginTransaction();

                         var querySqlWithTx = QuerySql.WithTransaction(transaction);
                         await querySqlWithTx.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = {{Consts.BojackId}}, Name = {{Consts.BojackAuthor}}, Bio = {{Consts.BojackTheme}} });

                         var actual = await QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = {{Consts.BojackAuthor}} });
                         ClassicAssert.IsNull(actual);

                         transaction.Commit();

                         var expected = new QuerySql.GetAuthorRow
                         {
                             Id = {{Consts.BojackId}},
                             Name = {{Consts.BojackAuthor}},
                             Bio = {{Consts.BojackTheme}}
                         };
                         actual = await QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = {{Consts.BojackAuthor}} });
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
                     public async Task TestSqliteTransactionRollbackAsync()
                     {
                         var connection = new Microsoft.Data.Sqlite.SqliteConnection(Environment.GetEnvironmentVariable(EndToEndCommon.SqliteConnectionStringEnv));
                         await connection.OpenAsync();
                         var transaction = connection.BeginTransaction();

                         var sqlQueryWithTx = QuerySql.WithTransaction(transaction);
                         await sqlQueryWithTx.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = {{Consts.BojackId}}, Name = {{Consts.BojackAuthor}}, Bio = {{Consts.BojackTheme}} });

                         transaction.Rollback();

                         var actual = await this.QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = {{Consts.BojackAuthor}} });
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
                     public async Task TestSqliteDataTypesOverrideAsync(
                        int? cInteger,
                        decimal cReal,
                        string cText)
                     {
                         await QuerySql.InsertSqliteTypesAsync(new QuerySql.InsertSqliteTypesArgs
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
                         var actual = await QuerySql.GetSqliteFunctionsAsync();
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
                     public async Task TestGetAuthorByIdWithMultipleNamedParamAsync()
                     {
                         {{Consts.CreateBojackAuthor}}
                         var expected = new QuerySql.GetAuthorByIdWithMultipleNamedParamRow
                         {
                             Id = {{Consts.BojackId}},
                             Name = {{Consts.BojackAuthor}},
                             Bio = {{Consts.BojackTheme}}
                         };
                         var actual = await this.QuerySql.GetAuthorByIdWithMultipleNamedParamAsync(new QuerySql.GetAuthorByIdWithMultipleNamedParamArgs
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