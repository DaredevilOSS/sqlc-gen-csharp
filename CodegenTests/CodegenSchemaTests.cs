using Microsoft.CodeAnalysis.CSharp.Syntax;
using SqlcGenCsharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodegenTests;

public class CodegenSchemaTests
{
    private CodeGenerator CodeGenerator { get; } = new();

    [Test]
    public void TestDefaultSchemaEnum()
    {
        const string filename = "DefaultSchemaEnum/request.json";
        var request = RequestHelper.ParseRequestFile(filename);
        var response = CodeGenerator.Generate(request);

        var generatedModelsFile = response.Result.Files.First(f => f.Name == "Models.cs");
        Assert.That(generatedModelsFile, Is.Not.Null);

        var generatedModelsFileContents = generatedModelsFile.Contents.ToStringUtf8();
        var modelsCode = ParseCompilationUnit(generatedModelsFileContents);
        
        var expected = new HashSet<string>
        {
            "DummyTable",
            "DummyTableDummyColumn",
            "DummyTableDummyColumnExtensions"
        };
        var actual = GetMemberNames(modelsCode);
        Assert.That(actual.IsSupersetOf(expected));
    }

    [Test]
    public void TestSchemaScopedEnum()
    {
        const string filename = "SchemaScopedEnum/request.json";
        var request = RequestHelper.ParseRequestFile(filename);
        var response = CodeGenerator.Generate(request);

        var generatedModelsFile = response.Result.Files.First(f => f.Name == "Models.cs");
        Assert.That(generatedModelsFile, Is.Not.Null);

        var generatedModelsFileContents = generatedModelsFile.Contents.ToStringUtf8();
        var modelsCode = ParseCompilationUnit(generatedModelsFileContents);

        var expected = new HashSet<string>
        {
            "DummySchemaDummyTable",
            "DummySchemaDummyTableDummyColumn",
            "DummySchemaDummyTableDummyColumnExtensions"
        };
        var actual = GetMemberNames(modelsCode);
        Assert.That(actual.IsSupersetOf(expected));
    }

    private static HashSet<string> GetMemberNames(CompilationUnitSyntax compilationUnit)
    {
        var members = compilationUnit.DescendantNodes().OfType<MemberDeclarationSyntax>().ToList();
        return members
            .FindAll(m => m is EnumDeclarationSyntax)
            .Select(m => ((EnumDeclarationSyntax)m).Identifier.Text)
            .Union(
                members
                    .FindAll(m => m is ClassDeclarationSyntax)
                    .Select(m => ((ClassDeclarationSyntax)m).Identifier.Text))
            .Union(
                members
                    .FindAll(m => m is RecordDeclarationSyntax)
                    .Select(m => ((RecordDeclarationSyntax)m).Identifier.Text))
            .ToHashSet();
    }
}