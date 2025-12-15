using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkRunner.Utils;
using EndToEndTests;
using Microsoft.Data.Sqlite;
using SqliteEFCoreImpl;
using SqliteSqlcImpl;

namespace BenchmarkRunner.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80, warmupCount: 2, iterationCount: 10)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class SqliteReadBenchmark : BaseReadBenchmark
{
    private static readonly string _connectionString = Config.GetSqliteConnectionString();
    private readonly QuerySql _sqlcImpl = new(_connectionString);

    /// <summary>
    /// SQLite supports read concurrency, but less so then other relational databases.
    /// </summary>
    [Params(5, 25, 50)]
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
            try
            {
                await using var dbContext = new SalesDbContext(_connectionString);
                var queries = new Queries(dbContext, useTracking: false);
                return await queries.GetCustomerOrders(new Queries.GetCustomerOrdersArgs(
                    CustomerId: Random.Shared.Next(1, CustomerCount),
                    Offset: 0,
                    Limit: Limit
                ));
            }
            catch (SqliteException e)
            {
                throw new Exception($"Sqlite connection string: {_connectionString}, error: {e.Message}", e);
            }
        });
    }

    [BenchmarkCategory("Read")]
    [Benchmark(Description = "EFCore (WithTracking) - GetCustomerOrders")]
    public override async Task EFCore_WithTracking_GetCustomerOrders()
    {
        await ExecuteConcurrentlyAsync(QueriesToRun, ConcurrentQueries, async _ =>
        {
            try
            {
                await using var dbContext = new SalesDbContext(_connectionString);
                var queries = new Queries(dbContext, useTracking: true);
                return await queries.GetCustomerOrders(new Queries.GetCustomerOrdersArgs(
                    CustomerId: Random.Shared.Next(1, CustomerCount),
                    Offset: 0,
                    Limit: Limit
                ));
            }
            catch (SqliteException e)
            {
                throw new Exception($"Sqlite connection string: {_connectionString}, error: {e.Message}", e);
            }
        });
    }

    public static Func<Task> GetSeedMethod()
    {
        return async () =>
        {
            EndToEndCommon.SetupBenchmarkSqliteDb();
            var seeder = new SqliteDatabaseSeeder(_connectionString);
            await seeder.SeedAsync(
                customerCount: 500, // with customer_id filter, this is 1/500 of the table returned
                productsPerCategory: 150,
                ordersPerCustomer: 500,
                itemsPerOrder: 10 // 10 * 500 = 5,000 possible rows returned
            );
        };
    }
}