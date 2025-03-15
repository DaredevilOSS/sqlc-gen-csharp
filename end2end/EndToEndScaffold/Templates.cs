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
        [KnownTestType.PostgresDataTypes] = new TestImpl
        {
            Impl = $$"""
                   [Test]
                   [TestCase(true, 35, -23423, 4235235263, 3.83f, 4.5534, 998.432, -8403284.321435, "2000-1-30", "1983-11-3 02:01:22", "E", "It takes a nation of millions to hold us back", "Rebel Without a Pause", "Prophets of Rage", new byte[] { 0x45, 0x42 }, new string[] { "Party", "Fight" }, new int[] { 543, -4234 })]
                   [TestCase(null, null, null, null, null, null, null, null, null, null, null, null, null, null, new byte[] { }, new string[] { }, new int[] { })]
                   [TestCase(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)]
                   public async Task TestPostgresTypes(
                       bool cBoolean,
                       short cSmallint, 
                       int cInteger,
                       long cBigint, 
                       float cReal, 
                       decimal cNumeric, 
                       decimal cDecimal, 
                       double cDoublePrecision,
                       DateTime cDate,
                       DateTime cTimestamp,
                       string cChar,
                       string cVarchar,
                       string cCharacterVarying,
                       string cText,
                       byte[] cBytea,
                       string[] cTextArray,
                       int[] cIntegerArray)
                   {
                       await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs
                       {
                           CBoolean = cBoolean,
                           CSmallint = cSmallint,
                           CInteger = cInteger,
                           CBigint = cBigint,
                           CReal = cReal,
                           CNumeric = cNumeric,
                           CDecimal = cDecimal,
                           CDoublePrecision = cDoublePrecision,
                           CDate = cDate,
                           CTimestamp = cTimestamp,
                           CChar = cChar,
                           CVarchar = cVarchar,
                           CCharacterVarying = cCharacterVarying,
                           CText = cText,
                           CBytea = cBytea,
                           CTextArray = cTextArray,
                           CIntegerArray = cIntegerArray
                       });
                   
                       var expected = new QuerySql.GetPostgresTypesRow
                       {
                           CBoolean = cBoolean,
                           CSmallint = cSmallint,
                           CInteger = cInteger,
                           CBigint = cBigint,
                           CReal = cReal,
                           CNumeric = cNumeric,
                           CDecimal = cDecimal,
                           CDoublePrecision = cDoublePrecision,
                           CDate = cDate,
                           CTimestamp = cTimestamp,
                           CChar = cChar,
                           CVarchar = cVarchar,
                           CCharacterVarying = cCharacterVarying,
                           CText = cText,
                           CBytea = cBytea,
                           CTextArray = cTextArray,
                           CIntegerArray = cIntegerArray
                       };
                       var actual = await QuerySql.GetPostgresTypes();
                       AssertSingularEquals(expected, actual{{UnknownRecordValuePlaceholder}});
                   }
                   
                   private static void AssertSingularEquals(QuerySql.GetPostgresTypesRow expected, QuerySql.GetPostgresTypesRow actual)
                   {
                       Assert.That(actual.CBoolean, Is.EqualTo(expected.CBoolean));
                       Assert.That(actual.CSmallint, Is.EqualTo(expected.CSmallint));
                       Assert.That(actual.CInteger, Is.EqualTo(expected.CInteger));
                       Assert.That(actual.CBigint, Is.EqualTo(expected.CBigint));
                       Assert.That(actual.CReal, Is.EqualTo(expected.CReal));
                       Assert.That(actual.CNumeric, Is.EqualTo(expected.CNumeric));
                       Assert.That(actual.CDecimal, Is.EqualTo(expected.CDecimal));
                       Assert.That(actual.CDoublePrecision, Is.EqualTo(expected.CDoublePrecision));
                       Assert.That(actual.CDate, Is.EqualTo(expected.CDate));
                       Assert.That(actual.CTimestamp, Is.EqualTo(expected.CTimestamp));
                       Assert.That(actual.CChar, Is.EqualTo(expected.CChar));
                       Assert.That(actual.CVarchar, Is.EqualTo(expected.CVarchar));
                       Assert.That(actual.CCharacterVarying, Is.EqualTo(expected.CCharacterVarying));
                       Assert.That(actual.CText, Is.EqualTo(expected.CText));
                       Assert.That(actual.CBytea, Is.EqualTo(expected.CBytea));
                       Assert.That(actual.CTextArray, Is.EqualTo(expected.CTextArray));
                       Assert.That(actual.CIntegerArray, Is.EqualTo(expected.CIntegerArray));
                   }
                   """
        },
        [KnownTestType.PostgresCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, true, 3, 453, -1445214231, 666.6f, 336.3431, -99.999, -1377.996, "1973-12-3", "1960-11-3 02:01:22", "z", "Sex Pistols", "Anarchy in the U.K", "Never Mind the Bollocks...", new byte[] { 0x53, 0x56 })]
                     [TestCase(500, false, -4, 867, 8768769709, -64.8f, -324.8671, 127.4793, 423.9869, "2024-12-31", "1999-3-1 03:00:10", "1", "Fugazi", "Waiting Room", "13 Songs", new byte[] { 0x03 })]
                     [TestCase(10, null, null, null, null, null, null, null, null, null, null, null, null, null, null, new byte[] { })]
                     [TestCase(10, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)]
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
                        DateTime? cDate, 
                        DateTime? cTimestamp, 
                        string{{UnknownNullableIndicatorPlaceholder}} cChar, 
                        string{{UnknownNullableIndicatorPlaceholder}} cVarchar, 
                        string{{UnknownNullableIndicatorPlaceholder}} cCharacterVarying, 
                        string{{UnknownNullableIndicatorPlaceholder}} cText,
                        byte[] cBytea)
                     {
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
                                 CDate = cDate,
                                 CTimestamp = cTimestamp,
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
                             CDate = cDate,
                             CTimestamp = cTimestamp,
                             CChar = cChar,
                             CVarchar = cVarchar,
                             CCharacterVarying = cCharacterVarying,
                             CText = cText,
                             CBytea = cBytea
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
                         Assert.That(actual.CReal, Is.EqualTo(expected.CReal));
                         Assert.That(actual.CDecimal, Is.EqualTo(expected.CDecimal));
                         Assert.That(actual.CNumeric, Is.EqualTo(expected.CNumeric));
                         Assert.That(actual.CDoublePrecision, Is.EqualTo(expected.CDoublePrecision));
                         Assert.That(actual.CDate, Is.EqualTo(expected.CDate));
                         Assert.That(actual.CTimestamp, Is.EqualTo(expected.CTimestamp));
                         Assert.That(actual.CChar, Is.EqualTo(expected.CChar));
                         Assert.That(actual.CVarchar, Is.EqualTo(expected.CVarchar));
                         Assert.That(actual.CCharacterVarying, Is.EqualTo(expected.CCharacterVarying));
                         Assert.That(actual.CText, Is.EqualTo(expected.CText));
                         Assert.That(actual.CBytea, Is.EqualTo(expected.CBytea));
                     }
                     """
        },

        [KnownTestType.MySqlDataTypes] = new TestImpl
        {
            Impl = $$"""
                   [Test]
                   [TestCase(false, true, 0x32, 13, 2084, 3124, -54355, 324245, -67865, 9787668656, "&", "\u1857", "\u2649", "Sheena is a Punk Rocker", "Holiday in Cambodia", "London's Calling", "London's Burning", "Police & Thieves", "2000-1-30", "1983-11-3 02:01:22", new byte[] { 0x15, 0x16, 0x17 }, new byte[] { 0x15, 0x24 }, new byte[] { 0x23 }, new byte[] { 0x33, 0x13 }, new byte[] { 0x11, 0x62, 0x10 }, new byte[] { 0x38, 0x45, 0x06 })]
                   [TestCase(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, "1970-1-1 00:00:01", new byte[] { 0x0, 0x0, 0x0 }, new byte[] { }, new byte[] { }, new byte[] { }, new byte[] { }, new byte[] { })]
                   [TestCase(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, "1970-1-1 00:00:01", null, null, null, null, null, null)]
                   public async Task TestMySqlTypes(
                       bool cBool, 
                       bool cBoolean, 
                       byte cBit,
                       short cTinyint, 
                       short cYear,
                       short cSmallint,
                       int cMediumint,
                       int cInt, 
                       int cInteger,
                       long cBigint, 
                       string cChar,
                       string cNchar,
                       string cNationalChar,
                       string cVarchar,
                       string cTinytext,
                       string cMediumtext,
                       string cText,
                       string cLongtext,
                       DateTime cDate, 
                       DateTime cTimestamp,
                       byte[] cBinary,
                       byte[] cVarbinary, 
                       byte[] cTinyblob, 
                       byte[] cBlob, 
                       byte[] cMediumblob, 
                       byte[] cLongblob)
                   {
                       await QuerySql.InsertMysqlTypes(new QuerySql.InsertMysqlTypesArgs
                       {
                            CBool = cBool,
                            CBoolean = cBoolean,
                            CBit = cBit,
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
                            CYear = cYear,
                            CDate = cDate,
                            CTimestamp = cTimestamp,
                            CBinary = cBinary,
                            CVarbinary = cVarbinary, 
                            CTinyblob = cTinyblob,
                            CBlob = cBlob,
                            CMediumblob = cMediumblob,
                            CLongblob = cLongblob
                       });
                   
                       var expected = new QuerySql.GetMysqlTypesRow
                       {
                            CBool = cBool,
                            CBoolean = cBoolean,
                            CBit = cBit,
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
                            CYear = cYear,
                            CDate = cDate,
                            CTimestamp = cTimestamp,
                            CBinary = cBinary,
                            CVarbinary = cVarbinary, 
                            CTinyblob = cTinyblob,
                            CBlob = cBlob,
                            CMediumblob = cMediumblob,
                            CLongblob = cLongblob
                       };
                       var actual = await QuerySql.GetMysqlTypes();
                       AssertSingularEquals(expected, actual{{UnknownRecordValuePlaceholder}});
                   }
                   
                   private static void AssertSingularEquals(QuerySql.GetMysqlTypesRow expected, QuerySql.GetMysqlTypesRow actual)
                   {
                       Assert.That(actual.CBool, Is.EqualTo(expected.CBool));
                       Assert.That(actual.CBoolean, Is.EqualTo(expected.CBoolean));
                       Assert.That(actual.CBit, Is.EqualTo(expected.CBit));
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
                       Assert.That(actual.CYear, Is.EqualTo(expected.CYear));
                       Assert.That(actual.CDate, Is.EqualTo(expected.CDate));
                       Assert.That(actual.CTimestamp, Is.EqualTo(expected.CTimestamp));
                       Assert.That(actual.CBinary, Is.EqualTo(expected.CBinary));
                       Assert.That(actual.CVarbinary, Is.EqualTo(expected.CVarbinary));
                       Assert.That(actual.CTinyblob, Is.EqualTo(expected.CTinyblob));
                       Assert.That(actual.CBlob, Is.EqualTo(expected.CBlob));
                       Assert.That(actual.CMediumblob, Is.EqualTo(expected.CMediumblob));
                       Assert.That(actual.CLongblob, Is.EqualTo(expected.CLongblob));
                   }
                   """
        },
        [KnownTestType.MySqlCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     [TestCase(100, true, false, 0x05, -13, 324, -98760, 987965, 3132423, -7785442, 3.4f, -31.555666, 11.098643, 34.4424, 423.2445, 998.9994542, 21.214312452534, "D", "\u4321", "\u2345", "Parasite", "Clockwork Orange", "Dr. Strangelove", "Interview with a Vampire", "Memento", 1993, "2000-1-30", "1983-11-3 02:01:22", "2010-1-30 08:11:00", new byte[] { 0x15, 0x16, 0x17 }, new byte[] { 0x15, 0x20 }, new byte[] { 0x23 }, new byte[] { 0x33, 0x13 }, new byte[] { 0x11, 0x62, 0x10 }, new byte[] { 0x38, 0x45, 0x06, 0x04 })]
                     [TestCase(500, false, true, 0x12, 8, -555, 66979, -423425, -9798642, 3297398, 1.23f, 99.35542, 32.33345, -12.3456, -55.55556, -11.1123334, 33.423542356346, "3", "\u1234", "\u6543", "Splendor in the Grass", "Pulp Fiction", "Chinatown", "Repulsion", "Million Dollar Baby", 2025, "2012-9-20", "2012-1-20 22:12:34", "1984-6-5 20:12:12", new byte[] { 0x0, 0x0, 0x0 }, new byte[] { }, new byte[] { }, new byte[] { }, new byte[] { }, new byte[] { })]
                     [TestCase(10, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, "1970-1-1 00:00:01", null, null, null, null, null, null)]
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
                        string{{UnknownNullableIndicatorPlaceholder}} cChar,
                        string{{UnknownNullableIndicatorPlaceholder}} cNchar,
                        string{{UnknownNullableIndicatorPlaceholder}} cNationalChar,
                        string{{UnknownNullableIndicatorPlaceholder}} cVarchar, 
                        string{{UnknownNullableIndicatorPlaceholder}} cTinytext, 
                        string{{UnknownNullableIndicatorPlaceholder}} cMediumtext, 
                        string{{UnknownNullableIndicatorPlaceholder}} cText, 
                        string{{UnknownNullableIndicatorPlaceholder}} cLongtext,
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
                         AssertSingularEquals(expected, actual{{UnknownRecordValuePlaceholder}});
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
        },
        [KnownTestType.SqliteDataTypes] = new TestImpl
        {
            Impl = $$"""
                   [Test]
                   [TestCase(-54355, 9787.66, "Songs of Love and Hate", new byte[] { 0x15, 0x20, 0x33 })]
                   [TestCase(null, null, null, new byte[] { })]
                   [TestCase(null, null, null, null)]
                   public async Task TestSqliteTypes(
                        int cInteger,
                        decimal cReal,
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
                       AssertSingularEquals(expected, actual{{UnknownRecordValuePlaceholder}});
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