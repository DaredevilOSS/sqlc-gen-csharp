using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;
using BenchmarkRunner.Utils;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

public class SqliteRunner(string connectionString, ILogger<SqliteRunner> logger)
{
    private readonly string _connectionString = connectionString;
    private readonly ILogger<SqliteRunner> _logger = logger;
    public string ConnectionString => _connectionString;
    private readonly Stopwatch _stopwatch = new();

    public Task RunReadsAsync()
    {
        var path = Path.Combine("benchmark", "BenchmarkDotNet.Artifacts", "sqlite", "reads");
        var config = DefaultConfig.Instance.WithArtifactsPath(path);

        _logger.LogInformation("Running SQLite Reads benchmarks...");
        _stopwatch.Restart();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteReadBenchmark>(config);
        _stopwatch.Stop();
        var readTime = _stopwatch.Elapsed;

        _logger.LogInformation("SQLite Reads benchmarks completed in {ElapsedTime}", Helpers.FormatElapsedTime(readTime));
        return Task.CompletedTask;
    }

    public Task RunWritesAsync()
    {
        var path = Path.Combine("benchmark", "BenchmarkDotNet.Artifacts", "sqlite", "writes");
        var config = DefaultConfig.Instance.WithArtifactsPath(path);

        _logger.LogInformation("Running SQLite Writes benchmarks...");
        _stopwatch.Restart();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteWriteBenchmark>(config);
        _stopwatch.Stop();
        var writeTime = _stopwatch.Elapsed; ;

        _logger.LogInformation("SQLite Writes benchmarks completed in {ElapsedTime}", Helpers.FormatElapsedTime(writeTime));
        return Task.CompletedTask;
    }
}