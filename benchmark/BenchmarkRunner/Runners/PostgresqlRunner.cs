using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;

public sealed class PostgresqlRunner : BaseRunner
{
    public override string GetBasePath() => Path.Combine(base.GetBasePath(), "postgresql");

    public override async Task RunReadsAsync()
    {
        await PostgresqlReadBenchmark.GetSeedMethod().Invoke();
        var path = Path.Combine(GetBasePath(), "reads");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlReadBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
    }

    public override async Task RunWritesAsync()
    {
        await PostgresqlWriteBenchmark.GetSeedMethod().Invoke();
        var path = Path.Combine(GetBasePath(), "writes");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<PostgresqlWriteBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
    }
}