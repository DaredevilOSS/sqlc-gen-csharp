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
public class MysqlReadBenchmark : BaseReadBenchmark
{
    private static readonly string _connectionString = Config.GetMysqlConnectionString();
    private readonly QuerySql _sqlcImpl = new(_connectionString);

    [Params(25, 100, 200)]
    public int ConcurrentQueries { get; set; }

    [BenchmarkCategory("Read")]
    [Benchmark(Baseline = true, Description = "SQLC - GetCustomerOrders")]
    public override async Task Sqlc_GetCustomerOrders()
    {
        await ExecuteConcurrentlyAsync(QueriesToRun, ConcurrentQueries, _ =>
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
    public override async Task EFCore_NoTracking_GetCustomerOrders()
    {
        await ExecuteConcurrentlyAsync(QueriesToRun, ConcurrentQueries, async _ =>
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
    public override async Task EFCore_WithTracking_GetCustomerOrders()
    {
        await ExecuteConcurrentlyAsync(QueriesToRun, ConcurrentQueries, async _ =>
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

    public static Func<Task> GetSeedMethod()
    {
        return async () =>
        {
            await MysqlDatabaseHelper.CleanupDatabaseAsync(_connectionString);
            var seeder = new MysqlDatabaseSeeder(_connectionString);
            await seeder.SeedAsync(
                customerCount: 500, // selectivity: 1/500 of the table returned
                productsPerCategory: 150,
                ordersPerCustomer: 1000,
                itemsPerOrder: 20 // 20 * 1000 = 20,000 possible rows returned
            );
        };
    }
}