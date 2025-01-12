using NUnit.Framework;
using NUnit.Framework.Legacy;
using SqliteDapperExampleGen;
using System;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public partial class SqliteDapperTester : IExecLastIdTester
{
    private QuerySql QuerySql { get; } = new(
        Environment.GetEnvironmentVariable(EndToEndCommon.SqliteConnectionStringEnv)!);
    [TearDown]
    public async Task EmptyTestsTable()
    {
        await QuerySql.DeleteAllAuthors();
    }

    public async Task TestExecLastId()
    {
        var bojackCreateAuthorArgs = new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = DataGenerator.GenericAuthor,
            Bio = DataGenerator.GenericQuote1
        };
        var insertedId = await QuerySql.CreateAuthorReturnId(bojackCreateAuthorArgs);

        var actual = await QuerySql.GetAuthorById(new QuerySql.GetAuthorByIdArgs
        {
            Id = insertedId
        });
        Assert.That(actual is
        {
            Name: DataGenerator.GenericAuthor,
            Bio: DataGenerator.GenericQuote1
        });
    }
}