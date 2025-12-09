using BenchmarkRunner.Benchmarks;
using BenchmarkDotNet.Configs;
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
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlReadBenchmark>(config);
        _logger.LogInformation("Running MySQL Writes benchmarks...");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlWriteBenchmark>(config);
        return Task.CompletedTask;
    }
}