using BenchmarkDotNet.Attributes;

public abstract class BaseWriteBenchmark
{
    [Params(2000000)]
    public int TotalRecords { get; set; }

    public abstract Task Sqlc_AddOrderItems(int batchSize);
    public abstract Task EFCore_AddOrderItems(int batchSize);
}