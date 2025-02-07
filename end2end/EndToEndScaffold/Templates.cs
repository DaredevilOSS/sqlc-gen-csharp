using System.Collections.Generic;

namespace EndToEndScaffold;

public readonly record struct TestImpl(string Impl);

public static class Templates
{
    public const string UnknownRecordValuePlaceholder = "#Unknown#";

    private const string CreateBojackAuthor = """
        await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs 
        {
            Name = DataGenerator.BojackAuthor,
            Bio = DataGenerator.BojackTheme
        });
        """;

    private const string CreateBojackAuthorWithId = """
        var bojackId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs 
        {
            Name = DataGenerator.BojackAuthor,
            Bio = DataGenerator.BojackTheme
        });
        """;

    private const string CreateBookByBojack = """
        await QuerySql.CreateBook(new QuerySql.CreateBookArgs
        {
            Name = DataGenerator.BojackBookTitle,
            AuthorId = bojackId
        });
        """;

    private const string CreateDrSeussAuthor = """
        await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs 
        {
            Name = DataGenerator.DrSeussAuthor,
            Bio = DataGenerator.DrSeussQuote
        });
        """;

    private const string CreateDrSeussAuthorWithId = """
        var drSeussId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = DataGenerator.DrSeussAuthor,
            Bio = DataGenerator.DrSeussQuote
        });
        """;

    private const string CreateBookByDrSeuss = """
        await QuerySql.CreateBook(new QuerySql.CreateBookArgs
        {
            Name = DataGenerator.DrSeussBookTitle,
            AuthorId = drSeussId
        });
        """;

    private const string CreateGenericAuthor = """
        await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.GenericAuthor,
            Bio = DataGenerator.GenericQuote1
        });
        """;

