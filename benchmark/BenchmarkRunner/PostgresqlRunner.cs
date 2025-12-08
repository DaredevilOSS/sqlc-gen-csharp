using BenchmarkRunner.Benchmarks;
using Microsoft.Extensions.Logging;

public class PostgresqlRunner(string connectionString, ILogger<PostgresqlRunner> logger)
{
    private readonly string _connectionString = connectionString;
    private readonly ILogger<PostgresqlRunner> _logger = logger;

    public string ConnectionString => _connectionString;

    public Task RunAsync()
    {
        _logger.LogInformation("Running PostgreSQL Reads benchmarks...");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlReadBenchmark>();
        _logger.LogInformation("Running PostgreSQL Writes benchmarks...");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlWriteBenchmark>();
        return Task.CompletedTask;
    }
}