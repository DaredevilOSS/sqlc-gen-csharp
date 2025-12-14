
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkRunner.Utils;
using MysqlEFCoreImpl;
using MysqlSqlcImpl;

namespace BenchmarkRunner.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80, warmupCount: 2, iterationCount: 10)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class MysqlWriteBenchmark
{
    private static readonly string _connectionString = Config.GetMysqlConnectionString();
    private readonly QuerySql _sqlcImpl = new(_connectionString);
    private readonly Queries _efCoreImpl = new(new SalesDbContext(_connectionString), useTracking: false);
    private List<QuerySql.AddOrderItemsArgs> _testOrderItems = null!;
    private static bool _isInitialized = false;
    private static readonly SemaphoreSlim _initLock = new(1, 1);

    [Params(WriteBenchmarkConsts.TotalRecords)]
    public int TotalRecords { get; set; }

    [Params(100, 500, 1000)]
    public int BatchSize { get; set; }

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        if (_isInitialized) return;
        await _initLock.WaitAsync();
        try
        {
            if (_isInitialized) return;

            await MysqlDatabaseHelper.CleanupDatabaseAsync(_connectionString);
            var seeder = new MysqlDatabaseSeeder(_connectionString);
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

            _isInitialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    [IterationSetup]
    public void IterationSetup()
    {
        MysqlDatabaseHelper.CleanupWriteTableAsync(_connectionString).GetAwaiter().GetResult();
        Helpers.InvokeGarbageCollection();
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
}