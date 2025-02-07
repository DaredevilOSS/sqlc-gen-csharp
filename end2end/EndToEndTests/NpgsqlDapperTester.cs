using NpgsqlDapperExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public partial class NpgsqlDapperTester
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
            CInteger = 1,
            CText = "ab",
            CVarchar = "abc",
            CCharacterVarying = "abcd",
            CTextArray = ["a", "b"],
            CIntegerArray = [1, 2]
        };
        var insertedId = await QuerySql.InsertNodePostgresType(nodePostgresTypeArgs);

        var actual = await QuerySql.GetNodePostgresType(new QuerySql.GetNodePostgresTypeArgs
        {
            Id = insertedId
        });
        Assert.That(actual is
        {
            CBigint: 1,
            CReal: 1.0f,
            CSerial: 1,
            CNumeric: 1,
            CDecimal: 1,
            CSmallint: 1,
            CBoolean: true,
            CChar: "a",
            CInteger: 1,
            CText: "ab",
            CVarchar: "abc",
            CTextArray: ["a", "b"],
            CIntegerArray: [1, 2]
        });
    }
}