using BenchmarkRunner.Benchmarks;
using Microsoft.Extensions.Logging;

public class MysqlRunner(string connectionString, ILogger<MysqlRunner> logger)
{
    private readonly string _connectionString = connectionString;
    private readonly ILogger<MysqlRunner> _logger = logger;

    public string ConnectionString => _connectionString;

    public Task RunAsync()
    {
        _logger.LogInformation("Running MySQL Reads benchmarks...");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlReadBenchmark>();
        _logger.LogInformation("Running MySQL Writes benchmarks...");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlWriteBenchmark>();
        return Task.CompletedTask;
    }
}