using System.Linq;

namespace BenchmarkRunner.Utils;

public static class Helpers
{
    public static void InvokeGarbageCollection()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    public static async Task InsertInBatchesAsync<T>(List<T> items, int batchSize, Func<List<T>, Task> insertBatch)
    {
        for (int i = 0; i < items.Count; i += batchSize)
        {
            var batch = items.Skip(i).Take(batchSize).ToList();
            await insertBatch(batch);
        }
    }

    public static async Task<List<T>> ExecuteConcurrentlyAsync<T>(
        int totalTasks,
        int maxConcurrency,
        Func<int, Task<List<T>>> taskFactory)
    {
        var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
        var tasks = new List<Task<List<T>>>();
        for (int i = 0; i < totalTasks; i++)
            tasks.Add(ExecuteWithThrottleAsync(semaphore, () => taskFactory(i)));

        var results = await Task.WhenAll(tasks);
        return [.. results.SelectMany(r => r)];
    }

    private static async Task<T> ExecuteWithThrottleAsync<T>(SemaphoreSlim? semaphore, Func<Task<T>> taskFactory)
    {
        if (semaphore == null)
            return await taskFactory();

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