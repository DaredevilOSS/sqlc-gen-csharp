using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public abstract class SqlDriverTester
{
    public async Task TestBasicFlow()
    {
        var firstInsertedId = await CreateFirstAuthorAndTest();
        await CreateSecondAuthorAndTest();
        await DeleteFirstAuthorAndTest(firstInsertedId);
    }

    protected abstract Task<long> CreateFirstAuthorAndTest();

    protected abstract Task CreateSecondAuthorAndTest();

    protected abstract Task DeleteFirstAuthorAndTest(long idToDelete);
}