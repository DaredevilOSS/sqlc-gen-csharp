using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;
using BenchmarkRunner.Utils;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

public class PostgresqlRunner(string connectionString, ILogger<PostgresqlRunner> logger)
{
    private readonly string _connectionString = connectionString;
    private readonly ILogger<PostgresqlRunner> _logger = logger;
    public string ConnectionString => _connectionString;
    private readonly Stopwatch _stopwatch = new();

    public Task RunReadsAsync()
    {
        var path = Path.Combine("benchmark", "BenchmarkDotNet.Artifacts", "postgresql", "reads");
        var config = DefaultConfig.Instance.WithArtifactsPath(path);

        _logger.LogInformation("Running PostgreSQL Reads benchmarks...");
        _stopwatch.Restart();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlReadBenchmark>(config);
        _stopwatch.Stop();
        var readTime = _stopwatch.Elapsed;

        _logger.LogInformation("PostgreSQL Reads benchmarks completed in {ElapsedTime}", Helpers.FormatElapsedTime(readTime));
        return Task.CompletedTask;
    }

    public Task RunWritesAsync()
    {
        var path = Path.Combine("benchmark", "BenchmarkDotNet.Artifacts", "postgresql", "writes");
        var config = DefaultConfig.Instance.WithArtifactsPath(path);

        _logger.LogInformation("Running PostgreSQL Writes benchmarks...");
        _stopwatch.Restart();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlWriteBenchmark>(config);
        _stopwatch.Stop();
        var writeTime = _stopwatch.Elapsed;

        _logger.LogInformation("PostgreSQL Writes benchmarks completed in {ElapsedTime}", Helpers.FormatElapsedTime(writeTime));
        return Task.CompletedTask;
    }
}