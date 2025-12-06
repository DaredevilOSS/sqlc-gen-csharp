using BenchmarkRunner;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Test mode to capture SQL
        if (args.Length > 0 && args[0] == "--test-sql")
        {
            TestSqlCapture.CaptureEfCoreSql();
            return;
        }
        
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Information);
        });
        
        // Determine which database(s) to benchmark
        var runPostgresql = args.Length == 0 || args.Contains("--postgresql", StringComparer.OrdinalIgnoreCase);
        var runSqlite = args.Length == 0 || args.Contains("--sqlite", StringComparer.OrdinalIgnoreCase);
        var runMysql = args.Length == 0 || args.Contains("--mysql", StringComparer.OrdinalIgnoreCase);
        
        if (runPostgresql)
        {
            var logger = loggerFactory.CreateLogger<PostgresqlRunner>();
            var connectionString = Config.GetPostgresConnectionString();
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
        
        if (runMysql)
        {
            var logger = loggerFactory.CreateLogger<MysqlRunner>();
            var connectionString = Config.GetMysqlConnectionString();
            var mysqlRunner = new MysqlRunner(connectionString, logger);
            await mysqlRunner.RunAsync();
        }
    }
}
