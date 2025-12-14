using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;

public class SqliteRunner
{
    public static Task RunReadsAsync()
    {
        var path = Path.Combine("benchmark", "BenchmarkDotNet.Artifacts", "sqlite", "reads");
        var config = DefaultConfig.Instance.WithArtifactsPath(path);
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteReadBenchmark>(config);
        return Task.CompletedTask;
    }

    public static Task RunWritesAsync()
    {
        var path = Path.Combine("benchmark", "BenchmarkDotNet.Artifacts", "sqlite", "writes");
        var config = DefaultConfig.Instance.WithArtifactsPath(path);
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteWriteBenchmark>(config);
        return Task.CompletedTask;
    }
}