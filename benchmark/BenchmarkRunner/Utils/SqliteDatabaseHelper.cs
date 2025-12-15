using Microsoft.Data.Sqlite;

namespace BenchmarkRunner.Utils;

public static partial class SqliteDatabaseHelper
{
    public static async Task CleanupWriteTableAsync(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();
        using var cmd = new SqliteCommand("DELETE FROM order_items", connection);
        await cmd.ExecuteNonQueryAsync();
    }
}