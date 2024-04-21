using System.Threading.Tasks;
using NUnit.Framework;

namespace SqlcGenCsharpTests;

public interface IDriverTester
{
    [Test]
    public Task TestFlowOnDriver();
}