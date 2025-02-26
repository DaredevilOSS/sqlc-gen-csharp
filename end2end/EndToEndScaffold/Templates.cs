using System.Collections.Generic;

namespace EndToEndScaffold;

public readonly record struct TestImpl(string Impl);

public static class Templates
{
    public const string UnknownRecordValuePlaceholder = "#UnknownValue#";
    public const string UnknownNullableIndicatorPlaceholder = "#UnknownNullableIndicator#";

    private const long BojackId = 1111;
    private const string BojackAuthor = "\"Bojack Horseman\"";
    private const string BojackTheme = "\"Back in the 90s he was in a very famous TV show\"";
    private const string BojackBookTitle = "\"One Trick Pony\"";

    private const long DrSeussId = 2222;
    private const string DrSeussAuthor = "\"Dr. Seuss\"";
    private const string DrSeussQuote = "\"You'll miss the best things if you keep your eyes shut\"";
    private const string DrSeussBookTitle = "\"How the Grinch Stole Christmas!\"";

    private const string GenericAuthor = "\"Albert Einstein\"";
    private const string GenericQuote1 = "\"Quote that everyone always attribute to Einstein\"";
    private const string GenericQuote2 = "\"Only 2 things are infinite, the universe and human stupidity\"";

    private static readonly string CreateBojackAuthor = $$"""
        await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs 
        {
            Id = {{BojackId}},
            Name = {{BojackAuthor}},
            Bio = {{BojackTheme}}
        });
        """;

    private const string CreateBojackAuthorWithId = $$"""
        var bojackId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs 
        {
            Name = {{BojackAuthor}},
            Bio = {{BojackTheme}}
        });
        """;

    private const string CreateBookByBojack = $$"""
        var bojackBookId = await QuerySql.CreateBook(new QuerySql.CreateBookArgs
        {
            Name = {{BojackBookTitle}},
            AuthorId = bojackId
        });
        """;

    private static readonly string CreateDrSeussAuthor = $$"""
       await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs 
       {
           Id = {{DrSeussId}},
           Name = {{DrSeussAuthor}},
           Bio = {{DrSeussQuote}}
       });
       """;

    private const string CreateDrSeussAuthorWithId = $$"""
        var drSeussId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = {{DrSeussAuthor}},
            Bio = {{DrSeussQuote}}
        });
        """;

    private const string CreateBookByDrSeuss = $$"""
        var drSeussBookId = await QuerySql.CreateBook(new QuerySql.CreateBookArgs
        {
            AuthorId = drSeussId,
            Name = {{DrSeussBookTitle}}
        });
        """;

    private const string CreateFirstGenericAuthor = $$"""
        var id1 = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = {{GenericAuthor}},
            Bio = {{GenericQuote1}}
        });
        """;

    private const string CreateSecondGenericAuthor = $$"""
        var id2 = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = {{GenericAuthor}},
            Bio = {{GenericQuote2}}
        });
        """;

