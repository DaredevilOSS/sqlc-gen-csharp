using NUnit.Framework;

namespace EndToEndTests;

[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void SetUp()
    {
        EndToEndCommon.SetupTestsSqliteDb();
        EndToEndCommon.SetupTestsMssqlDb();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        EndToEndCommon.RemoveExistingSqliteDb();
    }
}