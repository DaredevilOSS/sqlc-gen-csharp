using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkRunner.Utils;
using PostgresEFCoreImpl;
using PostgresSqlcImpl;

namespace BenchmarkRunner.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80, warmupCount: 2, iterationCount: 10)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class PostgresqlReadBenchmark
{
    private const int CustomerCount = 500;
    private readonly string _connectionString = Config.GetPostgresConnectionString();
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

        await PostgresqlDatabaseHelper.CleanupDatabaseAsync(_connectionString);
        var seeder = new PostgresqlDatabaseSeeder(_connectionString);
        await seeder.SeedAsync(
            customerCount: CustomerCount,
            productsPerCategory: 150, // with customer_id filter, this is 1/500 of the table returned
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
        GC.Collect();
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
        await PostgresqlDatabaseHelper.CleanupDatabaseAsync(_connectionString);
    }
}