using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;

public sealed partial class SqliteRunner : BaseRunner
{
    public override string GetOutputBasePath() => Path.Combine(base.GetOutputBasePath(), "sqlite");

    public override async Task RunReadsAsync()
    {
        await SqliteReadBenchmark.GetSeedMethod()();
        var path = Path.Combine(GetOutputBasePath(), "reads");
        var summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteReadBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );

        if (summary.HasCriticalValidationErrors)
            throw new InvalidProgramException("Sqlite reads benchmark failed");
    }

    public override async Task RunWritesAsync()
    {
        await SqliteWriteBenchmark.GetSeedMethod()();
        var path = Path.Combine(GetOutputBasePath(), "writes");
        var summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteWriteBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );

        if (summary.HasCriticalValidationErrors)
            throw new InvalidProgramException("Sqlite writes benchmark failed");
    }
}