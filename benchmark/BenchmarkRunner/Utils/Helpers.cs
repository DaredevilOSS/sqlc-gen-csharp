namespace BenchmarkRunner.Utils;

public static class Helpers
{
    public static string GetBasePath() => Path.Combine("benchmark", "BenchmarkDotNet.Artifacts");

    public static void InvokeGarbageCollection()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    /// <summary>
    /// Formats elapsed time in a human-readable format (e.g., "1m 23.45s" or "45.67s")
    /// </summary>
    public static string FormatElapsedTime(TimeSpan elapsed)
    {
        if (elapsed.TotalMinutes >= 1)
        {
            var minutes = (int)elapsed.TotalMinutes;
            var seconds = elapsed.TotalSeconds % 60;
            return $"{minutes}m {seconds:F2}s";
        }
        return $"{elapsed.TotalSeconds:F2}s";
    }

    public static async Task InsertInBatchesAsync<T>(List<T> items, int batchSize, Func<List<T>, Task> insertBatch)
    {
        var batches = items.Chunk(batchSize);
        foreach (var batch in batches)
            await insertBatch([.. batch]);
    }

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