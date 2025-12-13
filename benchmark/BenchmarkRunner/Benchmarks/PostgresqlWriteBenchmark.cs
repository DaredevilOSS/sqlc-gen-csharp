using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkRunner.Utils;
using PostgresEFCoreImpl;
using PostgresSqlcImpl;

namespace BenchmarkRunner.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80, warmupCount: 2, iterationCount: 15)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class PostgresqlWriteBenchmark
{
    private const int TotalRecords = 5000000; // 5 million records
    private readonly string _connectionString = Config.GetPostgresConnectionString();
    private QuerySql _sqlcImpl = null!;
    private Queries _efCoreImpl = null!;
    private List<QuerySql.AddOrderItemsArgs> _testOrderItems = null!;

    [Params(500, 1000, 2000)]
    public int BatchSize { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _sqlcImpl = new QuerySql(_connectionString);
        _efCoreImpl = new Queries(new SalesDbContext(_connectionString), useTracking: false);
        PrepareTestDataAsync().GetAwaiter().GetResult();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        PostgresqlDatabaseHelper.CleanupWriteTableAsync(_connectionString).GetAwaiter().GetResult();
        Helpers.InvokeGarbageCollection();
    }

    private async Task PrepareTestDataAsync()
    {
        PostgresqlDatabaseHelper.CleanupDatabaseAsync(_connectionString).GetAwaiter().GetResult();
        var seeder = new PostgresqlDatabaseSeeder(_connectionString);
        await seeder.SeedAsync(
            customerCount: 10,
            productsPerCategory: 15,
            ordersPerCustomer: 300,
            itemsPerOrder: 0
        );

        var orderIds = await _sqlcImpl.GetOrderIdsAsync(new QuerySql.GetOrderIdsArgs(Limit: 1000));
        var productIds = await _sqlcImpl.GetProductIdsAsync(new QuerySql.GetProductIdsArgs(Limit: 1000));

        _testOrderItems = [.. Enumerable.Range(0, TotalRecords).Select(i => new QuerySql.AddOrderItemsArgs(
            OrderId: orderIds[i % orderIds.Count].OrderId,
            ProductId: productIds[i % productIds.Count].ProductId,
            Quantity: Random.Shared.Next(1, 10),
            UnitPrice: (decimal)(Random.Shared.NextDouble() * 100 + 5)
        ))];
    }

    [BenchmarkCategory("Write")]
    [Benchmark(Baseline = true, Description = "SQLC - AddOrderItems")]
    public async Task Sqlc_AddOrderItems()
    {
        await Helpers.InsertInBatchesAsync(_testOrderItems, BatchSize, _sqlcImpl.AddOrderItemsAsync);
    }

    [BenchmarkCategory("Write")]
    [Benchmark(Description = "EFCore - AddOrderItems")]
    public async Task EFCore_AddOrderItems()
    {
        var args = _testOrderItems.Select(i => new Queries.AddOrderItemsArgs(
            i.OrderId, i.ProductId, i.Quantity, i.UnitPrice
        )).ToList();
        await Helpers.InsertInBatchesAsync(args, BatchSize, _efCoreImpl.AddOrderItems);
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        await _efCoreImpl.DbContext.DisposeAsync();
        await PostgresqlDatabaseHelper.CleanupDatabaseAsync(_connectionString);
    }
}