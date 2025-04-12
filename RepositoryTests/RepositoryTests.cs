using dotenv.net;

namespace RepositoryTests;

public class RepositoryTests
{
    private const string EnvFile = ".env";

    [Test]
    public void TestMySqlEnvVarsConsistent()
    {
        Assert.That(File.Exists(EnvFile));
        DotEnv.Load(options: new DotEnvOptions(envFilePaths: [EnvFile]));

        var testsDb = Environment.GetEnvironmentVariable("TESTS_DB");
        var mysqlConnectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
        Assert.That(mysqlConnectionString, Is.Not.Null);
        var mysqlConnParts = mysqlConnectionString.Split(';');
        Assert.That(mysqlConnParts, Does.Contain($"database={testsDb}"));
    }

    [Test]
    public void TestPostgresEnvVarsConsistent()
    {
        Assert.That(File.Exists(EnvFile));
        DotEnv.Load(options: new DotEnvOptions(envFilePaths: [EnvFile]));

        var testsDb = Environment.GetEnvironmentVariable("TESTS_DB");
        var postgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
        var postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
        var postgresConnectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
        Assert.That(postgresConnectionString, Is.Not.Null);
        var postgresConnParts = postgresConnectionString.Split(';');
        Assert.That(postgresConnParts, Does.Contain($"database={testsDb}"));
        Assert.That(postgresConnParts, Does.Contain($"username={postgresUser}"));
        Assert.That(postgresConnParts, Does.Contain($"password={postgresPassword}"));
    }
}