using System.Collections.Generic;

namespace EndToEndScaffold.Templates;

public static class MySqlTests
{
    public static Dictionary<KnownTestType, TestImpl> TestImplementations { get; } = new()
    {
        [KnownTestType.MySqlStringDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase("&", "\u1857", "\u2649", "Sheena is a Punk Rocker", "Holiday in Cambodia", "London's Calling", "London's Burning", "Police & Thieves")]
                     [TestCase(null, null, null, null, null, null, null, null)]
                     public async Task TestMySqlStringTypes(
                         string cChar,
                         string cNchar,
                         string cNationalChar,
                         string cVarchar,
                         string cTinytext,
                         string cMediumtext,
                         string cText,
                         string cLongtext)
                     {
                         await QuerySql.InsertMysqlStringTypesAsync(new QuerySql.InsertMysqlStringTypesArgs
                         {
                              CChar = cChar,
                              CNchar = cNchar,
                              CNationalChar = cNationalChar,
                              CVarchar = cVarchar,
                              CTinytext = cTinytext,
                              CMediumtext = cMediumtext,
                              CText = cText,
                              CLongtext = cLongtext
                         });
                         
                         var expected = new QuerySql.GetMysqlStringTypesRow
                         {
                              CChar = cChar,
                              CNchar = cNchar,
                              CNationalChar = cNationalChar,
                              CVarchar = cVarchar,
                              CTinytext = cTinytext,
                              CMediumtext = cMediumtext,
                              CText = cText,
                              CLongtext = cLongtext
                         };
                         var actual = await QuerySql.GetMysqlStringTypesAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetMysqlStringTypesRow x, QuerySql.GetMysqlStringTypesRow y)
                         {
                             Assert.That(x.CChar, Is.EqualTo(y.CChar));
                             Assert.That(x.CNchar, Is.EqualTo(y.CNchar));
                             Assert.That(x.CNationalChar, Is.EqualTo(y.CNationalChar));
                             Assert.That(x.CVarchar, Is.EqualTo(y.CVarchar));
                             Assert.That(x.CTinytext, Is.EqualTo(y.CTinytext));
                             Assert.That(x.CMediumtext, Is.EqualTo(y.CMediumtext));
                             Assert.That(x.CText, Is.EqualTo(y.CText));
                             Assert.That(x.CLongtext, Is.EqualTo(y.CLongtext));
                         }
                     }
                     """
        },
        [KnownTestType.MySqlIntegerDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(false, true, 13, 2084, 3124, -54355, 324245, -67865, 9787668656L)]
                     [TestCase(null, null, null, null, null, null, null, null, null)]
                     public async Task TestMySqlIntegerTypes(
                         bool? cBool, 
                         bool? cBoolean, 
                         short? cTinyint, 
                         short? cYear,
                         short? cSmallint,
                         int? cMediumint,
                         int? cInt, 
                         int? cInteger,
                         long? cBigint)
                     {
                         await QuerySql.InsertMysqlNumericTypesAsync(new QuerySql.InsertMysqlNumericTypesArgs
                         {
                              CBool = cBool,
                              CBoolean = cBoolean,
                              CTinyint = cTinyint,
                              CSmallint = cSmallint,
                              CMediumint = cMediumint,
                              CInt = cInt,
                              CInteger = cInteger,
                              CBigint = cBigint
                         });
                     
                         var expected = new QuerySql.GetMysqlNumericTypesRow
                         {
                             CBool = cBool,
                             CBoolean = cBoolean,
                             CTinyint = cTinyint,
                             CSmallint = cSmallint,
                             CMediumint = cMediumint,
                             CInt = cInt,
                             CInteger = cInteger,
                             CBigint = cBigint
                         };
                         var actual = await QuerySql.GetMysqlNumericTypesAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetMysqlNumericTypesRow x, QuerySql.GetMysqlNumericTypesRow y)
                         {
                             Assert.That(x.CBool, Is.EqualTo(y.CBool));
                             Assert.That(x.CBoolean, Is.EqualTo(y.CBoolean));
                             Assert.That(x.CTinyint, Is.EqualTo(y.CTinyint));
                             Assert.That(x.CSmallint, Is.EqualTo(y.CSmallint));
                             Assert.That(x.CMediumint, Is.EqualTo(y.CMediumint));
                             Assert.That(x.CInt, Is.EqualTo(y.CInt));
                             Assert.That(x.CInteger, Is.EqualTo(y.CInteger));
                             Assert.That(x.CBigint, Is.EqualTo(y.CBigint));
                         }
                     }
                     """
        },
        [KnownTestType.MySqlFloatingPointDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(3.4f, -31.555666, 11.098643, 34.4424, 423.2445, 998.9994542, 21.214312452534)]
                     [TestCase(null, null, null, null, null, null, null)]
                     public async Task TestMySqlFloatingPointTypes(
                         float? cFloat,
                         decimal? cNumeric,
                         decimal? cDecimal,
                         decimal? cDec,
                         decimal? cFixed,
                         double? cDouble,
                         double? cDoublePrecision)
                     {
                         await QuerySql.InsertMysqlNumericTypesAsync(new QuerySql.InsertMysqlNumericTypesArgs
                         {
                              CFloat = cFloat,
                              CNumeric = cNumeric,
                              CDecimal = cDecimal,
                              CDec = cDec,
                              CFixed = cFixed,
                              CDouble = cDouble,
                              CDoublePrecision = cDoublePrecision
                         });
                     
                         var expected = new QuerySql.GetMysqlNumericTypesRow
                         {
                              CFloat = cFloat,
                              CNumeric = cNumeric,
                              CDecimal = cDecimal,
                              CDec = cDec,
                              CFixed = cFixed,
                              CDouble = cDouble,
                              CDoublePrecision = cDoublePrecision
                         };
                         var actual = await QuerySql.GetMysqlNumericTypesAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetMysqlNumericTypesRow x, QuerySql.GetMysqlNumericTypesRow y)
                         {
                             Assert.That(x.CFloat, Is.EqualTo(y.CFloat));
                             Assert.That(x.CNumeric, Is.EqualTo(y.CNumeric));
                             Assert.That(x.CDecimal, Is.EqualTo(y.CDecimal));
                             Assert.That(x.CDec, Is.EqualTo(y.CDec));
                             Assert.That(x.CFixed, Is.EqualTo(y.CFixed));
                             Assert.That(x.CDouble, Is.EqualTo(y.CDouble));
                             Assert.That(x.CDoublePrecision, Is.EqualTo(y.CDoublePrecision));
                         }
                     }
                     """
        },
        [KnownTestType.MySqlDateTimeDataTypes] = new TestImpl
        {
            Impl = $$"""

                     private static IEnumerable<TestCaseData> MySqlDateTimeTypesTestCases
                     {
                         get
                         {
                             yield return new TestCaseData(
                                (short) 1999, 
                                DateTime.Parse("2000-1-30"), 
                                DateTime.Parse("1983-11-3 02:01:22"), 
                                DateTime.Parse("2010-1-30 08:11:00"), 
                                TimeSpan.Parse("02:01:22"), 
                                Instant.FromUtc(2025, 10, 15, 19, 55, 2)
                             ).SetName("DateTimeTypes with values");
                             yield return new TestCaseData(
                                null, 
                                null, 
                                null, 
                                null, 
                                null, 
                                null
                            ).SetName("DateTimeTypes with null values");
                         }
                     }

                     [Test]
                     [TestCaseSource(nameof(MySqlDateTimeTypesTestCases))]
                     public async Task TestMySqlDateTimeTypes(
                         short? cYear,
                         DateTime? cDate, 
                         DateTime? cDatetime,
                         DateTime? cTimestamp,
                         TimeSpan? cTime,
                         Instant? cTimestampNodaInstantOverride)
                     {
                         await QuerySql.InsertMysqlDatetimeTypesAsync(new QuerySql.InsertMysqlDatetimeTypesArgs
                         {
                              CYear = cYear,
                              CDate = cDate,
                              CDatetime = cDatetime,
                              CTimestamp = cTimestamp,
                              CTime = cTime,
                              CTimestampNodaInstantOverride = cTimestampNodaInstantOverride
                         });
                     
                         var expected = new QuerySql.GetMysqlDatetimeTypesRow
                         {
                              CYear = cYear,
                              CDate = cDate,
                              CDatetime = cDatetime,
                              CTimestamp = cTimestamp,
                              CTime = cTime,
                              CTimestampNodaInstantOverride = cTimestampNodaInstantOverride
                         };
                         var actual = await QuerySql.GetMysqlDatetimeTypesAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});
                     
                         void AssertSingularEquals(QuerySql.GetMysqlDatetimeTypesRow x, QuerySql.GetMysqlDatetimeTypesRow y)
                         {
                             Assert.That(x.CYear, Is.EqualTo(y.CYear));
                             Assert.That(x.CDate, Is.EqualTo(y.CDate));
                             Assert.That(x.CDatetime, Is.EqualTo(y.CDatetime));
                             Assert.That(x.CTimestamp, Is.EqualTo(y.CTimestamp));
                             Assert.That(x.CTime, Is.EqualTo(y.CTime));
                             Assert.That(x.CTimestampNodaInstantOverride, Is.EqualTo(y.CTimestampNodaInstantOverride));
                         }
                     }
                     """
        },
        [KnownTestType.MySqlBinaryDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(0x32, new byte[] { 0x15, 0x16, 0x17 }, new byte[] { 0x15, 0x24 }, new byte[] { 0x23 }, new byte[] { 0x33, 0x13 }, new byte[] { 0x11, 0x62, 0x10 }, new byte[] { 0x38, 0x45, 0x06 })]
                     [TestCase(null, new byte[] { 0x0, 0x0, 0x0 }, new byte[] { }, new byte[] { }, new byte[] { }, new byte[] { }, new byte[] { })]
                     [TestCase(null, null, null, null, null, null, null)]
                     public async Task TestMySqlBinaryTypes(
                         byte? cBit,
                         byte[] cBinary,
                         byte[] cVarbinary, 
                         byte[] cTinyblob, 
                         byte[] cBlob, 
                         byte[] cMediumblob, 
                         byte[] cLongblob)
                     {
                         await QuerySql.InsertMysqlBinaryTypesAsync(new QuerySql.InsertMysqlBinaryTypesArgs
                         {
                              CBit = cBit,
                              CBinary = cBinary,
                              CVarbinary = cVarbinary, 
                              CTinyblob = cTinyblob,
                              CBlob = cBlob,
                              CMediumblob = cMediumblob,
                              CLongblob = cLongblob
                         });
                     
                         var expected = new QuerySql.GetMysqlBinaryTypesRow
                         {
                              CBit = cBit,
                              CBinary = cBinary,
                              CVarbinary = cVarbinary, 
                              CTinyblob = cTinyblob,
                              CBlob = cBlob,
                              CMediumblob = cMediumblob,
                              CLongblob = cLongblob
                         };

                         var actual = await QuerySql.GetMysqlBinaryTypesAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetMysqlBinaryTypesRow x, QuerySql.GetMysqlBinaryTypesRow y)
                         {
                             Assert.That(x.CBit, Is.EqualTo(y.CBit));
                             Assert.That(x.CBinary, Is.EqualTo(y.CBinary));
                             Assert.That(x.CVarbinary, Is.EqualTo(y.CVarbinary));
                             Assert.That(x.CTinyblob, Is.EqualTo(y.CTinyblob));
                             Assert.That(x.CBlob, Is.EqualTo(y.CBlob));
                             Assert.That(x.CMediumblob, Is.EqualTo(y.CMediumblob));
                             Assert.That(x.CLongblob, Is.EqualTo(y.CLongblob));
                         }
                     }
                     """
        },
        [KnownTestType.MySqlStringCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, "D", "\u4321", "\u2345", "Parasite", "Clockwork Orange", "Dr. Strangelove", "Interview with a Vampire", "Memento")]
                     [TestCase(10, null, null, null, null, null, null, null, null)]
                     public async Task TestStringCopyFrom(
                        int batchSize, 
                        string cChar,
                        string cNchar,
                        string cNationalChar,
                        string cVarchar, 
                        string cTinytext, 
                        string cMediumtext, 
                        string cText, 
                        string cLongtext)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertMysqlStringTypesBatchArgs
                             {
                                 CChar = cChar,
                                 CNchar = cNchar,
                                 CNationalChar = cNationalChar,
                                 CVarchar = cVarchar,
                                 CTinytext = cTinytext,
                                 CMediumtext = cMediumtext,
                                 CText = cText,
                                 CLongtext = cLongtext
                             })
                             .ToList();
                         await QuerySql.InsertMysqlStringTypesBatchAsync(batchArgs);
                         var expected = new QuerySql.GetMysqlStringTypesCntRow
                         {
                             Cnt = batchSize,
                             CChar = cChar,
                             CNchar = cNchar,
                             CNationalChar = cNationalChar,
                             CVarchar = cVarchar,
                             CTinytext = cTinytext,
                             CMediumtext = cMediumtext,
                             CText = cText,
                             CLongtext = cLongtext
                         };

                         var actual = await QuerySql.GetMysqlStringTypesCntAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetMysqlStringTypesCntRow x, QuerySql.GetMysqlStringTypesCntRow y)
                         {          
                             Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                             Assert.That(x.CChar, Is.EqualTo(y.CChar));
                             Assert.That(x.CNchar, Is.EqualTo(y.CNchar));
                             Assert.That(x.CNationalChar, Is.EqualTo(y.CNationalChar));
                             Assert.That(x.CVarchar, Is.EqualTo(y.CVarchar));
                             Assert.That(x.CTinytext, Is.EqualTo(y.CTinytext));
                             Assert.That(x.CMediumtext, Is.EqualTo(y.CMediumtext));
                             Assert.That(x.CText, Is.EqualTo(y.CText));
                             Assert.That(x.CLongtext, Is.EqualTo(y.CLongtext));
                         }
                     }
                     """
        },
        [KnownTestType.MySqlIntegerCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, true, false, -13, 324, -98760, 987965, 3132423, -7785442L)]
                     [TestCase(10, null, null, null, null, null, null, null, null)]
                     public async Task TestIntegerCopyFrom(
                        int batchSize, 
                        bool? cBool,
                        bool? cBoolean,
                        short? cTinyint,
                        short? cSmallint,
                        int? cMediumint,
                        int? cInt, 
                        int? cInteger,
                        long? cBigint)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertMysqlNumericTypesBatchArgs
                             {
                                 CBool = cBool,
                                 CBoolean = cBoolean,
                                 CTinyint = cTinyint,
                                 CSmallint = cSmallint,
                                 CMediumint = cMediumint,
                                 CInt = cInt,
                                 CInteger = cInteger,
                                 CBigint = cBigint
                             })
                             .ToList();
                         await QuerySql.InsertMysqlNumericTypesBatchAsync(batchArgs);
                         var expected = new QuerySql.GetMysqlNumericTypesCntRow
                         {
                             Cnt = batchSize,
                             CBool = cBool,
                             CBoolean = cBoolean,
                             CTinyint = cTinyint,
                             CSmallint = cSmallint,
                             CMediumint = cMediumint,
                             CInt = cInt,
                             CInteger = cInteger,
                             CBigint = cBigint
                         };

                         var actual = await QuerySql.GetMysqlNumericTypesCntAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                        void AssertSingularEquals(QuerySql.GetMysqlNumericTypesCntRow x, QuerySql.GetMysqlNumericTypesCntRow y)
                        {
                            Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                            Assert.That(x.CBool, Is.EqualTo(y.CBool));
                            Assert.That(x.CBoolean, Is.EqualTo(y.CBoolean));
                            Assert.That(x.CTinyint, Is.EqualTo(y.CTinyint));
                            Assert.That(x.CSmallint, Is.EqualTo(y.CSmallint));
                            Assert.That(x.CMediumint, Is.EqualTo(y.CMediumint));
                            Assert.That(x.CInt, Is.EqualTo(y.CInt));
                            Assert.That(x.CInteger, Is.EqualTo(y.CInteger));
                            Assert.That(x.CBigint, Is.EqualTo(y.CBigint));
                        }
                     }
                     """
        },
        [KnownTestType.MySqlFloatingPointCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, 3.4f, -31.55566, 11.09643, 34.4424, 423.2445, 998.999442, 21.214314)]
                     [TestCase(10, null, null, null, null, null, null, null)]
                     public async Task TestFloatingPointCopyFrom(
                        int batchSize, 
                        float? cFloat,
                        decimal? cNumeric,
                        decimal? cDecimal,
                        decimal? cDec,
                        decimal? cFixed,
                        double? cDouble,
                        double? cDoublePrecision)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertMysqlNumericTypesBatchArgs
                             {
                                 CFloat = cFloat,
                                 CNumeric = cNumeric,
                                 CDecimal = cDecimal,
                                 CDec = cDec,
                                 CFixed = cFixed,
                                 CDouble = cDouble,
                                 CDoublePrecision = cDoublePrecision
                             })
                             .ToList();
                         await QuerySql.InsertMysqlNumericTypesBatchAsync(batchArgs);
                         var expected = new QuerySql.GetMysqlNumericTypesCntRow
                         {
                             Cnt = batchSize,
                             CFloat = cFloat,
                             CNumeric = cNumeric,
                             CDecimal = cDecimal,
                             CDec = cDec,
                             CFixed = cFixed,
                             CDouble = cDouble,
                             CDoublePrecision = cDoublePrecision
                         };
                         var actual = await QuerySql.GetMysqlNumericTypesCntAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});
                         
                         void AssertSingularEquals(QuerySql.GetMysqlNumericTypesCntRow x, QuerySql.GetMysqlNumericTypesCntRow y)
                         {
                             Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                             Assert.That(x.CFloat, Is.EqualTo(y.CFloat));
                             Assert.That(x.CNumeric, Is.EqualTo(y.CNumeric));
                             Assert.That(x.CDecimal, Is.EqualTo(y.CDecimal));
                             Assert.That(x.CDec, Is.EqualTo(y.CDec));
                             Assert.That(x.CFixed, Is.EqualTo(y.CFixed));
                             Assert.That(x.CDouble, Is.EqualTo(y.CDouble));
                             Assert.That(x.CDoublePrecision, Is.EqualTo(y.CDoublePrecision));
                         }
                     }
                     """
        },
        [KnownTestType.MySqlDateTimeCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, 1993, "2000-1-30", "1983-11-3 02:01:22", "2010-1-30 08:11:00", "02:01:22")]
                     [TestCase(10, null, null, null, null, null)]
                     public async Task TestDateTimeCopyFrom(
                        int batchSize, 
                        short? cYear,
                        DateTime? cDate, 
                        DateTime? cDatetime,
                        DateTime? cTimestamp,
                        TimeSpan? cTime)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertMysqlDatetimeTypesBatchArgs
                             {
                                 CYear = cYear,
                                 CDate = cDate,
                                 CDatetime = cDatetime,
                                 CTimestamp = cTimestamp,
                                 CTime = cTime
                             })
                             .ToList();
                         await QuerySql.InsertMysqlDatetimeTypesBatchAsync(batchArgs);
                         var expected = new QuerySql.GetMysqlDatetimeTypesCntRow
                         {
                             Cnt = batchSize,
                             CYear = cYear,
                             CDate = cDate,
                             CDatetime = cDatetime,
                             CTimestamp = cTimestamp,
                             CTime = cTime
                         };
                         var actual = await QuerySql.GetMysqlDatetimeTypesCntAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});
                         
                         void AssertSingularEquals(QuerySql.GetMysqlDatetimeTypesCntRow x, QuerySql.GetMysqlDatetimeTypesCntRow y)
                         {
                             Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                             Assert.That(x.CYear, Is.EqualTo(y.CYear));
                             Assert.That(x.CDate, Is.EqualTo(y.CDate));
                             Assert.That(x.CDatetime, Is.EqualTo(y.CDatetime));
                             Assert.That(x.CTimestamp, Is.EqualTo(y.CTimestamp));
                             Assert.That(x.CTime, Is.EqualTo(y.CTime));
                         }
                     }
                     """
        },
        [KnownTestType.MySqlBinaryCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, 0x05, new byte[] { 0x15, 0x16, 0x17 }, new byte[] { 0x15, 0x20 }, new byte[] { 0x23 }, new byte[] { 0x33, 0x13 }, new byte[] { 0x11, 0x62, 0x10 }, new byte[] { 0x38, 0x45, 0x06, 0x04 })]
                     [TestCase(500, null, new byte[] { 0x0, 0x0, 0x0 }, new byte[] { }, new byte[] { }, new byte[] { }, new byte[] { }, new byte[] { })]
                     [TestCase(10, null, null, null, null, null, null, null)]
                     public async Task TestBinaryCopyFrom(
                        int batchSize, 
                        byte? cBit,
                        byte[] cBinary,
                        byte[] cVarbinary, 
                        byte[] cTinyblob, 
                        byte[] cBlob, 
                        byte[] cMediumblob, 
                        byte[] cLongblob)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertMysqlBinaryTypesBatchArgs
                             {
                                 CBit = cBit,
                                 CBinary = cBinary,
                                 CVarbinary = cVarbinary, 
                                 CTinyblob = cTinyblob,
                                 CBlob = cBlob,
                                 CMediumblob = cMediumblob,
                                 CLongblob = cLongblob
                             })
                             .ToList();
                         await QuerySql.InsertMysqlBinaryTypesBatchAsync(batchArgs);
                         var expected = new QuerySql.GetMysqlBinaryTypesCntRow
                         {
                             Cnt = batchSize,
                             CBit = cBit,
                             CBinary = cBinary,
                             CVarbinary = cVarbinary, 
                             CTinyblob = cTinyblob,
                             CBlob = cBlob,
                             CMediumblob = cMediumblob,
                             CLongblob = cLongblob
                         };
                         var actual = await QuerySql.GetMysqlBinaryTypesCntAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetMysqlBinaryTypesCntRow x, QuerySql.GetMysqlBinaryTypesCntRow y)
                         {
                             Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                             Assert.That(x.CBit, Is.EqualTo(y.CBit));
                             Assert.That(x.CBinary, Is.EqualTo(y.CBinary));
                             Assert.That(x.CVarbinary, Is.EqualTo(y.CVarbinary));
                             Assert.That(x.CTinyblob, Is.EqualTo(y.CTinyblob));
                             Assert.That(x.CBlob, Is.EqualTo(y.CBlob));
                             Assert.That(x.CMediumblob, Is.EqualTo(y.CMediumblob));
                             Assert.That(x.CLongblob, Is.EqualTo(y.CLongblob));
                         }
                     }
                     """
        },
        [KnownTestType.MySqlTransaction] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestMySqlTransaction()
                     {
                         var connection = new MySqlConnector.MySqlConnection(Environment.GetEnvironmentVariable(EndToEndCommon.MySqlConnectionStringEnv));
                         await connection.OpenAsync();
                         var transaction = connection.BeginTransaction();

                         var querySqlWithTx = QuerySql.WithTransaction(transaction);
                         await querySqlWithTx.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = {{Consts.BojackId}}, Name = {{Consts.BojackAuthor}}, Bio = {{Consts.BojackTheme}} });

                         var actual = await QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = {{Consts.BojackAuthor}} });
                         ClassicAssert.IsNull(actual);

                         await transaction.CommitAsync();

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
        [KnownTestType.MySqlTransactionRollback] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestMySqlTransactionRollback()
                     {
                         var connection = new MySqlConnector.MySqlConnection(Environment.GetEnvironmentVariable(EndToEndCommon.MySqlConnectionStringEnv));
                         await connection.OpenAsync();
                         var transaction = connection.BeginTransaction();

                         var querySqlWithTx = QuerySql.WithTransaction(transaction);
                         await querySqlWithTx.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = {{Consts.BojackId}}, Name = {{Consts.BojackAuthor}}, Bio = {{Consts.BojackTheme}} });

                         await transaction.RollbackAsync();

                         var actual = await QuerySql.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = {{Consts.BojackAuthor}} });
                         ClassicAssert.IsNull(actual);
                     }
                     """
        },
        [KnownTestType.MySqlEnumDataType] = new TestImpl
        {
            Impl = $$"""
                     private static IEnumerable<TestCaseData> MySqlEnumTypesTestCases
                     {
                         get
                         {
                             yield return new TestCaseData(
                                 MysqlStringTypesCEnum.Medium, 
                                 new HashSet<MysqlStringTypesCSet> { MysqlStringTypesCSet.Tea, MysqlStringTypesCSet.Coffee }
                             ).SetName("Valid Enum values");

                             yield return new TestCaseData(
                                 null, 
                                 null
                             ).SetName("Enum with null values");
                         }
                     }
            
                     [Test]
                     [TestCaseSource(nameof(MySqlEnumTypesTestCases))]
                     public async Task TestMySqlStringTypes(
                         MysqlStringTypesCEnum? cEnum,
                         HashSet<MysqlStringTypesCSet> cSet)
                     {
                         await QuerySql.InsertMysqlStringTypesAsync(new QuerySql.InsertMysqlStringTypesArgs
                         {
                              CEnum = cEnum,
                              CSet = cSet
                         });
                         
                         var expected = new QuerySql.GetMysqlStringTypesRow
                         {
                              CEnum = cEnum,    
                              CSet = cSet
                         };
                         var actual = await QuerySql.GetMysqlStringTypesAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetMysqlStringTypesRow x, QuerySql.GetMysqlStringTypesRow y)
                         {
                             Assert.That(x.CEnum, Is.EqualTo(y.CEnum));
                             Assert.That(x.CSet, Is.EqualTo(y.CSet));
                         }
                     }
                     """
        },
        [KnownTestType.MySqlEnumCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     private static IEnumerable<TestCaseData> MySqlEnumCopyFromTestCases
                     {
                         get
                         {
                             yield return new TestCaseData(
                                 100, 
                                 MysqlStringTypesCEnum.Big, 
                                 new HashSet<MysqlStringTypesCSet> { MysqlStringTypesCSet.Tea, MysqlStringTypesCSet.Coffee }
                             ).SetName("Valid Enum values");

                             yield return new TestCaseData(
                                 10, 
                                 null, 
                                 null
                             ).SetName("Enum with null values");
                         }
                     }

                     [Test]
                     [TestCaseSource(nameof(MySqlEnumCopyFromTestCases))]
                     public async Task TestCopyFrom(
                        int batchSize, 
                        MysqlStringTypesCEnum? cEnum,
                        HashSet<MysqlStringTypesCSet> cSet)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertMysqlStringTypesBatchArgs
                             {
                                 CEnum = cEnum,
                                 CSet = cSet
                             })
                             .ToList();
                         await QuerySql.InsertMysqlStringTypesBatchAsync(batchArgs);
                         var expected = new QuerySql.GetMysqlStringTypesCntRow
                         {
                             Cnt = batchSize,
                             CEnum = cEnum,
                             CSet = cSet
                         };
                         var actual = await QuerySql.GetMysqlStringTypesCntAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetMysqlStringTypesCntRow x, QuerySql.GetMysqlStringTypesCntRow y)
                         {
                             Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                             Assert.That(x.CEnum, Is.EqualTo(y.CEnum));
                             Assert.That(x.CSet, Is.EqualTo(y.CSet));
                         }
                     }
                     """
        },
        [KnownTestType.MySqlScopedSchemaEnum] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestMySqlScopedSchemaEnum()
                     {
                         await this.QuerySql.CreateExtendedBioAsync(new QuerySql.CreateExtendedBioArgs
                         {
                             AuthorName = {{Consts.BojackAuthor}},
                             Name = {{Consts.BojackBookTitle}},
                             BioType = BiosBioType.Memoir,
                             AuthorType = new HashSet<BiosAuthorType> { BiosAuthorType.Author, BiosAuthorType.Translator }
                         });
                         var expected = new QuerySql.GetFirstExtendedBioByTypeRow
                         {
                             AuthorName = {{Consts.BojackAuthor}},
                             Name = {{Consts.BojackBookTitle}},
                             BioType = BiosBioType.Memoir,
                             AuthorType = new HashSet<BiosAuthorType> { BiosAuthorType.Author, BiosAuthorType.Translator }
                         };
                     
                         var actual = await this.QuerySql.GetFirstExtendedBioByTypeAsync(new QuerySql.GetFirstExtendedBioByTypeArgs
                         {
                             BioType = BiosBioType.Memoir
                         });
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetFirstExtendedBioByTypeRow x, QuerySql.GetFirstExtendedBioByTypeRow y)
                         {
                             Assert.That(x.AuthorName, Is.EqualTo(y.AuthorName));
                             Assert.That(x.Name, Is.EqualTo(y.Name));
                             Assert.That(x.BioType, Is.EqualTo(y.BioType));
                             Assert.That(x.AuthorType, Is.EqualTo(y.AuthorType));
                         }
                     }
                     """
        },
        [KnownTestType.MySqlJsonDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase("{\"age\": 42, \"name\": \"The Hitchhiker's Guide to the Galaxy\"}")]
                     [TestCase(null)]
                     public async Task TestMySqlJsonDataType(
                        string cJson)
                     {
                         JsonElement? cParsedJson = null;
                         if (cJson != null)
                             cParsedJson = JsonDocument.Parse(cJson).RootElement;
                             
                         await QuerySql.InsertMysqlStringTypesAsync(new QuerySql.InsertMysqlStringTypesArgs
                         {
                             CJson = cParsedJson,
                             CJsonStringOverride = cJson
                         });
                         var expected = new QuerySql.GetMysqlStringTypesRow
                         {
                             CJson = cParsedJson,
                             CJsonStringOverride = cJson
                         };
                         var actual = await QuerySql.GetMysqlStringTypesAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});
                         
                         void AssertSingularEquals(QuerySql.GetMysqlStringTypesRow x, QuerySql.GetMysqlStringTypesRow y)
                         {
                             Assert.That(x.CJson.HasValue, Is.EqualTo(y.CJson.HasValue));
                             if (x.CJson.HasValue)
                                 Assert.That(x.CJson.Value.GetRawText(), Is.EqualTo(y.CJson.Value.GetRawText()));
                             Assert.That(x.CJsonStringOverride, Is.EqualTo(y.CJsonStringOverride));
                         }
                     }
                     """
        },
        [KnownTestType.MySqlInvalidJson] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public void TestMySqlInvalidJson()
                     {
                         Assert.ThrowsAsync<MySqlConnector.MySqlException>(async () => await 
                            QuerySql.InsertMysqlStringTypesAsync(new QuerySql.InsertMysqlStringTypesArgs
                            {
                                CJsonStringOverride = "SOME INVALID JSON"
                            }));
                     }
                     """
        },
        [KnownTestType.MySqlJsonCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, "{\"name\": \"Swordfishtrombones\", \"year\": 1983}")]
                     [TestCase(10, null)]
                     public async Task TestJsonCopyFrom(
                        int batchSize, 
                        string cJson)
                     {
                         JsonElement? cParsedJson = null;
                         if (cJson != null)
                             cParsedJson = JsonDocument.Parse(cJson).RootElement;

                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertMysqlStringTypesBatchArgs
                             {
                                CJson = cParsedJson
                             })
                             .ToList();
                         await QuerySql.InsertMysqlStringTypesBatchAsync(batchArgs);
                         var expected = new QuerySql.GetMysqlStringTypesCntRow
                         {
                             Cnt = batchSize,
                             CJson = cParsedJson
                         };

                         var actual = await QuerySql.GetMysqlStringTypesCntAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetMysqlStringTypesCntRow x, QuerySql.GetMysqlStringTypesCntRow y)
                         {          
                             Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                             Assert.That(x.CJson.HasValue, Is.EqualTo(y.CJson.HasValue));
                             if (x.CJson.HasValue)
                                 Assert.That(x.CJson.Value.GetRawText(), Is.EqualTo(y.CJson.Value.GetRawText()));
                         }
                     }
                     """
        },
        [KnownTestType.MySqlDataTypesOverride] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(-54355, "Scream of the Butterfly", "2025-06-29 12:00:00")]
                     [TestCase(null, null, "1971-01-01 00:00:00")]
                     public async Task TestMySqlDataTypesOverride(
                        int? cInt,
                        string cVarchar,
                        DateTime cTimestamp)
                     {
                         await QuerySql.InsertMysqlNumericTypesAsync(new QuerySql.InsertMysqlNumericTypesArgs
                         {
                             CInt = cInt
                         });
                         await QuerySql.InsertMysqlStringTypesAsync(new QuerySql.InsertMysqlStringTypesArgs
                         {
                             CVarchar = cVarchar
                         });
                         await QuerySql.InsertMysqlDatetimeTypesAsync(new QuerySql.InsertMysqlDatetimeTypesArgs
                         {
                             CTimestamp = cTimestamp
                         });
                         
                         var expected = new QuerySql.GetMysqlFunctionsRow
                         {
                             MaxInt = cInt,
                             MaxVarchar = cVarchar,
                             MaxTimestamp = cTimestamp
                         };

                         var actual = await QuerySql.GetMysqlFunctionsAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetMysqlFunctionsRow x, QuerySql.GetMysqlFunctionsRow y)
                         {
                             Assert.That(x.MaxInt, Is.EqualTo(y.MaxInt));
                             Assert.That(x.MaxVarchar, Is.EqualTo(y.MaxVarchar));
                             Assert.That(x.MaxTimestamp, Is.EqualTo(y.MaxTimestamp));
                         }
                     }
                     """
        }
    };
}