using NUnit.Framework;

namespace SqlcGenCsharpTests;

[TestFixture]
public class FlowTests()
{
    private MySqlConnectorExample.QuerySql MysqlQuerySql { get; } = new("server=127.0.0.1;database=tests;user=root");
    private NpgsqlExample.QuerySql PostgresQuerySql { get; } = new("server=postgres-db;database=tests;user=root");

    [Test]
    public async Task TestFlow()
    {
        var examples = (Mysql: (object)MysqlQuerySql, Postgres: (object)PostgresQuerySql);
        foreach (var query in new[] { examples.Mysql, examples.Postgres })
        {
            switch (query)
            {
                case MySqlConnectorExample.QuerySql:
                    await TestFlowOnMySql(MysqlQuerySql);
                    break;
                // case NpgsqlExample.QuerySql postgres:
                //     await TestFlowOnMyPostgres(new NpgsqlExample.QuerySql(PostgresQuerySql));
                //     break;
            }
        }
    }
    
    private static async Task TestFlowOnMySql(MySqlConnectorExample.QuerySql querySql)
    {
        // test CreateAuthorReturnId works
        var insertedId = await querySql.CreateAuthorReturnId(new MySqlConnectorExample.QuerySql.CreateAuthorReturnIdArgs
        {
            Name = "Bojack Horseman",
            Bio = "Back in the 90s he was in a very famous TV show"
        });
        Assert.That(insertedId == 1);

        // test GetAuthor works
        var singleAuthor = await querySql.GetAuthor(new MySqlConnectorExample.QuerySql.GetAuthorArgs(insertedId));
        Assert.That(singleAuthor is { Name: "Bojack Horseman" });

        // test CreateAuthor works
        await querySql.CreateAuthor(new MySqlConnectorExample.QuerySql.CreateAuthorArgs
        {
            Name = "Dr. Seuss",
            Bio = "You'll miss the best things if you keep your eyes shut"
        });

        // test ListAuthors works
        var authors = await querySql.ListAuthors();
        Assert.That(authors.SequenceEqual(
            new List<MySqlConnectorExample.QuerySql.ListAuthorsRow>
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
        await querySql.DeleteAuthor(new MySqlConnectorExample.QuerySql.DeleteAuthorArgs(insertedId));
        var authorRows = await querySql.ListAuthors();
        Assert.That(authorRows.SequenceEqual(
            new List<MySqlConnectorExample.QuerySql.ListAuthorsRow>
            {
                new()
                {
                    Id = insertedId + 1,
                    Name = "Dr. Seuss",
                    Bio = "You'll miss the best things if you keep your eyes shut"
                }
            }));
    }
    
    // private static async Task TestFlowOnMyPostgres(NpgsqlExample.QuerySql querySql)
    // {
    //     // test CreateAuthorReturnId works
    //     var insertedId = await querySql.CreateAuthor(new NpgsqlExample.QuerySql.CreateAuthorArgs
    //     {
    //         Name = "Bojack Horseman",
    //         Bio = "Back in the 90s he was in a very famous TV show"
    //     });
    //     Assert.That(insertedId is { Name: "Bojack Horseman" });
    //
    //     // test GetAuthor works
    //     var singleAuthor = await querySql.GetAuthor(new MySqlConnectorExample.QuerySql.GetAuthorArgs(insertedId));
    //     Assert.That(singleAuthor is { Name: "Bojack Horseman" });
    //
    //     // test CreateAuthor works
    //     await querySql.CreateAuthor(new MySqlConnectorExample.QuerySql.CreateAuthorArgs
    //     {
    //         Name = "Dr. Seuss",
    //         Bio = "You'll miss the best things if you keep your eyes shut"
    //     });
    //
    //     // test ListAuthors works
    //     var authors = await querySql.ListAuthors();
    //     Assert.That(authors.SequenceEqual(
    //         new List<MySqlConnectorExample.QuerySql.ListAuthorsRow>
    //         {
    //             new()
    //             {
    //                 Id = insertedId,
    //                 Name = "Bojack Horseman",
    //                 Bio = "Back in the 90s he was in a very famous TV show"
    //             },
    //             new()
    //             {
    //                 Id = insertedId + 1,
    //                 Name = "Dr. Seuss",
    //                 Bio = "You'll miss the best things if you keep your eyes shut"
    //             }
    //         }));
    //
    //     // test DeleteAuthor works
    //     await querySql.DeleteAuthor(new MySqlConnectorExample.QuerySql.DeleteAuthorArgs(insertedId));
    //     var authorRows = await querySql.ListAuthors();
    //     Assert.That(authorRows.SequenceEqual(
    //         new List<MySqlConnectorExample.QuerySql.ListAuthorsRow>
    //         {
    //             new()
    //             {
    //                 Id = insertedId + 1,
    //                 Name = "Dr. Seuss",
    //                 Bio = "You'll miss the best things if you keep your eyes shut"
    //             }
    //         }));
    // }
}