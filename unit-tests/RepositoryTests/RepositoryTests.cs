using dotenv.net;

namespace RepositoryTests;

public class RepositoryTests
{
    [Test]
    [TestCase(".env")]
    [TestCase(".benchmark.env")]
    public void TestPostgresEnvVarsConsistent(string envFile)
    {
        Assert.That(File.Exists(envFile));
        DotEnv.Load(options: new DotEnvOptions(envFilePaths: [envFile]));

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