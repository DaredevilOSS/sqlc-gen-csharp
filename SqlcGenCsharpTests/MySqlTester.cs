using System;
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
        // test CreateAuthorReturnId + GetAuthor works
        var insertedId = await querySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = Consts.BojackAuthor,
            Bio = Consts.BojackTheme
        });
        var getAuthorArgs = new QuerySql.GetAuthorArgs { Id = insertedId };
        var singleAuthor = await querySql.GetAuthor(getAuthorArgs);
        Assert.That(singleAuthor is { Name: Consts.BojackAuthor, Bio: Consts.BojackTheme });

        // test CreateAuthor + ListAuthors works
        await querySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = Consts.DrSeussAuthor,
            Bio = Consts.DrSeussQuote
        });

        var actualAuthors = await querySql.ListAuthors();
        actualAuthors.ForEach(a => Console.WriteLine(a.ToString()));
        Assert.That(
            actualAuthors[0] is
            {
                Name: Consts.BojackAuthor,
                Bio: Consts.BojackTheme
            }
            && actualAuthors[1] is
            {
                Name: Consts.DrSeussAuthor,
                Bio: Consts.DrSeussQuote
            }
            && actualAuthors.Count == 2);

        // test DeleteAuthor works
        var deleteAuthorArgs = new QuerySql.DeleteAuthorArgs
        {
            Id = insertedId
        };
        await querySql.DeleteAuthor(deleteAuthorArgs);
        var authorRows = await querySql.ListAuthors();
        Assert.That(
            authorRows[0] is
            {
                Name: Consts.DrSeussAuthor,
                Bio: Consts.DrSeussQuote
            }
            && actualAuthors.Count == 1);
    }
}