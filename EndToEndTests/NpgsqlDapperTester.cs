using Newtonsoft.Json;
using NpgsqlDapperExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public class NpgsqlDapperTester : IOneTester, IManyTester, IExecTester, IExecRowsTester, IExecLastIdTester
{
    private QuerySql QuerySql { get; } = new(
        Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv)!);

    [TearDown]
    public async Task EmptyTestsTable()
    {
        await QuerySql.TruncateAuthors();
        await QuerySql.TruncateNodePostgresTypes();
    }

    [Test]
    public async Task TestOne()
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
    public async Task TestMany()
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
    }

    [Test]
    public async Task TestExec()
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

        var deleteAuthorArgs = new QuerySql.DeleteAuthorArgs { Name = DataGenerator.BojackAuthor };
        await QuerySql.DeleteAuthor(deleteAuthorArgs);

        var authorRows = await QuerySql.ListAuthors();
        ClassicAssert.AreEqual(1, authorRows.Count);
        Assert.That(authorRows[0] is
        {
            Name: DataGenerator.DrSeussAuthor,
            Bio: DataGenerator.DrSeussQuote
        });
    }

    [Test]
    public async Task TestExecRows()
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
    public async Task TestExecLastId()
    {
        var bojackCreateAuthorArgs = new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = DataGenerator.GenericAuthor,
            Bio = DataGenerator.GenericQuote1
        };
        var insertedId = await QuerySql.CreateAuthorReturnId(bojackCreateAuthorArgs);

        var getAuthorByIdArgs = new QuerySql.GetAuthorByIdArgs
        {
            Id = insertedId
        };
        var actual = await QuerySql.GetAuthorById(getAuthorByIdArgs);
        Assert.That(actual is
        {
            Name: DataGenerator.GenericAuthor,
            Bio: DataGenerator.GenericQuote1
        });
    }

    [Test]
    public async Task TestNodePostgresType()
    {
        var nodePostgresTypeArgs = new QuerySql.InsertNodePostgresTypeArgs
        {
            CBigint = 1,
            CReal = 1.0f,
            CNumeric = 1,
            CSerial = 1,
            CSmallint = 1,
            CDecimal = 1,
            CDate = DateTime.Now,
            CTimestamp = DateTime.Now,
            CBoolean = true,
            CChar = "a",
            CDoublePrecision = (float?)1.0,
            CInteger = 1,
            CText = "ab",
            CVarchar = "abc",
            CCharacterVarying = "abcd",
            CTextArray = ["a", "b"]
        };
        var insertedId = await QuerySql.InsertNodePostgresType(nodePostgresTypeArgs);

        var actual = await QuerySql.GetNodePostgresType(new QuerySql.GetNodePostgresTypeArgs
        {
            Id = insertedId
        });
        System.Console.WriteLine("insertedId: " + insertedId + ", Actual: " + JsonConvert.SerializeObject(actual));
        Assert.That(actual, Is.AssignableTo<object>().And
            .Property("CBigint").EqualTo(1).And
            .Property("CReal").EqualTo(1.0f).And
            .Property("CSerial").EqualTo(1).And
            .Property("CNumeric").EqualTo(1).And
            .Property("CDecimal").EqualTo(1).And
            .Property("CSmallint").EqualTo(1).And
            .Property("CBoolean").EqualTo(true).And
            .Property("CChar").EqualTo("a").And
            .Property("CDoublePrecision").EqualTo((float?)1.0).And
            .Property("CInteger").EqualTo(1).And
            .Property("CText").EqualTo("ab").And
            .Property("CVarchar").EqualTo("abc").And
            .Property("CCharacterVarying").EqualTo("abcd").And
            .Property("CTextArray").EqualTo(new[] { "a", "b" }));
    }
}