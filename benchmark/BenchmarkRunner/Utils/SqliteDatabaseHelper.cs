using Microsoft.Data.Sqlite;
using System.IO;

namespace BenchmarkRunner.Utils;

public static partial class SqliteDatabaseHelper
{
    public static async Task InitializeDatabaseAsync(string connectionString)
    {
        // For in-memory databases, we always need to create the schema
        // For file-based databases, delete the file to ensure a clean start
        var dbFilePath = GetDbFilePath(connectionString);
        if (!string.IsNullOrEmpty(dbFilePath) && File.Exists(dbFilePath))
        {
            File.Delete(dbFilePath);
        }

        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        var schemaSql = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "schema.sql"));
        
        // Modify CREATE TABLE statements to use IF NOT EXISTS to avoid errors
        // if tables were already created by EFCore
        var modifiedSchema = schemaSql.Replace("CREATE TABLE", "CREATE TABLE IF NOT EXISTS");
        
        using var command = new SqliteCommand(modifiedSchema, connection);
        try
        {
            await command.ExecuteNonQueryAsync();
        }
        catch (SqliteException)
        {
            // Ignore errors - tables might already exist (created by EFCore)
        }
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
            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqliteException)
            {
                // Ignore errors if tables don't exist yet
            }
        }
    }

    private static string? GetDbFilePath(string connectionString)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString);
        return builder.DataSource == ":memory:" ? null : builder.DataSource;
    }
}

