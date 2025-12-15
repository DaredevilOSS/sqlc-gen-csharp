using Npgsql;

namespace BenchmarkRunner.Utils;

public static partial class PostgresqlDatabaseHelper
{
    public static async Task CleanupWriteTableAsync(string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        using var cmd = new NpgsqlCommand("TRUNCATE TABLE sales.order_items", connection);
        await cmd.ExecuteNonQueryAsync();
    }
}