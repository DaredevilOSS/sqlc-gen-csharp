using Microsoft.Data.Sqlite;

using System.Text.RegularExpressions;

namespace BenchmarkRunner.Utils;

public static partial class SqliteDatabaseHelper
{
    public static async Task CleanupWriteTableAsync(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();
        using var cmd = new SqliteCommand("DROP TABLE IF EXISTS order_items", connection);
        await cmd.ExecuteNonQueryAsync();
        await CreateSchemaAsync(connectionString);
    }

    public static async Task InitializeDatabaseAsync(string connectionString)
    {
        if (!File.Exists("schema.sql"))
            return;
        await CreateSchemaAsync(connectionString);
    }

    private static async Task CreateSchemaAsync(string connectionString)
    {
        var schemaSql = await File.ReadAllTextAsync("schema.sql");
        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();
        using var command = new SqliteCommand(schemaSql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public static void CleanupDatabase(string connectionString)
    {
        var dbFilename = SqliteFilenameRegex().Match(connectionString).Groups[1].Value;
        if (string.IsNullOrEmpty(dbFilename))
            return;

        var filesToDelete = new[] {
            dbFilename,
            dbFilename + "-wal",
            dbFilename + "-shm"
        };

        foreach (var file in filesToDelete)
        {
            if (!File.Exists(file))
                continue;

            Console.WriteLine($"Removing sqlite db from {file}");
            File.Delete(file);
        }
    }

    [GeneratedRegex(@"Data Source=([\w\.\/\-]+\.db)", RegexOptions.Compiled)]
    private static partial Regex SqliteFilenameRegex();
}