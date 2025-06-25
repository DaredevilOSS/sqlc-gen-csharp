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
                          int cInteger,
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
                         var expected = new QuerySql.GetSqliteTypesAggRow
                         {
                             Cnt = batchSize,
                             CInteger = cInteger,
                             CReal = cReal,
                             CText = cText
                         };
                         var actual = await QuerySql.GetSqliteTypesAgg();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});
                     }

                     private static void AssertSingularEquals(QuerySql.GetSqliteTypesAggRow expected, QuerySql.GetSqliteTypesAggRow actual)
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

                         var sqlQueryWithTx = new QuerySql(transaction);
                         await sqlQueryWithTx.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });

                         // The GetAuthor method in SqliteExampleGen returns QuerySql.GetAuthorRow? (nullable record struct/class)
                         var actualNull = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
                         Assert.That(actualNull == null, "there is author"); // This is correct for nullable types

                         await transaction.CommitAsync();

                         var expected = new QuerySql.GetAuthorRow
                         {
                             Id = 1111,
                             Name = "Bojack Horseman",
                             Bio = "Back in the 90s he was in a very famous TV show"
                         };
                         var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
                         Assert.That(SingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}})); // Apply placeholder here
                     }
                     """
        },
    };
}