using NUnit.Framework;
using SqlcGenCsharp;

namespace CodegenTests;

public class ExampleTests
{
    [Test]
    public static async Task TestNpgsqlExample() => await AssertRequestSuccess("NpgsqlExampleRequest.message");

    [Test]
    public static async Task TestNpgsqlDapperExample() => await AssertRequestSuccess("NpgsqlDapperExampleRequest.message");

    [Test]
    public static async Task TestMySqlConnectorExample() => await AssertRequestSuccess("MySqlConnectorExampleRequest.message");

    [Test]
    public static async Task TestMySqlConnectorDapperExample() => await AssertRequestSuccess("MySqlConnectorDapperExampleRequest.message");

    [Test]
    public static async Task TestMssqlExample() => await AssertRequestSuccess("MssqlExampleRequest.message");

    [Test]
    public static async Task TestMssqlDapperExample() => await AssertRequestSuccess("MssqlDapperExampleRequest.message");

    [Test]
    public static async Task TestSqliteExample() => await AssertRequestSuccess("SqliteExampleRequest.message");

    [Test]
    public static async Task TestSqliteDapperExample() => await AssertRequestSuccess("SqliteDapperExampleRequest.message");

    [Test]
    public static async Task TestSqliteAsyncEnumerableExample() => await AssertRequestSuccess("SqliteAsyncEnumerableExampleRequest.message");

    [Test]
    public static async Task TestNpgsqlDapperAsyncEnumerableExample() => await AssertRequestSuccess("NpgsqlDapperAsyncEnumerableExampleRequest.message");

    private static async Task AssertRequestSuccess(string requestFile)
    {
        await Assert.DoesNotThrowAsync(async () =>
        {
            var request = Plugin.GenerateRequest.Parser.ParseFrom(File.ReadAllBytes(requestFile));
            var codeGenerator = new CodeGenerator();
            await codeGenerator.Generate(request);
        });
    }
}