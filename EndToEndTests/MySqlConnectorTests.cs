using MySqlConnectorExample;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public class MySqlConnectorTests
{
    private static string ConnectionStringEnv => "MYSQL_CONNECTION_STRING";

    private QuerySql QuerySql { get; } = new(Environment.GetEnvironmentVariable(ConnectionStringEnv)!);

    [Test]
    public async Task TestBasicFlow()
    {
        var createAuthorReturnIdArgs = new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = DataGenerator.BojackAuthor,
            Bio = DataGenerator.BojackTheme
        };
        var insertedId = await QuerySql.CreateAuthorReturnId(createAuthorReturnIdArgs);
        var getBojackAuthorArgs = new QuerySql.GetAuthorArgs { Id = insertedId };
        var bojackAuthor = await QuerySql.GetAuthor(getBojackAuthorArgs);
        Assert.That(bojackAuthor is
        {
            Name: DataGenerator.BojackAuthor,
            Bio: DataGenerator.BojackTheme
        });

        var createAuthorArgs = new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.DrSeussAuthor,
            Bio = DataGenerator.DrSeussQuote
        };
        await QuerySql.CreateAuthor(createAuthorArgs);
        var actualAuthors = await QuerySql.ListAuthors();
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
            Id = insertedId
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