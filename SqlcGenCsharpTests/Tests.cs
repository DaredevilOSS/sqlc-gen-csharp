using System.Threading.Tasks;
using NUnit.Framework;

namespace SqlcGenCsharpTests;

public class Tests
{
    [Test]
    public async Task TestFlowOnMySql()
    {
        ISqlDriverTester tester = new MySqlTester();
        await tester.TestFlow();
    }

    [Test]
    public async Task TestFlowOnPostgres()
    {
        ISqlDriverTester tester = new PostgresTester();
        await tester.TestFlow();
    }
}