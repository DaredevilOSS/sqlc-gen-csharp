using NpgsqlExample;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public class NpgsqlTester : ISqlDriverTester
{
    private static string ConnectionStringEnv => "POSTGRES_CONNECTION_STRING";

    private QuerySql QuerySql { get; } =
        new(Environment.GetEnvironmentVariable(ConnectionStringEnv)!);

    public async Task<long> CreateFirstAuthorAndTest()
    {
        var bojackCreateAuthorArgs = new QuerySql.CreateAuthorArgs
        {
            Name = Consts.BojackAuthor,
            Bio = Consts.BojackTheme
        };
        var createdBojackAuthor = await QuerySql.CreateAuthor(bojackCreateAuthorArgs);
        Assert.That(createdBojackAuthor is
        {
            Name: Consts.BojackAuthor,
            Bio: Consts.BojackTheme
        });
        var bojackInsertedId = GetId(createdBojackAuthor);

        var getAuthorArgs = new QuerySql.GetAuthorArgs
        {
            Id = bojackInsertedId
        };
        var singleAuthor = await QuerySql.GetAuthor(getAuthorArgs);
        Assert.That(singleAuthor is
        {
            Name: Consts.BojackAuthor,
            Bio: Consts.BojackTheme
        });
        return bojackInsertedId;

        long GetId(QuerySql.CreateAuthorRow? createdAuthorRow)
        {
            var type = typeof(QuerySql.CreateAuthorRow);
            var valueProperty = type.GetProperty("Value");
            var idProperty = type.GetProperty("Id");

            if (valueProperty == null)
                return (long)(idProperty?.GetValue(createdAuthorRow) ?? throw new InvalidOperationException());
            var value = valueProperty.GetValue(createdAuthorRow);
            return (long)(idProperty?.GetValue(value) ?? throw new InvalidOperationException());
        }
    }

    public async Task CreateSecondAuthorAndTest()
    {
        var createAuthorArgs = new QuerySql.CreateAuthorArgs
        {
            Name = Consts.DrSeussAuthor,
            Bio = Consts.DrSeussQuote
        };
        await QuerySql.CreateAuthor(createAuthorArgs);
        var authors = await QuerySql.ListAuthors();
        Assert.That(authors[0] is
        {
            Name: Consts.BojackAuthor,
            Bio: Consts.BojackTheme
        });
        Assert.That(authors[1] is
        {
            Name: Consts.DrSeussAuthor,
            Bio: Consts.DrSeussQuote
        });
        ClassicAssert.AreEqual(2, authors.Count);
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