using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;
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
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlReadBenchmark>(config);
        _logger.LogInformation("Running PostgreSQL Writes benchmarks...");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlWriteBenchmark>(config);
        return Task.CompletedTask;
    }
}