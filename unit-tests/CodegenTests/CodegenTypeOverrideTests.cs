using Google.Protobuf;
using NUnit.Framework;
using Plugin;
using SqlcGenCsharp;
using System.Text;

namespace CodegenTests;

public class CodegenTypeOverrideTests
{
    private readonly Settings _postgresSettings = new()
    {
        Engine = "postgresql",
        Codegen = new Codegen { Out = "DummyProject" }
    };

    private readonly Catalog _emptyCatalog = new()
    {
        Schemas =
        {
            new Schema
            {
                Name = string.Empty,
                Tables = { Capacity = 0 },
                Enums = { Capacity = 0 },
            }
        }
    };

    private CodeGenerator CodeGenerator { get; } = new();

    [Test]
    public void TestOverrideQueryColumnDataType()
    {
        var request = new GenerateRequest
        {
            Settings = _postgresSettings,
            Catalog = _emptyCatalog,
            PluginOptions = ByteString.CopyFrom("{\"overrides\":[{\"column\":\"GetPostgresFunctions:max_integer\",\"csharp_type\":{\"type\":\"int\"}},{\"column\":\"GetPostgresFunctions:max_varchar\",\"csharp_type\":{\"type\":\"string\"}},{\"column\":\"GetPostgresFunctions:max_timestamp\",\"csharp_type\":{\"type\":\"DateTime\"}}]}", Encoding.UTF8)
        };

        var response = CodeGenerator.Generate(request);
    }
}