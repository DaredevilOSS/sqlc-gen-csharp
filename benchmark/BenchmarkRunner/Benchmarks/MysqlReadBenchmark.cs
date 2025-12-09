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
public class MysqlReadBenchmark
{
    private readonly string _connectionString = Config.GetMysqlConnectionString();
    private QuerySql _sqlcImpl = null!;
    private const int CustomerCount = 500;
    private const int QueriesToRun = 10000;

    [Params(5000, 10000, 20000)]
    public int Limit { get; set; }

    [Params(10, 100)]
    public int ConcurrentQueries { get; set; }

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        _sqlcImpl = new(_connectionString);
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

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        await MysqlDatabaseHelper.CleanupDatabaseAsync(_connectionString);
    }
}