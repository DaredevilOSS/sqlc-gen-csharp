using MySqlConnectorDapperExampleGen;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public partial class MySqlConnectorDapperTester : IExecLastIdTester
{
    private QuerySql QuerySql { get; } = new(
        Environment.GetEnvironmentVariable(EndToEndCommon.MySqlConnectionStringEnv)!);

    [TearDown]
    public async Task EmptyTestsTable()
    {
        await QuerySql.TruncateAuthors();
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
}