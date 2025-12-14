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
    public static async Task CleanupDatabaseAsync(string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var cleanupCommands = new[]
        {
            "TRUNCATE TABLE sales.orders CASCADE",
            "TRUNCATE TABLE sales.products CASCADE",
            "TRUNCATE TABLE sales.customers CASCADE"
        };

        foreach (var command in cleanupCommands)
        {
            using var cmd = new NpgsqlCommand(command, connection);
            await cmd.ExecuteNonQueryAsync();
        }
        await CleanupWriteTableAsync(connectionString);
    }
}