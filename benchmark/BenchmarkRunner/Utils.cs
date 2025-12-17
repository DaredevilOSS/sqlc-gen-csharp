namespace BenchmarkRunner.Utils;

public static class Helpers
{
    /* 
    InitializeOnceAsync is a helper method to ensure that a task is only executed once.
    _initLock is a semaphore to ensure that only one task is executed at a time.
    isInitialized is a flag to check if the task has been executed.
     */
    public static bool _isInitialized = false;
    public static SemaphoreSlim _initLock = new(1, 1);
    public static async Task InitializeOnceAsync(Func<Task> task)
    {
        if (_isInitialized) return;
        await _initLock.WaitAsync();
        try
        {
            if (_isInitialized) return;
            await task.Invoke();
            _isInitialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public static async Task InsertInBatchesAsync<T>(List<T> items, int batchSize, Func<List<T>, Task> insertBatch)
    {
        var batches = items.Chunk(batchSize);
        foreach (var batch in batches)
            await insertBatch([.. batch]);
    }

    public static void InvokeGarbageCollection()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}