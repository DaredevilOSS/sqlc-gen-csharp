using System.Diagnostics;
using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;
using BenchmarkRunner.Utils;
using Microsoft.Extensions.Logging;

public class PostgresqlRunner(string connectionString, ILogger<PostgresqlRunner> logger)
{
    private readonly string _connectionString = connectionString;
    private readonly ILogger<PostgresqlRunner> _logger = logger;
    public string ConnectionString => _connectionString;

    public Task RunAsync()
    {
        var path = Path.Combine("benchmark", "BenchmarkDotNet.Artifacts", "results", "postgresql");
        var config = DefaultConfig.Instance.WithArtifactsPath(path);

        _logger.LogInformation("Running PostgreSQL Reads benchmarks...");
        var stopwatch = Stopwatch.StartNew();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlReadBenchmark>(config);
        stopwatch.Stop();
        var readTime = stopwatch.Elapsed;

        _logger.LogInformation("Running PostgreSQL Writes benchmarks...");
        stopwatch.Restart();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlWriteBenchmark>(config);
        stopwatch.Stop();
        var writeTime = stopwatch.Elapsed;

        _logger.LogInformation("PostgreSQL Reads benchmarks completed in {ElapsedTime}", Helpers.FormatElapsedTime(readTime));
        _logger.LogInformation("PostgreSQL Writes benchmarks completed in {ElapsedTime}", Helpers.FormatElapsedTime(writeTime));
        return Task.CompletedTask;
    }
}