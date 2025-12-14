using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;
using BenchmarkRunner.Utils;

public class PostgresqlRunner : IBenchmarkRunner
{
    public static string GetBasePath() => Path.Combine(Helpers.GetBasePath(), "postgresql");

    public Task RunReadsAsync()
    {
        var path = Path.Combine(GetBasePath(), "reads");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlReadBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
        return Task.CompletedTask;
    }

    public Task RunWritesAsync()
    {
        var path = Path.Combine(GetBasePath(), "writes");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlWriteBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
        return Task.CompletedTask;
    }
}