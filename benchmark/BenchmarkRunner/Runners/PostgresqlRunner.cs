using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;

public sealed class PostgresqlRunner : BaseBenchmarkRunner
{
    public override string GetBasePath() => Path.Combine(base.GetBasePath(), "postgresql");

    public override Task RunReadsAsync()
    {
        var path = Path.Combine(GetBasePath(), "reads");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlReadBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
        return Task.CompletedTask;
    }

    public override Task RunWritesAsync()
    {
        var path = Path.Combine(GetBasePath(), "writes");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlWriteBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
        return Task.CompletedTask;
    }
}