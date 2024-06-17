using MySqlConnectorExample;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public class MySqlConnectorTester : ISqlDriverTester
{
    private static string ConnectionStringEnv => "MYSQL_CONNECTION_STRING";

    private QuerySql QuerySql { get; } =
        new(Environment.GetEnvironmentVariable(ConnectionStringEnv)!);

    public async Task<long> CreateFirstAuthorAndTest()
    {
        var createAuthorReturnIdArgs = new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = Consts.BojackAuthor,
            Bio = Consts.BojackTheme
        };
        var insertedId = await QuerySql.CreateAuthorReturnId(createAuthorReturnIdArgs);
        var getBojackAuthorArgs = new QuerySql.GetAuthorArgs { Id = insertedId };
        var bojackAuthor = await QuerySql.GetAuthor(getBojackAuthorArgs);
        Assert.That(bojackAuthor is
        {
            Name: Consts.BojackAuthor,
            Bio: Consts.BojackTheme
        });
        return insertedId;
    }

    public async Task CreateSecondAuthorAndTest()
    {
        var createAuthorArgs = new QuerySql.CreateAuthorArgs
        {
            Name = Consts.DrSeussAuthor,
            Bio = Consts.DrSeussQuote
        };
        await QuerySql.CreateAuthor(createAuthorArgs);
        var actualAuthors = await QuerySql.ListAuthors();
        Assert.That(actualAuthors[0] is
        {
            Name: Consts.BojackAuthor,
            Bio: Consts.BojackTheme
        });
        Assert.That(actualAuthors[1] is
        {
            Name: Consts.DrSeussAuthor,
            Bio: Consts.DrSeussQuote
        });
        ClassicAssert.AreEqual(2, actualAuthors.Count);
    }

    public async Task DeleteFirstAuthorAndTest(long idToDelete)
    {
        var deleteAuthorArgs = new QuerySql.DeleteAuthorArgs
        {
            Id = idToDelete
        };
        await QuerySql.DeleteAuthor(deleteAuthorArgs);
        var authorRows = await QuerySql.ListAuthors();
        Assert.That(authorRows[0] is
        {
            Name: Consts.DrSeussAuthor,
            Bio: Consts.DrSeussQuote
        });
        ClassicAssert.AreEqual(1, authorRows.Count);
    }
}