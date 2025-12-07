using MySqlConnector;

namespace BenchmarkRunner.Utils;

public static partial class MysqlDatabaseHelper
{
    public static async Task CleanupDatabaseAsync(string connectionString)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        using (var disableFkCmd = new MySqlCommand("SET FOREIGN_KEY_CHECKS = 0", connection))
            await disableFkCmd.ExecuteNonQueryAsync();

        try
        {
            var cleanupCommands = new[]
            {
                "TRUNCATE TABLE sales.order_items",
                "TRUNCATE TABLE sales.orders",
                "TRUNCATE TABLE sales.products",
                "TRUNCATE TABLE sales.customers"
            };

            foreach (var command in cleanupCommands)
            {
                using var cmd = new MySqlCommand(command, connection);
                await cmd.ExecuteNonQueryAsync();
            }
        }
        finally
        {
            // Re-enable foreign key checks
            using var enableFkCmd = new MySqlCommand("SET FOREIGN_KEY_CHECKS = 1", connection);
            await enableFkCmd.ExecuteNonQueryAsync();
        }
    }
}