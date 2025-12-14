using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;

public sealed class MysqlRunner : BaseBenchmarkRunner
{
    public override string GetBasePath() => Path.Combine(base.GetBasePath(), "mysql");

    public override Task RunReadsAsync()
    {
        var path = Path.Combine(GetBasePath(), "reads");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlReadBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
        return Task.CompletedTask;
    }

    public override Task RunWritesAsync()
    {
        var path = Path.Combine(GetBasePath(), "writes");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlWriteBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
        return Task.CompletedTask;
    }
}