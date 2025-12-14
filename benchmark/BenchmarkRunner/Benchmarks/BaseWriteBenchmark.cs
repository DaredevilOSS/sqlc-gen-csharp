using BenchmarkDotNet.Attributes;

public abstract class BaseWriteBenchmark : BaseBenchmark
{
    [Params(2000000)]
    public int TotalRecords { get; set; }
    public abstract Task Sqlc_AddOrderItems();
    public abstract Task EFCore_AddOrderItems();
}