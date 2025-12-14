using BenchmarkDotNet.Attributes;

public abstract class WriteBenchmark
{
    protected bool _isInitialized = false;
    protected SemaphoreSlim _initLock = new(1, 1);

    [Params(2000000)]
    protected int TotalRecords { get; set; }

    public abstract Task Sqlc_AddOrderItems();
    public abstract Task EFCore_AddOrderItems();
}