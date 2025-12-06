using Microsoft.Data.Sqlite;
using System.IO;

namespace BenchmarkRunner.Utils;

public static partial class SqliteDatabaseHelper
{
    public static async Task InitializeDatabaseAsync(string connectionString)
    {
        var dbFilePath = GetDbFilePath(connectionString);
        if (!string.IsNullOrEmpty(dbFilePath) && File.Exists(dbFilePath))
            File.Delete(dbFilePath);

        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        var schemaSql = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "schema.sql"));
        using var command = new SqliteCommand(schemaSql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public static async Task CleanupDatabaseAsync(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        var cleanupCommands = new[]
        {
            "DELETE FROM order_items",
            "DELETE FROM orders",
            "DELETE FROM products",
            "DELETE FROM customers"
        };

        foreach (var command in cleanupCommands)
        {
            using var cmd = new SqliteCommand(command, connection);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private static string? GetDbFilePath(string connectionString)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString);
        return builder.DataSource == ":memory:" ? null : builder.DataSource;
    }
}