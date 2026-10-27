using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System;
using System.IO;

namespace EndToEndTests;

[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void SetUp()
    {
        EndToEndCommon.SetupTestsSqliteDb();
        SetupMssqlDb();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        EndToEndCommon.RemoveExistingSqliteDb();
    }

    // SQL Server has no init hook in its docker image, so the database and its schema are created here
    private static void SetupMssqlDb()
    {
        var connectionString = Environment.GetEnvironmentVariable(EndToEndCommon.MssqlConnectionStringEnv);
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"{EndToEndCommon.MssqlConnectionStringEnv} environment variable is not set");

        var builder = new SqlConnectionStringBuilder(connectionString);
        var database = builder.InitialCatalog;
        builder.InitialCatalog = "master";
        using (var connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();
            Execute(connection, $"DROP DATABASE IF EXISTS [{database}]");
            Execute(connection, $"CREATE DATABASE [{database}]");
            connection.ChangeDatabase(database);
            foreach (var schemaFile in new[] { "authors.mssql.schema.sql", "types.mssql.schema.sql" })
                Execute(connection, File.ReadAllText(Path.Combine(AppContext.BaseDirectory, schemaFile)));
        }
    }

    private static void Execute(SqlConnection connection, string sql)
    {
        using (var command = new SqlCommand(sql, connection))
            command.ExecuteNonQuery();
    }
}
