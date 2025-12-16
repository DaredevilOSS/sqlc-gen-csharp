using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkRunner.Utils;
using Npgsql;
using PostgresEFCoreImpl;
using PostgresSqlcImpl;

namespace BenchmarkRunner.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80, warmupCount: 2, iterationCount: 10)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class PostgresqlWriteBenchmark : BaseWriteBenchmark
{
    private static readonly string _connectionString = Config.GetPostgresConnectionString();
    private readonly QuerySql _sqlcImpl = new(_connectionString);
    private readonly Queries _efCoreImpl = new(new SalesDbContext(_connectionString), useTracking: false);
    private List<QuerySql.AddOrderItemsArgs> _testOrderItems = null!;

    public static IEnumerable<WriteBenchmarkArgs> GetSqlcArguments()
    {
        yield return new WriteBenchmarkArgs(TotalRecords: 200_000, BatchSize: 1_000);
        yield return new WriteBenchmarkArgs(TotalRecords: 200_000, BatchSize: 2_000);
    }

    [BenchmarkCategory("Write")]
    [Benchmark(Baseline = true, Description = "SQLC - AddOrderItems")]
    [ArgumentsSource(nameof(GetSqlcArguments))]
    public override async Task Sqlc_AddOrderItems(WriteBenchmarkArgs args)
    {
        await Helpers.InsertInBatchesAsync(_testOrderItems, args.BatchSize, _sqlcImpl.AddOrderItemsAsync);
    }

    public static IEnumerable<WriteBenchmarkArgs> GetEFCoreArguments()
    {
        yield return new WriteBenchmarkArgs(TotalRecords: 200_000, BatchSize: 500);
    }

    [BenchmarkCategory("Write")]
    [Benchmark(Description = "EFCore - AddOrderItems")]
    [ArgumentsSource(nameof(GetEFCoreArguments))]
    public override async Task EFCore_AddOrderItems(WriteBenchmarkArgs args)
    {
        var batchArgs = _testOrderItems.Select(i => new Queries.AddOrderItemsArgs(
            i.OrderId, i.ProductId, i.Quantity, i.UnitPrice
        )).ToList();
        await Helpers.InsertInBatchesAsync(batchArgs, args.BatchSize, _efCoreImpl.AddOrderItems);
    }

    public static Func<Task> GetSeedMethod()
    {
        return async () =>
        {
            var seeder = new PostgresqlSeeder(_connectionString);
            await seeder.SeedAsync(GetSeedConfig());
        };
    }

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        var orderIds = await _sqlcImpl.GetOrderIdsAsync(new QuerySql.GetOrderIdsArgs(Limit: 1000));
        var productIds = await _sqlcImpl.GetProductIdsAsync(new QuerySql.GetProductIdsArgs(Limit: 1000));

        _testOrderItems = [.. Enumerable.Range(0, 5000).Select(i => new QuerySql.AddOrderItemsArgs(
            OrderId: orderIds[i % orderIds.Count].OrderId,
            ProductId: productIds[i % productIds.Count].ProductId,
            Quantity: Random.Shared.Next(1, 10),
            UnitPrice: (decimal)(Random.Shared.NextDouble() * 100 + 5)
        ))];
    }

    [IterationSetup]
    public static void IterationSetup()
    {
        CleanupWriteTableAsync(_connectionString).GetAwaiter().GetResult();
        Helpers.InvokeGarbageCollection();
    }

    private static async Task CleanupWriteTableAsync(string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        using var cmd = new NpgsqlCommand("TRUNCATE TABLE sales.order_items", connection);
        await cmd.ExecuteNonQueryAsync();
    }
}