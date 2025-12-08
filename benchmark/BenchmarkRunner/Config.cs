using dotenv.net;

namespace BenchmarkRunner;

public static class Config
{
    static Config()
    {
        var envFile = Path.Combine(AppContext.BaseDirectory, ".env");
        if (File.Exists(envFile))
            DotEnv.Load(options: new DotEnvOptions(envFilePaths: [envFile]));
    }

    public static string GetPostgresConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable("POSTGRES_BENCHMARK_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "POSTGRES_CONNECTION_STRING environment variable is not set. " +
                "Please set it to your PostgreSQL connection string (e.g., " +
                "Host=localhost;Port=5432;Database=sales;Username=postgres;Password=postgres)");
        }
        return connectionString;
    }

    public static string GetSqliteConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable("SQLITE_BENCHMARK_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            // Default to file-based database with WAL mode for benchmarks
            // WAL mode enables better concurrency (readers don't block writers)
            // Using shared cache allows multiple connections to access the same database
            return "Data Source=benchmark.db;Cache=Shared;Journal Mode=WAL";
        }
        return connectionString;
    }

    public static string GetMysqlConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable("MYSQL_BENCHMARK_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "MYSQL_CONNECTION_STRING environment variable is not set. " +
                "Please set it to your MySQL connection string (e.g., " +
                "Server=localhost;Port=3306;Database=sales;User=root;Password=)");
        }
        return connectionString;
    }
}