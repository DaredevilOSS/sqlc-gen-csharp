using NUnit.Framework;
using NUnit.Framework.Legacy;
using SqliteExample;
using System;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public class SqliteTests
{
    private static string ConnectionStringEnv => "SQLITE_CONNECTION_STRING";

    private QuerySql QuerySql { get; } = new(Environment.GetEnvironmentVariable(ConnectionStringEnv)!);

    [Test]
    public async Task TestBasicFlow()
    {
        var createAuthorArgs = new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.BojackAuthor,
            Bio = DataGenerator.BojackTheme
        };
        await QuerySql.CreateAuthor(createAuthorArgs);

        var actualAuthors = await QuerySql.ListAuthors();
        Assert.That(actualAuthors[0] is
        {
            Id: 1,
            Name: DataGenerator.BojackAuthor,
            Bio: DataGenerator.BojackTheme
        });
        var insertedId = actualAuthors[0].Id;
        var getBojackAuthorArgs = new QuerySql.GetAuthorArgs { Id = insertedId };
        var bojackAuthor = await QuerySql.GetAuthor(getBojackAuthorArgs);
        Assert.That(bojackAuthor is
        {
            Name: DataGenerator.BojackAuthor,
            Bio: DataGenerator.BojackTheme
        });

        createAuthorArgs = new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.DrSeussAuthor,
            Bio = DataGenerator.DrSeussQuote
        };
        await QuerySql.CreateAuthor(createAuthorArgs);
        actualAuthors = await QuerySql.ListAuthors();
        Assert.That(actualAuthors[0] is
        {
            Name: DataGenerator.BojackAuthor,
            Bio: DataGenerator.BojackTheme
        });
        Assert.That(actualAuthors[1] is
        {
            Name: DataGenerator.DrSeussAuthor,
            Bio: DataGenerator.DrSeussQuote
        });
        ClassicAssert.AreEqual(2, actualAuthors.Count);

        var deleteAuthorArgs = new QuerySql.DeleteAuthorArgs
        {
            Id = Convert.ToInt32(insertedId)
        };
        await QuerySql.DeleteAuthor(deleteAuthorArgs);
        var authorRows = await QuerySql.ListAuthors();
        Assert.That(authorRows[0] is
        {
            Name: DataGenerator.DrSeussAuthor,
            Bio: DataGenerator.DrSeussQuote
        });
        ClassicAssert.AreEqual(1, authorRows.Count);
    }
}