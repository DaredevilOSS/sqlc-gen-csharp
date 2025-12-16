public readonly record struct WriteBenchmarkArgs(
    int TotalRecords,
    int BatchSize
);

public abstract class BaseWriteBenchmark
{
    public static DatabaseSeedConfig GetSeedConfig() => new(
        CustomerCount: 500,
        ProductsPerCategory: 150,
        OrdersPerCustomer: 500,
        ItemsPerOrder: 0
    );

    public abstract Task Sqlc_AddOrderItems(WriteBenchmarkArgs args);
    public abstract Task EFCore_AddOrderItems(WriteBenchmarkArgs args);
}