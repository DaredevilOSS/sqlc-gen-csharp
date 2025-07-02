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
                         await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs
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
                         
                         var expected = new QuerySql.GetMysqlTypesRow
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
                         var actual = await QuerySql.GetMysqlTypes();
                     
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CChar, Is.EqualTo(expected.CChar));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CNchar, Is.EqualTo(expected.CNchar));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CNationalChar, Is.EqualTo(expected.CNationalChar));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CVarchar, Is.EqualTo(expected.CVarchar));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CTinytext, Is.EqualTo(expected.CTinytext));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CMediumtext, Is.EqualTo(expected.CMediumtext));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CText, Is.EqualTo(expected.CText));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CLongtext, Is.EqualTo(expected.CLongtext));
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
                         await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs
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
                     
                         var expected = new QuerySql.GetMysqlTypesRow
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
                         var actual = await QuerySql.GetMysqlTypes();
                     
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBool, Is.EqualTo(expected.CBool));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBoolean, Is.EqualTo(expected.CBoolean));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CTinyint, Is.EqualTo(expected.CTinyint));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CSmallint, Is.EqualTo(expected.CSmallint));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CMediumint, Is.EqualTo(expected.CMediumint));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CInt, Is.EqualTo(expected.CInt));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CInteger, Is.EqualTo(expected.CInteger));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBigint, Is.EqualTo(expected.CBigint));
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
                         await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs
                         {
                              CFloat = cFloat,
                              CNumeric = cNumeric,
                              CDecimal = cDecimal,
                              CDec = cDec,
                              CFixed = cFixed,
                              CDouble = cDouble,
                              CDoublePrecision = cDoublePrecision
                         });
                     
                         var expected = new QuerySql.GetMysqlTypesRow
                         {
                              CFloat = cFloat,
                              CNumeric = cNumeric,
                              CDecimal = cDecimal,
                              CDec = cDec,
                              CFixed = cFixed,
                              CDouble = cDouble,
                              CDoublePrecision = cDoublePrecision
                         };
                         var actual = await QuerySql.GetMysqlTypes();
                         
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CFloat, Is.EqualTo(expected.CFloat));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CNumeric, Is.EqualTo(expected.CNumeric));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDecimal, Is.EqualTo(expected.CDecimal));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDec, Is.EqualTo(expected.CDec));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CFixed, Is.EqualTo(expected.CFixed));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDouble, Is.EqualTo(expected.CDouble));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDoublePrecision, Is.EqualTo(expected.CDoublePrecision));
                     }
                     """
        },
        [KnownTestType.MySqlDateTimeDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(1999, "2000-1-30", "1983-11-3 02:01:22")]
                     [TestCase(null, null, "1970-1-1 00:00:01")]
                     public async Task TestMySqlDateTimeTypes(
                         short? cYear,
                         DateTime? cDate, 
                         DateTime? cTimestamp)
                     {
                         await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs
                         {
                              CYear = cYear,
                              CDate = cDate,
                              CTimestamp = cTimestamp
                         });
                     
                         var expected = new QuerySql.GetMysqlTypesRow
                         {
                              CYear = cYear,
                              CDate = cDate,
                              CTimestamp = cTimestamp
                         };
                         var actual = await QuerySql.GetMysqlTypes();
                     
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CYear, Is.EqualTo(expected.CYear));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDate, Is.EqualTo(expected.CDate));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CTimestamp, Is.EqualTo(expected.CTimestamp));
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
                         await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs
                         {
                              CBit = cBit,
                              CBinary = cBinary,
                              CVarbinary = cVarbinary, 
                              CTinyblob = cTinyblob,
                              CBlob = cBlob,
                              CMediumblob = cMediumblob,
                              CLongblob = cLongblob
                         });
                     
                         var expected = new QuerySql.GetMysqlTypesRow
                         {
                              CBit = cBit,
                              CBinary = cBinary,
                              CVarbinary = cVarbinary, 
                              CTinyblob = cTinyblob,
                              CBlob = cBlob,
                              CMediumblob = cMediumblob,
                              CLongblob = cLongblob
                         };
                         var actual = await QuerySql.GetMysqlTypes();
                     
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBit, Is.EqualTo(expected.CBit));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBinary, Is.EqualTo(expected.CBinary));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CVarbinary, Is.EqualTo(expected.CVarbinary));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CTinyblob, Is.EqualTo(expected.CTinyblob));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBlob, Is.EqualTo(expected.CBlob));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CMediumblob, Is.EqualTo(expected.CMediumblob));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CLongblob, Is.EqualTo(expected.CLongblob));
                     }
                     """
        },
        [KnownTestType.MySqlEnumDataType] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(MysqlTypesCEnum.Medium)]
                     [TestCase(null)]
                     public async Task TestMySqlStringTypes(MysqlTypesCEnum? cEnum)
                     {
                         await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs
                         {
                              CEnum = cEnum
                         });
                         
                         var expected = new QuerySql.GetMysqlTypesRow
                         {
                              CEnum = cEnum
                         };
                         var actual = await QuerySql.GetMysqlTypes();
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CEnum, Is.EqualTo(expected.CEnum));
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
                             .Select(_ => new QuerySql.InsertMysqlTypesBatchArgs
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
                         await QuerySql.InsertMysqlTypesBatch(batchArgs);
                         var expected = new QuerySql.GetMysqlTypesCntRow
                         {
                             Cnt = batchSize,
                             CChar = cChar,
                             CNchar = cNchar,
                             CNationalChar = cNationalChar,
                             CVarchar = cVarchar,
                             CTinytext = cTinytext,
                             CMediumtext = cMediumtext,
                             CText = cText,
                             CLongtext = cLongtext,
                         };
                         var actual = await QuerySql.GetMysqlTypesCnt();
                         
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.Cnt, Is.EqualTo(expected.Cnt));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CChar, Is.EqualTo(expected.CChar));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CNchar, Is.EqualTo(expected.CNchar));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CNationalChar, Is.EqualTo(expected.CNationalChar));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CVarchar, Is.EqualTo(expected.CVarchar));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CTinytext, Is.EqualTo(expected.CTinytext));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CMediumtext, Is.EqualTo(expected.CMediumtext));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CText, Is.EqualTo(expected.CText));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CLongtext, Is.EqualTo(expected.CLongtext));
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
                             .Select(_ => new QuerySql.InsertMysqlTypesBatchArgs
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
                         await QuerySql.InsertMysqlTypesBatch(batchArgs);
                         var expected = new QuerySql.GetMysqlTypesCntRow
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
                         var actual = await QuerySql.GetMysqlTypesCnt();
                         
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.Cnt, Is.EqualTo(expected.Cnt));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBool, Is.EqualTo(expected.CBool));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBoolean, Is.EqualTo(expected.CBoolean));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CTinyint, Is.EqualTo(expected.CTinyint));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CSmallint, Is.EqualTo(expected.CSmallint));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CMediumint, Is.EqualTo(expected.CMediumint));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CInt, Is.EqualTo(expected.CInt));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CInteger, Is.EqualTo(expected.CInteger));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBigint, Is.EqualTo(expected.CBigint));
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
                             .Select(_ => new QuerySql.InsertMysqlTypesBatchArgs
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
                         await QuerySql.InsertMysqlTypesBatch(batchArgs);
                         var expected = new QuerySql.GetMysqlTypesCntRow
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
                         var actual = await QuerySql.GetMysqlTypesCnt();
                         
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CFloat, Is.EqualTo(expected.CFloat));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CNumeric, Is.EqualTo(expected.CNumeric));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDecimal, Is.EqualTo(expected.CDecimal));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDec, Is.EqualTo(expected.CDec));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CFixed, Is.EqualTo(expected.CFixed));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDouble, Is.EqualTo(expected.CDouble));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDoublePrecision, Is.EqualTo(expected.CDoublePrecision));
                     }
                     """
        },
        [KnownTestType.MySqlDateTimeCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, 1993, "2000-1-30", "1983-11-3 02:01:22", "2010-1-30 08:11:00")]
                     [TestCase(10, null, null, null, null)]
                     public async Task TestDateTimeCopyFrom(
                        int batchSize, 
                        short? cYear,
                        DateTime? cDate, 
                        DateTime? cDatetime,
                        DateTime? cTimestamp)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertMysqlTypesBatchArgs
                             {
                                 CYear = cYear,
                                 CDate = cDate,
                                 CDatetime = cDatetime,
                                 CTimestamp = cTimestamp
                             })
                             .ToList();
                         await QuerySql.InsertMysqlTypesBatch(batchArgs);
                         var expected = new QuerySql.GetMysqlTypesCntRow
                         {
                             Cnt = batchSize,
                             CYear = cYear,
                             CDate = cDate,
                             CDatetime = cDatetime,
                             CTimestamp = cTimestamp
                         };
                         var actual = await QuerySql.GetMysqlTypesCnt();
                         
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.Cnt, Is.EqualTo(expected.Cnt));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CYear, Is.EqualTo(expected.CYear));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDate, Is.EqualTo(expected.CDate));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CDatetime, Is.EqualTo(expected.CDatetime));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CTimestamp, Is.EqualTo(expected.CTimestamp));
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
                             .Select(_ => new QuerySql.InsertMysqlTypesBatchArgs
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
                         await QuerySql.InsertMysqlTypesBatch(batchArgs);
                         var expected = new QuerySql.GetMysqlTypesCntRow
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
                         var actual = await QuerySql.GetMysqlTypesCnt();

                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.Cnt, Is.EqualTo(expected.Cnt));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBit, Is.EqualTo(expected.CBit));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBinary, Is.EqualTo(expected.CBinary));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CVarbinary, Is.EqualTo(expected.CVarbinary));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CTinyblob, Is.EqualTo(expected.CTinyblob));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CBlob, Is.EqualTo(expected.CBlob));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CMediumblob, Is.EqualTo(expected.CMediumblob));
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CLongblob, Is.EqualTo(expected.CLongblob));
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

                         var sqlQueryWithTx = QuerySql.WithTransaction(transaction);
                         await sqlQueryWithTx.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });

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
        [KnownTestType.MySqlTransactionRollback] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestMySqlTransactionRollback()
                     {
                         var connection = new MySqlConnector.MySqlConnection(Environment.GetEnvironmentVariable(EndToEndCommon.MySqlConnectionStringEnv));
                         await connection.OpenAsync();
                         var transaction = connection.BeginTransaction();

                         var sqlQueryWithTx = QuerySql.WithTransaction(transaction);
                         await sqlQueryWithTx.CreateAuthor(new QuerySql.CreateAuthorArgs { Id = 1111, Name = "Bojack Horseman", Bio = "Back in the 90s he was in a very famous TV show" });

                         await transaction.RollbackAsync();

                         var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = "Bojack Horseman" });
                         Assert.That(actual == null, "author should not exist after rollback");
                     }
                     """
        },
        [KnownTestType.MySqlEnumCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, MysqlTypesCEnum.Big)]
                     [TestCase(500, MysqlTypesCEnum.Small)]
                     [TestCase(10, null)]
                     public async Task TestCopyFrom(int batchSize, MysqlTypesCEnum? cEnum)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertMysqlTypesBatchArgs
                             {
                                 CEnum = cEnum
                             })
                             .ToList();
                         await QuerySql.InsertMysqlTypesBatch(batchArgs);
                         var expected = new QuerySql.GetMysqlTypesCntRow
                         {
                             Cnt = batchSize,
                             CEnum = cEnum
                         };
                         var actual = await QuerySql.GetMysqlTypesCnt();
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CEnum, Is.EqualTo(expected.CEnum));
                     }
                     """
        },
        [KnownTestType.MySqlScopedSchemaEnum] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestMySqlScopedSchemaEnum()
                     {
                         await this.QuerySql.CreateExtendedBio(new QuerySql.CreateExtendedBioArgs
                         {
                             AuthorName = {{Consts.BojackAuthor}},
                             Name = {{Consts.BojackBookTitle}},
                             BioType = ExtendedBiosBioType.Memoir
                         });
                         var expected = new QuerySql.GetFirstExtendedBioByTypeRow
                         {
                             AuthorName = {{Consts.BojackAuthor}},
                             Name = {{Consts.BojackBookTitle}},
                             BioType = ExtendedBiosBioType.Memoir
                         };
                     
                         var actual = await this.QuerySql.GetFirstExtendedBioByType(new QuerySql.GetFirstExtendedBioByTypeArgs
                         {
                             BioType = ExtendedBiosBioType.Memoir
                         });
                         Assert.That(SingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}}));
                     }

                     private static bool SingularEquals(QuerySql.GetFirstExtendedBioByTypeRow x, QuerySql.GetFirstExtendedBioByTypeRow y)
                     {
                         return x.AuthorName.Equals(y.AuthorName) && x.Name.Equals(y.Name) && x.BioType.Equals(y.BioType);
                     }
                     """
        },
        [KnownTestType.MySqlDataTypesOverride] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(-54355, 9787876578, "Scream of the Butterfly", "2025-06-29 12:00:00")]
                     [TestCase(null, 0, null, "1971-01-01 00:00:00")]
                     public async Task TestMySqlDataTypesOverride(
                        int? cInt,
                        long cBigint,
                        string cVarchar,
                        DateTime cTimestamp)
                     {
                         await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs
                         {
                             CInt = cInt,
                             CBigint = cBigint,
                             CVarchar = cVarchar,
                             CTimestamp = cTimestamp
                         });
                         var expected = new QuerySql.GetMysqlFunctionsRow
                         {
                             MaxInt = cInt,
                             MaxBigint = cBigint,
                             MaxVarchar = cVarchar,
                             MaxTimestamp = cTimestamp
                         };

                         var actual = await QuerySql.GetMysqlFunctions();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});
                     }

                     private static void AssertSingularEquals(QuerySql.GetMysqlFunctionsRow expected, QuerySql.GetMysqlFunctionsRow actual)
                     {
                         Assert.That(actual.MaxInt, Is.EqualTo(expected.MaxInt));
                         Assert.That(actual.MaxBigint, Is.EqualTo(expected.MaxBigint));
                         Assert.That(actual.MaxVarchar, Is.EqualTo(expected.MaxVarchar));
                         Assert.That(actual.MaxTimestamp, Is.EqualTo(expected.MaxTimestamp));
                     }
                     """
        }
    };
}