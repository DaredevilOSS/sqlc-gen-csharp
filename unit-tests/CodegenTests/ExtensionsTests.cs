using NUnit.Framework;
using SqlcGenCsharp;

namespace CodegenTests;

public class NumberExtensionsTests
{
    [TestCase(0, ExpectedResult = "0")]
    [TestCase(999, ExpectedResult = "999")]
    [TestCase(1_000, ExpectedResult = "1K")]
    [TestCase(5_000, ExpectedResult = "5K")]
    [TestCase(999_000, ExpectedResult = "999K")]
    [TestCase(1_000_000, ExpectedResult = "1M")]
    [TestCase(25_000_000, ExpectedResult = "25M")]
    [TestCase(1_000_000_000, ExpectedResult = "1B")]
    [TestCase(int.MaxValue, ExpectedResult = "2B")]
    public static string TestStringifyLargeNumbers(int value)
    {
        return value.StringifyLargeNumbers();
    }
}

public class StringExtensionsTests
{
    [TestCase("author_name", ExpectedResult = "AuthorName")]
    [TestCase("get-author", ExpectedResult = "GetAuthor")]
    [TestCase("a", ExpectedResult = "A")]
    [TestCase("a_b_c", ExpectedResult = "ABC")]
    [TestCase("", ExpectedResult = "")]
    [TestCase("   ", ExpectedResult = "")]
    [TestCase("__", ExpectedResult = "")]
    [TestCase("already Pascal", ExpectedResult = "AlreadyPascal")]
    [TestCase("1st place", ExpectedResult = "_1stPlace")]
    [TestCase("e-mail", ExpectedResult = "EMail")]
    public static string TestToPascalCase(string value)
    {
        return value.ToPascalCase();
    }

    [TestCase("author_name", ExpectedResult = "authorName")]
    [TestCase("Name", ExpectedResult = "name")]
    public static string TestToCamelCase(string value)
    {
        return value.ToCamelCase();
    }

    [TestCase("get_author", true, ExpectedResult = "GetAuthorAsync")]
    [TestCase("get_author", false, ExpectedResult = "GetAuthor")]
    public static string TestToMethodName(string value, bool withAsyncSuffix)
    {
        return value.ToMethodName(withAsyncSuffix);
    }

    [TestCase("", ExpectedResult = "")]
    [TestCase("x = 1", ExpectedResult = "x = 1;")]
    public static string TestAppendSemicolonUnlessEmpty(string value)
    {
        return value.AppendSemicolonUnlessEmpty();
    }

    [Test]
    public void TestToModelNamePrefixesNonDefaultSchema()
    {
        Assert.That("authors".ToModelName("other", "public"), Is.EqualTo("OtherAuthor"));
        Assert.That("authors".ToModelName("public", "public"), Is.EqualTo("Author"));
    }
}

public class CollectionExtensionsTests
{
    [Test]
    public void TestDictionaryMergeOverridesAndAdds()
    {
        IDictionary<string, int> me = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var merged = me.Merge(new Dictionary<string, int> { ["b"] = 3, ["c"] = 4 });
        Assert.That(merged, Is.EquivalentTo(new Dictionary<string, int> { ["a"] = 1, ["b"] = 3, ["c"] = 4 }));
    }

    [Test]
    public void TestDictionaryMergeNullIsNoOp()
    {
        IDictionary<string, int> me = new Dictionary<string, int> { ["a"] = 1 };
        Assert.That(me.Merge(null), Is.SameAs(me));
        Assert.That(me, Has.Count.EqualTo(1));
    }

    [TestCase(true, 2)]
    [TestCase(false, 1)]
    public void TestDictionaryMergeIf(bool condition, int expectedCount)
    {
        IDictionary<string, int> me = new Dictionary<string, int> { ["a"] = 1 };
        me.MergeIf(new Dictionary<string, int> { ["b"] = 2 }, condition);
        Assert.That(me, Has.Count.EqualTo(expectedCount));
    }

    [Test]
    public void TestListAddRangeExcludeNullsSkipsNulls()
    {
        var result = new List<string> { "a" }.AddRangeExcludeNulls(["b", null, "c"]);
        Assert.That(result, Is.EqualTo(new[] { "a", "b", "c" }));
    }

    [TestCase(true, 2)]
    [TestCase(false, 1)]
    public void TestListAddRangeIf(bool condition, int expectedCount)
    {
        var result = new List<string> { "a" }.AddRangeIf(["b"], condition);
        Assert.That(result.Count(), Is.EqualTo(expectedCount));
    }

    [Test]
    public void TestListJoins()
    {
        string[] items = ["a", "b", "c"];
        Assert.That(items.JoinByNewLine(), Is.EqualTo("a\nb\nc"));
        Assert.That(items.JoinByComma(), Is.EqualTo("a, b, c"));
        Assert.That(Array.Empty<string>().JoinByComma(), Is.EqualTo(string.Empty));
    }

    [Test]
    public void TestSetAddRangeExcludeNullsSkipsNullsAndDuplicates()
    {
        ISet<string> set = new HashSet<string> { "a" };
        set.AddRangeExcludeNulls(["a", null, "b"]);
        Assert.That(set, Is.EquivalentTo(new[] { "a", "b" }));
    }

    [TestCase(true, 2)]
    [TestCase(false, 1)]
    public void TestSetAddRangeIf(bool condition, int expectedCount)
    {
        ISet<string> set = new HashSet<string> { "a" };
        set.AddRangeIf(["b"], condition);
        Assert.That(set, Has.Count.EqualTo(expectedCount));
    }
}

public class PluginOptionNamesTests
{
    [TestCase(DriverName.MySqlConnector, ExpectedResult = "MySqlConnector")]
    [TestCase(DriverName.Npgsql, ExpectedResult = "Npgsql")]
    [TestCase(DriverName.Sqlite, ExpectedResult = "Microsoft.Data.Sqlite")]
    [TestCase(DriverName.Mssql, ExpectedResult = "Microsoft.Data.SqlClient")]
    public static string TestDriverNameToName(DriverName driverName)
    {
        return driverName.ToName();
    }

    [TestCase(DotnetFramework.Dotnet80, "net8.0")]
    [TestCase(DotnetFramework.DotnetStandard20, "netstandard2.0")]
    [TestCase(DotnetFramework.DotnetStandard21, "netstandard2.1")]
    public void TestDotnetFrameworkNameRoundTrip(DotnetFramework framework, string name)
    {
        Assert.That(framework.ToName(), Is.EqualTo(name));
        Assert.That(DotnetFrameworkExtensions.ParseName(name), Is.EqualTo(framework));
    }

    [Test]
    public void TestParseUnknownFrameworkThrows()
    {
        Assert.Throws<ArgumentException>(() => DotnetFrameworkExtensions.ParseName("net6.0"));
    }

    [TestCase(DotnetFramework.Dotnet80, true)]
    [TestCase(DotnetFramework.DotnetStandard20, false)]
    [TestCase(DotnetFramework.DotnetStandard21, false)]
    public void TestIsDotnetCoreAndLegacy(DotnetFramework framework, bool isCore)
    {
        Assert.That(framework.IsDotnetCore(), Is.EqualTo(isCore));
        Assert.That(framework.IsDotnetLegacy(), Is.EqualTo(!isCore));
    }
}