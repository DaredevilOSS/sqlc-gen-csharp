using MySqlConnectorExample.Db;
using NUnit.Framework;

namespace SqlcGenCsharpTests;

[TestFixture]
public class FlowTests
{
    [Test]
    public async Task EntireFlow()
    {
        const string connectionString = "server=127.0.0.1;database=tests;user=root";
        var querySql = new QuerySql(connectionString);

        // test CreateAuthorReturnId works
        var insertedId = await querySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = "Bojack Horseman",
            Bio = "Back in the 90s he was in a very famous TV show"
        });
        Assert.That(insertedId == 1);

        // test GetAuthor works
        var singleAuthor = await querySql.GetAuthor(new QuerySql.GetAuthorArgs(insertedId));
        Assert.That(singleAuthor is { Name: "Bojack Horseman" });

        // test CreateAuthor works
        await querySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = "Dr. Seuss",
            Bio = "You'll miss the best things if you keep your eyes shut"
        });

        // test ListAuthors works
        var authors = await querySql.ListAuthors();
        Assert.That(authors.SequenceEqual(
            new List<QuerySql.ListAuthorsRow>
            {
                new()
                {
                    Id = insertedId,
                    Name = "Bojack Horseman",
                    Bio = "Back in the 90s he was in a very famous TV show"
                },
                new()
                {
                    Id = insertedId + 1,
                    Name = "Dr. Seuss",
                    Bio = "You'll miss the best things if you keep your eyes shut"
                }
            }));

        // test DeleteAuthor works
        await querySql.DeleteAuthor(new QuerySql.DeleteAuthorArgs(insertedId));
        var authorRows = await querySql.ListAuthors();
        Assert.That(authorRows.SequenceEqual(
            new List<QuerySql.ListAuthorsRow>
            {
                new()
                {
                    Id = insertedId + 1,
                    Name = "Dr. Seuss",
                    Bio = "You'll miss the best things if you keep your eyes shut"
                }
            }));
    }
}