using System;
using System.Threading.Tasks;
using NpgsqlExample;
using NUnit.Framework;

namespace SqlcGenCsharpTests;

[TestFixture]
public class PostgresTester : IDriverTester
{
    private static string ConnectionStringEnv => "POSTGRES_CONNECTION_STRING";

    private QuerySql PostgresQuerySql { get; } =
        new(Environment.GetEnvironmentVariable(ConnectionStringEnv)!);

    [Test]
    public async Task TestFlowOnDriver()
    {
        await TestFlowOnPostgres(PostgresQuerySql);
    }

    private static async Task TestFlowOnPostgres(QuerySql querySql)
    {
        // test CreateAuthorReturnId works
        var createdBojackAuthor = await querySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = Consts.BojackAuthor,
            Bio = Consts.BojackTheme
        });
        Assert.That(createdBojackAuthor is { Name: Consts.BojackAuthor });

        // test GetAuthor works
        var singleAuthor = await querySql.GetAuthor(
            new QuerySql.GetAuthorArgs(createdBojackAuthor!.Value.Id));
        Assert.That(singleAuthor is { Name: Consts.BojackAuthor });

        // test ListAuthors works
        await querySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = Consts.DrSeussAuthor,
            Bio = Consts.DrSeussQuote
        });
        var authors = await querySql.ListAuthors();
        Assert.That(
            authors[0] is
            {
                Name: Consts.BojackAuthor,
                Bio: Consts.BojackTheme
            }
            && authors[1] is
            {
                Name: Consts.DrSeussAuthor,
                Bio: Consts.DrSeussQuote
            }
            && authors.Count == 2);

        // test DeleteAuthor works
        await querySql.DeleteAuthor(
            new QuerySql.DeleteAuthorArgs(createdBojackAuthor.Value.Id));
        var authorRows = await querySql.ListAuthors();
        Assert.That(
            authorRows[0] is
            {
                Name: Consts.DrSeussAuthor,
                Bio: Consts.DrSeussQuote
            }
            && authors.Count == 1);
    }
}