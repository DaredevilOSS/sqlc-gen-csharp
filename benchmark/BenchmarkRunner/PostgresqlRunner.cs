using BenchmarkRunner.Benchmarks;
using Microsoft.Extensions.Logging;

public class PostgresqlRunner(string connectionString, ILogger<PostgresqlRunner> logger)
{
    private readonly string _connectionString = connectionString;
    private readonly ILogger<PostgresqlRunner> _logger = logger;

    public string ConnectionString => _connectionString;

    public Task RunAsync()
    {
        _logger.LogInformation("Initializing database...");

        _logger.LogInformation("Running benchmarks...");
        _logger.LogInformation(new string('=', 80));

        // Run read benchmarks
        _logger.LogInformation("\n📖 Read Benchmarks (GetCustomerOrders)");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlReadBenchmark>();

        // Run write benchmarks
        _logger.LogInformation("\n📝 Write Benchmarks (Bulk Inserts)");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlWriteBenchmark>();

        _logger.LogInformation("\n✅ Benchmarks completed!");
        return Task.CompletedTask;
    }
}