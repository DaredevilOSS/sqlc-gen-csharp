using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;

public sealed partial class SqliteRunner : BaseRunner
{
    public override string GetOutputBasePath() => Path.Combine(base.GetOutputBasePath(), "sqlite");

    public override async Task RunReadsAsync()
    {
        await SqliteReadBenchmark.GetSeedMethod()();
        var path = Path.Combine(GetOutputBasePath(), "reads");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteReadBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
    }

    public override async Task RunWritesAsync()
    {
        await SqliteWriteBenchmark.GetSeedMethod()();
        var path = Path.Combine(GetOutputBasePath(), "writes");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteWriteBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
    }
}