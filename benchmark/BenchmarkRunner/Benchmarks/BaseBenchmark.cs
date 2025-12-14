public abstract class BaseBenchmark
{
    protected bool _isInitialized = false;
    protected SemaphoreSlim _initLock = new(1, 1);

    public async Task InitializeOnceAsync(Func<Task> task)
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

    public static void InvokeGarbageCollection()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}