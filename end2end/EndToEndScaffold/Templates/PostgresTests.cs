using System.Collections.Generic;

namespace EndToEndScaffold.Templates;

public static class PostgresTests
{
    public static Dictionary<KnownTestType, TestImpl> TestImplementations { get; } = new()
    {
        [KnownTestType.PostgresStringDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase("E", "It takes a nation of millions to hold us back", "Rebel Without a Pause", "Prophets of Rage")]
                     [TestCase(null, null, null, null)]
                     public async Task TestPostgresStringTypes(
                         string cChar, 
                         string cVarchar, 
                         string cCharacterVarying, 
                         string cText)
                     {
                         await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs
                         {
                             CChar = cChar,
                             CVarchar = cVarchar,
                             CCharacterVarying = cCharacterVarying,
                             CText = cText,
                         });
                     
                         var expected = new QuerySql.GetPostgresTypesRow
                         {
                             CChar = cChar,
                             CVarchar = cVarchar,
                             CCharacterVarying = cCharacterVarying,
                             CText = cText,
                         };
                         var actual = await QuerySql.GetPostgresTypes();
                         
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CChar, Is.EqualTo(expected.CChar));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CVarchar, Is.EqualTo(expected.CVarchar));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CCharacterVarying, Is.EqualTo(expected.CCharacterVarying));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CText, Is.EqualTo(expected.CText));
                     }
                     """
        },
        [KnownTestType.PostgresIntegerDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(true, 35, -23423, 4235235263L)]
                     [TestCase(null, null, null, null)]
                     public async Task TestPostgresIntegerTypes(
                         bool cBoolean,
                         short cSmallint, 
                         int cInteger,
                         long cBigint)
                     {
                         await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs
                         {
                             CBoolean = cBoolean,
                             CSmallint = cSmallint,
                             CInteger = cInteger,
                             CBigint = cBigint
                         });
                     
                         var expected = new QuerySql.GetPostgresTypesRow
                         {
                             CBoolean = cBoolean,
                             CSmallint = cSmallint,
                             CInteger = cInteger,
                             CBigint = cBigint
                         };
                         var actual = await QuerySql.GetPostgresTypes();

                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBoolean, Is.EqualTo(expected.CBoolean));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CSmallint, Is.EqualTo(expected.CSmallint));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CInteger, Is.EqualTo(expected.CInteger));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBigint, Is.EqualTo(expected.CBigint));
                     }
                     """
        },
        [KnownTestType.PostgresFloatingPointDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(3.83f, 4.5534, 998.432, -8403284.321435, 42332.53)]
                     [TestCase(null, null, null, null, null)]
                     public async Task TestPostgresFloatingPointTypes(
                         float? cReal, 
                         decimal? cNumeric, 
                         decimal? cDecimal, 
                         double? cDoublePrecision,
                         decimal? cMoney)
                     {
                         await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs
                         {
                             CReal = cReal,
                             CNumeric = cNumeric,
                             CDecimal = cDecimal,
                             CDoublePrecision = cDoublePrecision,
                             CMoney = cMoney
                         });
                     
                         var expected = new QuerySql.GetPostgresTypesRow
                         {
                             CReal = cReal,
                             CNumeric = cNumeric,
                             CDecimal = cDecimal,
                             CDoublePrecision = cDoublePrecision,
                             CMoney = cMoney
                         };
                         var actual = await QuerySql.GetPostgresTypes();

                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CReal, Is.EqualTo(expected.CReal));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CNumeric, Is.EqualTo(expected.CNumeric));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDecimal, Is.EqualTo(expected.CDecimal));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDoublePrecision, Is.EqualTo(expected.CDoublePrecision));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CMoney, Is.EqualTo(expected.CMoney));
                     }
                     """
        },
        [KnownTestType.PostgresDateTimeDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase("2000-1-30", "12:13:14", "1983-11-3 02:01:22", "2022-10-2 15:44:01+09:00")]
                     [TestCase(null, null, null, null)]
                     public async Task TestPostgresDateTimeTypes(
                         DateTime? cDate,
                         TimeSpan? cTime, 
                         DateTime? cTimestamp,
                         DateTime? cTimestampWithTz)
                     {
                         await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs
                         {
                             CDate = cDate,
                             CTime = cTime,
                             CTimestamp = cTimestamp,
                             CTimestampWithTz = cTimestampWithTz
                         });
                     
                         var expected = new QuerySql.GetPostgresTypesRow
                         {
                             CDate = cDate,
                             CTime = cTime,
                             CTimestamp = cTimestamp,
                             CTimestampWithTz = cTimestampWithTz
                         };
                         var actual = await QuerySql.GetPostgresTypes();

                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDate, Is.EqualTo(expected.CDate));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CTime, Is.EqualTo(expected.CTime));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CTimestamp, Is.EqualTo(expected.CTimestamp));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CTimestampWithTz, Is.EqualTo(expected.CTimestampWithTz));
                     }
                     """
        },
        [KnownTestType.PostgresArrayDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(new byte[] { 0x45, 0x42 }, new string[] { "Party", "Fight" }, new int[] { 543, -4234 })]
                     [TestCase(new byte[] { }, new string[] { }, new int[] { })]
                     [TestCase(null, null, null)]
                     public async Task TestPostgresArrayTypes(
                         byte[] cBytea,
                         string[] cTextArray,
                         int[] cIntegerArray)
                     {
                         await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs
                         {
                             CBytea = cBytea,
                             CTextArray = cTextArray,
                             CIntegerArray = cIntegerArray
                         });
                     
                         var expected = new QuerySql.GetPostgresTypesRow
                         {
                             CBytea = cBytea,
                             CTextArray = cTextArray,
                             CIntegerArray = cIntegerArray
                         };
                         var actual = await QuerySql.GetPostgresTypes();
                         
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBytea, Is.EqualTo(expected.CBytea));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CTextArray, Is.EqualTo(expected.CTextArray));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CIntegerArray, Is.EqualTo(expected.CIntegerArray));
                     }
                     """
        },
        [KnownTestType.PostgresCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, true, 3, 453, -1445214231L, 666.6f, 336.3431, -99.999, -1377.996, -43242.43, "1973-12-3", "00:34:00", "1960-11-3 02:01:22", "2030-07-20 15:44:01+09:00", "z", "Sex Pistols", "Anarchy in the U.K", "Never Mind the Bollocks...", new byte[] { 0x53, 0x56 })]
                     [TestCase(500, false, -4, 867, 8768769709L, -64.8f, -324.8671, 127.4793, 423.9869, 32143.99, "2024-12-31", "03:06:44", "1999-3-1 03:00:10", "1999-9-13 08:30:11-04:00", "1", "Fugazi", "Waiting Room", "13 Songs", new byte[] { 0x03 })]
                     [TestCase(10, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, new byte[] { })]
                     [TestCase(10, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)]
                     public async Task TestCopyFrom(
                        int batchSize, 
                        bool? cBoolean, 
                        short? cSmallint, 
                        int? cInteger, 
                        long? cBigint, 
                        float? cReal, 
                        decimal? cDecimal, 
                        decimal? cNumeric, 
                        double? cDoublePrecision,
                        decimal? cMoney,
                        DateTime? cDate, 
                        TimeSpan? cTime, 
                        DateTime? cTimestamp, 
                        DateTime? cTimestampWithTz,
                        string cChar, 
                        string cVarchar, 
                        string cCharacterVarying, 
                        string cText,
                        byte[] cBytea)
                     {
                         DateTime? cTimestampWithTzAsUtc = null;
                         if (cTimestampWithTz != null)
                            cTimestampWithTzAsUtc = DateTime.SpecifyKind(cTimestampWithTz.Value, DateTimeKind.Utc);
                            
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertPostgresTypesBatchArgs
                             {
                                 CBoolean = cBoolean,
                                 CSmallint = cSmallint,
                                 CInteger = cInteger,
                                 CBigint = cBigint,
                                 CReal = cReal,
                                 CDecimal = cDecimal,
                                 CNumeric = cNumeric,
                                 CDoublePrecision = cDoublePrecision,
                                 CMoney = cMoney,
                                 CDate = cDate,
                                 CTime = cTime,
                                 CTimestamp = cTimestamp,
                                 CTimestampWithTz = cTimestampWithTzAsUtc,
                                 CChar = cChar,
                                 CVarchar = cVarchar,
                                 CCharacterVarying = cCharacterVarying,
                                 CText = cText,
                                 CBytea = cBytea
                             })
                             .ToList();
                         await QuerySql.InsertPostgresTypesBatch(batchArgs);
                         var expected = new QuerySql.GetPostgresTypesAggRow
                         {
                             Cnt = batchSize,
                             CBoolean = cBoolean,
                             CSmallint = cSmallint,
                             CInteger = cInteger,
                             CBigint = cBigint,
                             CReal = cReal,
                             CDecimal = cDecimal,
                             CNumeric = cNumeric,
                             CDoublePrecision = cDoublePrecision,
                             CMoney = cMoney,
                             CDate = cDate,
                             CTime = cTime,
                             CTimestamp = cTimestamp,
                             CTimestampWithTz = cTimestampWithTz,
                             CChar = cChar,
                             CVarchar = cVarchar,
                             CCharacterVarying = cCharacterVarying,
                             CText = cText,
                             CBytea = cBytea
                         };
                         var actual = await QuerySql.GetPostgresTypesAgg();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});
                     }

                     private static void AssertSingularEquals(QuerySql.GetPostgresTypesAggRow expected, QuerySql.GetPostgresTypesAggRow actual)
                     {
                         Assert.That(actual.Cnt, Is.EqualTo(expected.Cnt));
                         Assert.That(actual.CBoolean, Is.EqualTo(expected.CBoolean));
                         Assert.That(actual.CSmallint, Is.EqualTo(expected.CSmallint));
                         Assert.That(actual.CInteger, Is.EqualTo(expected.CInteger));
                         Assert.That(actual.CBigint, Is.EqualTo(expected.CBigint));
                         Assert.That(actual.CReal, Is.EqualTo(expected.CReal));
                         Assert.That(actual.CDecimal, Is.EqualTo(expected.CDecimal));
                         Assert.That(actual.CNumeric, Is.EqualTo(expected.CNumeric));
                         Assert.That(actual.CDoublePrecision, Is.EqualTo(expected.CDoublePrecision));
                         Assert.That(actual.CMoney, Is.EqualTo(expected.CMoney));
                         Assert.That(actual.CDate, Is.EqualTo(expected.CDate));
                         Assert.That(actual.CTime, Is.EqualTo(expected.CTime));
                         Assert.That(actual.CTimestamp, Is.EqualTo(expected.CTimestamp));
                         Assert.That(actual.CTimestampWithTz, Is.EqualTo(expected.CTimestampWithTz));
                         Assert.That(actual.CChar, Is.EqualTo(expected.CChar));
                         Assert.That(actual.CVarchar, Is.EqualTo(expected.CVarchar));
                         Assert.That(actual.CCharacterVarying, Is.EqualTo(expected.CCharacterVarying));
                         Assert.That(actual.CText, Is.EqualTo(expected.CText));
                         Assert.That(actual.CBytea, Is.EqualTo(expected.CBytea));
                     }
                     """
        },
        [KnownTestType.ArrayAsParam] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestArray()
                     {
                         {{Consts.CreateFirstGenericAuthor}}
                         {{Consts.CreateBojackAuthorWithId}}
                         var actual = await QuerySql.GetAuthorsByIds(new QuerySql.GetAuthorsByIdsArgs 
                         { 
                             LongArr1 = new[] { id1, bojackId } 
                         });
                         ClassicAssert.AreEqual(2, actual.Count);
                     }
                     """
        },
        [KnownTestType.MultipleArraysAsParams] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestMultipleArrays()
                     {
                         {{Consts.CreateFirstGenericAuthor}}
                         {{Consts.CreateSecondGenericAuthor}}
                         {{Consts.CreateBojackAuthorWithId}}
                     
                         var actual = await QuerySql.GetAuthorsByIdsAndNames(new QuerySql.GetAuthorsByIdsAndNamesArgs 
                         { 
                             LongArr1 = new[] { id1, bojackId }, 
                             StringArr2 = new[] { {{Consts.GenericAuthor}} } 
                         });
                         ClassicAssert.AreEqual(1, actual.Count);
                     }
                     """
        },
    };
}