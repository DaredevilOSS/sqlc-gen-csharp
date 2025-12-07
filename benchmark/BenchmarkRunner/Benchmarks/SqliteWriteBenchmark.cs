using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkRunner.Utils;
using Microsoft.EntityFrameworkCore;
using SqliteEFCoreImpl;
using SqliteSqlcImpl;

namespace BenchmarkRunner.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80, warmupCount: 2, iterationCount: 8)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class SqliteWriteBenchmark
{
    private readonly string _connectionString = Config.GetSqliteConnectionString();
    private QuerySql _sqlcImpl = null!;
    private Queries _efCoreImpl = null!;
    private List<QuerySql.AddOrderItemsArgs> _testOrderItems = null!;

    [Params(100, 200, 500)]
    public int BatchSize { get; set; }


    [GlobalSetup]
    public async Task GlobalSetup()
    {
        _sqlcImpl = new QuerySql(_connectionString);
        _efCoreImpl = new Queries(new SalesDbContext(_connectionString));
        
        await SqliteDatabaseHelper.CleanupDatabaseAsync(_connectionString);
        await SqliteDatabaseHelper.InitializeDatabaseAsync(_connectionString);
        PrepareTestDataAsync().GetAwaiter().GetResult();
    }
    
    private async Task PrepareTestDataAsync()
    {
        var seeder = new SqliteDatabaseSeeder(_connectionString);
        await seeder.SeedAsync(
            customerCount: 10, 
            productsPerCategory: 15, 
            ordersPerCustomer: 300, 
            itemsPerOrder: 0
        );

        var orderIds = await _sqlcImpl.GetOrderIdsAsync(new QuerySql.GetOrderIdsArgs(Limit: 1000));
        var productIds = await _sqlcImpl.GetProductIdsAsync(new QuerySql.GetProductIdsArgs(Limit: 1000));

        _testOrderItems = [.. Enumerable.Range(0, BatchSize).Select(i => new QuerySql.AddOrderItemsArgs(
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
        await _sqlcImpl.AddOrderItemsAsync(_testOrderItems);
    }

    [BenchmarkCategory("Write")]
    [Benchmark(Description = "EFCore - AddOrderItems")]
    public async Task EFCore_AddOrderItems()
    {
        var args = _testOrderItems.Select(i => new Queries.AddOrderItemsArgs(
            i.OrderId, i.ProductId, i.Quantity, i.UnitPrice
        )).ToList();
        await _efCoreImpl.AddOrderItems(args);
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        await _efCoreImpl.DbContext.DisposeAsync();
        await SqliteDatabaseHelper.CleanupDatabaseAsync(_connectionString);
    }
}