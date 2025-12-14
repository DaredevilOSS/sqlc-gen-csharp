public interface IBenchmarkRunner
{
    Task RunReadsAsync();
    Task RunWritesAsync();
}