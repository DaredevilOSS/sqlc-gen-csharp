using MySqlConnectorExample.Db;
using NUnit.Framework;

namespace MySqlConnectorExample;

[TestFixture]
public class DbTests
{
    [Test]
    public async Task EntireFlow()
    {
        var connectionString = "server=127.0.0.1;database=tests;user=root";
        var querySql = new QuerySql(connectionString);
        await querySql.CreateAuthor(new QuerySql.CreateAuthorArgs(
            "Bojack Horseman", "Back in the 90s he was in a very famous TV show"));
        var authorRows = await querySql.ListAuthors();
        Assert.That(authorRows.Count == 1);
        Assert.That(authorRows[0] is { Name: "Bojack Horseman" });
    }
}