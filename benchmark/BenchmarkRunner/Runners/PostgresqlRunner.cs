using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;

public sealed class PostgresqlRunner : BaseRunner
{
    public override string GetOutputBasePath() => Path.Combine(base.GetOutputBasePath(), "postgresql");

    public override async Task RunReadsAsync()
    {
        await PostgresqlReadBenchmark.GetSeedMethod()();
        var path = Path.Combine(GetOutputBasePath(), "reads");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlReadBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
    }

    public override async Task RunWritesAsync()
    {
        await PostgresqlWriteBenchmark.GetSeedMethod()();
        var path = Path.Combine(GetOutputBasePath(), "writes");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlWriteBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
    }
}