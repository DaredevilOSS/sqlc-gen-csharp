using BenchmarkDotNet.Attributes;
using BenchmarkRunner.Utils;

public readonly record struct ReadBenchmarkParams(
    int Limit,
    int Concurrency,
    int QueriesToSubmit
) {
    public override string ToString() => $"Limit={Limit}, Concurrency={Concurrency}, Queries={QueriesToSubmit:N0}";
}

public abstract class BaseReadBenchmark
{

    [IterationSetup]
    public static void IterationSetup() => Helpers.InvokeGarbageCollection();
    public abstract Task Sqlc_GetCustomerOrders();
    public abstract Task EFCore_NoTracking_GetCustomerOrders();
    public abstract Task EFCore_WithTracking_GetCustomerOrders();
    public static DatabaseSeedConfig GetSeedConfig() => new(
        CustomerCount: 500,
        ProductsPerCategory: 150,
        OrdersPerCustomer: 500,
        ItemsPerOrder: 20
    );
    private static int CalculateMaxConcurrency(int totalTasks, int maxConcurrency)
    {
        return new int[] {
            maxConcurrency, totalTasks, Environment.ProcessorCount
        }.Min(x => x);
    }
    protected static async Task<List<T>> ExecuteConcurrentlyAsync<T>(
        int totalTasks,
        int maxConcurrency,
        Func<int, Task<List<T>>> taskFactory)
    {
        maxConcurrency = CalculateMaxConcurrency(totalTasks, maxConcurrency);
        using var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
        var tasks = new List<Task<List<T>>>();
        for (int i = 0; i < totalTasks; i++)
        {
            var index = i; // Capture for closure
            tasks.Add(ExecuteWithThrottleAsync(semaphore, () => taskFactory(index)));
        }

        var results = await Task.WhenAll([.. tasks]);
        return [.. results.SelectMany(r => r)];
    }

    private static async Task<List<T>> ExecuteWithThrottleAsync<T>(
        SemaphoreSlim semaphore,
        Func<Task<List<T>>> taskFactory)
    {
        await semaphore.WaitAsync();
        try
        {
            return await taskFactory();
        }
        finally
        {
            semaphore.Release();
        }
    }
}