using System.Collections.Generic;

namespace EndToEndScaffold;

public readonly record struct TestImpl(string Modern, string Legacy);

public static class Templates
{
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
    public static Dictionary<KnownTestType, TestImpl> TestImplementations { get; } =
        new()
        {
            [KnownTestType.One] = new TestImpl
            {
                Modern = $$"""
                    [Test]
                    public async Task TestOne()
                    {
                        {{CreateBojackAuthor}}
                        {{CreateDrSeussAuthor}}

                        var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs 
                        {
                            Name = DataGenerator.BojackAuthor
                        });
                        Assert.That(actual is 
                        {
                            Name: DataGenerator.BojackAuthor,
                            Bio: DataGenerator.BojackTheme
                        });
                    }
                    """,
                Legacy = $$"""
                    [Test]
                    public async Task TestOne()
                    {
                        {{CreateBojackAuthor}}
                        {{CreateDrSeussAuthor}}

                        var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs
                        {
                            Name = DataGenerator.BojackAuthor
                        });
                        var expected = new QuerySql.GetAuthorRow
                        {
                            Name = DataGenerator.BojackAuthor,
                            Bio = DataGenerator.BojackTheme
                        };
                        Assert.That(Equals(expected, actual));
                    }

                    private static bool Equals(QuerySql.GetAuthorRow x, QuerySql.GetAuthorRow y)
                    {
                        return x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
                    }
                    """
            },
            [KnownTestType.Many] = new TestImpl
            {
                Modern = $$"""
                    [Test]
                    public async Task TestMany()
                    {
                        {{CreateBojackAuthor}}
                        {{CreateDrSeussAuthor}}

                        var actual = await this.QuerySql.ListAuthors();
                        Assert.That(actual is 
                        [{
                            Name: DataGenerator.BojackAuthor,
                            Bio: DataGenerator.BojackTheme
                        },
                        {
                            Name: DataGenerator.DrSeussAuthor,
                            Bio: DataGenerator.DrSeussQuote
                        }]);
                    }
                    """,
                Legacy = $$"""
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

                    private static bool Equals(QuerySql.ListAuthorsRow x, QuerySql.ListAuthorsRow y)
                    {
                        return x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
                    }

                    private static bool SequenceEquals(List<QuerySql.ListAuthorsRow> x, List<QuerySql.ListAuthorsRow> y)
                    {
                        if (x.Count != y.Count) return false;
                        x = x.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Name + o.Bio).ToList();
                        y = y.OrderBy<QuerySql.ListAuthorsRow, object>(o => o.Name + o.Bio).ToList();
                        return !x.Where((t, i) => !Equals(t, y[i])).Any();
                    }
                    """
            },
            [KnownTestType.Exec] = new TestImpl
            {
                Modern = $$"""
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
                Legacy = $$"""
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
            // TODO implement this test better
            {
                Modern = $$"""
                    [Test]
                    public async Task TestExecRows()
                    {
                        {{CreateGenericAuthor}}
                        {{CreateGenericAuthor}}

                        var affectedRows = await this.QuerySql.UpdateAuthors(new QuerySql.UpdateAuthorsArgs
                        {
                            Bio = DataGenerator.BojackTheme
                        });
                        ClassicAssert.AreEqual(2, affectedRows);
                    }
                    """,
                Legacy = $$"""
                    [Test]
                    public async Task TestExecRows()
                    {
                        {{CreateGenericAuthor}}
                        {{CreateGenericAuthor}}

                        var affectedRows = await this.QuerySql.UpdateAuthors(new QuerySql.UpdateAuthorsArgs
                        {
                            Bio = DataGenerator.BojackTheme
                        });
                        ClassicAssert.AreEqual(2, affectedRows);
                    }
                    """,
            },
            [KnownTestType.ExecLastId] = new TestImpl
            {
                Modern = $$"""
                    [Test]
                    public async Task TestExecLastId()
                    {
                        {{CreateGenericAuthorWithId}}

                        var actual = await QuerySql.GetAuthorById(new QuerySql.GetAuthorByIdArgs 
                        {
                            Id = genericId
                        });
                        Assert.That(actual is 
                        {
                            Name: DataGenerator.GenericAuthor,
                            Bio: DataGenerator.GenericQuote1
                        });
                    }
                    """,
                Legacy = $$"""
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
                        Assert.That(Equals(expected, actual));
                    }

                    private static bool Equals(QuerySql.GetAuthorByIdRow x, QuerySql.GetAuthorByIdRow y)
                    {
                        return x.Id.Equals(y.Id) && x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio);
                    }
                    """
            },
            [KnownTestType.JoinEmbed] = new TestImpl
            {
                Modern = $$"""
                    [Test]
                    public async Task TestJoinEmbed()
                    {
                        {{CreateBojackAuthorWithId}}
                        {{CreateBookByBojack}}

                        {{CreateDrSeussAuthorWithId}}
                        {{CreateBookByDrSeuss}}

                        var actual = await QuerySql.ListAllAuthorsBooks();
                        Assert.That(actual is
                        [{
                            Author:
                            {
                                Name: DataGenerator.BojackAuthor,
                                Bio: DataGenerator.BojackTheme
                            },
                            Book.Name: DataGenerator.BojackBookTitle
                        },
                        {
                            Author:
                            {
                                Name: DataGenerator.DrSeussAuthor,
                                Bio: DataGenerator.DrSeussQuote
                            },
                            Book.Name: DataGenerator.DrSeussBookTitle
                        }]);
                    }
                    """,
                Legacy = $$"""
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
                                Author = new Author { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme },
                                Book = new Book { Name = DataGenerator.BojackBookTitle }
                            },
                            new QuerySql.ListAllAuthorsBooksRow
                            {
                                Author = new Author { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote },
                                Book = new Book { Name = DataGenerator.DrSeussBookTitle }
                            }
                        };
                        var actual = await QuerySql.ListAllAuthorsBooks();
                        Assert.That(SequenceEquals(expected, actual));
                    }

                    private static bool Equals(QuerySql.ListAllAuthorsBooksRow x, QuerySql.ListAllAuthorsBooksRow y)
                    {
                        return x.Author.Name.Equals(y.Author.Name) && x.Author.Bio.Equals(y.Author.Bio) && x.Book.Name.Equals(y.Book.Name);
                    }

                    private static bool SequenceEquals(List<QuerySql.ListAllAuthorsBooksRow> x, List<QuerySql.ListAllAuthorsBooksRow> y)
                    {
                        if (x.Count != y.Count)
                            return false;
                        x = x.OrderBy<QuerySql.ListAllAuthorsBooksRow, object>(o => o.Author.Name + o.Book.Name).ToList();
                        y = y.OrderBy<QuerySql.ListAllAuthorsBooksRow, object>(o => o.Author.Name + o.Book.Name).ToList();
                        return !x.Where((t, i) => !Equals(t, y[i])).Any();
                    }
                    """
            },
            [KnownTestType.SelfJoinEmbed] = new TestImpl
            {
                Modern = $$"""
                    [Test]
                    public async Task TestSelfJoinEmbed()
                    {
                        {{CreateBojackAuthor}}
                        {{CreateBojackAuthor}}

                        var actual = await QuerySql.GetDuplicateAuthors();
                        Assert.That(actual is
                        [{
                            Author: { Name: DataGenerator.BojackAuthor, Bio: DataGenerator.BojackTheme },
                            Author2: { Name: DataGenerator.BojackAuthor, Bio: DataGenerator.BojackTheme }
                        }]);
                        Assert.That(actual[0].Author.Id, Is.Not.EqualTo(actual[0].Author2.Id));
                    }
                    """,
                Legacy = $$"""
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
                        Assert.That(actual[0].Author.Id, Is.Not.EqualTo(actual[0].Author2.Id));
                    }

                    private static bool Equals(QuerySql.GetDuplicateAuthorsRow x, QuerySql.GetDuplicateAuthorsRow y)
                    {
                        return x.Author.Name.Equals(y.Author.Name) && x.Author.Bio.Equals(y.Author.Bio) &&
                            x.Author2.Name.Equals(y.Author2.Name) && x.Author2.Bio.Equals(y.Author2.Bio);
                    }

                    private static bool SequenceEquals(List<QuerySql.GetDuplicateAuthorsRow> x, List<QuerySql.GetDuplicateAuthorsRow> y)
                    {
                        if (x.Count != y.Count)
                            return false;
                        x = x.OrderBy<QuerySql.GetDuplicateAuthorsRow, object>(o => o.Author.Name + o.Author2.Name).ToList();
                        y = y.OrderBy<QuerySql.GetDuplicateAuthorsRow, object>(o => o.Author.Name + o.Author2.Name).ToList();
                        return !x.Where((t, i) => !Equals(t, y[i])).Any();
                    }
                    """
            },
            [KnownTestType.PartialEmbed] = new TestImpl
            {
                Modern = $$"""
                    [Test]
                    public async Task TestPartialEmbed()
                    {
                        {{CreateBojackAuthorWithId}}
                        {{CreateBookByBojack}}

                        {{CreateDrSeussAuthorWithId}}
                        {{CreateBookByDrSeuss}}

                        var actual = await QuerySql.GetAuthorsByBookName(new QuerySql.GetAuthorsByBookNameArgs 
                        { 
                            Name = DataGenerator.BojackBookTitle 
                        });
                        Assert.That(actual is 
                        [{
                            Name: DataGenerator.BojackAuthor,
                            Bio: DataGenerator.BojackTheme,
                            Book.Name: DataGenerator.BojackBookTitle
                        }]);
                    }
                    """,
                Legacy = $$"""
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

                    private static bool Equals(QuerySql.GetAuthorsByBookNameRow x, QuerySql.GetAuthorsByBookNameRow y)
                    {
                        return x.Name.Equals(y.Name) && x.Bio.Equals(y.Bio) && x.Book.Name.Equals(y.Book.Name);
                    }

                    private static bool SequenceEquals(List<QuerySql.GetAuthorsByBookNameRow> x, List<QuerySql.GetAuthorsByBookNameRow> y)
                    {
                        if (x.Count != y.Count)
                            return false;
                        x = x.OrderBy<QuerySql.GetAuthorsByBookNameRow, object>(o => o.Name + o.Book.Name).ToList();
                        y = y.OrderBy<QuerySql.GetAuthorsByBookNameRow, object>(o => o.Name + o.Book.Name).ToList();
                        return !x.Where((t, i) => !Equals(t, y[i])).Any();
                    }
                    """
            },
            [KnownTestType.Slice] = new TestImpl
            {
                Modern = $$"""
                    [Test]
                    public async Task TestSlice()
                    {
                        {{CreateGenericAuthorWithId}}
                        {{CreateBojackAuthorWithId}}

                        var actual = await QuerySql.GetAuthorsByIds(new QuerySql.GetAuthorsByIdsArgs 
                        { 
                            Ids = [genericId, bojackId] 
                        });
                        ClassicAssert.AreEqual(2, actual.Count);
                    }
                    """,
                Legacy = $$"""
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
            [KnownTestType.Array] = new TestImpl
            {
                Modern = $$"""
                    [Test]
                    public async Task TestArray()
                    {
                        {{CreateGenericAuthorWithId}}
                        {{CreateBojackAuthorWithId}}

                        var actual = await QuerySql.GetAuthorsByIds(new QuerySql.GetAuthorsByIdsArgs 
                        { 
                            LongArr1 = [genericId, bojackId] 
                        });
                        ClassicAssert.AreEqual(2, actual.Count);
                    }
                    """,
                Legacy = $$"""
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
                Modern = $$"""
                    [Test]
                    public async Task TestMultipleSlices()
                    {
                        {{CreateGenericAuthorWithId}}
                        {{CreateBojackAuthorWithId}}

                        var actual = await QuerySql.GetAuthorsByIdsAndNames(new QuerySql.GetAuthorsByIdsAndNamesArgs 
                        { 
                            Ids = [genericId, bojackId], 
                            Names = [DataGenerator.GenericAuthor] 
                        });
                        ClassicAssert.AreEqual(1, actual.Count);
                    }
                    """,
                Legacy = $$"""
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
            [KnownTestType.MultipleArrays] = new TestImpl
            {
                Modern = $$"""
                    [Test]
                    public async Task TestMultipleArrays()
                    {
                        {{CreateGenericAuthorWithId}}
                        {{CreateGenericAuthor}}
                        {{CreateBojackAuthorWithId}}

                        var actual = await QuerySql.GetAuthorsByIdsAndNames(new QuerySql.GetAuthorsByIdsAndNamesArgs 
                        { 
                            LongArr1 = [genericId, bojackId], 
                            StringArr2 = [DataGenerator.GenericAuthor] 
                        });
                        ClassicAssert.AreEqual(1, actual.Count);
                    }
                    """,
                Legacy = $$"""
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
            }
        };
}