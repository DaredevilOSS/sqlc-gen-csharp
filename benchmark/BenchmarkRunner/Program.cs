using BenchmarkRunner;
using Microsoft.Extensions.Logging;
using System.CommandLine;

public class Program
{
    private static readonly HashSet<string> _databases = ["mysql", "postgresql", "sqlite"];
    private static readonly HashSet<string> _types = ["reads", "writes"];

    public static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand("Run benchmarks");

        var databaseOption = GetDatabaseOption();
        rootCommand.AddOption(databaseOption);

        var typeOption = GetTypeOption();
        rootCommand.AddOption(typeOption);

        rootCommand.SetHandler(CommandHandler, databaseOption, typeOption);
        await rootCommand.InvokeAsync(args);
    }

    private static Option<string> GetDatabaseOption()
    {
        var option = new Option<string>(
            "--database",
            "Database to benchmark (mysql, postgresql, or sqlite)")
        {
            IsRequired = true
        };
        option.AddValidator(result =>
        {
            var value = result.GetValueForOption(option);
            if (value != null && !_databases.Contains(value))
                result.ErrorMessage = $"Invalid database: {value}. Must be one of: {string.Join(", ", _databases)}";
        });
        return option;
    }

    private static Option<string> GetTypeOption()
    {
        var option = new Option<string>(
            "--type",
            $"Type of benchmark to run ({string.Join(", ", _types)})")
        {
            IsRequired = true
        };
        option.AddValidator(result =>
        {
            var value = result.GetValueForOption(option);
            if (value != null && !_types.Contains(value))
                result.ErrorMessage = $"Invalid type: {value}. Must be one of: {string.Join(", ", _types)}";
        });
        return option;
    }
    private static async Task CommandHandler(string database, string type)
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
            builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Information));

        var mysqlRunner = new MysqlRunner(
            Config.GetMysqlConnectionString(),
            loggerFactory.CreateLogger<MysqlRunner>());
        var postgresqlRunner = new PostgresqlRunner(
            Config.GetPostgresConnectionString(),
            loggerFactory.CreateLogger<PostgresqlRunner>());
        var sqliteRunner = new SqliteRunner(
            Config.GetSqliteConnectionString(),
            loggerFactory.CreateLogger<SqliteRunner>());

        switch (database, type)
        {
            case ("mysql", "read"):
                await mysqlRunner.RunReadsAsync();
                break;
            case ("mysql", "write"):
                await mysqlRunner.RunWritesAsync();
                break;
            case ("postgresql", "read"):
                await postgresqlRunner.RunReadsAsync();
                break;
            case ("postgresql", "write"):
                await postgresqlRunner.RunWritesAsync();
                break;
            case ("sqlite", "read"):
                await sqliteRunner.RunReadsAsync();
                break;
            case ("sqlite", "write"):
                await sqliteRunner.RunWritesAsync();
                break;
            default:
                throw new ArgumentException($"Invalid database - {database}, type - {type}");
        }
    }
}