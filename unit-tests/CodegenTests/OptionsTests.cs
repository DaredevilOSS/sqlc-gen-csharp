using Google.Protobuf;
using NUnit.Framework;
using Plugin;
using SqlcGenCsharp;
using System.Text;

namespace CodegenTests;

public class OptionsTests
{
    private static GenerateRequest CreateRequest(string engine, string options, bool wasm = false)
    {
        var codegen = new Codegen { Out = "DummyProject" };
        if (wasm)
            codegen.Wasm = new Codegen.Types.WASM();
        return new GenerateRequest
        {
            Settings = new Settings { Engine = engine, Codegen = codegen },
            Catalog = new Catalog { Schemas = { new Schema { Name = string.Empty } } },
            PluginOptions = ByteString.CopyFrom(options, Encoding.UTF8)
        };
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase("{}")]
    public void TestDefaultsWhenNoOptionsProvided(string optionsText)
    {
        var options = new Options(CreateRequest("postgresql", optionsText));

        Assert.That(options.DriverName, Is.EqualTo(DriverName.Npgsql));
        Assert.That(options.DotnetFramework, Is.EqualTo(DotnetFramework.Dotnet80));
        Assert.That(options.GenerateCsproj, Is.True);
        Assert.That(options.WithAsyncSuffix, Is.True);
        Assert.That(options.UseDapper, Is.False);
        Assert.That(options.WithCancellationToken, Is.False);
        Assert.That(options.UseCentralPackageManagement, Is.False);
        Assert.That(options.UseProperSingularization, Is.False);
        Assert.That(options.DebugRequest, Is.False);
        Assert.That(options.NamespaceName, Is.Empty);
        Assert.That(options.OverrideDriverVersion, Is.Empty);
        Assert.That(options.OverrideDapperVersion, Is.Empty);
        Assert.That(options.Overrides, Is.Empty);
    }

    [TestCase("mysql", ExpectedResult = DriverName.MySqlConnector)]
    [TestCase("postgresql", ExpectedResult = DriverName.Npgsql)]
    [TestCase("sqlite", ExpectedResult = DriverName.Sqlite)]
    public static DriverName TestEngineMapsToDriver(string engine)
    {
        return new Options(CreateRequest(engine, "{}")).DriverName;
    }

    [Test]
    public void TestUnknownEngineThrows()
    {
        Assert.Throws<KeyNotFoundException>(() => new Options(CreateRequest("oracle", "{}")));
    }

    [Test]
    public void TestAllOptionsAreParsed()
    {
        const string json = """
        {
            "overrideDriverVersion": "1.2.3",
            "generateCsproj": false,
            "targetFramework": "netstandard2.1",
            "namespaceName": "MyNamespace",
            "useDapper": true,
            "overrideDapperVersion": "4.5.6",
            "useCentralPackageManagement": true,
            "withAsyncSuffix": false,
            "withCancellationToken": true,
            "useProperSingularization": true,
            "overrides": [
                { "column": "GetAuthor:name", "csharp_type": { "type": "string", "notNull": true } }
            ]
        }
        """;

        var options = new Options(CreateRequest("mysql", json));

        Assert.That(options.OverrideDriverVersion, Is.EqualTo("1.2.3"));
        Assert.That(options.GenerateCsproj, Is.False);
        Assert.That(options.DotnetFramework, Is.EqualTo(DotnetFramework.DotnetStandard21));
        Assert.That(options.NamespaceName, Is.EqualTo("MyNamespace"));
        Assert.That(options.UseDapper, Is.True);
        Assert.That(options.OverrideDapperVersion, Is.EqualTo("4.5.6"));
        Assert.That(options.UseCentralPackageManagement, Is.True);
        Assert.That(options.WithAsyncSuffix, Is.False);
        Assert.That(options.WithCancellationToken, Is.True);
        Assert.That(options.UseProperSingularization, Is.True);
        Assert.That(options.Overrides, Has.Count.EqualTo(1));
        Assert.That(options.Overrides[0].Column, Is.EqualTo("GetAuthor:name"));
        Assert.That(options.Overrides[0].CsharpType.Type, Is.EqualTo("string"));
        Assert.That(options.Overrides[0].CsharpType.NotNull, Is.True);
    }

    [Test]
    public void TestUnknownTargetFrameworkThrows()
    {
        Assert.Throws<ArgumentException>(() =>
            new Options(CreateRequest("sqlite", """{"targetFramework": "net6.0"}""")));
    }

    [Test]
    public void TestDebugRequestWithWasmThrows()
    {
        Assert.Throws<ArgumentException>(() =>
            new Options(CreateRequest("sqlite", """{"debugRequest": true}""", wasm: true)));
    }

    [Test]
    public void TestDebugRequestWithProcessPluginIsAllowed()
    {
        var options = new Options(CreateRequest("sqlite", """{"debugRequest": true}"""));
        Assert.That(options.DebugRequest, Is.True);
    }
}

public class DebugRequestGenerationTests
{
    [Test]
    public void TestDebugRequestEmitsRequestFilesWithDebugFlagCleared()
    {
        var request = new GenerateRequest
        {
            Settings = new Settings { Engine = "sqlite", Codegen = new Codegen { Out = "DummyProject" } },
            Catalog = new Catalog { Schemas = { new Schema { Name = string.Empty } } },
            PluginOptions = ByteString.CopyFromUtf8("""{"debugRequest": true, "namespaceName": "Ns"}""")
        };

        var response = new CodeGenerator().Generate(request).Result;

        Assert.That(response.Files.Select(f => f.Name), Is.EqualTo(new[] { "request.json", "request.message" }));

        var json = response.Files[0].Contents.ToStringUtf8();
        Assert.That(json, Does.Contain("DummyProject"));

        var roundTripped = GenerateRequest.Parser.ParseFrom(response.Files[1].Contents);
        var roundTrippedOptions = new Options(roundTripped);
        Assert.That(roundTrippedOptions.DebugRequest, Is.False);
        Assert.That(roundTrippedOptions.NamespaceName, Is.EqualTo("Ns"));
    }
}