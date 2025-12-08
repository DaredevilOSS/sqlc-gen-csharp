using BenchmarkRunner.Benchmarks;
using Microsoft.Extensions.Logging;

public class SqliteRunner(string connectionString, ILogger<SqliteRunner> logger)
{
    private readonly string _connectionString = connectionString;
    private readonly ILogger<SqliteRunner> _logger = logger;

    public string ConnectionString => _connectionString;

    public Task RunAsync()
    {
        _logger.LogInformation("Running SQLite Reads benchmarks...");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteReadBenchmark>();
        _logger.LogInformation("Running SQLite Writes benchmarks...");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteWriteBenchmark>();
        return Task.CompletedTask;
    }
}