    public static Dictionary<KnownTestType, TestImpl> TestImplementations { get; } = new()
    {
        [KnownTestType.One] = new TestImpl
        {
            Impl = $$"""
                    [Test]
                    public async Task TestOne()
                    {
                        {{CreateBojackAuthor}}
                        {{CreateDrSeussAuthor}}
                        var expected = new QuerySql.GetAuthorRow
                        {
                            Id = {{BojackId}},
                            Name = {{BojackAuthor}},
                            Bio = {{BojackTheme}}
                        };
                        var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs
                        {
                            Name = {{BojackAuthor}}
                        });
                        Assert.That(SingularEquals(expected, actual{{UnknownRecordValuePlaceholder}}));
                    }

                    private static bool SingularEquals(QuerySql.GetAuthorRow x, QuerySql.GetAuthorRow y)
                    {
                        return x.Id.Equals(y.Id) && 
                            x.Name.Equals(y.Name) && 
                            x.Bio.Equals(y.Bio);
                    }
                    """
        },
        [KnownTestType.Many] = new TestImpl
        {
            Impl = $$"""
                    [Test]
                    public async Task TestMany()
                    {
                        {{CreateBojackAuthor}}
                        {{CreateDrSeussAuthor}}
                        var expected = new List<QuerySql.ListAuthorsRow>
                        {
                            new QuerySql.ListAuthorsRow 
                            { 
                                Id = {{BojackId}},
                                Name = {{BojackAuthor}}, 
                                Bio = {{BojackTheme}}
                            },
                            new QuerySql.ListAuthorsRow 
                            { 
                                Id = {{DrSeussId}},
                                Name = {{DrSeussAuthor}}, 
                                Bio = {{DrSeussQuote}}
                            }
                        };
                        var actual = await this.QuerySql.ListAuthors();
                        Assert.That(SequenceEquals(expected, actual));
                    }

                    private static bool SingularEquals(QuerySql.ListAuthorsRow x, QuerySql.ListAuthorsRow y)
                    {
                        return x.Id.Equals(y.Id) && 
                            x.Name.Equals(y.Name) && 
                            x.Bio.Equals(y.Bio);
                    }

                    private static bool SequenceEquals(List<QuerySql.ListAuthorsRow> x, List<QuerySql.ListAuthorsRow> y)
                    {
                        if (x.Count != y.Count) return false;
                        x = x.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Id).ToList();
                        y = y.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Id).ToList();
                        return !x.Where((t, i) => !SingularEquals(t, y[i])).Any();
                    }
                    """
        },
        [KnownTestType.Exec] = new TestImpl
        {
            Impl = $$"""
                    [Test]
                    public async Task TestExec()
                    {
                        {{CreateBojackAuthor}}
                        {{CreateDrSeussAuthor}}
                        await this.QuerySql.DeleteAuthor(new QuerySql.DeleteAuthorArgs 
                        { 
                            Name = {{BojackAuthor}} 
                        });
                        var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs 
                        {
                            Name = {{BojackAuthor}}
                        });
                        ClassicAssert.IsNull(actual);
                    }
                    """,
        },
        [KnownTestType.ExecRows] = new TestImpl
        {
            Impl = $$"""
                    [Test]
                    public async Task TestExecRows()
                    {
                        {{CreateBojackAuthor}}
                        {{CreateDrSeussAuthor}}
                        var affectedRows = await this.QuerySql.UpdateAuthors(new QuerySql.UpdateAuthorsArgs
                        {
                            Bio = {{GenericQuote1}}
                        });
                        ClassicAssert.AreEqual(2, affectedRows);
                        var expected = new List<QuerySql.ListAuthorsRow>
                        {
                            new QuerySql.ListAuthorsRow 
                            { 
                                Id = {{BojackId}},
                                Name = {{BojackAuthor}}, 
                                Bio = {{GenericQuote1}}
                            },
                            new QuerySql.ListAuthorsRow 
                            { 
                                Id = {{DrSeussId}},
                                Name = {{DrSeussAuthor}}, 
                                Bio = {{GenericQuote1}}
                            }
                        };
                        var actual = await this.QuerySql.ListAuthors();
                        Assert.That(SequenceEquals(expected, actual));
                    }
                    """,
        },
        [KnownTestType.ExecLastId] = new TestImpl
        {
            Impl = $$"""
                    [Test]
                    public async Task TestExecLastId()
                    {
                        {{CreateFirstGenericAuthor}}
                        var expected = new QuerySql.GetAuthorByIdRow 
                        {
                            Id = id1,
                            Name = {{GenericAuthor}},
                            Bio = {{GenericQuote1}}
                        };
                        var actual = await QuerySql.GetAuthorById(new QuerySql.GetAuthorByIdArgs 
                        {
                            Id = id1
                        });
                        Assert.That(SingularEquals(expected, actual{{UnknownRecordValuePlaceholder}}));
                    }

                    private static bool SingularEquals(QuerySql.GetAuthorByIdRow x, QuerySql.GetAuthorByIdRow y)
                    {
                        return x.Id.Equals(y.Id) && 
                            x.Name.Equals(y.Name) && 
                            x.Bio.Equals(y.Bio);
                    }
                    """
        },
        [KnownTestType.JoinEmbed] = new TestImpl
        {
            Impl = $$"""
                    [Test]
                    public async Task TestJoinEmbed()
                    {
                        {{CreateBojackAuthorWithId}}
                        {{CreateBookByBojack}}
                        {{CreateDrSeussAuthorWithId}}
                        {{CreateBookByDrSeuss}}
                        var expected = new List<QuerySql.ListAllAuthorsBooksRow>()
                        {
                            new QuerySql.ListAllAuthorsBooksRow
                            {
                                Author = new Author 
                                { 
                                    Id = bojackId, 
                                    Name = {{BojackAuthor}}, 
                                    Bio = {{BojackTheme}}
                                },
                                Book = new Book 
                                { 
                                    Id = bojackBookId,
                                    AuthorId = bojackId, 
                                    Name = {{BojackBookTitle}}
                                }
                            },
                            new QuerySql.ListAllAuthorsBooksRow
                            {
                                Author = new Author 
                                { 
                                    Id = drSeussId, 
                                    Name = {{DrSeussAuthor}}, 
                                    Bio = {{DrSeussQuote}}
                                },
                                Book = new Book 
                                { 
                                    Id = drSeussBookId,
                                    AuthorId = drSeussId, 
                                    Name = {{DrSeussBookTitle}} 
                                }
                            }
                        };
                        var actual = await QuerySql.ListAllAuthorsBooks();
                        Assert.That(SequenceEquals(expected, actual));
                    }

                    private static bool SingularEquals(QuerySql.ListAllAuthorsBooksRow x, QuerySql.ListAllAuthorsBooksRow y)
                    {
                        return SingularEquals(x.Author, y.Author) && SingularEquals(x.Book, y.Book);
                    }

                    private static bool SequenceEquals(List<QuerySql.ListAllAuthorsBooksRow> x, List<QuerySql.ListAllAuthorsBooksRow> y)
                    {
                        if (x.Count != y.Count)
                            return false;
                        x = x.OrderBy<QuerySql.ListAllAuthorsBooksRow, object>(o => o.Author.Name + o.Book.Name).ToList();
                        y = y.OrderBy<QuerySql.ListAllAuthorsBooksRow, object>(o => o.Author.Name + o.Book.Name).ToList();
                        return !x.Where((t, i) => !SingularEquals(t, y[i])).Any();
                    }
                    
                    private static bool SingularEquals(Author x, Author y)
                    {
                        return x.Id.Equals(y.Id) && 
                            x.Name.Equals(y.Name) && 
                            x.Bio.Equals(y.Bio);
                    }
                    
                    private static bool SingularEquals(Book x, Book y)
                    {
                        return x.Id.Equals(y.Id) && 
                            x.AuthorId.Equals(y.AuthorId) && 
                            x.Name.Equals(y.Name);
                    }
                    """
        },
        [KnownTestType.SelfJoinEmbed] = new TestImpl
        {
            Impl = $$"""
                    [Test]
                    public async Task TestSelfJoinEmbed()
                    {
                        {{CreateFirstGenericAuthor}}
                        {{CreateSecondGenericAuthor}}
                        var expected = new List<QuerySql.GetDuplicateAuthorsRow>()
                        {
                            new QuerySql.GetDuplicateAuthorsRow
                            {
                                Author = new Author 
                                {
                                    Id = id1,
                                    Name = {{GenericAuthor}}, 
                                    Bio = {{GenericQuote1}}
                                },
                                Author2 = new Author 
                                {
                                    Id = id2,
                                    Name = {{GenericAuthor}}, 
                                    Bio = {{GenericQuote2}}
                                }
                            }
                        };
                        var actual = await QuerySql.GetDuplicateAuthors();
                        Assert.That(SequenceEquals(expected, actual));
                    }

                    private static bool SingularEquals(QuerySql.GetDuplicateAuthorsRow x, QuerySql.GetDuplicateAuthorsRow y)
                    {
                        return SingularEquals(x.Author, y.Author) && SingularEquals(x.Author2, y.Author2);
                    }

                    private static bool SequenceEquals(List<QuerySql.GetDuplicateAuthorsRow> x, List<QuerySql.GetDuplicateAuthorsRow> y)
                    {
                        if (x.Count != y.Count)
                            return false;
                        return !x.Where((t, i) => !SingularEquals(t, y[i])).Any();
                    }
                    """
        },
        [KnownTestType.PartialEmbed] = new TestImpl
        {
            Impl = $$"""
                    [Test]
                    public async Task TestPartialEmbed()
                    {
                        {{CreateBojackAuthorWithId}}
                        {{CreateBookByBojack}}
                        {{CreateDrSeussAuthorWithId}}
                        {{CreateBookByDrSeuss}}
                        var expected = new List<QuerySql.GetAuthorsByBookNameRow>
                        {
                            new QuerySql.GetAuthorsByBookNameRow
                            {
                                Id = bojackId,
                                Name = {{BojackBookTitle}},
                                Bio = {{DrSeussBookTitle}},
                                Book = new Book 
                                { 
                                    Id = bojackBookId,
                                    AuthorId = bojackId,
                                    Name = {{DrSeussBookTitle}} 
                                }
                            }
                        };
                        var actual = await QuerySql.GetAuthorsByBookName(new QuerySql.GetAuthorsByBookNameArgs 
                        { 
                            Name = {{BojackBookTitle}} 
                        });
                        Assert.That(SequenceEquals(expected, actual));
                    }

                    private static bool SingularEquals(QuerySql.GetAuthorsByBookNameRow x, QuerySql.GetAuthorsByBookNameRow y)
                    {
                        return x.Id.Equals(y.Id) && 
                            x.Name.Equals(y.Name) && 
                            x.Bio.Equals(y.Bio) && 
                            SingularEquals(x.Book, y.Book);
                    }

                    private static bool SequenceEquals(List<QuerySql.GetAuthorsByBookNameRow> x, List<QuerySql.GetAuthorsByBookNameRow> y)
                    {
                        if (x.Count != y.Count)
                            return false;
                        return !x.Where((t, i) => !SingularEquals(t, y[i])).Any();
                    }
                    """
        },
        [KnownTestType.Slice] = new TestImpl
        {
            Impl = $$"""
                    [Test]
                    public async Task TestSlice()
                    {
                        {{CreateFirstGenericAuthor}}
                        {{CreateBojackAuthorWithId}}
                        var actual = await QuerySql.GetAuthorsByIds(new QuerySql.GetAuthorsByIdsArgs 
                        { 
                            Ids = new[] { id1, bojackId } 
                        });
                        ClassicAssert.AreEqual(2, actual.Count);
                    }
                    """
        },
        [KnownTestType.ArrayAsParam] = new TestImpl
        {
            Impl = $$"""
                    [Test]
                    public async Task TestArray()
                    {
                        {{CreateFirstGenericAuthor}}
                        {{CreateBojackAuthorWithId}}
                        var actual = await QuerySql.GetAuthorsByIds(new QuerySql.GetAuthorsByIdsArgs 
                        { 
                            LongArr1 = new[] { id1, bojackId } 
                        });
                        ClassicAssert.AreEqual(2, actual.Count);
                    }
                    """
        },
        [KnownTestType.MultipleSlices] = new TestImpl
        {
            Impl = $$"""
                    [Test]
                    public async Task TestMultipleSlices()
                    {
                        {{CreateFirstGenericAuthor}}
                        {{CreateBojackAuthorWithId}}
                        var actual = await QuerySql.GetAuthorsByIdsAndNames(new QuerySql.GetAuthorsByIdsAndNamesArgs 
                        { 
                            Ids = new[] { id1, bojackId }, 
                            Names = new[] { {{GenericAuthor}} } 
                        });
                        ClassicAssert.AreEqual(1, actual.Count);
                    }
                    """
        },
        [KnownTestType.MultipleArraysAsParams] = new TestImpl
        {
            Impl = $$"""
                    [Test]
                    public async Task TestMultipleArrays()
                    {
                        {{CreateFirstGenericAuthor}}
                        {{CreateSecondGenericAuthor}}
                        {{CreateBojackAuthorWithId}}

                        var actual = await QuerySql.GetAuthorsByIdsAndNames(new QuerySql.GetAuthorsByIdsAndNamesArgs 
                        { 
                            LongArr1 = new[] { id1, bojackId }, 
                            StringArr2 = new[] { {{GenericAuthor}} } 
                        });
                        ClassicAssert.AreEqual(1, actual.Count);
                    }
                    """
        },
        [KnownTestType.PostgresCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, true, 3, 453, -1445214231, 336.3431, -99.999, 666.6f, "1973-12-3", "1960-11-3 02:01:22", "z", "Sex Pistols", "Anarchy in the U.K", "Never Mind the Bollocks...")]
                     [TestCase(500, false, -4, 867, 8768769709, -662.8671, 127.4793, -64.8f, "2024-12-31", "1999-3-1 03:00:10", "1", "Fugazi", "Waiting Room", "13 Songs")]
                     [TestCase(10, null, null, null, null, null, null, null, null, null, null, null, null, null)]
                     public async Task TestCopyFrom(
                        int batchSize, 
                        bool? cBoolean, 
                        short? cSmallint, 
                        int? cInteger, 
                        long? cBigint, 
                        decimal? cDecimal, 
                        decimal? cNumeric, 
                        float? cReal, 
                        DateTime? cDate, 
                        DateTime? cTimestamp, 
                        string{{UnknownNullableIndicatorPlaceholder}} cChar, 
                        string{{UnknownNullableIndicatorPlaceholder}} cVarchar, 
                        string{{UnknownNullableIndicatorPlaceholder}} cCharacterVarying, 
                        string{{UnknownNullableIndicatorPlaceholder}} cText)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertPostgresTypesBatchArgs
                             {
                                 CBoolean = cBoolean,
                                 CSmallint = cSmallint,
                                 CInteger = cInteger,
                                 CBigint = cBigint,
                                 CDecimal = cDecimal,
                                 CNumeric = cNumeric,
                                 CReal = cReal,
                                 CDate = cDate,
                                 CTimestamp = cTimestamp,
                                 CChar = cChar,
                                 CVarchar = cVarchar,
                                 CCharacterVarying = cCharacterVarying,
                                 CText = cText
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
                             CDecimal = cDecimal,
                             CNumeric = cNumeric,
                             CReal = cReal,
                             CDate = cDate,
                             CTimestamp = cTimestamp,
                             CChar = cChar,
                             CVarchar = cVarchar,
                             CCharacterVarying = cCharacterVarying,
                             CText = cText
                         };
                         var actual = await QuerySql.GetPostgresTypesAgg();
                         AssertSingularEquals(expected, actual{{UnknownRecordValuePlaceholder}});
                     }
                     
                     private static void AssertSingularEquals(QuerySql.GetPostgresTypesAggRow expected, QuerySql.GetPostgresTypesAggRow actual)
                     {
                         Assert.That(actual.Cnt, Is.EqualTo(expected.Cnt));
                         Assert.That(actual.CBoolean, Is.EqualTo(expected.CBoolean));
                         Assert.That(actual.CSmallint, Is.EqualTo(expected.CSmallint));
                         Assert.That(actual.CInteger, Is.EqualTo(expected.CInteger));
                         Assert.That(actual.CBigint, Is.EqualTo(expected.CBigint));
                         Assert.That(actual.CDecimal, Is.EqualTo(expected.CDecimal));
                         Assert.That(actual.CNumeric, Is.EqualTo(expected.CNumeric));
                         Assert.That(actual.CReal, Is.EqualTo(expected.CReal));
                         Assert.That(actual.CDate, Is.EqualTo(expected.CDate));
                         Assert.That(actual.CTimestamp, Is.EqualTo(expected.CTimestamp));
                         Assert.That(actual.CChar, Is.EqualTo(expected.CChar));
                         Assert.That(actual.CVarchar, Is.EqualTo(expected.CVarchar));
                         Assert.That(actual.CCharacterVarying, Is.EqualTo(expected.CCharacterVarying));
                         Assert.That(actual.CText, Is.EqualTo(expected.CText));
                     }
                     """
        },
        [KnownTestType.MySqlCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, 53, "Parasite", "2000-1-30", "1983-11-3 02:01:22")]
                     [TestCase(500, 6697, "Splendor in the Grass", "2012-9-20", "2012-1-20 22:12:34")]
                     [TestCase(10, null, null, null, null)]
                     public async Task TestCopyFrom(
                        int batchSize, 
                        int? cInt, 
                        string{{UnknownNullableIndicatorPlaceholder}} cVarchar, 
                        DateTime? cDate, 
                        DateTime? cTimestamp)
                     {
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.InsertMysqlTypesBatchArgs
                             {
                                 CInt = cInt,
                                 CVarchar = cVarchar,
                                 CDate = cDate,
                                 CTimestamp = cTimestamp
                             })
                             .ToList();
                         await QuerySql.InsertMysqlTypesBatch(batchArgs);
                         var expected = new QuerySql.GetMysqlTypesAggRow
                         {
                             Cnt = batchSize,
                             CInt = cInt,
                             CVarchar = cVarchar,
                             CDate = cDate,
                             CTimestamp = cTimestamp
                         };
                         var actual = await QuerySql.GetMysqlTypesAgg();
                         AssertSingularEquals(expected, actual{{UnknownRecordValuePlaceholder}});
                     }
                     
                     private static void AssertSingularEquals(QuerySql.GetMysqlTypesAggRow expected, QuerySql.GetMysqlTypesAggRow actual)
                     {
                         Assert.That(actual.Cnt, Is.EqualTo(expected.Cnt));
                         Assert.That(actual.CInt, Is.EqualTo(expected.CInt));
                         Assert.That(actual.CVarchar, Is.EqualTo(expected.CVarchar));
                         Assert.That(actual.CDate, Is.EqualTo(expected.CDate));
                         
                         if (expected.CTimestamp == null)
                            Assert.That(actual.CTimestamp, Is.EqualTo(DateTime.MinValue));
                         else   
                            Assert.That(actual.CTimestamp, Is.EqualTo(expected.CTimestamp));
                     }
                     """
        },
        [KnownTestType.SqliteCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, 312, 1.33f, "Johnny B. Good")]
                     [TestCase(500, 768, 83.56f, "Bad to the Bone")]
                     [TestCase(10, null, null, null)]
                     public async Task TestCopyFrom(
                        int batchSize, 
                        int? cInteger, 
                        float? cReal, 
                        string{{UnknownNullableIndicatorPlaceholder}} cText)
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
                         AssertSingularEquals(expected, actual{{UnknownRecordValuePlaceholder}});
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
        [KnownTestType.PostgresDataTypes] = new TestImpl
        {
            Impl = $$"""
                   [Test]
                   public async Task TestPostgresTypes()
                   {
                       await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs
                       {
                           CBigint = 1,
                           CReal = 1.0f,
                           CNumeric = 1,
                           CSmallint = 1,
                           CDecimal = 1,
                           CDate = new DateTime(1985, 9, 29),
                           CTimestamp = new DateTime(2022, 9, 30, 23, 0, 3),
                           CBoolean = true,
                           CChar = "a",
                           CInteger = 1,
                           CText = "ab",
                           CVarchar = "abc",
                           CCharacterVarying = "abcd",
                           CTextArray = new string[] { "a", "b" },
                           CIntegerArray = new int[] { 1, 2 }
                       });
                   
                       var expected = new QuerySql.GetPostgresTypesRow
                       {
                           CBigint = 1,
                           CReal = 1.0f,
                           CNumeric = 1,
                           CSmallint = 1,
                           CDecimal = 1,
                           CDate = new DateTime(1985, 9, 29),
                           CTimestamp = new DateTime(2022, 9, 30, 23, 0, 3),
                           CBoolean = true,
                           CChar = "a",
                           CInteger = 1,
                           CText = "ab",
                           CVarchar = "abc",
                           CCharacterVarying = "abcd",
                           CTextArray = new string[] { "a", "b" },
                           CIntegerArray = new int[] { 1, 2 }
                       };
                       var actual = await QuerySql.GetPostgresTypes();
                       AssertSingularEquals(expected, actual{{UnknownRecordValuePlaceholder}});
                   }
                   
                   private static void AssertSingularEquals(QuerySql.GetPostgresTypesRow expected, QuerySql.GetPostgresTypesRow actual)
                   {
                       Assert.That(actual.CBigint, Is.EqualTo(expected.CBigint));
                       Assert.That(actual.CReal, Is.EqualTo(expected.CReal));
                       Assert.That(actual.CNumeric, Is.EqualTo(expected.CNumeric));
                       Assert.That(actual.CDate, Is.EqualTo(expected.CDate));
                       Assert.That(actual.CTimestamp, Is.EqualTo(expected.CTimestamp));
                       Assert.That(actual.CBoolean, Is.EqualTo(expected.CBoolean));
                       Assert.That(actual.CChar, Is.EqualTo(expected.CChar));
                       Assert.That(actual.CInteger, Is.EqualTo(expected.CInteger));
                       Assert.That(actual.CText, Is.EqualTo(expected.CText));
                       Assert.That(actual.CVarchar, Is.EqualTo(expected.CVarchar));
                       Assert.That(actual.CCharacterVarying, Is.EqualTo(expected.CCharacterVarying));
                       Assert.That(actual.CTextArray.SequenceEqual(expected.CTextArray));
                       Assert.That(actual.CIntegerArray.SequenceEqual(expected.CIntegerArray));
                   }
                   """
        },
        [KnownTestType.MySqlDataTypes] = new TestImpl
        {
            Impl = $$"""
                   [Test]
                   public async Task TestMySqlTypes()
                   {
                       await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs
                       {
                            CBit = false,
                            CTinyint = true,
                            CBool = true,
                            CBoolean = false,
                            CInt = 312,
                            CVarchar = "321fds",
                            CDate = new DateTime(1985, 9, 29, 23, 59, 59),
                            CTimestamp = new DateTime(2022, 9, 30, 23, 0, 3)
                       });
                   
                       var expected = new QuerySql.GetMysqlTypesRow
                       {
                           CBit = false,
                           CTinyint = true,
                           CBool = true,
                           CBoolean = false,
                           CInt = 312,
                           CVarchar = "321fds",
                           CDate = new DateTime(1985, 9, 29),
                           CTimestamp = new DateTime(2022, 9, 30, 23, 0, 3)
                       };
                       var actual = await QuerySql.GetMysqlTypes();
                       AssertSingularEquals(expected, actual{{UnknownRecordValuePlaceholder}});
                   }
                   
                   private static void AssertSingularEquals(QuerySql.GetMysqlTypesRow expected, QuerySql.GetMysqlTypesRow actual)
                   {
                       Assert.That(actual.CBit, Is.EqualTo(expected.CBit));
                       Assert.That(actual.CTinyint, Is.EqualTo(expected.CTinyint));
                       Assert.That(actual.CBool, Is.EqualTo(expected.CBool));
                       Assert.That(actual.CBoolean, Is.EqualTo(expected.CBoolean));
                       Assert.That(actual.CInt, Is.EqualTo(expected.CInt));
                       Assert.That(actual.CVarchar, Is.EqualTo(expected.CVarchar));
                       Assert.That(actual.CDate, Is.EqualTo(expected.CDate));
                       Assert.That(actual.CTimestamp, Is.EqualTo(expected.CTimestamp));
                   }
                   """
        },
        [KnownTestType.SqliteDataTypes] = new TestImpl
        {
            Impl = $$"""
                   [Test]
                   public async Task TestSqliteTypes()
                   {
                       await QuerySql.InsertSqliteTypes(new QuerySql.InsertSqliteTypesArgs
                       {
                           CInteger = 312,
                           CReal = 1.33f,
                           CText = "fdsfsd",
                           CBlob = new byte[] { 0x15, 0x20, 0x22 },
                       });
                   
                       var expected = new QuerySql.GetSqliteTypesRow
                       {
                           CInteger = 312,
                           CReal = 1.33f,
                           CText = "fdsfsd",
                           CBlob = new byte[] { 0x15, 0x20, 0x22 },
                       };
                       var actual = await QuerySql.GetSqliteTypes();
                       Assert.That(SingularEquals(expected, actual{{UnknownRecordValuePlaceholder}}));
                   }
                   
                   private static bool SingularEquals(QuerySql.GetSqliteTypesRow x, QuerySql.GetSqliteTypesRow y)
                   {
                       return x.CInteger.Equals(y.CInteger) &&
                           x.CReal.Equals(y.CReal) &&
                           x.CText.Equals(y.CText);
                           // TODO add CBlob.Equals - fix impl
                   }
                   """
        },
        [KnownTestType.NargNull] = new TestImpl
        {
            Impl = $$"""
                    [Test]
                    public async Task TestNargNull()
                    {
                        {{CreateBojackAuthor}}
                        {{CreateDrSeussAuthor}}
                        var expected = new List<QuerySql.GetAuthorByNamePatternRow>
                        {
                            new QuerySql.GetAuthorByNamePatternRow
                            {
                                Id = {{BojackId}},
                                Name = {{BojackAuthor}},
                                Bio = {{BojackTheme}}
                            },
                            new QuerySql.GetAuthorByNamePatternRow
                            {
                                Id = {{DrSeussId}},
                                Name = {{DrSeussAuthor}},
                                Bio = {{DrSeussQuote}}
                            }
                        };

                        var actual = await this.QuerySql.GetAuthorByNamePattern(new QuerySql.GetAuthorByNamePatternArgs());
                        Assert.That(SequenceEquals(expected, actual));
                    }

                    private static bool SequenceEquals(List<QuerySql.GetAuthorByNamePatternRow> x, List<QuerySql.GetAuthorByNamePatternRow> y)
                    {
                        if (x.Count != y.Count)
                            return false;
                        x = x.OrderBy<QuerySql.GetAuthorByNamePatternRow, object>(o => o.Id).ToList();
                        y = y.OrderBy<QuerySql.GetAuthorByNamePatternRow, object>(o => o.Id).ToList();
                        return !x.Where((t, i) => !SingularEquals(t, y[i])).Any();
                    }
                    private static bool SingularEquals(QuerySql.GetAuthorByNamePatternRow x, QuerySql.GetAuthorByNamePatternRow y)
                    {
                        return x.Id.Equals(y.Id) && x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
                    }
                   """
        },
        [KnownTestType.NargNotNull] = new TestImpl
        {
            Impl = $$"""
                    [Test]
                    public async Task TestNargNotNull()
                    {
                        {{CreateBojackAuthor}}
                        {{CreateDrSeussAuthor}}

                        var expected = new List<QuerySql.GetAuthorByNamePatternRow>
                        {
                            new QuerySql.GetAuthorByNamePatternRow
                            {
                                Id = {{BojackId}},
                                Name = {{BojackAuthor}},
                                Bio = {{BojackTheme}}
                            }
                        };

                        var actual = await this.QuerySql.GetAuthorByNamePattern(new QuerySql.GetAuthorByNamePatternArgs { NamePattern = "Bojack%" });
                        Assert.That(SequenceEquals(expected, actual));
                    }
                   """
        }
    };
}