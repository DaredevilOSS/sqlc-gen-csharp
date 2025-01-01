using dotenv.net;
using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SqlcGenCsharpTests
{
    public static class EndToEndCommon
    {
        private const string EnvFile = ".env";
        private const string SchemaFile = "sqlite.schema.sql";

        public const string PostgresConnectionStringEnv = "POSTGRES_CONNECTION_STRING";
        public const string MySqlConnectionStringEnv = "MYSQL_CONNECTION_STRING";
        public const string SqliteConnectionStringEnv = "SQLITE_CONNECTION_STRING";

        public static void SetUp()
        {
            Console.WriteLine("");
            if (File.Exists(EnvFile))
                DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { EnvFile }));
            RemoveExistingSqliteDb();
            if (File.Exists(SchemaFile))
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
            if (File.Exists(dbFilename))
                File.Delete(dbFilename);
        }
        private static void InitSqliteDb()
        {
            var schemaSql = File.ReadAllText(SchemaFile);
            var connectionString = Environment.GetEnvironmentVariable(EndToEndCommon.SqliteConnectionStringEnv);
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = schemaSql;
                    command.ExecuteNonQuery();
                }
            }
        }

        private static readonly Regex SqliteFilenameRegex = new Regex(@"Data Source=([\w\.\/\-]+\.db);", RegexOptions.Compiled);
    }
}