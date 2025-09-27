using NUnit.Framework;
using SqlcGenCsharp;

namespace CodegenTests;

public class ExampleTests
{
    [Test]
    public static void TestNpgsqlExample() => AssertRequestSuccess("NpgsqlExampleRequest.message");

    [Test]
    public static void TestNpgsqlDapperExample() => AssertRequestSuccess("NpgsqlDapperExampleRequest.message");

    [Test]
    public static void TestMySqlConnectorExample() => AssertRequestSuccess("MySqlConnectorExampleRequest.message");

    [Test]
    public static void TestMySqlConnectorDapperExample() => AssertRequestSuccess("MySqlConnectorDapperExampleRequest.message");

    [Test]
    public static void TestSqliteExample() => AssertRequestSuccess("SqliteExampleRequest.message");

    [Test]
    public static void TestSqliteDapperExample() => AssertRequestSuccess("SqliteDapperExampleRequest.message");

    private static void AssertRequestSuccess(string requestFile)
    {
        Assert.DoesNotThrowAsync(async () =>
        {
            var request = Plugin.GenerateRequest.Parser.ParseFrom(File.ReadAllBytes(requestFile));
            var codeGenerator = new CodeGenerator();
            await codeGenerator.Generate(request);
        });
    }
}