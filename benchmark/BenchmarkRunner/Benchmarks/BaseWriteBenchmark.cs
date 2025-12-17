using SqlcGenCsharp;

public readonly record struct WriteBenchmarkArgs(
    int TotalRecordsToLoad,
    int BatchSize
)
{
    public override string ToString() => $"Records={TotalRecordsToLoad.StringifyLargeNumbers()}, BatchSize={BatchSize.StringifyLargeNumbers()}";
}

public abstract class BaseWriteBenchmark
{
    public const int TotalRecordsForSetup = 300000; // 3 million records
    public const int OrderIdsCountForSetup = 1000;
    public const int ProductIdsCountForSetup = 1000;

    public static DatabaseSeedConfig GetSeedConfig() => new(
        CustomerCount: 500,
        ProductsPerCategory: 150,
        OrdersPerCustomer: 500,
        ItemsPerOrder: 0
    );

    public abstract Task Sqlc_AddOrderItems(WriteBenchmarkArgs args);
    public abstract Task EFCore_AddOrderItems(WriteBenchmarkArgs args);
}