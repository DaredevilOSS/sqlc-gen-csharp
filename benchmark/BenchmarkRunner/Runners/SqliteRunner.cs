using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;

public sealed class SqliteRunner : BaseRunner
{
    public override string GetBasePath() => Path.Combine(base.GetBasePath(), "sqlite");

    public override async Task RunReadsAsync()
    {
        await SqliteReadBenchmark.GetSeedMethod().Invoke();
        var path = Path.Combine(GetBasePath(), "reads");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteReadBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
    }

    public override async Task RunWritesAsync()
    {
        await SqliteWriteBenchmark.GetSeedMethod().Invoke();
        var path = Path.Combine(GetBasePath(), "writes");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteWriteBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
    }
}