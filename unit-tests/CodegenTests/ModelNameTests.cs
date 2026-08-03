using NUnit.Framework;
using SqlcGenCsharp;

namespace CodegenTests;

public class ModelNameTests
{
    private const string DefaultSchema = "public";

    [TestCase("authors", ExpectedResult = "Author")]
    [TestCase("devices", ExpectedResult = "Device")]
    [TestCase("policies", ExpectedResult = "Policy")]
    [TestCase("retries", ExpectedResult = "Retry")]
    [TestCase("statuses", ExpectedResult = "Status")]
    [TestCase("Accesses", ExpectedResult = "Access")]
    [TestCase("boxes", ExpectedResult = "Box")]
    [TestCase("matches", ExpectedResult = "Match")]
    [TestCase("responses", ExpectedResult = "Response")]
    [TestCase("closes", ExpectedResult = "Close")]
    [TestCase("prizes", ExpectedResult = "Prize")]
    [TestCase("address", ExpectedResult = "Address")]
    [TestCase("access", ExpectedResult = "Access")]
    [TestCase("people", ExpectedResult = "People")] // irregular plurals pass through
    [TestCase("data", ExpectedResult = "Data")]
    [TestCase("ies", ExpectedResult = "Ie")] // too short for the ies rule
    public static string TestProperSingularization(string tableName)
    {
        return tableName.ToModelName(DefaultSchema, DefaultSchema, properSingularization: true);
    }

    [TestCase("authors", ExpectedResult = "Author")]
    [TestCase("policies", ExpectedResult = "Policie")]
    [TestCase("accesses", ExpectedResult = "Accesse")]
    [TestCase("access", ExpectedResult = "Acce")]
    public static string TestLegacySingularizationIsTheDefault(string tableName)
    {
        return tableName.ToModelName(DefaultSchema, DefaultSchema);
    }

    [Test]
    public void TestNonDefaultSchemaIsPrefixed()
    {
        Assert.That(
            "launchtargets_view".ToModelName(
                "launcher_api",
                DefaultSchema,
                properSingularization: true
            ),
            Is.EqualTo("LauncherApiLaunchtargetsView")
        );
    }
}