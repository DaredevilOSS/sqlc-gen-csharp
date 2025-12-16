using BenchmarkDotNet.Attributes;

public abstract class BaseWriteBenchmark
{
    [Params(2000000)]
    public int TotalRecords { get; set; }

    [Params(500)]
    public int EFCoreBatchSize { get; set; }
    public abstract Task Sqlc_AddOrderItems();
    public abstract Task EFCore_AddOrderItems();
}