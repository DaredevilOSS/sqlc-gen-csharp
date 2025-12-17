
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkRunner.Utils;
using MySqlConnector;
using MysqlEFCoreImpl;
using MysqlSqlcImpl;
using NUnit.Framework;

namespace BenchmarkRunner.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80, warmupCount: 2, iterationCount: 10)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class MysqlWriteBenchmark : BaseWriteBenchmark
{
    private static readonly string _connectionString = Config.GetMysqlConnectionString();
    private readonly QuerySql _sqlcImpl = new(_connectionString);
    private readonly Queries _efCoreImpl = new(new SalesDbContext(_connectionString), useTracking: false);

    private List<QuerySql.AddOrderItemsArgs> _sqlcTestOrderItems = null!;
    private List<Queries.AddOrderItemsArgs> _efCoreTestOrderItems = null!;

    public static IEnumerable<WriteBenchmarkArgs> GetSqlcArguments()
    {
        yield return new WriteBenchmarkArgs(TotalRecordsToLoad: TotalRecordsForSetup, BatchSize: 1000);
        yield return new WriteBenchmarkArgs(TotalRecordsToLoad: TotalRecordsForSetup, BatchSize: 5000);
    }

    [BenchmarkCategory("Write")]
    [Benchmark(Baseline = true, Description = "SQLC - AddOrderItems")]
    [ArgumentsSource(nameof(GetSqlcArguments))]
    public override async Task Sqlc_AddOrderItems(WriteBenchmarkArgs args)
    {
        await Helpers.InsertInBatchesAsync(_sqlcTestOrderItems[..args.TotalRecordsToLoad], args.BatchSize, _sqlcImpl.AddOrderItemsAsync);
        var result = await _sqlcImpl.GetOrderItemsCountAsync();
        Assert.That(result?.Cnt, Is.EqualTo(args.TotalRecordsToLoad));
    }

    public static IEnumerable<WriteBenchmarkArgs> GetEFCoreArguments()
    {
        yield return new WriteBenchmarkArgs(TotalRecordsToLoad: TotalRecordsForSetup, BatchSize: 500);
    }

    [BenchmarkCategory("Write")]
    [Benchmark(Description = "EFCore - AddOrderItems")]
    [ArgumentsSource(nameof(GetEFCoreArguments))]
    public override async Task EFCore_AddOrderItems(WriteBenchmarkArgs args)
    {
        await Helpers.InsertInBatchesAsync(_efCoreTestOrderItems[..args.TotalRecordsToLoad], args.BatchSize, _efCoreImpl.AddOrderItems);
        var result = await _sqlcImpl.GetOrderItemsCountAsync();
        Assert.That(result?.Cnt, Is.EqualTo(args.TotalRecordsToLoad));
    }

    public static Func<Task> GetSeedMethod()
    {
        return async () =>
        {
            var seeder = new MysqlSeeder(_connectionString);
            await seeder.SeedAsync(GetSeedConfig());
        };
    }

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        var orderIds = await _sqlcImpl.GetOrderIdsAsync(new QuerySql.GetOrderIdsArgs(Limit: OrderIdsCountForSetup));
        var productIds = await _sqlcImpl.GetProductIdsAsync(new QuerySql.GetProductIdsArgs(Limit: ProductIdsCountForSetup));
        _sqlcTestOrderItems = GetSqlcTestOrderItemsAsync(orderIds, productIds);
        _efCoreTestOrderItems = GetEFCoreTestOrderItems(orderIds, productIds);
    }

    [IterationSetup]
    public static void IterationSetup()
    {
        CleanupWriteTableAsync(_connectionString).GetAwaiter().GetResult();
        Helpers.InvokeGarbageCollection();
    }

    private static List<QuerySql.AddOrderItemsArgs> GetSqlcTestOrderItemsAsync(
        List<QuerySql.GetOrderIdsRow> orderIds,
        List<QuerySql.GetProductIdsRow> productIds)
    {
        return [.. Enumerable.Range(0, TotalRecordsForSetup).Select(i => new QuerySql.AddOrderItemsArgs(
            OrderId: orderIds[i % OrderIdsCountForSetup].OrderId,
            ProductId: productIds[i % ProductIdsCountForSetup].ProductId,
            Quantity: Random.Shared.Next(1, 10),
            UnitPrice: (decimal)(Random.Shared.NextDouble() * 100 + 5)
        ))];
    }

    private static List<Queries.AddOrderItemsArgs> GetEFCoreTestOrderItems(
        List<QuerySql.GetOrderIdsRow> orderIds,
        List<QuerySql.GetProductIdsRow> productIds)
    {
        return [.. Enumerable.Range(0, TotalRecordsForSetup).Select(i => new Queries.AddOrderItemsArgs(
            OrderId: orderIds[i % OrderIdsCountForSetup].OrderId,
            ProductId: productIds[i % ProductIdsCountForSetup].ProductId,
            Quantity: Random.Shared.Next(1, 10),
            UnitPrice: (decimal)(Random.Shared.NextDouble() * 100 + 5)
        ))];
    }

    private static async Task CleanupWriteTableAsync(string connectionString)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        using var cmd = new MySqlCommand("TRUNCATE TABLE sales.order_items", connection);
        await cmd.ExecuteNonQueryAsync();
    }
}