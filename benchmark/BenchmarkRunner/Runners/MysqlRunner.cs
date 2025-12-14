using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;

public sealed class MysqlRunner : BaseRunner
{
    public override string GetBasePath() => Path.Combine(base.GetBasePath(), "mysql");

    public override async Task RunReadsAsync()
    {
        await MysqlReadBenchmark.GetSeedMethod().Invoke();
        var path = Path.Combine(GetBasePath(), "reads");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlReadBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
    }

    public override async Task RunWritesAsync()
    {
        await MysqlWriteBenchmark.GetSeedMethod().Invoke();
        var path = Path.Combine(GetBasePath(), "writes");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<MysqlWriteBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
    }
}