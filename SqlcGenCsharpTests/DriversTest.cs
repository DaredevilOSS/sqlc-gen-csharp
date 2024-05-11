using System.Threading.Tasks;
using NUnit.Framework;

namespace SqlcGenCsharpTests;

public class DriversTest
{
    [Test]
    public async Task TestFlowOnMySql()
    {
        IDriverTester tester = new MySqlTester();
        await tester.TestFlow();
    }

    [Test]
    public async Task TestFlowOnPostgres()
    {
        IDriverTester tester = new PostgresTester();
        await tester.TestFlow();
    }
}