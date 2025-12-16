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

    [Params(2_500_000)]
    public int TotalRecords { get; set; }

    [BenchmarkCategory("Write")]
    [Benchmark(Baseline = true, Description = "SQLC - AddOrderItems")]
    [Arguments(500)]
    [Arguments(1_000)]
    public override async Task Sqlc_AddOrderItems(int batchSize)
    {
        await Helpers.InsertInBatchesAsync(_testOrderItems, batchSize, _sqlcImpl.AddOrderItemsAsync);
    }

    [BenchmarkCategory("Write")]
    [Benchmark(Description = "EFCore - AddOrderItems")]
    [Arguments(500)]
    public override async Task EFCore_AddOrderItems(int batchSize)
    {
        var args = _testOrderItems.Select(i => new Queries.AddOrderItemsArgs(
            i.OrderId, i.ProductId, i.Quantity, i.UnitPrice
        )).ToList();
        await Helpers.InsertInBatchesAsync(args, batchSize, _efCoreImpl.AddOrderItems);
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

        _testOrderItems = [.. Enumerable.Range(0, TotalRecords).Select(i => new QuerySql.AddOrderItemsArgs(
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