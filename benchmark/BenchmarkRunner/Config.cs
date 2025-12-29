using EndToEndTests;

namespace BenchmarkRunner;

public static partial class Config
{
    static Config()
    {
        EndToEndCommon.LoadEnvFile();
    }

    public static string GetPostgresConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv);
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                $"{EndToEndCommon.PostgresConnectionStringEnv} environment variable is not set. " +
                "Please set it to your PostgreSQL connection string (e.g., " +
                "Host=localhost;Port=5432;Database=sales;Username=postgres;Password=postgres)");
        return connectionString;
    }

    public static string GetSqliteConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable(EndToEndCommon.SqliteConnectionStringEnv);
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                $"{EndToEndCommon.SqliteConnectionStringEnv} environment variable is not set. " +
                "Please set it to your SQLite connection string (e.g., " +
                "Data Source=benchmark.db;Mode=ReadWriteCreate)");
        return connectionString;
    }

    public static string GetMysqlConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable(EndToEndCommon.MySqlConnectionStringEnv);
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                $"{EndToEndCommon.MySqlConnectionStringEnv} environment variable is not set. " +
                "Please set it to your MySQL connection string (e.g., " +
                "Server=localhost;Port=3306;Database=sales;User=root;Password=)");
        return connectionString;
    }
}