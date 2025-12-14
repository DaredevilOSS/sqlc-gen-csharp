using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkRunner.Utils;
using SqliteEFCoreImpl;
using SqliteSqlcImpl;

namespace BenchmarkRunner.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80, warmupCount: 2, iterationCount: 10)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class SqliteWriteBenchmark : BaseWriteBenchmark
{
    private static readonly string _connectionString = Config.GetSqliteConnectionString();
    private readonly QuerySql _sqlcImpl = new(_connectionString);
    private readonly Queries _efCoreImpl = new(new SalesDbContext(_connectionString), useTracking: false);
    private List<QuerySql.AddOrderItemsArgs> _testOrderItems = null!;

    /// <summary>
    /// SQLite batch size for SQLC can be large but due to the plugin implementation of batch inserts 
    /// for SQLite, there is a limitation of 1000 bind variables in the resulting SQL.
    /// e.g. If we insert to a table with 10 columns, the maximum batch size is 1000 / 10 = 100.
    /// </summary>
    [Params(50, 100, 200)]
    public int BatchSize { get; set; }

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        var orderIds = await _sqlcImpl.GetOrderIdsAsync(new QuerySql.GetOrderIdsArgs(Limit: 1000));
        var productIds = await _sqlcImpl.GetProductIdsAsync(new QuerySql.GetProductIdsArgs(Limit: 1000));

        _testOrderItems = [.. Enumerable.Range(0, TotalRecords).Select(i => new QuerySql.AddOrderItemsArgs(
            OrderId: orderIds[i % orderIds.Count].OrderId,
            ProductId: productIds[i % productIds.Count].ProductId,
            Quantity: Random.Shared.Next(1, 10),
            UnitPrice: (decimal)(Random.Shared.NextDouble() * 100 + 5)
        ))];
    }

    [IterationSetup]
    public static void IterationSetup()
    {
        SqliteDatabaseHelper.CleanupWriteTableAsync(_connectionString).GetAwaiter().GetResult();
        Helpers.InvokeGarbageCollection();
    }

    [BenchmarkCategory("Write")]
    [Benchmark(Baseline = true, Description = "SQLC - AddOrderItems")]
    public override async Task Sqlc_AddOrderItems()
    {
        await Helpers.InsertInBatchesAsync(_testOrderItems, BatchSize, _sqlcImpl.AddOrderItemsAsync);
    }

    [BenchmarkCategory("Write")]
    [Benchmark(Description = "EFCore - AddOrderItems")]
    public override async Task EFCore_AddOrderItems()
    {
        var args = _testOrderItems.Select(i => new Queries.AddOrderItemsArgs(
            i.OrderId, i.ProductId, i.Quantity, i.UnitPrice
        )).ToList();
        await Helpers.InsertInBatchesAsync(args, BatchSize, _efCoreImpl.AddOrderItems);
    }

    public static Func<Task> GetSeedMethod()
    {
        return async () =>
        {
            SqliteDatabaseHelper.CleanupDatabase(_connectionString);
            await SqliteDatabaseHelper.InitializeDatabaseAsync(_connectionString);
            var seeder = new SqliteDatabaseSeeder(_connectionString);
            await seeder.SeedAsync(
                customerCount: 10,
                productsPerCategory: 15,
                ordersPerCustomer: 300,
                itemsPerOrder: 0
            );
        };
    }
}