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

    public static async Task<List<T>> ExecuteConcurrentlyAsync<T>(int concurrency, Func<int, Task<List<T>>> taskFactory)
    {
        var tasks = new List<Task<List<T>>>();
        for (int i = 0; i < concurrency; i++)
        {
            tasks.Add(taskFactory(i));
        }

        var results = await Task.WhenAll(tasks);
        return [.. results.SelectMany(r => r)];
    }
}