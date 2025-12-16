using BenchmarkDotNet.Attributes;

public abstract class BaseWriteBenchmark
{
    public static DatabaseSeedConfig GetSeedConfig() => new(
        CustomerCount: 500,
        ProductsPerCategory: 150,
        OrdersPerCustomer: 500,
        ItemsPerOrder: 0
    );

    public abstract Task Sqlc_AddOrderItems(int batchSize);
    public abstract Task EFCore_AddOrderItems(int batchSize);
}