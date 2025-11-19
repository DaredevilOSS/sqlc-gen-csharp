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
        var logger = loggerFactory.CreateLogger<PostgresRunner>();
        
        var connectionString = Config.GetConnectionString();
        var postgresRunner = new PostgresRunner(connectionString, logger);
        await postgresRunner.RunAsync();
    }
}
