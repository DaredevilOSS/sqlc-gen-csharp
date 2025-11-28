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

    public static string GetSqliteConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable("SQLITE_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            // Default to shared in-memory database for benchmarks
            // Using shared cache allows multiple connections to access the same in-memory database
            return "Data Source=:memory:;Cache=Shared";
        }
        return connectionString;
    }
}

