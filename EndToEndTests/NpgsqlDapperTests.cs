using NpgsqlDapperExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public class NpgsqlDapperTests
{
    private static readonly Random Randomizer = new();

    private static string ConnectionStringEnv => "POSTGRES_CONNECTION_STRING";

    private QuerySql QuerySql { get; } = new(Environment.GetEnvironmentVariable(ConnectionStringEnv)!);

    [TearDown]
    public async Task EmptyTestsTable()
    {
        await QuerySql.TruncateAuthors();
    }

    [Test]
    public async Task TestCreateAndListAuthors()
    {
        await QuerySql.CreateAuthor(
            new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.BojackAuthor,
                Bio = DataGenerator.BojackTheme
            }
        );
        await QuerySql.CreateAuthor(
            new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.DrSeussAuthor,
                Bio = DataGenerator.DrSeussQuote
            }
        );

        var actualAuthors = await QuerySql.ListAuthors();
        ClassicAssert.AreEqual(2, actualAuthors.Count);
        Assert.That(
            actualAuthors
                is [
                { Name: DataGenerator.BojackAuthor, Bio: DataGenerator.BojackTheme },
                { Name: DataGenerator.DrSeussAuthor, Bio: DataGenerator.DrSeussQuote }
                ]
        );
    }

    [Test]
    public async Task TestGetAuthor()
    {
        await QuerySql.CreateAuthor(
            new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.BojackAuthor,
                Bio = DataGenerator.BojackTheme
            }
        );
        await QuerySql.CreateAuthor(
            new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.DrSeussAuthor,
                Bio = DataGenerator.DrSeussQuote
            }
        );

        var actualAuthor = await QuerySql.GetAuthor(
            new QuerySql.GetAuthorArgs { Name = DataGenerator.BojackAuthor }
        );
        ClassicAssert.IsNotNull(actualAuthor);
        Assert.That(
            actualAuthor is { Name: DataGenerator.BojackAuthor, Bio: DataGenerator.BojackTheme }
        );
    }

    [Test]
    public async Task TestDeleteAuthor()
    {
        await QuerySql.CreateAuthor(
            new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.BojackAuthor,
                Bio = DataGenerator.BojackTheme
            }
        );
        await QuerySql.CreateAuthor(
            new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.DrSeussAuthor,
                Bio = DataGenerator.DrSeussQuote
            }
        );

        await QuerySql.DeleteAuthor(
            new QuerySql.DeleteAuthorArgs { Name = DataGenerator.BojackAuthor }
        );
        var authorRows = await QuerySql.ListAuthors();
        ClassicAssert.AreEqual(1, authorRows.Count);
        Assert.That(
            authorRows[0] is { Name: DataGenerator.DrSeussAuthor, Bio: DataGenerator.DrSeussQuote }
        );
    }

    [Test]
    public async Task TestExecRowsFlow()
    {
        var bojackCreateAuthorArgs = new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.GenericAuthor,
            Bio = DataGenerator.GenericQuote1
        };
        await QuerySql.CreateAuthor(bojackCreateAuthorArgs);
        await QuerySql.CreateAuthor(bojackCreateAuthorArgs);

        var updateAuthorsArgs = new QuerySql.UpdateAuthorsArgs
        {
            Bio = DataGenerator.GenericQuote2
        };
        var affectedRows = await QuerySql.UpdateAuthors(updateAuthorsArgs);
        ClassicAssert.AreEqual(2, affectedRows);
    }
}