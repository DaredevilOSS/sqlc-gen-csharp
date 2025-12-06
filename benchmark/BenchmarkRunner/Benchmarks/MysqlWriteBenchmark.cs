using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Microsoft.EntityFrameworkCore;
using MysqlEFCoreImpl;
using MysqlSqlcImpl;
using BenchmarkRunner.Utils;

namespace BenchmarkRunner.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80, warmupCount: 3, iterationCount: 10)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class MysqlWriteBenchmark
{
    private QuerySql _sqlcImpl = null!;
    private Queries _efCoreImpl = null!;
    private SalesDbContext _dbContext = null!;
    private string _connectionString = null!;

    private List<QuerySql.AddProductsArgs> _testProducts = null!;
    private List<QuerySql.AddOrdersArgs> _testOrders = null!;
    private List<QuerySql.AddOrderItemsArgs> _testOrderItems = null!;

    [Params(1000, 10000, 50000)]
    public int BatchSize { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _connectionString = Config.GetMysqlConnectionString();
        _sqlcImpl = new QuerySql(_connectionString);
        _dbContext = new SalesDbContext(_connectionString);
        _efCoreImpl = new Queries(_dbContext);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        // Clean up before each iteration to ensure fair comparison
        MysqlDatabaseHelper.CleanupDatabaseAsync(_connectionString).GetAwaiter().GetResult();
        PrepareTestDataAsync().GetAwaiter().GetResult();
    }

    private async Task PrepareTestDataAsync()
    {
        var random = new Random(42);
        var categories = new[] { "Electronics", "Clothing", "Books", "Food", "Toys" };

        _testProducts = Enumerable.Range(0, BatchSize).Select(i => new QuerySql.AddProductsArgs(
            Name: $"Test Product {i}",
            Category: categories[i % categories.Length],
            UnitPrice: (decimal)(random.NextDouble() * 1000 + 10),
            StockQuantity: random.Next(0, 1000),
            Description: i % 2 == 0 ? $"Description {i}" : null
        )).ToList();

        // For orders, we need existing customers - seed a few
        var seeder = new MysqlDatabaseSeeder(_connectionString);
        await seeder.SeedAsync(customerCount: 10, productsPerCategory: 0, ordersPerCustomer: 0, itemsPerOrder: 0);

        // Use AsNoTracking() to prevent EFCore from tracking these entities
        var customerIds = await _dbContext.Customers.AsNoTracking().Select(c => c.CustomerId).Take(10).ToListAsync();
        var orderStates = new[] { "Pending", "Delivered", "Cancelled" };

        _testOrders = Enumerable.Range(0, BatchSize).Select(i => new QuerySql.AddOrdersArgs(
            CustomerId: customerIds[i % customerIds.Count],
            OrderState: orderStates[i % orderStates.Length],
            TotalAmount: (decimal)(random.NextDouble() * 5000 + 10)
        )).ToList();

        // For order items, we need existing orders and products
        // First add products and orders (these will be cleaned up in IterationSetup)
        await _sqlcImpl.AddProductsAsync(_testProducts);
        await _sqlcImpl.AddOrdersAsync(_testOrders);

        // Use AsNoTracking() to prevent EFCore from tracking these entities
        // This avoids conflicts when the benchmark methods try to add new entities
        var orderIds = await _dbContext.Orders.AsNoTracking().OrderByDescending(o => o.OrderId).Take(BatchSize).Select(o => o.OrderId).ToListAsync();
        var productIds = await _dbContext.Products.AsNoTracking().Select(p => p.ProductId).Take(100).ToListAsync();

        _testOrderItems = Enumerable.Range(0, BatchSize).Select(i => new QuerySql.AddOrderItemsArgs(
            OrderId: orderIds[i % orderIds.Count],
            ProductId: productIds[i % productIds.Count],
            Quantity: random.Next(1, 10),
            UnitPrice: (decimal)(random.NextDouble() * 100 + 5)
        )).ToList();
    }

    [BenchmarkCategory("Write-Products")]
    [Benchmark(Baseline = true, Description = "SQLC - AddProducts")]
    public async Task Sqlc_AddProducts()
    {
        await _sqlcImpl.AddProductsAsync(_testProducts);
    }

    [BenchmarkCategory("Write-Products")]
    [Benchmark(Description = "EFCore - AddProducts")]
    public async Task EFCore_AddProducts()
    {
        // Clear change tracker to avoid conflicts with entities tracked during PrepareTestDataAsync
        _dbContext.ChangeTracker.Clear();
        var args = _testProducts.Select(p => new Queries.AddProductsArgs(
            p.Name, p.Category, p.UnitPrice, p.StockQuantity, p.Description
        )).ToList();
        await _efCoreImpl.AddProducts(args);
    }

    [BenchmarkCategory("Write-Orders")]
    [Benchmark(Baseline = true, Description = "SQLC - AddOrders")]
    public async Task Sqlc_AddOrders()
    {
        await _sqlcImpl.AddOrdersAsync(_testOrders);
    }

    [BenchmarkCategory("Write-Orders")]
    [Benchmark(Description = "EFCore - AddOrders")]
    public async Task EFCore_AddOrders()
    {
        // Clear change tracker to avoid conflicts with entities tracked during PrepareTestDataAsync
        _dbContext.ChangeTracker.Clear();
        var args = _testOrders.Select(o => new Queries.AddOrdersArgs(
            o.CustomerId, o.OrderState, o.TotalAmount
        )).ToList();
        await _efCoreImpl.AddOrders(args);
    }

    [BenchmarkCategory("Write-OrderItems")]
    [Benchmark(Baseline = true, Description = "SQLC - AddOrderItems")]
    public async Task Sqlc_AddOrderItems()
    {
        await _sqlcImpl.AddOrderItemsAsync(_testOrderItems);
    }

    [BenchmarkCategory("Write-OrderItems")]
    [Benchmark(Description = "EFCore - AddOrderItems")]
    public async Task EFCore_AddOrderItems()
    {
        // Clear change tracker to avoid conflicts with entities tracked during PrepareTestDataAsync
        _dbContext.ChangeTracker.Clear();
        var args = _testOrderItems.Select(i => new Queries.AddOrderItemsArgs(
            i.OrderId, i.ProductId, i.Quantity, i.UnitPrice
        )).ToList();
        await _efCoreImpl.AddOrderItems(args);
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        await _dbContext.DisposeAsync();
        await MysqlDatabaseHelper.CleanupDatabaseAsync(_connectionString);
    }
}

