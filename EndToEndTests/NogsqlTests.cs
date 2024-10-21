using NpgsqlExample;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public class NogsqlTests
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
    public async Task TestBasicFlow()
    {
        var bojackCreateAuthorArgs = new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.BojackAuthor,
            Bio = DataGenerator.BojackTheme
        };
        var createdBojackAuthor = await QuerySql.CreateAuthor(bojackCreateAuthorArgs);
        Assert.That(createdBojackAuthor is
        {
            Name: DataGenerator.BojackAuthor,
            Bio: DataGenerator.BojackTheme
        });
        var bojackInsertedId = GetId(createdBojackAuthor);

        var getAuthorArgs = new QuerySql.GetAuthorArgs
        {
            Id = bojackInsertedId
        };
        var singleAuthor = await QuerySql.GetAuthor(getAuthorArgs);
        Assert.That(singleAuthor is
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
        var authors = await QuerySql.ListAuthors();
        ClassicAssert.AreEqual(2, authors.Count);
        Assert.That(authors[0] is
        {
            Name: DataGenerator.BojackAuthor,
            Bio: DataGenerator.BojackTheme
        });
        Assert.That(authors[1] is
        {
            Name: DataGenerator.DrSeussAuthor,
            Bio: DataGenerator.DrSeussQuote
        });

        var deleteAuthorArgs = new QuerySql.DeleteAuthorArgs
        {
            Id = bojackInsertedId
        };
        await QuerySql.DeleteAuthor(deleteAuthorArgs);
        var authorRows = await QuerySql.ListAuthors();
        Assert.That(authorRows[0] is
        {
            Name: DataGenerator.DrSeussAuthor,
            Bio: DataGenerator.DrSeussQuote
        });
        ClassicAssert.AreEqual(1, authorRows.Count);
        return;

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

    [Test]
    public async Task TestCopyFlow()
    {
        const int batchSize = 100;
        var beforeCountRows = QuerySql.ListAuthors().Result.Count;
        var createAuthorBatchArgs = Enumerable.Range(0, batchSize)
            .Select(_ => GenerateRandom())
            .ToList();
        await QuerySql.CreateAuthorBatch(createAuthorBatchArgs);
        var afterCountRows = QuerySql.ListAuthors().Result.Count;
        ClassicAssert.AreEqual(beforeCountRows + batchSize, afterCountRows);

        QuerySql.CreateAuthorBatchArgs GenerateRandom()
        {
            return new QuerySql.CreateAuthorBatchArgs
            {
                Name = $"Author-{Randomizer.Next()}",
                Bio = $"Bio-{Randomizer.Next()}"
            };
        }
    }
}