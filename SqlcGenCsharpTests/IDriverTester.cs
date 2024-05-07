using System.Threading.Tasks;
using NUnit.Framework;

namespace SqlcGenCsharpTests;

public abstract class DriverTester
{
    protected async Task TestFlow()
    {
        var firstInsertedId = await CreateFirstAuthorAndTest();
        await CreateSecondAuthorAndTest();
        await DeleteFirstAuthorAndTest(firstInsertedId);
    }

    protected abstract Task<long> CreateFirstAuthorAndTest();

    protected abstract Task CreateSecondAuthorAndTest();

    protected abstract Task DeleteFirstAuthorAndTest(long idToDelete);
}