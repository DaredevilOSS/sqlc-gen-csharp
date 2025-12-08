using BenchmarkRunner;
using Microsoft.Extensions.Logging;
using System.CommandLine;

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

        var rootCommand = new RootCommand("Run database benchmarks");
        var databaseOption = new Option<string>(
            "--database",
            "Database to benchmark (mysql, postgresql, or sqlite)")
        {
            IsRequired = true
        };
        databaseOption.AddValidator(result =>
        {
            var value = result.GetValueForOption(databaseOption);
            if (value != null && value != "mysql" && value != "postgresql" && value != "sqlite")
            {
                result.ErrorMessage = $"Invalid database: {value}. Must be one of: mysql, postgresql, sqlite";
            }
        });
        rootCommand.AddOption(databaseOption);

        rootCommand.SetHandler(async (database) =>
        {
            switch (database)
            {
                case "mysql":
                    var mysqlRunner = new MysqlRunner(
                        Config.GetMysqlConnectionString(), 
                        loggerFactory.CreateLogger<MysqlRunner>());
                    await mysqlRunner.RunAsync();
                    break;
                case "postgresql":
                    var postgresqlRunner = new PostgresqlRunner(
                        Config.GetPostgresConnectionString(), 
                        loggerFactory.CreateLogger<PostgresqlRunner>());
                    await postgresqlRunner.RunAsync();
                    break;
                case "sqlite":
                    var sqliteRunner = new SqliteRunner(
                        Config.GetSqliteConnectionString(), 
                        loggerFactory.CreateLogger<SqliteRunner>());
                    await sqliteRunner.RunAsync();
                    break;
                default:
                    throw new ArgumentException($"Invalid database: {database}");
            }
        }, databaseOption);

        await rootCommand.InvokeAsync(args);
    }
}