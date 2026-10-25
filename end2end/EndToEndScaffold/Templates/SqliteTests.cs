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
        [KnownTestType.SqliteNumericDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(1, -2, 3, 4, 5, 9000000000, 9223372036854775807, -9000000000, 1.5, 2.25, 3.125, 12345.67, -1.25001)]
                     [TestCase(null, null, null, null, null, null, null, null, null, null, null, null, null)]
                     public async Task TestSqliteNumericTypesAsync(
                         int? cInt,
                         int? cTinyint,
                         int? cSmallint,
                         int? cMediumint,
                         int? cInt2,
                         long? cBigint,
                         long? cUnsignedBigInt,
                         long? cInt8,
                         double? cDouble,
                         double? cDoublePrecision,
                         double? cFloat,
                         decimal? cNumeric,
                         decimal? cDecimal)
                     {
                         await QuerySql.InsertSqliteNumericTypesAsync(new QuerySql.InsertSqliteNumericTypesArgs
                         {
                             CInt = cInt,
                             CTinyint = cTinyint,
                             CSmallint = cSmallint,
                             CMediumint = cMediumint,
                             CInt2 = cInt2,
                             CBigint = cBigint,
                             CUnsignedBigInt = cUnsignedBigInt,
                             CInt8 = cInt8,
                             CDouble = cDouble,
                             CDoublePrecision = cDoublePrecision,
                             CFloat = cFloat,
                             CNumeric = cNumeric,
                             CDecimal = cDecimal
                         });

                         var expected = new QuerySql.GetSqliteNumericTypesRow
                         {
                             CInt = cInt,
                             CTinyint = cTinyint,
                             CSmallint = cSmallint,
                             CMediumint = cMediumint,
                             CInt2 = cInt2,
                             CBigint = cBigint,
                             CUnsignedBigInt = cUnsignedBigInt,
                             CInt8 = cInt8,
                             CDouble = cDouble,
                             CDoublePrecision = cDoublePrecision,
                             CFloat = cFloat,
                             CNumeric = cNumeric,
                             CDecimal = cDecimal
                         };
                         var actual = await QuerySql.GetSqliteNumericTypesAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetSqliteNumericTypesRow x, QuerySql.GetSqliteNumericTypesRow y)
                         {
                             Assert.That(x.CInt, Is.EqualTo(y.CInt));
                             Assert.That(x.CTinyint, Is.EqualTo(y.CTinyint));
                             Assert.That(x.CSmallint, Is.EqualTo(y.CSmallint));
                             Assert.That(x.CMediumint, Is.EqualTo(y.CMediumint));
                             Assert.That(x.CInt2, Is.EqualTo(y.CInt2));
                             Assert.That(x.CBigint, Is.EqualTo(y.CBigint));
                             Assert.That(x.CUnsignedBigInt, Is.EqualTo(y.CUnsignedBigInt));
                             Assert.That(x.CInt8, Is.EqualTo(y.CInt8));
                             Assert.That(x.CDouble, Is.EqualTo(y.CDouble));
                             Assert.That(x.CDoublePrecision, Is.EqualTo(y.CDoublePrecision));
                             Assert.That(x.CFloat, Is.EqualTo(y.CFloat));
                             Assert.That(x.CNumeric, Is.EqualTo(y.CNumeric));
                             Assert.That(x.CDecimal, Is.EqualTo(y.CDecimal));
                         }
                     }
                     """
        },
        [KnownTestType.SqliteStringDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase("Rock", "Roll", "Hail", "Hail!", "To the", "Chief", "Yes, Chief")]
                     [TestCase(null, null, null, null, null, null, null)]
                     public async Task TestSqliteStringTypesAsync(
                         string cCharacter,
                         string cVarchar,
                         string cVaryingCharacter,
                         string cNchar,
                         string cNativeCharacter,
                         string cNvarchar,
                         string cClob)
                     {
                         await QuerySql.InsertSqliteStringTypesAsync(new QuerySql.InsertSqliteStringTypesArgs
                         {
                             CCharacter = cCharacter,
                             CVarchar = cVarchar,
                             CVaryingCharacter = cVaryingCharacter,
                             CNchar = cNchar,
                             CNativeCharacter = cNativeCharacter,
                             CNvarchar = cNvarchar,
                             CClob = cClob
                         });

                         var expected = new QuerySql.GetSqliteStringTypesRow
                         {
                             CCharacter = cCharacter,
                             CVarchar = cVarchar,
                             CVaryingCharacter = cVaryingCharacter,
                             CNchar = cNchar,
                             CNativeCharacter = cNativeCharacter,
                             CNvarchar = cNvarchar,
                             CClob = cClob
                         };
                         var actual = await QuerySql.GetSqliteStringTypesAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetSqliteStringTypesRow x, QuerySql.GetSqliteStringTypesRow y)
                         {
                             Assert.That(x.CCharacter, Is.EqualTo(y.CCharacter));
                             Assert.That(x.CVarchar, Is.EqualTo(y.CVarchar));
                             Assert.That(x.CVaryingCharacter, Is.EqualTo(y.CVaryingCharacter));
                             Assert.That(x.CNchar, Is.EqualTo(y.CNchar));
                             Assert.That(x.CNativeCharacter, Is.EqualTo(y.CNativeCharacter));
                             Assert.That(x.CNvarchar, Is.EqualTo(y.CNvarchar));
                             Assert.That(x.CClob, Is.EqualTo(y.CClob));
                         }
                     }
                     """
        },
        [KnownTestType.SqliteMiscDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(true, false, "2020-01-01", "2025-01-01 17:18:19", "1993-09-27 03:55:02", "{\"a\": 1}", "[1, 2, 3]")]
                     [TestCase(null, null, null, null, null, null, null)]
                     public async Task TestSqliteMiscTypesAsync(
                         bool? cBoolean,
                         bool? cBool,
                         string cDateStr,
                         string cDatetimeStr,
                         string cTimestampStr,
                         string cJson,
                         string cJsonb)
                     {
                         DateTime? cDate = cDateStr == null ? (DateTime?)null : DateTime.Parse(cDateStr);
                         DateTime? cDatetime = cDatetimeStr == null ? (DateTime?)null : DateTime.Parse(cDatetimeStr);
                         DateTime? cTimestamp = cTimestampStr == null ? (DateTime?)null : DateTime.Parse(cTimestampStr);
                         JsonElement? cParsedJson = null;
                         if (cJson != null)
                             cParsedJson = JsonDocument.Parse(cJson).RootElement;
                         JsonElement? cParsedJsonb = null;
                         if (cJsonb != null)
                             cParsedJsonb = JsonDocument.Parse(cJsonb).RootElement;
                         await QuerySql.InsertSqliteMiscTypesAsync(new QuerySql.InsertSqliteMiscTypesArgs
                         {
                             CBoolean = cBoolean,
                             CBool = cBool,
                             CDate = cDate,
                             CDatetime = cDatetime,
                             CTimestamp = cTimestamp,
                             CJson = cParsedJson,
                             CJsonb = cParsedJsonb
                         });

                         var expected = new QuerySql.GetSqliteMiscTypesRow
                         {
                             CBoolean = cBoolean,
                             CBool = cBool,
                             CDate = cDate,
                             CDatetime = cDatetime,
                             CTimestamp = cTimestamp,
                             CJson = cParsedJson,
                             CJsonb = cParsedJsonb
                         };
                         var actual = await QuerySql.GetSqliteMiscTypesAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetSqliteMiscTypesRow x, QuerySql.GetSqliteMiscTypesRow y)
                         {
                             Assert.That(x.CBoolean, Is.EqualTo(y.CBoolean));
                             Assert.That(x.CBool, Is.EqualTo(y.CBool));
                             Assert.That(x.CDate, Is.EqualTo(y.CDate));
                             Assert.That(x.CDatetime, Is.EqualTo(y.CDatetime));
                             Assert.That(x.CTimestamp, Is.EqualTo(y.CTimestamp));
                             Assert.That(x.CJson.HasValue, Is.EqualTo(y.CJson.HasValue));
                             if (x.CJson.HasValue)
                                 Assert.That(x.CJson.Value.GetRawText(), Is.EqualTo(y.CJson.Value.GetRawText()));
                             Assert.That(x.CJsonb.HasValue, Is.EqualTo(y.CJsonb.HasValue));
                             if (x.CJsonb.HasValue)
                                 Assert.That(x.CJsonb.Value.GetRawText(), Is.EqualTo(y.CJsonb.Value.GetRawText()));
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