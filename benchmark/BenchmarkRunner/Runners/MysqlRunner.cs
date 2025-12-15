using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;

public sealed class MysqlRunner : BaseRunner
{
    public override string GetOutputBasePath() => Path.Combine(base.GetOutputBasePath(), "mysql");

    public override async Task RunReadsAsync()
    {
        await MysqlReadBenchmark.GetSeedMethod()();
        var path = Path.Combine(GetOutputBasePath(), "reads");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlReadBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
    }

    public override async Task RunWritesAsync()
    {
        await MysqlWriteBenchmark.GetSeedMethod()();
        var path = Path.Combine(GetOutputBasePath(), "writes");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlWriteBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
    }
}