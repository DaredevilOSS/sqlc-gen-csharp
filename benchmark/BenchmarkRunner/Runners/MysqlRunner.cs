using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;

public sealed class MysqlRunner : BaseRunner
{
    public override string GetOutputBasePath() => Path.Combine(base.GetOutputBasePath(), "mysql");

    public override async Task RunReadsAsync()
    {
        await MysqlReadBenchmark.GetSeedMethod()();
        var path = Path.Combine(GetOutputBasePath(), "reads");
        var summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlReadBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );

        if (summary.HasCriticalValidationErrors)
            throw new InvalidProgramException("Mysql reads benchmark failed");
    }

    public override async Task RunWritesAsync()
    {
        await MysqlWriteBenchmark.GetSeedMethod()();
        var path = Path.Combine(GetOutputBasePath(), "writes");
        var summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlWriteBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );

        if (summary.HasCriticalValidationErrors)
            throw new InvalidProgramException("Mysql writes benchmark failed");
    }
}