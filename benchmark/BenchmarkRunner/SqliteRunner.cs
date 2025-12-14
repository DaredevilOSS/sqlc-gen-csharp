using BenchmarkDotNet.Configs;
using BenchmarkRunner.Benchmarks;
using BenchmarkRunner.Utils;

public class SqliteRunner : IBenchmarkRunner
{
    public static string GetBasePath() => Path.Combine(Helpers.GetBasePath(), "sqlite");

    public Task RunReadsAsync()
    {
        var path = Path.Combine(GetBasePath(), "reads");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteReadBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
        return Task.CompletedTask;
    }

    public Task RunWritesAsync()
    {
        var path = Path.Combine(GetBasePath(), "writes");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<SqliteWriteBenchmark>(
            DefaultConfig.Instance.WithArtifactsPath(path)
        );
        return Task.CompletedTask;
    }
}