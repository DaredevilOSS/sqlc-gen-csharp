using System.Diagnostics;
using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;
using BenchmarkRunner.Utils;
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
        var stopwatch = Stopwatch.StartNew();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteReadBenchmark>(config);
        stopwatch.Stop();
        var readTime = stopwatch.Elapsed;

        _logger.LogInformation("Running SQLite Writes benchmarks...");
        stopwatch.Restart();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteWriteBenchmark>(config);
        stopwatch.Stop();
        var writeTime = stopwatch.Elapsed;;

        _logger.LogInformation("SQLite Reads benchmarks completed in {ElapsedTime}", Helpers.FormatElapsedTime(readTime));
        _logger.LogInformation("SQLite Writes benchmarks completed in {ElapsedTime}", Helpers.FormatElapsedTime(writeTime))
        return Task.CompletedTask;
    }
}