using System.Collections.Generic;

namespace EndToEndScaffold.Templates;

public static class MssqlTests
{
    public static Dictionary<KnownTestType, TestImpl> TestImplementations { get; } = new()
    {
        [KnownTestType.MssqlIntegerDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestMssqlIntegerTypes()
                     {
                         await AssertRoundTrip(true);
                         await QuerySql.TruncateMssqlNumericTypesAsync();
                         await AssertRoundTrip(false);

                         async Task AssertRoundTrip(bool withValues)
                         {
                             await QuerySql.InsertMssqlNumericTypesAsync(new QuerySql.InsertMssqlNumericTypesArgs
                             {
                                 CBit = withValues ? (bool?)true : null,
                                 CTinyint = withValues ? (byte?)(byte)13 : null,
                                 CSmallint = withValues ? (short?)(short)2084 : null,
                                 CInt = withValues ? (int?)-54355 : null,
                                 CBigint = withValues ? (long?)9787668656L : null,
                                 CReal = withValues ? (float?)3.4f : null,
                                 CFloat = withValues ? (double?)21.214312452534 : null,
                                 CDecimal = withValues ? (decimal?)11.0986m : null,
                                 CNumeric = withValues ? (decimal?)-31.5556m : null,
                                 CMoney = withValues ? (decimal?)922.5000m : null,
                                 CSmallmoney = withValues ? (decimal?)12.3400m : null
                             });
                             var expected = new QuerySql.GetMssqlNumericTypesRow
                             {
                                 CBit = withValues ? (bool?)true : null,
                                 CTinyint = withValues ? (byte?)(byte)13 : null,
                                 CSmallint = withValues ? (short?)(short)2084 : null,
                                 CInt = withValues ? (int?)-54355 : null,
                                 CBigint = withValues ? (long?)9787668656L : null,
                                 CReal = withValues ? (float?)3.4f : null,
                                 CFloat = withValues ? (double?)21.214312452534 : null,
                                 CDecimal = withValues ? (decimal?)11.0986m : null,
                                 CNumeric = withValues ? (decimal?)-31.5556m : null,
                                 CMoney = withValues ? (decimal?)922.5000m : null,
                                 CSmallmoney = withValues ? (decimal?)12.3400m : null
                             };
                             var actual = await QuerySql.GetMssqlNumericTypesAsync();
                             AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});
                         }

                         void AssertSingularEquals(QuerySql.GetMssqlNumericTypesRow x, QuerySql.GetMssqlNumericTypesRow y)
                         {
                             Assert.That(x.CBit, Is.EqualTo(y.CBit));
                             Assert.That(x.CTinyint, Is.EqualTo(y.CTinyint));
                             Assert.That(x.CSmallint, Is.EqualTo(y.CSmallint));
                             Assert.That(x.CInt, Is.EqualTo(y.CInt));
                             Assert.That(x.CBigint, Is.EqualTo(y.CBigint));
                             Assert.That(x.CReal, Is.EqualTo(y.CReal));
                             Assert.That(x.CFloat, Is.EqualTo(y.CFloat));
                             Assert.That(x.CDecimal, Is.EqualTo(y.CDecimal));
                             Assert.That(x.CNumeric, Is.EqualTo(y.CNumeric));
                             Assert.That(x.CMoney, Is.EqualTo(y.CMoney));
                             Assert.That(x.CSmallmoney, Is.EqualTo(y.CSmallmoney));
                         }
                     }
                     """
        },
        [KnownTestType.MssqlIntegerCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, true)]
                     [TestCase(10, false)]
                     public async Task TestMssqlIntegerCopyFrom(int batchSize, bool withValues)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertMssqlNumericTypesBatchArgs
                             {
                                 CBit = withValues ? (bool?)true : null,
                                 CTinyint = withValues ? (byte?)(byte)13 : null,
                                 CSmallint = withValues ? (short?)(short)2084 : null,
                                 CInt = withValues ? (int?)-54355 : null,
                                 CBigint = withValues ? (long?)9787668656L : null,
                                 CReal = withValues ? (float?)3.4f : null,
                                 CFloat = withValues ? (double?)21.214312452534 : null,
                                 CDecimal = withValues ? (decimal?)11.0986m : null,
                                 CNumeric = withValues ? (decimal?)-31.5556m : null,
                                 CMoney = withValues ? (decimal?)922.5000m : null,
                                 CSmallmoney = withValues ? (decimal?)12.3400m : null
                             })
                             .ToList();
                         await QuerySql.InsertMssqlNumericTypesBatchAsync(batchArgs);
                         var expected = new QuerySql.GetMssqlNumericTypesCntRow
                         {
                             Cnt = batchSize,
                             CBit = withValues ? (bool?)true : null,
                             CTinyint = withValues ? (byte?)(byte)13 : null,
                             CSmallint = withValues ? (short?)(short)2084 : null,
                             CInt = withValues ? (int?)-54355 : null,
                             CBigint = withValues ? (long?)9787668656L : null,
                             CReal = withValues ? (float?)3.4f : null,
                             CFloat = withValues ? (double?)21.214312452534 : null,
                             CDecimal = withValues ? (decimal?)11.0986m : null,
                             CNumeric = withValues ? (decimal?)-31.5556m : null,
                             CMoney = withValues ? (decimal?)922.5000m : null,
                             CSmallmoney = withValues ? (decimal?)12.3400m : null
                         };
                         var actual = await QuerySql.GetMssqlNumericTypesCntAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetMssqlNumericTypesCntRow x, QuerySql.GetMssqlNumericTypesCntRow y)
                         {
                             Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                             Assert.That(x.CBit, Is.EqualTo(y.CBit));
                             Assert.That(x.CTinyint, Is.EqualTo(y.CTinyint));
                             Assert.That(x.CSmallint, Is.EqualTo(y.CSmallint));
                             Assert.That(x.CInt, Is.EqualTo(y.CInt));
                             Assert.That(x.CBigint, Is.EqualTo(y.CBigint));
                             Assert.That(x.CReal, Is.EqualTo(y.CReal));
                             Assert.That(x.CFloat, Is.EqualTo(y.CFloat));
                             Assert.That(x.CDecimal, Is.EqualTo(y.CDecimal));
                             Assert.That(x.CNumeric, Is.EqualTo(y.CNumeric));
                             Assert.That(x.CMoney, Is.EqualTo(y.CMoney));
                             Assert.That(x.CSmallmoney, Is.EqualTo(y.CSmallmoney));
                         }
                     }
                     """
        },
        [KnownTestType.MssqlStringDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestMssqlStringTypes()
                     {
                         await AssertRoundTrip(true);
                         await QuerySql.TruncateMssqlStringTypesAsync();
                         await AssertRoundTrip(false);

                         async Task AssertRoundTrip(bool withValues)
                         {
                             await QuerySql.InsertMssqlStringTypesAsync(new QuerySql.InsertMssqlStringTypesArgs
                             {
                                 CChar = withValues ? (string?)"abcdefghij" : null,
                                 CNchar = withValues ? (string?)"\u1857\u2649abcdefgh" : null,
                                 CVarchar = withValues ? (string?)"Holiday in Cambodia" : null,
                                 CNvarchar = withValues ? (string?)"Sheena \u2649 is a Punk Rocker" : null,
                                 CVarcharMax = withValues ? (string?)"London's Calling" : null,
                                 CNvarcharMax = withValues ? (string?)"Police & Thieves \u1857" : null
                             });
                             var expected = new QuerySql.GetMssqlStringTypesRow
                             {
                                 CChar = withValues ? (string?)"abcdefghij" : null,
                                 CNchar = withValues ? (string?)"\u1857\u2649abcdefgh" : null,
                                 CVarchar = withValues ? (string?)"Holiday in Cambodia" : null,
                                 CNvarchar = withValues ? (string?)"Sheena \u2649 is a Punk Rocker" : null,
                                 CVarcharMax = withValues ? (string?)"London's Calling" : null,
                                 CNvarcharMax = withValues ? (string?)"Police & Thieves \u1857" : null
                             };
                             var actual = await QuerySql.GetMssqlStringTypesAsync();
                             AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});
                         }

                         void AssertSingularEquals(QuerySql.GetMssqlStringTypesRow x, QuerySql.GetMssqlStringTypesRow y)
                         {
                             Assert.That(x.CChar, Is.EqualTo(y.CChar));
                             Assert.That(x.CNchar, Is.EqualTo(y.CNchar));
                             Assert.That(x.CVarchar, Is.EqualTo(y.CVarchar));
                             Assert.That(x.CNvarchar, Is.EqualTo(y.CNvarchar));
                             Assert.That(x.CVarcharMax, Is.EqualTo(y.CVarcharMax));
                             Assert.That(x.CNvarcharMax, Is.EqualTo(y.CNvarcharMax));
                         }
                     }
                     """
        },
        [KnownTestType.MssqlStringCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, true)]
                     [TestCase(10, false)]
                     public async Task TestMssqlStringCopyFrom(int batchSize, bool withValues)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertMssqlStringTypesBatchArgs
                             {
                                 CChar = withValues ? (string?)"abcdefghij" : null,
                                 CNchar = withValues ? (string?)"\u1857\u2649abcdefgh" : null,
                                 CVarchar = withValues ? (string?)"Holiday in Cambodia" : null,
                                 CNvarchar = withValues ? (string?)"Sheena \u2649 is a Punk Rocker" : null,
                                 CVarcharMax = withValues ? (string?)"London's Calling" : null,
                                 CNvarcharMax = withValues ? (string?)"Police & Thieves \u1857" : null
                             })
                             .ToList();
                         await QuerySql.InsertMssqlStringTypesBatchAsync(batchArgs);
                         var expected = new QuerySql.GetMssqlStringTypesCntRow
                         {
                             Cnt = batchSize,
                             CChar = withValues ? (string?)"abcdefghij" : null,
                             CNchar = withValues ? (string?)"\u1857\u2649abcdefgh" : null,
                             CVarchar = withValues ? (string?)"Holiday in Cambodia" : null,
                             CNvarchar = withValues ? (string?)"Sheena \u2649 is a Punk Rocker" : null,
                             CVarcharMax = withValues ? (string?)"London's Calling" : null,
                             CNvarcharMax = withValues ? (string?)"Police & Thieves \u1857" : null
                         };
                         var actual = await QuerySql.GetMssqlStringTypesCntAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetMssqlStringTypesCntRow x, QuerySql.GetMssqlStringTypesCntRow y)
                         {
                             Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                             Assert.That(x.CChar, Is.EqualTo(y.CChar));
                             Assert.That(x.CNchar, Is.EqualTo(y.CNchar));
                             Assert.That(x.CVarchar, Is.EqualTo(y.CVarchar));
                             Assert.That(x.CNvarchar, Is.EqualTo(y.CNvarchar));
                             Assert.That(x.CVarcharMax, Is.EqualTo(y.CVarcharMax));
                             Assert.That(x.CNvarcharMax, Is.EqualTo(y.CNvarcharMax));
                         }
                     }
                     """
        },
        [KnownTestType.MssqlDateTimeDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestMssqlDateTimeTypes()
                     {
                         await AssertRoundTrip(true);
                         await QuerySql.TruncateMssqlDatetimeTypesAsync();
                         await AssertRoundTrip(false);

                         async Task AssertRoundTrip(bool withValues)
                         {
                             await QuerySql.InsertMssqlDatetimeTypesAsync(new QuerySql.InsertMssqlDatetimeTypesArgs
                             {
                                 CDate = withValues ? (DateTime?)new DateTime(2020, 1, 2) : null,
                                 CDatetime = withValues ? (DateTime?)new DateTime(2020, 5, 6, 7, 8, 9) : null,
                                 CDatetime2 = withValues ? (DateTime?)new DateTime(2020, 5, 6, 7, 8, 9, 123) : null,
                                 CSmalldatetime = withValues ? (DateTime?)new DateTime(2020, 5, 6, 7, 8, 0) : null,
                                 CDatetimeoffset = withValues ? (DateTimeOffset?)new DateTimeOffset(2020, 5, 6, 7, 8, 9, TimeSpan.FromHours(2)) : null,
                                 CTime = withValues ? (TimeSpan?)new TimeSpan(7, 8, 9) : null
                             });
                             var expected = new QuerySql.GetMssqlDatetimeTypesRow
                             {
                                 CDate = withValues ? (DateTime?)new DateTime(2020, 1, 2) : null,
                                 CDatetime = withValues ? (DateTime?)new DateTime(2020, 5, 6, 7, 8, 9) : null,
                                 CDatetime2 = withValues ? (DateTime?)new DateTime(2020, 5, 6, 7, 8, 9, 123) : null,
                                 CSmalldatetime = withValues ? (DateTime?)new DateTime(2020, 5, 6, 7, 8, 0) : null,
                                 CDatetimeoffset = withValues ? (DateTimeOffset?)new DateTimeOffset(2020, 5, 6, 7, 8, 9, TimeSpan.FromHours(2)) : null,
                                 CTime = withValues ? (TimeSpan?)new TimeSpan(7, 8, 9) : null
                             };
                             var actual = await QuerySql.GetMssqlDatetimeTypesAsync();
                             AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});
                         }

                         void AssertSingularEquals(QuerySql.GetMssqlDatetimeTypesRow x, QuerySql.GetMssqlDatetimeTypesRow y)
                         {
                             Assert.That(x.CDate, Is.EqualTo(y.CDate));
                             Assert.That(x.CDatetime, Is.EqualTo(y.CDatetime));
                             Assert.That(x.CDatetime2, Is.EqualTo(y.CDatetime2));
                             Assert.That(x.CSmalldatetime, Is.EqualTo(y.CSmalldatetime));
                             Assert.That(x.CDatetimeoffset, Is.EqualTo(y.CDatetimeoffset));
                             Assert.That(x.CTime, Is.EqualTo(y.CTime));
                         }
                     }
                     """
        },
        [KnownTestType.MssqlDateTimeCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, true)]
                     [TestCase(10, false)]
                     public async Task TestMssqlDateTimeCopyFrom(int batchSize, bool withValues)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertMssqlDatetimeTypesBatchArgs
                             {
                                 CDate = withValues ? (DateTime?)new DateTime(2020, 1, 2) : null,
                                 CDatetime = withValues ? (DateTime?)new DateTime(2020, 5, 6, 7, 8, 9) : null,
                                 CDatetime2 = withValues ? (DateTime?)new DateTime(2020, 5, 6, 7, 8, 9, 123) : null,
                                 CSmalldatetime = withValues ? (DateTime?)new DateTime(2020, 5, 6, 7, 8, 0) : null,
                                 CDatetimeoffset = withValues ? (DateTimeOffset?)new DateTimeOffset(2020, 5, 6, 7, 8, 9, TimeSpan.FromHours(2)) : null,
                                 CTime = withValues ? (TimeSpan?)new TimeSpan(7, 8, 9) : null
                             })
                             .ToList();
                         await QuerySql.InsertMssqlDatetimeTypesBatchAsync(batchArgs);
                         var expected = new QuerySql.GetMssqlDatetimeTypesCntRow
                         {
                             Cnt = batchSize,
                             CDate = withValues ? (DateTime?)new DateTime(2020, 1, 2) : null,
                             CDatetime = withValues ? (DateTime?)new DateTime(2020, 5, 6, 7, 8, 9) : null,
                             CDatetime2 = withValues ? (DateTime?)new DateTime(2020, 5, 6, 7, 8, 9, 123) : null,
                             CSmalldatetime = withValues ? (DateTime?)new DateTime(2020, 5, 6, 7, 8, 0) : null,
                             CDatetimeoffset = withValues ? (DateTimeOffset?)new DateTimeOffset(2020, 5, 6, 7, 8, 9, TimeSpan.FromHours(2)) : null,
                             CTime = withValues ? (TimeSpan?)new TimeSpan(7, 8, 9) : null
                         };
                         var actual = await QuerySql.GetMssqlDatetimeTypesCntAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetMssqlDatetimeTypesCntRow x, QuerySql.GetMssqlDatetimeTypesCntRow y)
                         {
                             Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                             Assert.That(x.CDate, Is.EqualTo(y.CDate));
                             Assert.That(x.CDatetime, Is.EqualTo(y.CDatetime));
                             Assert.That(x.CDatetime2, Is.EqualTo(y.CDatetime2));
                             Assert.That(x.CSmalldatetime, Is.EqualTo(y.CSmalldatetime));
                             Assert.That(x.CDatetimeoffset, Is.EqualTo(y.CDatetimeoffset));
                             Assert.That(x.CTime, Is.EqualTo(y.CTime));
                         }
                     }
                     """
        },
        [KnownTestType.MssqlBinaryDataTypes] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestMssqlBinaryTypes()
                     {
                         await AssertRoundTrip(true);
                         await QuerySql.TruncateMssqlBinaryTypesAsync();
                         await AssertRoundTrip(false);

                         async Task AssertRoundTrip(bool withValues)
                         {
                             await QuerySql.InsertMssqlBinaryTypesAsync(new QuerySql.InsertMssqlBinaryTypesArgs
                             {
                                 CBinary = withValues ? (byte[]?)new byte[] { 1, 2, 3 } : null,
                                 CVarbinary = withValues ? (byte[]?)new byte[] { 4, 5, 6, 7 } : null,
                                 CVarbinaryMax = withValues ? (byte[]?)new byte[] { 8, 9, 10, 11, 12 } : null,
                                 CUniqueidentifier = withValues ? (Guid?)Guid.Parse("a3b1c5d7-1234-4abc-9def-0123456789ab") : null
                             });
                             var expected = new QuerySql.GetMssqlBinaryTypesRow
                             {
                                 CBinary = withValues ? (byte[]?)new byte[] { 1, 2, 3 } : null,
                                 CVarbinary = withValues ? (byte[]?)new byte[] { 4, 5, 6, 7 } : null,
                                 CVarbinaryMax = withValues ? (byte[]?)new byte[] { 8, 9, 10, 11, 12 } : null,
                                 CUniqueidentifier = withValues ? (Guid?)Guid.Parse("a3b1c5d7-1234-4abc-9def-0123456789ab") : null
                             };
                             var actual = await QuerySql.GetMssqlBinaryTypesAsync();
                             AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});
                         }

                         void AssertSingularEquals(QuerySql.GetMssqlBinaryTypesRow x, QuerySql.GetMssqlBinaryTypesRow y)
                         {
                             Assert.That(x.CBinary, Is.EqualTo(y.CBinary));
                             Assert.That(x.CVarbinary, Is.EqualTo(y.CVarbinary));
                             Assert.That(x.CVarbinaryMax, Is.EqualTo(y.CVarbinaryMax));
                             Assert.That(x.CUniqueidentifier, Is.EqualTo(y.CUniqueidentifier));
                         }
                     }
                     """
        },
        [KnownTestType.MssqlBinaryCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, true)]
                     [TestCase(10, false)]
                     public async Task TestMssqlBinaryCopyFrom(int batchSize, bool withValues)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertMssqlBinaryTypesBatchArgs
                             {
                                 CBinary = withValues ? (byte[]?)new byte[] { 1, 2, 3 } : null,
                                 CVarbinary = withValues ? (byte[]?)new byte[] { 4, 5, 6, 7 } : null,
                                 CVarbinaryMax = withValues ? (byte[]?)new byte[] { 8, 9, 10, 11, 12 } : null,
                                 CUniqueidentifier = withValues ? (Guid?)Guid.Parse("a3b1c5d7-1234-4abc-9def-0123456789ab") : null
                             })
                             .ToList();
                         await QuerySql.InsertMssqlBinaryTypesBatchAsync(batchArgs);
                         var expected = new QuerySql.GetMssqlBinaryTypesCntRow
                         {
                             Cnt = batchSize,
                             CBinary = withValues ? (byte[]?)new byte[] { 1, 2, 3 } : null,
                             CVarbinary = withValues ? (byte[]?)new byte[] { 4, 5, 6, 7 } : null,
                             CVarbinaryMax = withValues ? (byte[]?)new byte[] { 8, 9, 10, 11, 12 } : null,
                             CUniqueidentifier = withValues ? (Guid?)Guid.Parse("a3b1c5d7-1234-4abc-9def-0123456789ab") : null
                         };
                         var actual = await QuerySql.GetMssqlBinaryTypesCntAsync();
                         AssertSingularEquals(expected, actual{{Consts.UnknownRecordValuePlaceholder}});

                         void AssertSingularEquals(QuerySql.GetMssqlBinaryTypesCntRow x, QuerySql.GetMssqlBinaryTypesCntRow y)
                         {
                             Assert.That(x.Cnt, Is.EqualTo(y.Cnt));
                             Assert.That(x.CBinary, Is.EqualTo(y.CBinary));
                             Assert.That(x.CVarbinary, Is.EqualTo(y.CVarbinary));
                             Assert.That(x.CVarbinaryMax, Is.EqualTo(y.CVarbinaryMax));
                             Assert.That(x.CUniqueidentifier, Is.EqualTo(y.CUniqueidentifier));
                         }
                     }
                     """
        },
        [KnownTestType.MssqlTransaction] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestMssqlTransaction()
                     {
                         var connection = new Microsoft.Data.SqlClient.SqlConnection(Environment.GetEnvironmentVariable(EndToEndCommon.MssqlConnectionStringEnv));
                         await connection.OpenAsync();
                         var transaction = connection.BeginTransaction();

                         var querySqlWithTx = QuerySql.WithTransaction(transaction);
                         await querySqlWithTx.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = {{Consts.BojackId}}, Name = {{Consts.BojackAuthor}}, Bio = {{Consts.BojackTheme}} });

                         // the uncommitted row is locked by the transaction, so it is read from within it
                         var actual = await querySqlWithTx.GetAuthorAsync(new QuerySql.GetAuthorArgs { Name = {{Consts.BojackAuthor}} });
                         ClassicAssert.IsNotNull(actual);

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
        [KnownTestType.MssqlTransactionRollback] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestMssqlTransactionRollback()
                     {
                         var connection = new Microsoft.Data.SqlClient.SqlConnection(Environment.GetEnvironmentVariable(EndToEndCommon.MssqlConnectionStringEnv));
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
    };
}