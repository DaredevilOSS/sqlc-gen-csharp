using BenchmarkDotNet.Attributes;

public abstract class ReadBenchmark
{
    protected bool _isInitialized = false;
    protected SemaphoreSlim _initLock = new(1, 1);

    [Params(1000)]
    protected int QueriesToRun { get; set; }

    [Params(500)]
    protected int CustomerCount { get; set; }

    [Params(100, 500)]
    public int Limit { get; set; }

    public abstract Task Sqlc_GetCustomerOrders();
    public abstract Task EFCore_NoTracking_GetCustomerOrders();
    public abstract Task EFCore_WithTracking_GetCustomerOrders();
}