public abstract class BaseRunner
{
    public virtual string GetOutputBasePath() => Path.Combine("BenchmarkDotNet.Artifacts");
    public abstract Task RunReadsAsync();
    public abstract Task RunWritesAsync();
}