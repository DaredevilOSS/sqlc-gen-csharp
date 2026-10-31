using dotenv.net;
using Microsoft.Data.SqlClient;
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
        public const string MssqlConnectionStringEnv = "MSSQL_CONNECTION_STRING";
        public const string SqliteConnectionStringEnv = "SQLITE_CONNECTION_STRING";

        public static void SetupTestsMssqlDb() => SetupMssqlDb(
            new string[] { "authors.mssql.schema.sql", "types.mssql.schema.sql" }
        );

        // SQL Server has no init hook in its docker image, so the database is (re)created here. The schema files are
        // split on GO, since CREATE SCHEMA / VIEW / PROCEDURE must start a batch.
        public static void SetupMssqlDb(string[] schemaFiles)
        {
            LoadEnvFile();
            var connectionString = Environment.GetEnvironmentVariable(MssqlConnectionStringEnv);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"{MssqlConnectionStringEnv} environment variable is not set");

            var builder = new SqlConnectionStringBuilder(connectionString);
            var database = builder.InitialCatalog;
            builder.InitialCatalog = "master";
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                ExecuteMssql(connection, $"IF DB_ID('{database}') IS NOT NULL BEGIN ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{database}]; END");
                ExecuteMssql(connection, $"CREATE DATABASE [{database}]");
                connection.ChangeDatabase(database);
                foreach (var schemaFile in schemaFiles)
                {
                    var schemaSql = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, schemaFile));
                    foreach (var batch in MssqlBatchSeparatorRegex.Split(schemaSql))
                        if (batch.Trim().Length > 0)
                            ExecuteMssql(connection, batch);
                }
            }
            Console.WriteLine($"Initialized SQL Server database {database}");
        }

        private static readonly Regex MssqlBatchSeparatorRegex = new Regex(@"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        private static void ExecuteMssql(SqlConnection connection, string sql)
        {
            using (var command = new SqlCommand(sql, connection))
                command.ExecuteNonQuery();
        }

        public static void SetupTestsSqliteDb() => SetupSqliteDb(
            new string[] { "authors.sqlite.schema.sql", "types.sqlite.schema.sql" }
        );
        public static void SetupBenchmarkSqliteDb() => SetupSqliteDb(
            new string[] { "sqlite.schema.sql" }
        );

        public static void LoadEnvFile()
        {
            if (!File.Exists(EnvFile))
                throw new FileNotFoundException($"{EnvFile} not found");
            // variables already set (e.g. by CI, pointing at another server) win over the file
            DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { EnvFile }, overwriteExistingVars: false));
        }

        public static void SetupSqliteDb(string[] schemaFiles)
        {
            LoadEnvFile();
            RemoveExistingSqliteDb();
            InitSqliteDb(schemaFiles);
        }
        public static void RemoveExistingSqliteDb()
        {
            var connectionString = Environment.GetEnvironmentVariable(SqliteConnectionStringEnv);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"{SqliteConnectionStringEnv} environment variable is not set");

            var dbFilename = SqliteFilenameRegex.Match(connectionString).Groups[1].Value;
            if (!File.Exists(dbFilename)) return;

            Console.WriteLine($"Removing sqlite db from {dbFilename}");
            // pooled connections keep the file open, which makes the delete fail on Windows
            SqliteConnection.ClearAllPools();
            File.Delete(dbFilename);
        }
        private static void InitSqliteDb(string[] schemaFiles)
        {
            var connectionString = Environment.GetEnvironmentVariable(SqliteConnectionStringEnv);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"{SqliteConnectionStringEnv} environment variable is not set");

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