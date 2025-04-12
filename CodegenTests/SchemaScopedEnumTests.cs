using SqlcGenCsharp;

namespace CodegenTests;

public class SchemaScopedEnumTests
{
    private CodeGenerator CodeGenerator { get; } = new();

    [Test]
    public void TestSchemaScopedEnum()
    {
        const string filename = "SchemaScopedEnum/request.json";
        var request = RequestHelper.ParseRequestFile(filename);
        var response = CodeGenerator.Generate(request);
    }
}