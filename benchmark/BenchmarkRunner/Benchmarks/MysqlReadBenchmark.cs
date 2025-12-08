using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkRunner.Utils;
using MysqlEFCoreImpl;
using MysqlSqlcImpl;

namespace BenchmarkRunner.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80, warmupCount: 2, iterationCount: 8)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class MysqlReadBenchmark
{
    private const int CustomerCount = 500;
    private readonly string _connectionString = Config.GetMysqlConnectionString();
    private QuerySql _sqlcImpl = null!;
    private Queries _efCoreImplNoTracking = null!;
    private Queries _efCoreImplWithTracking = null!;

    [Params(5000, 10000, 20000)]
    public int Limit { get; set; }

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        _sqlcImpl = new(_connectionString);
        _efCoreImplNoTracking = new(new SalesDbContext(_connectionString), useTracking: false);
        _efCoreImplWithTracking = new(new SalesDbContext(_connectionString), useTracking: true);

        await MysqlDatabaseHelper.CleanupDatabaseAsync(_connectionString);
        var seeder = new MysqlDatabaseSeeder(_connectionString);
        await seeder.SeedAsync(
             customerCount: CustomerCount, // selectivity: 1/500 of the table returned
             productsPerCategory: 150,
             ordersPerCustomer: 1000,
             itemsPerOrder: 20
             // 20 * 1000 = 20,000 possible rows returned
         );
    }

    [IterationSetup]
    public static void IterationSetup()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    [BenchmarkCategory("Read")]
    [Benchmark(Baseline = true, Description = "SQLC - GetCustomerOrders")]
    public async Task<List<QuerySql.GetCustomerOrdersRow>> Sqlc_GetCustomerOrders()
    {
        return await _sqlcImpl.GetCustomerOrdersAsync(new QuerySql.GetCustomerOrdersArgs(
            CustomerId: Random.Shared.Next(1, CustomerCount),
            Offset: 0,
            Limit: Limit
        ));
    }

    [BenchmarkCategory("Read")]
    [Benchmark(Description = "EFCore (NoTracking) - GetCustomerOrders")]
    public async Task<List<Queries.GetCustomerOrdersRow>> EFCore_NoTracking_GetCustomerOrders()
    {
        return await _efCoreImplNoTracking.GetCustomerOrders(new Queries.GetCustomerOrdersArgs(
            CustomerId: Random.Shared.Next(1, CustomerCount),
            Offset: 0,
            Limit: Limit
        ));
    }

    [BenchmarkCategory("Read")]
    [Benchmark(Description = "EFCore (WithTracking) - GetCustomerOrders")]
    public async Task<List<Queries.GetCustomerOrdersRow>> EFCore_WithTracking_GetCustomerOrders()
    {
        return await _efCoreImplWithTracking.GetCustomerOrders(new Queries.GetCustomerOrdersArgs(
            CustomerId: Random.Shared.Next(1, CustomerCount),
            Offset: 0,
            Limit: Limit
        ));
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        await _efCoreImplNoTracking.DbContext.DisposeAsync();
        await _efCoreImplWithTracking.DbContext.DisposeAsync();
        await MysqlDatabaseHelper.CleanupDatabaseAsync(_connectionString);
    }
}