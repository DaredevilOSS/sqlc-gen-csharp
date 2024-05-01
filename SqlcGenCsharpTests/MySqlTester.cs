using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnectorExample;
using NUnit.Framework;

namespace SqlcGenCsharpTests;

[TestFixture]
public class MySqlTester : IDriverTester
{
    private static string ConnectionStringEnv => "MYSQL_CONNECTION_STRING";

    private QuerySql MysqlQuerySql { get; } =
        new(Environment.GetEnvironmentVariable(ConnectionStringEnv)!);

    [Test]
    public async Task TestFlowOnDriver()
    {
        await TestFlowOnMySql(MysqlQuerySql);
    }

    private static async Task TestFlowOnMySql(QuerySql querySql)
    {
        // test CreateAuthorReturnId works
        var insertedId = await querySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = "Bojack Horseman",
            Bio = "Back in the 90s he was in a very famous TV show"
        });

        // test GetAuthor works
        var singleAuthor = await querySql.GetAuthor(new QuerySql.GetAuthorArgs(insertedId));
        Assert.That(singleAuthor is { Name: "Bojack Horseman" });

        // test UpdateAuthor works
        await querySql.UpdateAuthor(new QuerySql.UpdateAuthorArgs
        {
            Bio = ""
        });

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