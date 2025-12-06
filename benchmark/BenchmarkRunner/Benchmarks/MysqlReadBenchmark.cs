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
public class MysqlReadBenchmark
{
    private QuerySql _sqlcImpl = null!;
    private Queries _efCoreImplNoTracking = null!;
    private Queries _efCoreImplWithTracking = null!;
    private SalesDbContext _dbContextNoTracking = null!;
    private SalesDbContext _dbContextWithTracking = null!;
    private int _testCustomerId = 1;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        var connectionString = Config.GetMysqlConnectionString();
        await MysqlDatabaseHelper.CleanupDatabaseAsync(connectionString);

        _sqlcImpl = new QuerySql(connectionString);
        _dbContextNoTracking = new SalesDbContext(connectionString);
        _dbContextWithTracking = new SalesDbContext(connectionString);
        _efCoreImplNoTracking = new Queries(_dbContextNoTracking, useTracking: false);
        _efCoreImplWithTracking = new Queries(_dbContextWithTracking, useTracking: true);

        var seeder = new MysqlDatabaseSeeder(connectionString);
       await seeder.SeedAsync(
            customerCount: 500, 
            productsPerCategory: 150, 
            ordersPerCustomer: 100, 
            itemsPerOrder: 5
        );

        var firstCustomer = await _efCoreImplNoTracking.DbContext.Set<MysqlEFCoreImpl.Customer>().FirstAsync();
        _testCustomerId = firstCustomer.CustomerId;
    }

    [BenchmarkCategory("Read")]
    [Benchmark(Baseline = true, Description = "SQLC - GetCustomerOrders")]
    public async Task<List<QuerySql.GetCustomerOrdersRow>> Sqlc_GetCustomerOrders()
    {
        var results = new List<QuerySql.GetCustomerOrdersRow>();
        // Run fewer iterations since each query will process more data (target: 3+ seconds per query)
        for (int i = 0; i < 5; i++)
        {
            var result = await _sqlcImpl.GetCustomerOrdersAsync(new QuerySql.GetCustomerOrdersArgs(
                CustomerId: _testCustomerId,
                Offset: 0,
                Limit: 5000  // Increased from 100 to return more rows per query
            ));
            results.AddRange(result);
        }
        return results;
    }

    [BenchmarkCategory("Read")]
    [Benchmark(Description = "EFCore (NoTracking) - GetCustomerOrders")]
    public async Task<List<Queries.GetCustomerOrdersRow>> EFCore_NoTracking_GetCustomerOrders()
    {
        var results = new List<Queries.GetCustomerOrdersRow>();
        // Run fewer iterations since each query will process more data (target: 3+ seconds per query)
        for (int i = 0; i < 5; i++)
        {
            var result = await _efCoreImplNoTracking.GetCustomerOrders(new Queries.GetCustomerOrdersArgs(
                CustomerId: _testCustomerId,
                Offset: 0,
                Limit: 5000  // Increased from 100 to return more rows per query
            ));
            results.AddRange(result);
        }
        return results;
    }

    [BenchmarkCategory("Read")]
    [Benchmark(Description = "EFCore (WithTracking) - GetCustomerOrders")]
    public async Task<List<Queries.GetCustomerOrdersRow>> EFCore_WithTracking_GetCustomerOrders()
    {
        var results = new List<Queries.GetCustomerOrdersRow>();
        // Run fewer iterations since each query will process more data (target: 3+ seconds per query)
        for (int i = 0; i < 5; i++)
        {
            var result = await _efCoreImplWithTracking.GetCustomerOrders(new Queries.GetCustomerOrdersArgs(
                CustomerId: _testCustomerId,
                Offset: 0,
                Limit: 5000  // Increased from 100 to return more rows per query
            ));
            results.AddRange(result);
        }
        return results;
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        await _dbContextNoTracking.DisposeAsync();
        await _dbContextWithTracking.DisposeAsync();
        var connectionString = Config.GetMysqlConnectionString();
        await MysqlDatabaseHelper.CleanupDatabaseAsync(connectionString);
    }
}

