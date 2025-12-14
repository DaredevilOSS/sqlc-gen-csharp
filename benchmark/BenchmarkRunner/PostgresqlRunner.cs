using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;

public class PostgresqlRunner
{
    public static Task RunReadsAsync()
    {
        var path = Path.Combine("benchmark", "BenchmarkDotNet.Artifacts", "postgresql", "reads");
        var config = DefaultConfig.Instance.WithArtifactsPath(path);
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlReadBenchmark>(config);
        return Task.CompletedTask;
    }

    public static Task RunWritesAsync()
    {
        var path = Path.Combine("benchmark", "BenchmarkDotNet.Artifacts", "postgresql", "writes");
        var config = DefaultConfig.Instance.WithArtifactsPath(path);
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlWriteBenchmark>(config);
        return Task.CompletedTask;
    }
}