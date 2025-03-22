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
                     [TestCase("&", "\u1857", "\u2649", "Sheena is a Punk Rocker", "Holiday in Cambodia", "London's Calling", "London's Burning", "Police & Thieves", "Medium")]
                     [TestCase(null, null, null, null, null, null, null, null, null)]
                     public async Task TestMySqlStringTypes(
                         string cChar,
                         string cNchar,
                         string cNationalChar,
                         string cVarchar,
                         string cTinytext,
                         string cMediumtext,
                         string cText,
                         string cLongtext,
                         string cEnum)
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
                              CLongtext = cLongtext,
                              CEnum = cEnum
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
                              CLongtext = cLongtext,
                              CEnum = cEnum
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
                         Assert.That(actual{{Consts.UnknownRecordValuePlaceholder}}.CEnum, Is.EqualTo(expected.CEnum));
                     }
                     """
        },
        [KnownTestType.MySqlIntegerDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(false, true, 13, 2084, 3124, -54355, 324245, -67865, 9787668656l)]
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
        [KnownTestType.MySqlCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, true, false, 0x05, -13, 324, -98760, 987965, 3132423, -7785442l, 3.4f, -31.555666, 11.098643, 34.4424, 423.2445, 998.9994542, 21.214312452534, "D", "\u4321", "\u2345", "Parasite", "Clockwork Orange", "Dr. Strangelove", "Interview with a Vampire", "Memento", "Big", 1993, "2000-1-30", "1983-11-3 02:01:22", "2010-1-30 08:11:00", new byte[] { 0x15, 0x16, 0x17 }, new byte[] { 0x15, 0x20 }, new byte[] { 0x23 }, new byte[] { 0x33, 0x13 }, new byte[] { 0x11, 0x62, 0x10 }, new byte[] { 0x38, 0x45, 0x06, 0x04 })]
                     [TestCase(500, false, true, 0x12, 8, -555, 66979, -423425, -9798642, 3297398l, 1.23f, 99.35542, 32.33345, -12.3456, -55.55556, -11.1123334, 33.423542356346, "3", "\u1234", "\u6543", "Splendor in the Grass", "Pulp Fiction", "Chinatown", "Repulsion", "Million Dollar Baby", "Small", 2025, "2012-9-20", "2012-1-20 22:12:34", "1984-6-5 20:12:12", new byte[] { 0x0, 0x0, 0x0 }, new byte[] { }, new byte[] { }, new byte[] { }, new byte[] { }, new byte[] { })]
                     [TestCase(10, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, "1970-1-1 00:00:01", null, null, null, null, null, null)]
                     public async Task TestCopyFrom(
                        int batchSize, 
                        
                        bool? cBool,
                        bool? cBoolean,
                        byte? cBit,
                        short? cTinyint,
                        short? cSmallint,
                        int? cMediumint,
                        int? cInt, 
                        int? cInteger,
                        long? cBigint,
                        
                        float? cFloat,
                        decimal? cNumeric,
                        decimal? cDecimal,
                        decimal? cDec,
                        decimal? cFixed,
                        double? cDouble,
                        double? cDoublePrecision,
                        
                        string cChar,
                        string cNchar,
                        string cNationalChar,
                        string cVarchar, 
                        string cTinytext, 
                        string cMediumtext, 
                        string cText, 
                        string cLongtext,
                        string cEnum,
                        
                        short? cYear,
                        DateTime? cDate, 
                        DateTime? cDatetime,
                        DateTime? cTimestamp,
                        
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
                                 CBool = cBool,
                                 CBoolean = cBoolean,
                                 CTinyint = cTinyint,
                                 CSmallint = cSmallint,
                                 CMediumint = cMediumint,
                                 CInt = cInt,
                                 CInteger = cInteger,
                                 CBigint = cBigint,
                                 
                                 CChar = cChar,
                                 CNchar = cNchar,
                                 CNationalChar = cNationalChar,
                                 CVarchar = cVarchar,
                                 CTinytext = cTinytext,
                                 CMediumtext = cMediumtext,
                                 CText = cText,
                                 CLongtext = cLongtext,
                                 CEnum = cEnum,
                                 
                                 CYear = cYear,
                                 CDate = cDate,
                                 CDatetime = cDatetime,
                                 CTimestamp = cTimestamp,
                                 
                                 CBinary = cBinary,
                                 CVarbinary = cVarbinary, 
                                 CTinyblob = cTinyblob,
                                 CBlob = cBlob,
                                 CMediumblob = cMediumblob,
                                 CLongblob = cLongblob
                             })
                             .ToList();
                         await QuerySql.InsertMysqlTypesBatch(batchArgs);
                         var expected = new QuerySql.GetMysqlTypesAggRow
                         {
                             Cnt = batchSize,
                             
                             CBool = cBool,
                             CBoolean = cBoolean,
                             CTinyint = cTinyint,
                             CSmallint = cSmallint,
                             CMediumint = cMediumint,
                             CInt = cInt,
                             CInteger = cInteger,
                             CBigint = cBigint,
                             
                             CChar = cChar,
                             CNchar = cNchar,
                             CNationalChar = cNationalChar,
                             CVarchar = cVarchar,
                             CTinytext = cTinytext,
                             CMediumtext = cMediumtext,
                             CText = cText,
                             CLongtext = cLongtext,
                             CEnum = cEnum,
                             
                             CYear = cYear,
                             CDate = cDate,
                             CDatetime = cDatetime,
                             CTimestamp = cTimestamp,
                             
                             CBinary = cBinary,
                             CVarbinary = cVarbinary, 
                             CTinyblob = cTinyblob,
                             CBlob = cBlob,
                             CMediumblob = cMediumblob,
                             CLongblob = cLongblob
                         };
                         var actual = await QuerySql.GetMysqlTypesAgg();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});
                     }

                     private static void AssertSingularEquals(QuerySql.GetMysqlTypesAggRow expected, QuerySql.GetMysqlTypesAggRow actual)
                     {
                         Assert.That(actual.Cnt, Is.EqualTo(expected.Cnt));
                         
                         Assert.That(actual.CBool, Is.EqualTo(expected.CBool));
                         Assert.That(actual.CBoolean, Is.EqualTo(expected.CBoolean));
                         Assert.That(actual.CTinyint, Is.EqualTo(expected.CTinyint));
                         Assert.That(actual.CSmallint, Is.EqualTo(expected.CSmallint));
                         Assert.That(actual.CMediumint, Is.EqualTo(expected.CMediumint));
                         Assert.That(actual.CInt, Is.EqualTo(expected.CInt));
                         Assert.That(actual.CInteger, Is.EqualTo(expected.CInteger));
                         Assert.That(actual.CBigint, Is.EqualTo(expected.CBigint));
                         
                         Assert.That(actual.CChar, Is.EqualTo(expected.CChar));
                         Assert.That(actual.CNchar, Is.EqualTo(expected.CNchar));
                         Assert.That(actual.CNationalChar, Is.EqualTo(expected.CNationalChar));
                         Assert.That(actual.CVarchar, Is.EqualTo(expected.CVarchar));
                         Assert.That(actual.CTinytext, Is.EqualTo(expected.CTinytext));
                         Assert.That(actual.CMediumtext, Is.EqualTo(expected.CMediumtext));
                         Assert.That(actual.CText, Is.EqualTo(expected.CText));
                         Assert.That(actual.CLongtext, Is.EqualTo(expected.CLongtext));
                         Assert.That(actual.CEnum, Is.EqualTo(expected.CEnum));
                         
                         Assert.That(actual.CYear, Is.EqualTo(expected.CYear));
                         Assert.That(actual.CDate, Is.EqualTo(expected.CDate));
                         Assert.That(actual.CDatetime, Is.EqualTo(expected.CDatetime));
                         Assert.That(actual.CTimestamp, Is.EqualTo(expected.CTimestamp));
                         
                         Assert.That(actual.CBinary, Is.EqualTo(expected.CBinary));
                         Assert.That(actual.CVarbinary, Is.EqualTo(expected.CVarbinary));
                         Assert.That(actual.CTinyblob, Is.EqualTo(expected.CTinyblob));
                         Assert.That(actual.CBlob, Is.EqualTo(expected.CBlob));
                         Assert.That(actual.CMediumblob, Is.EqualTo(expected.CMediumblob));
                         Assert.That(actual.CLongblob, Is.EqualTo(expected.CLongblob));
                     }
                     """
        }
    };
}