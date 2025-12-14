public abstract class BaseRunner
{
    public virtual string GetBasePath() => Path.Combine("benchmark", "BenchmarkDotNet.Artifacts");
    public abstract Task RunReadsAsync();
    public abstract Task RunWritesAsync();
}