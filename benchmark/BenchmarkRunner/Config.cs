using dotenv.net;

namespace BenchmarkRunner;

public static partial class Config
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
            throw new InvalidOperationException(
                "POSTGRES_BENCHMARK_CONNECTION_STRING environment variable is not set. " +
                "Please set it to your PostgreSQL connection string (e.g., " +
                "Host=localhost;Port=5432;Database=sales;Username=postgres;Password=postgres)");
        return connectionString;
    }

    public static string GetSqliteConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable("SQLITE_BENCHMARK_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "SQLITE_BENCHMARK_CONNECTION_STRING environment variable is not set. " +
                "Please set it to your SQLite connection string (e.g., " +
                "Data Source=benchmark.db;Mode=ReadWriteCreate)");
        return connectionString;
    }

    public static string GetMysqlConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable("MYSQL_BENCHMARK_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "MYSQL_BENCHMARK_CONNECTION_STRING environment variable is not set. " +
                "Please set it to your MySQL connection string (e.g., " +
                "Server=localhost;Port=3306;Database=sales;User=root;Password=)");
        return connectionString;
    }
}