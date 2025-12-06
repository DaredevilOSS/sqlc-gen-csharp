using MySqlConnector;

namespace BenchmarkRunner.Utils;

public static partial class MysqlDatabaseHelper
{
    public static async Task CleanupDatabaseAsync(string connectionString)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        // Disable foreign key checks to allow TRUNCATE on all tables
        using (var disableFkCmd = new MySqlCommand("SET FOREIGN_KEY_CHECKS = 0", connection))
            await disableFkCmd.ExecuteNonQueryAsync();

        try
        {
            // Now we can TRUNCATE all tables in any order
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
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (MySqlException)
                {
                    // Ignore errors if tables don't exist yet
                }
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