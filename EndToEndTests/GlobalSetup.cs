using dotenv.net;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SqlcGenCsharpTests;

[SetUpFixture]
public partial class GlobalSetup
{
    private const string EnvFile = ".env";
    private const string SchemaFile = "sqlite.schema.sql";

    public const string PostgresConnectionStringEnv = "POSTGRES_CONNECTION_STRING";
    public const string MySqlConnectionStringEnv = "MYSQL_CONNECTION_STRING";
    public const string SqliteConnectionStringEnv = "SQLITE_CONNECTION_STRING";

    [OneTimeSetUp]
    public void LoadEnvFile()
    {
        if (File.Exists(EnvFile))
            DotEnv.Load(options: new DotEnvOptions(envFilePaths: [EnvFile]));
        RemoveExistingDb();
        if (File.Exists(SchemaFile))
            InitSqliteDb();
    }

    private static void RemoveExistingDb()
    {
        var connectionString = Environment.GetEnvironmentVariable(SqliteConnectionStringEnv)!;
        var dbFilename = SqliteFilenameRegex().Match(connectionString).Groups[1].Value;
        if (File.Exists(dbFilename))
            File.Delete(dbFilename);
    }
    private static void InitSqliteDb()
    {
        var schemaSql = File.ReadAllText(SchemaFile);
        var connectionString = Environment.GetEnvironmentVariable(GlobalSetup.SqliteConnectionStringEnv);
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = schemaSql;
        command.ExecuteNonQuery();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        RemoveExistingDb();
    }

    [GeneratedRegex("Data Source=((/|\\w)+.db);")]
    private static partial Regex SqliteFilenameRegex();
}