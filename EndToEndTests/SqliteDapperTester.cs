using NUnit.Framework;
using NUnit.Framework.Legacy;
using SqliteDapperExampleGen;
using System;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public class SqliteDapperTests
{
    private static string ConnectionStringEnv => "SQLITE_CONNECTION_STRING";

    private QuerySql QuerySql { get; } = new(Environment.GetEnvironmentVariable(ConnectionStringEnv)!);
    // private QuerySql QuerySql { get; } = new("Data Source=/Users/doroneli/Coding/sqlc-gen-csharp/test.db;");

    [TearDown]
    public async Task EmptyTestsTable()
    {
        await QuerySql.DeleteAllAuthors();
    }

    [Test]
    public async Task TestCreateAndListAuthors()
    {
        await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.BojackAuthor,
            Bio = DataGenerator.BojackTheme
        });
        await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.DrSeussAuthor,
            Bio = DataGenerator.DrSeussQuote
        });

        var actualAuthors = await QuerySql.ListAuthors();
        Assert.That(actualAuthors is [
        {
            Name: DataGenerator.BojackAuthor,
            Bio: DataGenerator.BojackTheme
        },
        {
            Name: DataGenerator.DrSeussAuthor,
            Bio: DataGenerator.DrSeussQuote
        }
        ]);
    }

    [Test]
    public async Task TestGetAuthor()
    {
        await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.BojackAuthor,
            Bio = DataGenerator.BojackTheme
        });
        await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.DrSeussAuthor,
            Bio = DataGenerator.DrSeussQuote
        });

        var actualAuthor = await QuerySql.GetAuthor(new QuerySql.GetAuthorArgs
        {
            Name = DataGenerator.BojackAuthor
        });
        Assert.That(actualAuthor is
        {
            Name: DataGenerator.BojackAuthor,
            Bio: DataGenerator.BojackTheme
        });
    }

    [Test]
    public async Task TestDeleteAuthor()
    {
        await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.BojackAuthor,
            Bio = DataGenerator.BojackTheme
        });
        await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.DrSeussAuthor,
            Bio = DataGenerator.DrSeussQuote
        });

        var deleteAuthorArgs = new QuerySql.DeleteAuthorArgs { Name = DataGenerator.BojackAuthor };
        await QuerySql.DeleteAuthor(deleteAuthorArgs);

        var authorRows = await QuerySql.ListAuthors();
        ClassicAssert.AreEqual(1, authorRows.Count);
        Assert.That(authorRows[0] is
        {
            Name: DataGenerator.DrSeussAuthor,
            Bio: DataGenerator.DrSeussQuote
        });
    }
}