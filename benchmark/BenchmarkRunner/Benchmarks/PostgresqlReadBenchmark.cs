using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkRunner.Utils;
using NUnit.Framework;
using PostgresEFCoreImpl;
using PostgresSqlcImpl;

namespace BenchmarkRunner.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80, warmupCount: 2, iterationCount: 10)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class PostgresqlReadBenchmark : BaseReadBenchmark
{
    private static readonly string _connectionString = Config.GetPostgresConnectionString();
    private readonly QuerySql _sqlcImpl = new(_connectionString);

    public static IEnumerable<ReadBenchmarkParams> GetParams()
    {
        yield return new ReadBenchmarkParams(Limit: 50, Concurrency: 100, QueriesToSubmit: 2_000);
        yield return new ReadBenchmarkParams(Limit: 1000, Concurrency: 50, QueriesToSubmit: 1_000);
    }

    [ParamsSource(nameof(GetParams))]
    public ReadBenchmarkParams Params { get; set; }

    [BenchmarkCategory("Read")]
    [Benchmark(Baseline = true, Description = "SQLC - GetCustomerOrders")]
    public override async Task Sqlc_GetCustomerOrders()
    {
        await ExecuteConcurrentlyAsync(Params.QueriesToSubmit, Params.Concurrency, async _ =>
        {
            var results = await _sqlcImpl.GetCustomerOrdersAsync(new QuerySql.GetCustomerOrdersArgs(
                CustomerId: Random.Shared.Next(1, GetSeedConfig().CustomerCount),
                Offset: 0,
                Limit: Params.Limit
            ));

            Assert.That(results.Count, Is.EqualTo(Params.Limit));
            return results;
        });
    }

    [BenchmarkCategory("Read")]
    [Benchmark(Description = "EFCore (NoTracking) - GetCustomerOrders")]
    public override async Task EFCore_NoTracking_GetCustomerOrders()
    {
        await ExecuteConcurrentlyAsync(Params.QueriesToSubmit, Params.Concurrency, async _ =>
        {
            await using var dbContext = new SalesDbContext(_connectionString);
            var queries = new Queries(dbContext, useTracking: false);
            var results = await queries.GetCustomerOrders(new Queries.GetCustomerOrdersArgs(
                CustomerId: Random.Shared.Next(1, GetSeedConfig().CustomerCount),
                Offset: 0,
                Limit: Params.Limit
            ));

            Assert.That(results.Count, Is.EqualTo(Params.Limit));
            return results;
        });
    }

    [BenchmarkCategory("Read")]
    [Benchmark(Description = "EFCore (WithTracking) - GetCustomerOrders")]
    public override async Task EFCore_WithTracking_GetCustomerOrders()
    {
        await ExecuteConcurrentlyAsync(Params.QueriesToSubmit, Params.Concurrency, async _ =>
        {
            await using var dbContext = new SalesDbContext(_connectionString);
            var queries = new Queries(dbContext, useTracking: true);
            var results = await queries.GetCustomerOrders(new Queries.GetCustomerOrdersArgs(
                CustomerId: Random.Shared.Next(1, GetSeedConfig().CustomerCount),
                Offset: 0,
                Limit: Params.Limit
            ));

            Assert.That(results.Count, Is.EqualTo(Params.Limit));
            return results;
        });
    }

    public static Func<Task> GetSeedMethod()
    {
        return async () =>
        {
            var seeder = new PostgresqlSeeder(_connectionString);
            await seeder.SeedAsync(GetSeedConfig());
        };
    }
}