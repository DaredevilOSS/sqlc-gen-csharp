using SqlcGenCsharp;

namespace CodegenTests;

public class DefaultSchemaEnumTests
{
    private CodeGenerator CodeGenerator { get; } = new();

    [Test]
    public void TestDefaultSchemaEnum()
    {
        const string filename = "DefaultSchemaEnum/request.json";
        var request = RequestHelper.ParseRequestFile(filename);
        var response = CodeGenerator.Generate(request);
    }
}