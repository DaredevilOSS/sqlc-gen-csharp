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
    private static readonly string _connectionString = Config.GetPostgresConnectionString();
    private readonly QuerySql _sqlcImpl = new(_connectionString);
    private static bool _isInitialized = false;
    private static readonly SemaphoreSlim _initLock = new(1, 1);

    [Params(ReadBenchmarkConsts.CustomerCount)]
    public int CustomerCount { get; set; }

    [Params(ReadBenchmarkConsts.QueriesToRun)]
    public int QueriesToRun { get; set; }

    [Params(100, 1000)]
    public int Limit { get; set; }

    [Params(10, 50)]
    public int ConcurrentQueries { get; set; }

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        if (_isInitialized) return;
        await _initLock.WaitAsync();
        try
        {
            if (_isInitialized) return;

            await PostgresqlDatabaseHelper.CleanupDatabaseAsync(_connectionString);
            var seeder = new PostgresqlDatabaseSeeder(_connectionString);
            await seeder.SeedAsync(
                customerCount: CustomerCount,
                productsPerCategory: 150, // with customer_id filter, this is 1/500 of the table returned
                ordersPerCustomer: 1000,
                itemsPerOrder: 20
            // 20 * 1000 = 20,000 possible rows returned
            );

            _isInitialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    [IterationSetup]
    public static void IterationSetup() => Helpers.InvokeGarbageCollection();


    [BenchmarkCategory("Read")]
    [Benchmark(Baseline = true, Description = "SQLC - GetCustomerOrders")]
    public async Task<List<QuerySql.GetCustomerOrdersRow>> Sqlc_GetCustomerOrders()
    {
        return await Helpers.ExecuteConcurrentlyAsync(QueriesToRun, ConcurrentQueries, _ =>
        {
            return _sqlcImpl.GetCustomerOrdersAsync(new QuerySql.GetCustomerOrdersArgs(
                CustomerId: Random.Shared.Next(1, CustomerCount),
                Offset: 0,
                Limit: Limit
            ));
        });
    }

    [BenchmarkCategory("Read")]
    [Benchmark(Description = "EFCore (NoTracking) - GetCustomerOrders")]
    public async Task<List<Queries.GetCustomerOrdersRow>> EFCore_NoTracking_GetCustomerOrders()
    {
        return await Helpers.ExecuteConcurrentlyAsync(QueriesToRun, ConcurrentQueries, async _ =>
        {
            await using var dbContext = new SalesDbContext(_connectionString);
            var queries = new Queries(dbContext, useTracking: false);
            return await queries.GetCustomerOrders(new Queries.GetCustomerOrdersArgs(
                CustomerId: Random.Shared.Next(1, CustomerCount),
                Offset: 0,
                Limit: Limit
            ));
        });
    }

    [BenchmarkCategory("Read")]
    [Benchmark(Description = "EFCore (WithTracking) - GetCustomerOrders")]
    public async Task<List<Queries.GetCustomerOrdersRow>> EFCore_WithTracking_GetCustomerOrders()
    {
        return await Helpers.ExecuteConcurrentlyAsync(QueriesToRun, ConcurrentQueries, async _ =>
        {
            await using var dbContext = new SalesDbContext(_connectionString);
            var queries = new Queries(dbContext, useTracking: true);
            return await queries.GetCustomerOrders(new Queries.GetCustomerOrdersArgs(
                CustomerId: Random.Shared.Next(1, CustomerCount),
                Offset: 0,
                Limit: Limit
            ));
        });
    }
}