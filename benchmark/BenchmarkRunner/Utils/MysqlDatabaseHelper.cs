using MySqlConnector;

namespace BenchmarkRunner.Utils;

public static partial class MysqlDatabaseHelper
{
    public static async Task CleanupWriteTableAsync(string connectionString)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        using var cmd = new MySqlCommand("TRUNCATE TABLE sales.order_items", connection);
        await cmd.ExecuteNonQueryAsync();
    }
}