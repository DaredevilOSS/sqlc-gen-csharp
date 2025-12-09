using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;
using Microsoft.Extensions.Logging;

public class SqliteRunner(string connectionString, ILogger<SqliteRunner> logger)
{
    private readonly string _connectionString = connectionString;
    private readonly ILogger<SqliteRunner> _logger = logger;

    public string ConnectionString => _connectionString;

    public Task RunAsync()
    {
        var path = Path.Combine("benchmark", "BenchmarkDotNet.Artifacts", "results", "sqlite");
        var config = DefaultConfig.Instance.WithArtifactsPath(path);

        _logger.LogInformation("Running SQLite Reads benchmarks...");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteReadBenchmark>(config);
        _logger.LogInformation("Running SQLite Writes benchmarks...");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteWriteBenchmark>(config);
        return Task.CompletedTask;
    }
}