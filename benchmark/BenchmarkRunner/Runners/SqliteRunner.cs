using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;

public sealed class SqliteRunner : BaseBenchmarkRunner
{
    public override string GetBasePath() => Path.Combine(base.GetBasePath(), "sqlite");

    public override Task RunReadsAsync()
    {
        var path = Path.Combine(GetBasePath(), "reads");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteReadBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
        return Task.CompletedTask;
    }

    public override Task RunWritesAsync()
    {
        var path = Path.Combine(GetBasePath(), "writes");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteWriteBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
        return Task.CompletedTask;
    }
}