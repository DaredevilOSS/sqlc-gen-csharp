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
        private static readonly string[] SchemaFiles = new string[] {
            "authors.sqlite.schema.sql",
            "types.sqlite.schema.sql"
        };

        public const string PostgresConnectionStringEnv = "POSTGRES_CONNECTION_STRING";
        public const string MySqlConnectionStringEnv = "MYSQL_CONNECTION_STRING";
        public const string SqliteConnectionStringEnv = "SQLITE_CONNECTION_STRING";

        public static void SetUp()
        {
            if (File.Exists(EnvFile))
                DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { EnvFile }));
            RemoveExistingSqliteDb();
            InitSqliteDb();
        }

        public static void TearDown()
        {
            RemoveExistingSqliteDb();
        }

        private static void RemoveExistingSqliteDb()
        {
            var connectionString = Environment.GetEnvironmentVariable(SqliteConnectionStringEnv);
            if (connectionString == null) return;

            var dbFilename = SqliteFilenameRegex.Match(connectionString).Groups[1].Value;
            Console.WriteLine($"Removing sqlite db from {dbFilename}");
            if (!File.Exists(dbFilename)) return;

            try
            {
                File.Delete(dbFilename);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        private static void InitSqliteDb()
        {
            var connectionString = Environment.GetEnvironmentVariable(SqliteConnectionStringEnv);
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                foreach (var schemaFile in SchemaFiles)
                {
                    if (!File.Exists(schemaFile))
                        continue;

                    var schemaSql = File.ReadAllText(schemaFile);
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = schemaSql;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static readonly Regex SqliteFilenameRegex = new Regex(@"Data Source=([\w\.\/\-]+\.db);", RegexOptions.Compiled);
    }
}