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
                     }

                     private static void AssertSingularEquals(QuerySql.GetSqliteTypesRow expected, QuerySql.GetSqliteTypesRow actual)
                     {
                         Assert.That(actual.CInteger, Is.EqualTo(expected.CInteger));
                         Assert.That(actual.CReal, Is.EqualTo(expected.CReal));
                         Assert.That(actual.CText, Is.EqualTo(expected.CText));
                         Assert.That(actual.CBlob, Is.EqualTo(expected.CBlob));
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
                     }

                     private static void AssertSingularEquals(QuerySql.GetSqliteTypesCntRow expected, QuerySql.GetSqliteTypesCntRow actual)
                     {
                         Assert.That(actual.Cnt, Is.EqualTo(expected.Cnt));
                         Assert.That(actual.CInteger, Is.EqualTo(expected.CInteger));
                         Assert.That(actual.CReal, Is.EqualTo(expected.CReal));
                         Assert.That(actual.CText, Is.EqualTo(expected.CText));
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

                         // The GetAuthor method in SqliteExampleGen returns QuerySql.GetAuthorRow? (nullable record struct/class)
                         var actualNull = await QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = {{Consts.BojackAuthor}} });
                         Assert.That(actualNull == null, "there is author"); // This is correct for nullable types

                         transaction.Commit();

                         var expected = new QuerySql.GetAuthorRow
                         {
                             Id = {{Consts.BojackId}},
                             Name = {{Consts.BojackAuthor}},
                             Bio = {{Consts.BojackTheme}}
                         };
                         var actual = await QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = {{Consts.BojackAuthor}} });
                         Assert.That(SingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}})); // Apply placeholder here
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
                         Assert.That(actual == null, "author should not exist after rollback");
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
                     }

                     private static void AssertSingularEquals(QuerySql.GetSqliteFunctionsRow expected, QuerySql.GetSqliteFunctionsRow actual)
                     {
                         Assert.That(actual.MaxInteger, Is.EqualTo(expected.MaxInteger));
                         Assert.That(actual.MaxReal, Is.EqualTo(expected.MaxReal));
                         Assert.That(actual.MaxText, Is.EqualTo(expected.MaxText));
                     }
                     """
        }
    };
}