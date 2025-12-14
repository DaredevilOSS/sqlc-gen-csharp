using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;

public class MysqlRunner
{
    public static Task RunReadsAsync()
    {
        var path = Path.Combine("benchmark", "BenchmarkDotNet.Artifacts", "results", "mysql", "reads");
        var config = DefaultConfig.Instance.WithArtifactsPath(path);
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlReadBenchmark>(config);
        return Task.CompletedTask;
    }

    public static Task RunWritesAsync()
    {
        var path = Path.Combine("benchmark", "BenchmarkDotNet.Artifacts", "results", "mysql", "writes");
        var config = DefaultConfig.Instance.WithArtifactsPath(path);
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlWriteBenchmark>(config);
        return Task.CompletedTask;
    }
}