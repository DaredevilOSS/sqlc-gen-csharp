using BenchmarkDotNet.Attributes;
using BenchmarkRunner.Utils;

public abstract class BaseReadBenchmark
{
    [Params(1000)]
    public int QueriesToRun { get; set; }

    [Params(500)]
    public int CustomerCount { get; set; }

    [Params(100, 500)]
    public int Limit { get; set; }

    [IterationSetup]
    public static void IterationSetup() => Helpers.InvokeGarbageCollection();
    public abstract Task Sqlc_GetCustomerOrders();
    public abstract Task EFCore_NoTracking_GetCustomerOrders();
    public abstract Task EFCore_WithTracking_GetCustomerOrders();
    private static int CalculateMaxConcurrency(int totalTasks, int maxConcurrency)
    {
        return new int[] {
            maxConcurrency, totalTasks, Environment.ProcessorCount
        }.Min(x => x);
    }
    public static async Task<List<T>> ExecuteConcurrentlyAsync<T>(
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

    private static async Task<List<T>> ExecuteWithThrottleAsync<T>(SemaphoreSlim semaphore, Func<Task<List<T>>> taskFactory)
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