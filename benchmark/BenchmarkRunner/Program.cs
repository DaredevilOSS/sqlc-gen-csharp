using BenchmarkRunner;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Information);
        });
        
        // Determine which database(s) to benchmark
        var runPostgresql = args.Length == 0 || args.Contains("--postgresql", StringComparer.OrdinalIgnoreCase);
        var runSqlite = args.Length == 0 || args.Contains("--sqlite", StringComparer.OrdinalIgnoreCase);
        
        if (runPostgresql)
        {
            var logger = loggerFactory.CreateLogger<PostgresqlRunner>();
            var connectionString = Config.GetConnectionString();
            var postgresqlRunner = new PostgresqlRunner(connectionString, logger);
            await postgresqlRunner.RunAsync();
        }
        
        if (runSqlite)
        {
            var logger = loggerFactory.CreateLogger<SqliteRunner>();
            var connectionString = Config.GetSqliteConnectionString();
            var sqliteRunner = new SqliteRunner(connectionString, logger);
            await sqliteRunner.RunAsync();
        }
    }
}
