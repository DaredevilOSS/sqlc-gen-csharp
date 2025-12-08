using Microsoft.Data.Sqlite;

using System.Text.RegularExpressions;

namespace BenchmarkRunner.Utils;

public static partial class SqliteDatabaseHelper
{
    public static async Task InitializeDatabaseAsync(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();
        if (!File.Exists("schema.sql"))
            return;

        var schemaSql = await File.ReadAllTextAsync("schema.sql");
        using var command = new SqliteCommand(schemaSql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public static void CleanupDatabase(string connectionString)
    {
        var dbFilename = SqliteFilenameRegex().Match(connectionString).Groups[1].Value;
        if (string.IsNullOrEmpty(dbFilename))
            return;

        var filesToDelete = new [] {
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