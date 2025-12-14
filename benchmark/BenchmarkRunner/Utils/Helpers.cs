namespace BenchmarkRunner.Utils;

public static class Helpers
{
    public static async Task InsertInBatchesAsync<T>(List<T> items, int batchSize, Func<List<T>, Task> insertBatch)
    {
        var batches = items.Chunk(batchSize);
        foreach (var batch in batches)
            await insertBatch([.. batch]);
    }
}