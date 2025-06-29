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
        [KnownTestType.PostgresStringCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, "z", "Sex Pistols", "Anarchy in the U.K", "Never Mind the Bollocks...")]
                     [TestCase(10, null, null, null, null)]
                     public async Task TestStringCopyFrom(
                        int batchSize, 
                        string cChar, 
                        string cVarchar, 
                        string cCharacterVarying, 
                        string cText)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertPostgresTypesBatchArgs
                             {
                                 CChar = cChar,
                                 CVarchar = cVarchar,
                                 CCharacterVarying = cCharacterVarying,
                                 CText = cText
                             })
                             .ToList();
                         await QuerySql.InsertPostgresTypesBatch(batchArgs);
                         var expected = new QuerySql.GetPostgresTypesCntRow
                         {
                             Cnt = batchSize,
                             CChar = cChar,
                             CVarchar = cVarchar,
                             CCharacterVarying = cCharacterVarying,
                             CText = cText
                         };
                         var actual = await QuerySql.GetPostgresTypesCnt();
                         
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.Cnt, Is.EqualTo(expected.Cnt));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CChar, Is.EqualTo(expected.CChar));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CVarchar, Is.EqualTo(expected.CVarchar));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CCharacterVarying, Is.EqualTo(expected.CCharacterVarying));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CText, Is.EqualTo(expected.CText));
                     }
                     """
        },
        [KnownTestType.PostgresIntegerCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, true, 3, 453, -1445214231L)]
                     [TestCase(10, null, null, null, null)]
                     public async Task TestIntegerCopyFrom(
                        int batchSize, 
                        bool? cBoolean, 
                        short? cSmallint, 
                        int? cInteger, 
                        long? cBigint)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertPostgresTypesBatchArgs
                             {
                                 CBoolean = cBoolean,
                                 CSmallint = cSmallint,
                                 CInteger = cInteger,
                                 CBigint = cBigint
                             })
                             .ToList();
                         await QuerySql.InsertPostgresTypesBatch(batchArgs);
                         var expected = new QuerySql.GetPostgresTypesCntRow
                         {
                             Cnt = batchSize,
                             CBoolean = cBoolean,
                             CSmallint = cSmallint,
                             CInteger = cInteger,
                             CBigint = cBigint
                         };
                         var actual = await QuerySql.GetPostgresTypesCnt();

                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.Cnt, Is.EqualTo(expected.Cnt));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBoolean, Is.EqualTo(expected.CBoolean));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CSmallint, Is.EqualTo(expected.CSmallint));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CInteger, Is.EqualTo(expected.CInteger));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBigint, Is.EqualTo(expected.CBigint));
                     }

                     private static void AssertSingularEquals(QuerySql.GetPostgresTypesCntRow expected, QuerySql.GetPostgresTypesCntRow actual)
                     {

                     }
                     """
        },
        [KnownTestType.PostgresFloatingPointCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, 666.6f, 336.3431, -99.999, -1377.996, -43242.43)]
                     [TestCase(10, null, null, null, null, null)]
                     public async Task TestFloatingPointCopyFrom(
                        int batchSize, 
                        float? cReal, 
                        decimal? cDecimal, 
                        decimal? cNumeric, 
                        double? cDoublePrecision,
                        decimal? cMoney)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertPostgresTypesBatchArgs
                             {
                                 CReal = cReal,
                                 CDecimal = cDecimal,
                                 CNumeric = cNumeric,
                                 CDoublePrecision = cDoublePrecision,
                                 CMoney = cMoney
                             })
                             .ToList();
                         await QuerySql.InsertPostgresTypesBatch(batchArgs);
                         var expected = new QuerySql.GetPostgresTypesCntRow
                         {
                             Cnt = batchSize,
                             CReal = cReal,
                             CDecimal = cDecimal,
                             CNumeric = cNumeric,
                             CDoublePrecision = cDoublePrecision,
                             CMoney = cMoney
                         };
                         var actual = await QuerySql.GetPostgresTypesCnt();
                         
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.Cnt, Is.EqualTo(expected.Cnt));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CReal, Is.EqualTo(expected.CReal));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDecimal, Is.EqualTo(expected.CDecimal));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CNumeric, Is.EqualTo(expected.CNumeric));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDoublePrecision, Is.EqualTo(expected.CDoublePrecision));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CMoney, Is.EqualTo(expected.CMoney));
                     }
                     """
        },
        [KnownTestType.PostgresDateTimeCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, "1973-12-3", "00:34:00", "1960-11-3 02:01:22", "2030-07-20 15:44:01+09:00")]
                     [TestCase(10, null, null, null, null)]
                     public async Task TestDateTimeCopyFrom(
                        int batchSize, 
                        DateTime? cDate, 
                        TimeSpan? cTime, 
                        DateTime? cTimestamp, 
                        DateTime? cTimestampWithTz)
                     {
                         DateTime? cTimestampWithTzAsUtc = null;
                         if (cTimestampWithTz != null)
                            cTimestampWithTzAsUtc = DateTime.SpecifyKind(cTimestampWithTz.Value, DateTimeKind.Utc);
                            
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertPostgresTypesBatchArgs
                             {
                                 CDate = cDate,
                                 CTime = cTime,
                                 CTimestamp = cTimestamp,
                                 CTimestampWithTz = cTimestampWithTzAsUtc
                             })
                             .ToList();
                         await QuerySql.InsertPostgresTypesBatch(batchArgs);
                         var expected = new QuerySql.GetPostgresTypesCntRow
                         {
                             Cnt = batchSize,
                             CDate = cDate,
                             CTime = cTime,
                             CTimestamp = cTimestamp,
                             CTimestampWithTz = cTimestampWithTz,
                         };
                         var actual = await QuerySql.GetPostgresTypesCnt();
                         
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.Cnt, Is.EqualTo(expected.Cnt));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDate, Is.EqualTo(expected.CDate));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CTime, Is.EqualTo(expected.CTime));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CTimestamp, Is.EqualTo(expected.CTimestamp));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CTimestampWithTz, Is.EqualTo(expected.CTimestampWithTz));
                     }
                     """
        },
        [KnownTestType.PostgresArrayCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, new byte[] { 0x53, 0x56 })]
                     [TestCase(10,  new byte[] { })]
                     [TestCase(10, null)]
                     public async Task TestArrayCopyFrom(
                        int batchSize, 
                        byte[] cBytea)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertPostgresTypesBatchArgs
                             {
                                 CBytea = cBytea
                             })
                             .ToList();
                         await QuerySql.InsertPostgresTypesBatch(batchArgs);
                         var expected = new QuerySql.GetPostgresTypesCntRow
                         {
                             Cnt = batchSize,
                             CBytea = cBytea
                         };
                         var actual = await QuerySql.GetPostgresTypesCnt();

                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.Cnt, Is.EqualTo(expected.Cnt));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBytea, Is.EqualTo(expected.CBytea));
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
        [KnownTestType.PostgresGeoDataTypes] = new TestImpl
        {
            Impl = $$"""
                     public static IEnumerable<TestCaseData> PostgresGeoTypesTestCases
                     {
                         get
                         {
                             yield return new TestCaseData(
                                 new NpgsqlPoint(1, 2),
                                 new NpgsqlLine(3, 4, 5),
                                 new NpgsqlLSeg(1, 2, 3, 4),
                                 new NpgsqlBox(1, 2, 3, 4),
                                 new NpgsqlPath(new NpgsqlPoint[] { new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4) }),
                                 new NpgsqlPolygon(new NpgsqlPoint[] { new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4) }),
                                 new NpgsqlCircle(1, 2, 3)
                             ).SetName("Valid Geo Types");
 
                             yield return new TestCaseData(
                                 null,
                                 null,
                                 null,
                                 null,
                                 null,
                                 null,
                                 null
                             ).SetName("Null Geo Types");
                         }
                     }

                     [Test]
                     [TestCaseSource(nameof(PostgresGeoTypesTestCases))]
                     public async Task TestPostgresGeoTypes(NpgsqlPoint? cPoint, NpgsqlLine? cLine, NpgsqlLSeg? cLSeg, NpgsqlBox? cBox, NpgsqlPath? cPath, NpgsqlPolygon? cPolygon, NpgsqlCircle? cCircle)
                     {
                         await QuerySql.InsertPostgresGeoTypes(new QuerySql.InsertPostgresGeoTypesArgs { CPoint = cPoint, CLine = cLine, CLseg = cLSeg, CBox = cBox, CPath = cPath, CPolygon = cPolygon, CCircle = cCircle });
                         var expected = new QuerySql.GetPostgresGeoTypesRow
                         {
                             CPoint = cPoint,
                             CLine = cLine,
                             CLseg = cLSeg,
                             CBox = cBox,
                             CPath = cPath,
                             CPolygon = cPolygon,
                             CCircle = cCircle
                         };
                         var actual = await QuerySql.GetPostgresGeoTypes();
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CPoint, Is.EqualTo(expected.CPoint));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CLine, Is.EqualTo(expected.CLine));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CLseg, Is.EqualTo(expected.CLseg));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBox, Is.EqualTo(expected.CBox));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CPath, Is.EqualTo(expected.CPath));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CPolygon, Is.EqualTo(expected.CPolygon));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CCircle, Is.EqualTo(expected.CCircle));
                     }
                     """
        },
        [KnownTestType.PostgresDataTypesOverride] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(-54355, "White Light from the Mouth of Infinity", "2022-10-2 15:44:01+09:00")]
                     [TestCase(null, null, null)]
                     public async Task TestPostgresDataTypesOverride(
                        int? cInteger,
                        string cVarchar,
                        DateTime? cTimestamp)
                     {
                         await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs
                         {
                             CInteger = cInteger,
                             CVarchar = cVarchar,
                             CTimestamp = cTimestamp
                         });
                     
                         var expected = new QuerySql.GetPostgresFunctionsRow
                         {
                             MaxInteger = cInteger,
                             MaxVarchar = cVarchar,
                             MaxTimestamp = cTimestamp
                         };

                         var actual = await QuerySql.GetPostgresFunctions();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});
                     }

                     private static void AssertSingularEquals(QuerySql.GetPostgresFunctionsRow expected, QuerySql.GetPostgresFunctionsRow actual)
                     {
                         Assert.That(actual.MaxInteger, Is.EqualTo(expected.MaxInteger));
                         Assert.That(actual.MaxVarchar, Is.EqualTo(expected.MaxVarchar));
                         Assert.That(actual.MaxTimestamp, Is.EqualTo(expected.MaxTimestamp));
                     }
                     """
        }
    };
}