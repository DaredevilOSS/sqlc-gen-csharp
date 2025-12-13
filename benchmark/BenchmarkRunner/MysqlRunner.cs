using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;
using BenchmarkRunner.Utils;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

public class MysqlRunner(string connectionString, ILogger<MysqlRunner> logger)
{
    private readonly string _connectionString = connectionString;
    private readonly ILogger<MysqlRunner> _logger = logger;
    public string ConnectionString => _connectionString;
    private readonly Stopwatch _stopwatch = new();

    public Task RunReadsAsync()
    {
        var path = Path.Combine("benchmark", "BenchmarkDotNet.Artifacts", "results", "mysql", "reads");
        var config = DefaultConfig.Instance.WithArtifactsPath(path);

        _logger.LogInformation("Running MySQL Reads benchmarks...");
        _stopwatch.Restart();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlReadBenchmark>(config);
        _stopwatch.Stop();
        var readTime = _stopwatch.Elapsed;

        _logger.LogInformation("MySQL Reads benchmarks completed in {ElapsedTime}", Helpers.FormatElapsedTime(readTime));
        return Task.CompletedTask;
    }

    public Task RunWritesAsync()
    {
        var path = Path.Combine("benchmark", "BenchmarkDotNet.Artifacts", "results", "mysql", "writes");
        var config = DefaultConfig.Instance.WithArtifactsPath(path);

        _logger.LogInformation("Running MySQL Writes benchmarks...");
        _stopwatch.Restart();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlWriteBenchmark>(config);
        _stopwatch.Stop();
        var writeTime = _stopwatch.Elapsed;

        _logger.LogInformation("MySQL Writes benchmarks completed in {ElapsedTime}", Helpers.FormatElapsedTime(writeTime));
        return Task.CompletedTask;
    }
}