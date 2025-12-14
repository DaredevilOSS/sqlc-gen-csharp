using System.CommandLine;

public class Program
{
    private static readonly HashSet<string> _databases = ["mysql", "postgresql", "sqlite"];
    private static readonly HashSet<string> _types = ["reads", "writes"];
    private static readonly Dictionary<string, Dictionary<string, Func<Task>>> _benchmarkConfigs = new()
    {
        { "mysql", new()
        {
            { "reads", MysqlRunner.RunReadsAsync },
            { "writes", MysqlRunner.RunWritesAsync }
        }},
        { "postgresql", new()
        {
            { "reads", PostgresqlRunner.RunReadsAsync },
            { "writes", PostgresqlRunner.RunWritesAsync }
        }},
        { "sqlite", new()
        {
            { "reads", SqliteRunner.RunReadsAsync },
            { "writes", SqliteRunner.RunWritesAsync }
        }}
    };

    public static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand("Run benchmarks");

        var databaseOption = GetDatabaseOption();
        rootCommand.AddOption(databaseOption);

        var typeOption = GetTypeOption();
        rootCommand.AddOption(typeOption);

        rootCommand.SetHandler(
            async (database, type) => await _benchmarkConfigs[database][type](),
            databaseOption, typeOption);
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
}