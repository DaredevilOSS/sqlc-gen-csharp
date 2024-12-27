using MySqlConnectorDapperExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public class MySqlConnectorDapperTester
{
    private QuerySql QuerySql { get; set; }

    [OneTimeSetUp]
    public void SetUp()
    {
        var connectionString = Environment.GetEnvironmentVariable(GlobalSetup.MySqlConnectionStringEnv);
        QuerySql = new QuerySql(connectionString!);
    }

    [TearDown]
    public async Task EmptyTestsTable()
    {
        await QuerySql.TruncateAuthors();
    }

    [Test]
    public async Task TestCreateAndListAuthors()
    {
        await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.BojackAuthor,
            Bio = DataGenerator.BojackTheme
        });
        await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.DrSeussAuthor,
            Bio = DataGenerator.DrSeussQuote
        });

        var actualAuthors = await QuerySql.ListAuthors();
        ClassicAssert.AreEqual(2, actualAuthors.Count);
        Assert.That(actualAuthors is [
        {
            Name: DataGenerator.BojackAuthor,
            Bio: DataGenerator.BojackTheme
        },
        {
            Name: DataGenerator.DrSeussAuthor,
            Bio: DataGenerator.DrSeussQuote
        }
        ]);

        // foreach (var a in actualAuthors)
        // {
        //     Assert.That(a.Created >= DateTime.Now.Subtract(TimeSpan.FromSeconds(30)) && a.Created < DateTime.Now);
        // }
    }

    [Test]
    public async Task TestGetAuthor()
    {
        await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.BojackAuthor,
            Bio = DataGenerator.BojackTheme
        });
        await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.DrSeussAuthor,
            Bio = DataGenerator.DrSeussQuote
        });

        var actualAuthor = await QuerySql.GetAuthor(new QuerySql.GetAuthorArgs
        {
            Name = DataGenerator.BojackAuthor
        });
        ClassicAssert.IsNotNull(actualAuthor);
        Assert.That(actualAuthor is
        {
            Name: DataGenerator.BojackAuthor,
            Bio: DataGenerator.BojackTheme
        });
    }

    [Test]
    public async Task TestDeleteAuthor()
    {
        await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.BojackAuthor,
            Bio = DataGenerator.BojackTheme
        });
        await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.DrSeussAuthor,
            Bio = DataGenerator.DrSeussQuote
        });

        await QuerySql.DeleteAuthor(new QuerySql.DeleteAuthorArgs { Name = DataGenerator.BojackAuthor });
        var authorRows = await QuerySql.ListAuthors();
        ClassicAssert.AreEqual(1, authorRows.Count);
        Assert.That(authorRows[0] is
        {
            Name: DataGenerator.DrSeussAuthor,
            Bio: DataGenerator.DrSeussQuote
        });
    }
}