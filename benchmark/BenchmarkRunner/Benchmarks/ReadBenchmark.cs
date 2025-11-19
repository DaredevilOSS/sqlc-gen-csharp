using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Microsoft.EntityFrameworkCore;
using PostgresEFCoreImpl;
using PostgresSqlcImpl;
using BenchmarkRunner.Utils;

namespace BenchmarkRunner.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80, warmupCount: 3, iterationCount: 10)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class ReadBenchmark
{
    private QuerySql _sqlcImpl = null!;
    private Queries _efCoreImpl = null!;
    private SalesDbContext _dbContext = null!;
    private int _testCustomerId = 1;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        var connectionString = Config.GetConnectionString();
        await DatabaseHelper.CleanupDatabaseAsync(connectionString);

        _sqlcImpl = new QuerySql(connectionString);
        _dbContext = new SalesDbContext(connectionString);
        _efCoreImpl = new Queries(_dbContext);

        // Ensure database is seeded
        var seeder = new DatabaseSeeder(connectionString);
        await seeder.SeedAsync(customerCount: 100, productsPerCategory: 100, ordersPerCustomer: 50, itemsPerOrder: 3);

        // Get a valid customer ID for testing
        var firstCustomer = await _efCoreImpl.DbContext.Set<Customer>().FirstAsync();
        _testCustomerId = firstCustomer.CustomerId;
    }

    [BenchmarkCategory("Read")]
    [Benchmark(Baseline = true, Description = "SQLC - GetCustomerOrders")]
    public async Task<List<QuerySql.GetCustomerOrdersRow>> Sqlc_GetCustomerOrders()
    {
        var results = new List<QuerySql.GetCustomerOrdersRow>();
        // Run multiple queries to ensure measurable execution time
        for (int i = 0; i < 20; i++)
        {
            var result = await _sqlcImpl.GetCustomerOrdersAsync(new PostgresSqlcImpl.QuerySql.GetCustomerOrdersArgs(
                CustomerId: _testCustomerId,
                Offset: 0,
                Limit: 100
            ));
            results.AddRange(result);
        }
        return results;
    }

    [BenchmarkCategory("Read")]
    [Benchmark(Description = "EFCore - GetCustomerOrders")]
    public async Task<List<PostgresEFCoreImpl.Queries.GetCustomerOrdersRow>> EFCore_GetCustomerOrders()
    {
        var results = new List<PostgresEFCoreImpl.Queries.GetCustomerOrdersRow>();
        // Run multiple queries to ensure measurable execution time
        for (int i = 0; i < 20; i++)
        {
            var result = await _efCoreImpl.GetCustomerOrders(new PostgresEFCoreImpl.Queries.GetCustomerOrdersArgs(
                CustomerId: _testCustomerId,
                Offset: 0,
                Limit: 100
            ));
            results.AddRange(result);
        }
        return results;
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        await _dbContext.DisposeAsync();
        var connectionString = Config.GetConnectionString();
        await DatabaseHelper.CleanupDatabaseAsync(connectionString);
    }
}

