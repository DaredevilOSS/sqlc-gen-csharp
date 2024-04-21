using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SqlcGenCsharpTests;

[TestFixture]
public class PostgresTester: IDriverTester
{
    private static string ConnectionStringEnv => "POSTGRES_CONNECTION_STRING";
    private NpgsqlExample.QuerySql PostgresQuerySql { get; } = 
        new(connectionString: Environment.GetEnvironmentVariable(ConnectionStringEnv)!);

    [Test]
    public async Task TestFlowOnDriver()
    {
        await TestFlowOnPostgres(PostgresQuerySql);
    }
    
    private static async Task TestFlowOnPostgres(NpgsqlExample.QuerySql querySql)
    {
        // test CreateAuthorReturnId works
        var createdBojackAuthor = await querySql.CreateAuthor(new NpgsqlExample.QuerySql.CreateAuthorArgs
        {
            Name = "Bojack Horseman",
            Bio = "Back in the 90s he was in a very famous TV show"
        });
        Assert.That(createdBojackAuthor is { Name: "Bojack Horseman" });
    
        // test GetAuthor works
        var singleAuthor = await querySql.GetAuthor(
            new NpgsqlExample.QuerySql.GetAuthorArgs(createdBojackAuthor!.Value.Id));
        Assert.That(singleAuthor is { Name: "Bojack Horseman" });
    
        // test ListAuthors works
        await querySql.CreateAuthor(new NpgsqlExample.QuerySql.CreateAuthorArgs
        {
            Name = "Dr. Seuss",
            Bio = "You'll miss the best things if you keep your eyes shut"
        });
        var authors = await querySql.ListAuthors();
        Assert.That(authors.SequenceEqual(
            new List<NpgsqlExample.QuerySql.ListAuthorsRow>
            {
                new()
                {
                    Id = createdBojackAuthor.Value.Id,
                    Name = createdBojackAuthor.Value.Name,
                    Bio = createdBojackAuthor.Value.Bio
                },
                new()
                {
                    Id = createdBojackAuthor.Value.Id + 1,
                    Name = "Dr. Seuss",
                    Bio = "You'll miss the best things if you keep your eyes shut"
                }
            }));
    
        // test DeleteAuthor works
        await querySql.DeleteAuthor(
            new NpgsqlExample.QuerySql.DeleteAuthorArgs(createdBojackAuthor.Value.Id));
        var authorRows = await querySql.ListAuthors();
        Assert.That(authorRows.SequenceEqual(
            new List<NpgsqlExample.QuerySql.ListAuthorsRow>
            {
                new()
                {
                    Id = createdBojackAuthor.Value.Id + 1,
                    Name = "Dr. Seuss",
                    Bio = "You'll miss the best things if you keep your eyes shut"
                }
            }));
    }
}