using Google.Protobuf;
using Plugin;
using SqlcGenCsharp;
using System.Text;
using System.Xml;

namespace SqlcGenCsharpTests;

public class CodeGeneratorTests
{
    private CodeGenerator CodeGenerator { get; } = new();

    [Test]
    public void TestCsprojFileGenerated()
    {
        var request = new GenerateRequest
        {
            Settings = new Settings
            {
                Engine = "mysql",
                Codegen = new Codegen
                {
                    Out = "DummyProject"
                }
            },
            PluginOptions = ByteString.CopyFrom("{\"generateCsproj\": true}", Encoding.UTF8)
        };

        var actual = CodeGenerator.Generate(request);
        Assert.That(actual.Result.Files.Any(f => f.Name == $"{request.Settings.Codegen.Out}.csproj"), Is.True);
    }

    [Test]
    public void TestCsprojFileNotGenerated()
    {
        var request = new GenerateRequest
        {
            Settings = new Settings
            {
                Engine = "mysql",
                Codegen = new Codegen
                {
                    Out = "DummyProject"
                }
            },
            PluginOptions = ByteString.CopyFrom("{\"generateCsproj\": false}", Encoding.UTF8)
        };

        var actual = CodeGenerator.Generate(request);
        Assert.That(actual.Result.Files.Any(f => f.Name == $"{request.Settings.Codegen.Out}.csproj"), Is.False);
    }

    [Test]
    public void TestOverrideDriverVersion()
    {
        const string expected = "3.45.88";
        var request = new GenerateRequest
        {
            Settings = new Settings
            {
                Engine = "mysql",
                Codegen = new Codegen
                {
                    Out = "DummyProject"
                }
            },
            PluginOptions = ByteString.CopyFrom("{\"overrideDriverVersion\": \"" + expected + "\"}", Encoding.UTF8)
        };

        var response = CodeGenerator.Generate(request);
        var csprojFile = response.Result.Files.First(f => f.Name == $"{request.Settings.Codegen.Out}.csproj");
        var doc = new XmlDocument();
        doc.LoadXml(csprojFile.Contents.ToStringUtf8());

        var driverNode = doc.SelectSingleNode("//PackageReference[not(@Include='Dapper')]");
        Assert.That(driverNode, Is.Not.Null);
        var actual = driverNode!.Attributes["Version"].Value;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestOverrideDapperVersion()
    {
        const string expected = "12.34.67";
        var request = new GenerateRequest
        {
            Settings = new Settings
            {
                Engine = "mysql",
                Codegen = new Codegen
                {
                    Out = "DummyProject"
                }
            },
            PluginOptions = ByteString.CopyFrom("{\"useDapper\": true, \"overrideDapperVersion\": \"" + expected + "\"}", Encoding.UTF8)
        };

        var response = CodeGenerator.Generate(request);
        var csprojFile = response.Result.Files.First(f => f.Name == $"{request.Settings.Codegen.Out}.csproj");
        var doc = new XmlDocument();
        doc.LoadXml(csprojFile.Contents.ToStringUtf8());

        var dapperNode = doc.SelectSingleNode("//PackageReference[@Include='Dapper']");
        Assert.That(dapperNode, Is.Not.Null);
        var actual = dapperNode!.Attributes["Version"].Value;

        Assert.That(actual, Is.EqualTo(expected));
    }
}