using NUnit.Framework;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public class Tests
{
    [Test]
    public async Task TestFlowOnMySql()
    {
        var tester = new MySqlConnectorTester();
        await tester.TestBasicFlow();
    }

    [Test]
    public async Task TestFlowOnPostgres()
    {
        var tester = new NpgsqlTester();
        await tester.TestBasicFlow();
        await tester.TestCopyFlow();
    }
}