    private const string CreateGenericAuthorWithId = """
        var genericId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = DataGenerator.GenericAuthor,
            Bio = DataGenerator.GenericQuote1
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
                            Name = DataGenerator.BojackAuthor,
                            Bio = DataGenerator.BojackTheme
                        };
                        var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs
                        {
                            Name = DataGenerator.BojackAuthor
                        });
                        Assert.That(SingularEquals(expected, actual{{UnknownRecordValuePlaceholder}}));
                    }

                    private static bool SingularEquals(QuerySql.GetAuthorRow x, QuerySql.GetAuthorRow y)
                    {
                        return x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
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
                            new QuerySql.ListAuthorsRow { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme },
                            new QuerySql.ListAuthorsRow { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote }
                        };
                        var actual = await this.QuerySql.ListAuthors();
                        Assert.That(SequenceEquals(expected, actual));
                    }

                    private static bool SingularEquals(QuerySql.ListAuthorsRow x, QuerySql.ListAuthorsRow y)
                    {
                        return x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
                    }

                    private static bool SequenceEquals(List<QuerySql.ListAuthorsRow> x, List<QuerySql.ListAuthorsRow> y)
                    {
                        if (x.Count != y.Count) return false;
                        x = x.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Name + o.Bio).ToList();
                        y = y.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Name + o.Bio).ToList();
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
                            Name = DataGenerator.BojackAuthor 
                        });
                        var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs 
                        {
                            Name = DataGenerator.BojackAuthor
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
                        {{CreateGenericAuthor}}
                        {{CreateGenericAuthor}}
                        var affectedRows = await this.QuerySql.UpdateAuthors(new QuerySql.UpdateAuthorsArgs
                        {
                            Bio = DataGenerator.GenericQuote2
                        });
                        ClassicAssert.AreEqual(2, affectedRows);
                        var expected = new List<QuerySql.ListAuthorsRow>
                        {
                            new QuerySql.ListAuthorsRow { Name = DataGenerator.GenericAuthor, Bio = DataGenerator.GenericQuote2 },
                            new QuerySql.ListAuthorsRow { Name = DataGenerator.GenericAuthor, Bio = DataGenerator.GenericQuote2 }
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
                        {{CreateGenericAuthorWithId}}
                        var expected = new QuerySql.GetAuthorByIdRow 
                        {
                            Id = genericId,
                            Name = DataGenerator.GenericAuthor,
                            Bio = DataGenerator.GenericQuote1
                        };
                        var actual = await QuerySql.GetAuthorById(new QuerySql.GetAuthorByIdArgs 
                        {
                            Id = genericId
                        });
                        Assert.That(SingularEquals(expected, actual{{UnknownRecordValuePlaceholder}}));
                    }

                    private static bool SingularEquals(QuerySql.GetAuthorByIdRow x, QuerySql.GetAuthorByIdRow y)
                    {
                        return x.Id.Equals(y.Id) && x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
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
                                Author = new Author { Id = bojackId, Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme },
                                Book = new Book { AuthorId = bojackId, Name = DataGenerator.BojackBookTitle }
                            },
                            new QuerySql.ListAllAuthorsBooksRow
                            {
                                Author = new Author { Id = drSeussId, Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote },
                                Book = new Book { AuthorId = drSeussId, Name = DataGenerator.DrSeussBookTitle }
                            }
                        };
                        var actual = await QuerySql.ListAllAuthorsBooks();
                        Assert.That(SequenceEquals(expected, actual));
                    }

                    private static bool SingularEquals(QuerySql.ListAllAuthorsBooksRow x, QuerySql.ListAllAuthorsBooksRow y)
                    {
                        return x.Author.Id.Equals(y.Author.Id) &&
                            x.Author.Name.Equals(y.Author.Name) && 
                            x.Author.Bio.Equals(y.Author.Bio) && 
                            x.Book.AuthorId.Equals(y.Book.AuthorId) &&
                            x.Book.Name.Equals(y.Book.Name);
                    }

                    private static bool SequenceEquals(List<QuerySql.ListAllAuthorsBooksRow> x, List<QuerySql.ListAllAuthorsBooksRow> y)
                    {
                        if (x.Count != y.Count)
                            return false;
                        x = x.OrderBy<QuerySql.ListAllAuthorsBooksRow, object>(o => o.Author.Name + o.Book.Name).ToList();
                        y = y.OrderBy<QuerySql.ListAllAuthorsBooksRow, object>(o => o.Author.Name + o.Book.Name).ToList();
                        return !x.Where((t, i) => !SingularEquals(t, y[i])).Any();
                    }
                    """
        },
        [KnownTestType.SelfJoinEmbed] = new TestImpl
        {
            Impl = $$"""
                    [Test]
                    public async Task TestSelfJoinEmbed()
                    {
                        {{CreateBojackAuthor}}
                        {{CreateBojackAuthor}}
                        var expected = new List<QuerySql.GetDuplicateAuthorsRow>()
                        {
                            new QuerySql.GetDuplicateAuthorsRow
                            {
                                Author = new Author { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme },
                                Author2 = new Author { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme }
                            }
                        };
                        var actual = await QuerySql.GetDuplicateAuthors();
                        Assert.That(SequenceEquals(expected, actual));
                        Assert.That(actual[0].Author.Id != actual[0].Author2.Id);
                    }

                    private static bool SingularEquals(QuerySql.GetDuplicateAuthorsRow x, QuerySql.GetDuplicateAuthorsRow y)
                    {
                        return x.Author.Name.Equals(y.Author.Name) && 
                            x.Author.Bio.Equals(y.Author.Bio) &&
                            x.Author2.Name.Equals(y.Author2.Name) && 
                            x.Author2.Bio.Equals(y.Author2.Bio);
                    }

                    private static bool SequenceEquals(List<QuerySql.GetDuplicateAuthorsRow> x, List<QuerySql.GetDuplicateAuthorsRow> y)
                    {
                        if (x.Count != y.Count)
                            return false;
                        x = x.OrderBy<QuerySql.GetDuplicateAuthorsRow, object>(o => o.Author.Name + o.Author2.Name).ToList();
                        y = y.OrderBy<QuerySql.GetDuplicateAuthorsRow, object>(o => o.Author.Name + o.Author2.Name).ToList();
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
                                Name = DataGenerator.BojackBookTitle,
                                Bio = DataGenerator.DrSeussBookTitle,
                                Book = new Book { Name = DataGenerator.DrSeussBookTitle }
                            }
                        };
                        var actual = await QuerySql.GetAuthorsByBookName(new QuerySql.GetAuthorsByBookNameArgs 
                        { 
                            Name = DataGenerator.BojackBookTitle 
                        });
                        Assert.That(SequenceEquals(expected, actual));
                    }

                    private static bool SingularEquals(QuerySql.GetAuthorsByBookNameRow x, QuerySql.GetAuthorsByBookNameRow y)
                    {
                        return x.Id.Equals(y.Id) && 
                            x.Name.Equals(y.Name) && 
                            x.Bio.Equals(y.Bio) && 
                            x.Book.AuthorId.Equals(y.Book.AuthorId) &&
                            x.Book.Name.Equals(y.Book.Name);
                    }

                    private static bool SequenceEquals(List<QuerySql.GetAuthorsByBookNameRow> x, List<QuerySql.GetAuthorsByBookNameRow> y)
                    {
                        if (x.Count != y.Count)
                            return false;
                        x = x.OrderBy<QuerySql.GetAuthorsByBookNameRow, object>(o => o.Name + o.Book.Name).ToList();
                        y = y.OrderBy<QuerySql.GetAuthorsByBookNameRow, object>(o => o.Name + o.Book.Name).ToList();
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
                        {{CreateGenericAuthorWithId}}
                        {{CreateBojackAuthorWithId}}
                        var actual = await QuerySql.GetAuthorsByIds(new QuerySql.GetAuthorsByIdsArgs 
                        { 
                            Ids = new[] { genericId, bojackId } 
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
                        {{CreateGenericAuthorWithId}}
                        {{CreateBojackAuthorWithId}}
                        var actual = await QuerySql.GetAuthorsByIds(new QuerySql.GetAuthorsByIdsArgs 
                        { 
                            LongArr1 = new[] { genericId, bojackId } 
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
                        {{CreateGenericAuthorWithId}}
                        {{CreateBojackAuthorWithId}}
                        var actual = await QuerySql.GetAuthorsByIdsAndNames(new QuerySql.GetAuthorsByIdsAndNamesArgs 
                        { 
                            Ids = new[] { genericId, bojackId }, 
                            Names = new[] { DataGenerator.GenericAuthor } 
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
                        {{CreateGenericAuthorWithId}}
                        {{CreateGenericAuthor}}
                        {{CreateBojackAuthorWithId}}

                        var actual = await QuerySql.GetAuthorsByIdsAndNames(new QuerySql.GetAuthorsByIdsAndNamesArgs 
                        { 
                            LongArr1 = new[] { genericId, bojackId }, 
                            StringArr2 = new[] { DataGenerator.GenericAuthor } 
                        });
                        ClassicAssert.AreEqual(1, actual.Count);
                    }
                    """
        },
        [KnownTestType.PostgresCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestCopyFrom()
                     {
                         const int batchSize = 100;
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.CopyToTestsArgs
                             {
                                 CInt = 1,
                                 CVarchar = "abc",
                                 CDate = new DateTime(2020, 7, 22, 11, 7, 45, 35),
                                 CTimestamp = new DateTime(2020, 7, 22, 11, 7, 45, 35)
                             })
                             .ToList();
                         await QuerySql.CopyToTests(batchArgs);
                         var expected = new QuerySql.GetCopyStatsRow
                         {
                             Cnt = batchSize,
                             CInt = 1,
                             CVarchar = "abc",
                             CDate = new DateTime(2020, 7, 22),
                             CTimestamp = new DateTime(2020, 7, 22, 11, 7, 45, 35)
                         };
                         var actual = await QuerySql.GetCopyStats();
                         Assert.That(SingularEquals(expected, actual{{UnknownRecordValuePlaceholder}}));
                     }
                     
                     private static bool SingularEquals(QuerySql.GetCopyStatsRow x, QuerySql.GetCopyStatsRow y)
                     {
                         return x.Cnt.Equals(y.Cnt) &&
                            x.CInt.Equals(y.CInt) &&
                            x.CVarchar.Equals(y.CVarchar) &&
                            x.CDate.Equals(y.CDate) &&
                            x.CTimestamp.Equals(y.CTimestamp);
                     }
                     """
        },
        [KnownTestType.MySqlCopyFrom] = new TestImpl
        {
            Impl = $$"""
                     [Test]
                     public async Task TestCopyFrom()
                     {
                         const int batchSize = 100;
                         var batchArgs = Enumerable.Range(0, batchSize)
                             .Select(_ => new QuerySql.CopyToTestsArgs
                             {
                                 CInt = 1,
                                 CVarchar = "abc",
                                 CDate = new DateTime(2020, 7, 22, 11, 7, 45, 35),
                                 CTimestamp = new DateTime(2020, 7, 22, 11, 7, 45, 35)
                             })
                             .ToList();
                         await QuerySql.CopyToTests(batchArgs);
                         var expected = new QuerySql.GetCopyStatsRow
                         {
                             Cnt = batchSize
                         };
                         var actual = await QuerySql.GetCopyStats();
                         Assert.That(SingularEquals(expected, actual{{UnknownRecordValuePlaceholder}}));
                     }
                     
                     private static bool SingularEquals(QuerySql.GetCopyStatsRow x, QuerySql.GetCopyStatsRow y)
                     {
                         return x.Cnt.Equals(y.Cnt);
                     }
                     """
        },
        [KnownTestType.PostgresDataTypes] = new TestImpl
        {
            Impl = $$"""
                   [Test]
                   public async Task TestPostgresTypes()
                   {
                       var insertedId = await QuerySql.InsertPostgresTypes(new QuerySql.InsertPostgresTypesArgs
                       {
                           CBigint = 1,
                           CReal = 1.0f,
                           CNumeric = 1,
                           CSerial = 1,
                           CSmallint = 1,
                           CDecimal = 1,
                           CDate = DateTime.Now,
                           CTimestamp = DateTime.Now,
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
                           CSerial = 1,
                           CNumeric = 1,
                           CDecimal = 1,
                           CSmallint = 1,
                           CBoolean = true,
                           CChar = "a",
                           CInteger = 1,
                           CText = "ab",
                           CVarchar = "abc",
                           CCharacterVarying = "abcd",
                           CTextArray = new string[] { "a", "b" },
                           CIntegerArray = new int[] { 1, 2 }
                       };
                       var actual = await QuerySql.GetPostgresTypes(new QuerySql.GetPostgresTypesArgs
                       {
                           Id = insertedId
                       });
                       Assert.That(SingularEquals(expected, actual{{UnknownRecordValuePlaceholder}}));
                   }
                   
                   private static bool SingularEquals(QuerySql.GetPostgresTypesRow x, QuerySql.GetPostgresTypesRow y)
                   {
                       return x.CSmallint.Equals(y.CSmallint) &&
                           x.CBoolean.Equals(y.CBoolean) &&
                           x.CInteger.Equals(y.CInteger) &&
                           x.CBigint.Equals(y.CBigint) &&
                           x.CSerial.Equals(y.CSerial) &&
                           x.CDecimal.Equals(y.CDecimal) &&
                           x.CNumeric.Equals(y.CNumeric) &&
                           x.CReal.Equals(y.CReal) &&
                           x.CChar.Equals(y.CChar) &&
                           x.CVarchar.Equals(y.CVarchar) &&
                           x.CCharacterVarying.Equals(y.CCharacterVarying) &&
                           x.CText.Equals(y.CText) &&
                           x.CTextArray.SequenceEqual(y.CTextArray) &&
                           x.CIntegerArray.SequenceEqual(y.CIntegerArray);
                   }
                   """
        }
    };
}