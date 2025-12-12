using System.Diagnostics;
using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;
using BenchmarkRunner.Utils;
using Microsoft.Extensions.Logging;

public class MysqlRunner(string connectionString, ILogger<MysqlRunner> logger)
{
    private readonly string _connectionString = connectionString;
    private readonly ILogger<MysqlRunner> _logger = logger;
    public string ConnectionString => _connectionString;

    public Task RunAsync()
    {
        var path = Path.Combine("benchmark", "BenchmarkDotNet.Artifacts", "results", "mysql");
        var config = DefaultConfig.Instance.WithArtifactsPath(path);

        _logger.LogInformation("Running MySQL Reads benchmarks...");
        var stopwatch = Stopwatch.StartNew();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlReadBenchmark>(config);
        stopwatch.Stop();
        var readTime = stopwatch.Elapsed;

        _logger.LogInformation("Running MySQL Writes benchmarks...");
        stopwatch.Restart();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlWriteBenchmark>(config);
        stopwatch.Stop();
        var writeTime = stopwatch.Elapsed;

        _logger.LogInformation("MySQL Reads benchmarks completed in {ElapsedTime}", Helpers.FormatElapsedTime(readTime));
        _logger.LogInformation("MySQL Writes benchmarks completed in {ElapsedTime}", Helpers.FormatElapsedTime(writeTime));
        return Task.CompletedTask;
    }
}