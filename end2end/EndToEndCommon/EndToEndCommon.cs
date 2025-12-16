using dotenv.net;
using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace EndToEndTests
{
    public static class EndToEndCommon
    {
        private const string EnvFile = ".env";
        public const string PostgresConnectionStringEnv = "POSTGRES_CONNECTION_STRING";
        public const string MySqlConnectionStringEnv = "MYSQL_CONNECTION_STRING";
        public const string SqliteConnectionStringEnv = "SQLITE_CONNECTION_STRING";
        public const string SqliteBenchmarkConnectionStringEnv = "SQLITE_BENCHMARK_CONNECTION_STRING";

        public static void SetupTestsSqliteDb() => SetupSqliteDb(
            SqliteConnectionStringEnv,
            new string[] { "authors.sqlite.schema.sql", "types.sqlite.schema.sql" }
        );
        public static void SetupBenchmarkSqliteDb() => SetupSqliteDb(
            SqliteBenchmarkConnectionStringEnv,
            new string[] { "sqlite.schema.sql" }
        );
        public static void SetupSqliteDb(string connectionStringEnv, string[] schemaFiles)
        {
            if (!File.Exists(EnvFile))
                throw new FileNotFoundException($"{EnvFile} not found");

            DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { EnvFile }));
            RemoveExistingSqliteDb(connectionStringEnv);
            InitSqliteDb(connectionStringEnv, schemaFiles);
        }
        public static void RemoveExistingSqliteDb(string connectionStringEnv)
        {
            var connectionString = Environment.GetEnvironmentVariable(connectionStringEnv);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"{connectionStringEnv} environment variable is not set");

            var dbFilename = SqliteFilenameRegex.Match(connectionString).Groups[1].Value;
            if (!File.Exists(dbFilename)) return;

            Console.WriteLine($"Removing sqlite db from {dbFilename}");
            File.Delete(dbFilename);
        }
        private static void InitSqliteDb(string connectionStringEnv, string[] schemaFiles)
        {
            var connectionString = Environment.GetEnvironmentVariable(connectionStringEnv);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"{connectionStringEnv} environment variable is not set");

            connectionString = connectionString.Replace("Mode=ReadWrite", "Mode=ReadWriteCreate");
            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    foreach (var schemaFile in schemaFiles)
                    {
                        var schemaFileToUse = schemaFile;
                        if (!File.Exists(schemaFile))
                        {
                            schemaFileToUse = Path.Combine(AppContext.BaseDirectory, schemaFile);
                            if (!File.Exists(schemaFileToUse))
                                throw new FileNotFoundException($"Schema file {schemaFile} not found");
                        }

                        var schemaSql = File.ReadAllText(schemaFileToUse);
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = schemaSql;
                            command.ExecuteNonQuery();
                        }
                    }
                    Console.WriteLine($"Initialized sqlite db from {connectionString}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing sqlite db from {connectionString}: {ex.Message}");
                throw;
            }
        }
        private static readonly Regex SqliteFilenameRegex = new Regex(@"Data Source=([\w\.\/\-]+\.db);", RegexOptions.Compiled);
    }
}