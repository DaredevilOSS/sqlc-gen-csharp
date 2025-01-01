using NUnit.Framework;

namespace SqlcGenCsharpTests
{
    [SetUpFixture]
    public class GlobalSetup
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            EndToEndCommon.SetUp();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            EndToEndCommon.TearDown();
        }
    }
}