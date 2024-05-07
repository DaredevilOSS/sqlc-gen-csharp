using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public interface IDriverTester
{
    async Task TestFlow()
    {
        var firstInsertedId = await CreateFirstAuthorAndTest();
        await CreateSecondAuthorAndTest();
        await DeleteFirstAuthorAndTest(firstInsertedId);
    }

    protected Task<long> CreateFirstAuthorAndTest();

    protected Task CreateSecondAuthorAndTest();

    protected Task DeleteFirstAuthorAndTest(long idToDelete);
}