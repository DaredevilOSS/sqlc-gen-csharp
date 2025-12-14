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
        var schemaPath = Path.Combine(AppContext.BaseDirectory, "schema.sql");
        if (File.Exists(schemaPath))
        {
            await CreateSchemaAsync(connectionString, schemaPath);
        }
    }

    public static async Task InitializeDatabaseAsync(string connectionString)
    {
        var schemaPath = Path.Combine(AppContext.BaseDirectory, "schema.sql");
        if (!File.Exists(schemaPath))
        {
            throw new FileNotFoundException($"Schema file not found at {schemaPath}. Make sure schema.sql is copied to the output directory.");
        }
        await CreateSchemaAsync(connectionString, schemaPath);
    }

    private static async Task CreateSchemaAsync(string connectionString, string schemaPath)
    {
        var schemaSql = await File.ReadAllTextAsync(schemaPath);
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