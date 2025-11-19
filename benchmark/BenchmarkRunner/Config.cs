using dotenv.net;

namespace BenchmarkRunner;

public static class Config
{
    static Config()
    {
        var envFile = Path.Combine(AppContext.BaseDirectory, ".env");
        if (File.Exists(envFile))
            DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { envFile }));
    }

    public static string GetConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "POSTGRES_CONNECTION_STRING environment variable is not set. " +
                "Please set it to your PostgreSQL connection string (e.g., " +
                "Host=localhost;Port=5432;Database=benchmark;Username=postgres;Password=postgres)");
        }
        return connectionString;
    }
}

