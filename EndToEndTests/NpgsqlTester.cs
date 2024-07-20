using NpgsqlExample;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public class NpgsqlTester : SqlDriverTester
{
    private static readonly Random Randomizer = new();

    private static string ConnectionStringEnv => "POSTGRES_CONNECTION_STRING";

    private QuerySql QuerySql { get; } =
        new(Environment.GetEnvironmentVariable(ConnectionStringEnv)!);

    protected override async Task<long> CreateFirstAuthorAndTest()
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

    protected override async Task CreateSecondAuthorAndTest()
    {
        var createAuthorArgs = new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.DrSeussAuthor,
            Bio = DataGenerator.DrSeussQuote
        };
        await QuerySql.CreateAuthor(createAuthorArgs);
        var authors = await QuerySql.ListAuthors();
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
        ClassicAssert.AreEqual(2, authors.Count);
    }

    protected override async Task DeleteFirstAuthorAndTest(long idToDelete)
    {
        var deleteAuthorArgs = new QuerySql.DeleteAuthorArgs
        {
            Id = idToDelete
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