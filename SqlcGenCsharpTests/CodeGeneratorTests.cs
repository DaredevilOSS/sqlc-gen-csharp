using Google.Protobuf;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp;
using System.Text;
using System.Xml;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharpTests;

public class CodeGeneratorTests
{
    private readonly Settings _mysqlSettings = new()
    {
        Engine = "mysql",
        Codegen = new Codegen { Out = "DummyProject" }
    };

    private readonly Settings _sqliteSettings = new()
    {
        Engine = "sqlite",
        Codegen = new Codegen { Out = "DummyProject" }
    };

    private readonly Catalog _emptyCatalog = new() { Schemas = { Capacity = 0 } };

    private CodeGenerator CodeGenerator { get; } = new();

    [Test]
    public void TestCsprojFileGenerated()
    {
        var request = new GenerateRequest
        {
            Settings = _mysqlSettings,
            Catalog = _emptyCatalog,
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
            Settings = _mysqlSettings,
            Catalog = _emptyCatalog,
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
            Settings = _mysqlSettings,
            Catalog = _emptyCatalog,
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
            Settings = _mysqlSettings,
            Catalog = _emptyCatalog,
            PluginOptions =
                ByteString.CopyFrom("{\"useDapper\": true, \"overrideDapperVersion\": \"" + expected + "\"}",
                    Encoding.UTF8)
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

    [Test]
    public void TestSqliteCopyFromGenerateUtilsMembers()
    {
        // data
        var dummyColumn = new Column { Name = "col_1", Type = new Identifier { Name = "text" } };
        var query = new Query
        {
            Filename = "query.sql",
            Cmd = ":copyfrom",
            Name = "CopyFromQuery",
            Columns = { dummyColumn },
            Params = { new Parameter { Column = dummyColumn } },
            InsertIntoTable = new Identifier { Name = "tab_1" }
        };

        var request = new GenerateRequest
        {
            Settings = _sqliteSettings,
            Catalog = _emptyCatalog,
            Queries = { query },
            PluginOptions = ByteString.CopyFrom("{}", Encoding.UTF8)
        };

        // execute
        var response = CodeGenerator.Generate(request);

        // assertions
        var generatedFile = response.Result.Files.First(f => f.Name == "Utils.cs");
        Assert.That(generatedFile, Is.Not.Null);

        var generatedFileContents = generatedFile.Contents.ToStringUtf8();
        var utilsCode = ParseCompilationUnit(generatedFileContents);
        var members = utilsCode.DescendantNodes().OfType<MemberDeclarationSyntax>().ToList();

        var valuesRegexExists = ValuesRegexMemberExists();
        Assert.That(valuesRegexExists, Is.True);
        var transformForBatchExists = TransformForBatchExists();
        Assert.That(transformForBatchExists, Is.True);
        return;

        bool ValuesRegexMemberExists()
        {
            return members.Any(x =>
            {
                if (x is FieldDeclarationSyntax f)
                    return f.Declaration.Variables.Any(v => v.Identifier.Text is "ValuesRegex");
                return false;
            });
        }

        bool TransformForBatchExists()
        {
            return members.Any(x =>
            {
                if (x is MethodDeclarationSyntax m)
                    return m.Identifier.Text is "TransformQueryForSqliteBatch";
                return false;
            });
        }
    }

    [Test]
    public void TestSqliteMissingCopyFromDoesNotGenerateUtilsMembers()
    {
        // data
        var dummyColumn = new Column { Name = "col_1", Type = new Identifier { Name = "text" } };
        var query = new Query
        {
            Filename = "query.sql",
            Cmd = ":one",
            Name = "OneQuery",
            Columns = { dummyColumn },
            Params = { new Parameter { Column = dummyColumn } },
            InsertIntoTable = new Identifier { Name = "tab_1" }
        };

        var request = new GenerateRequest
        {
            Settings = _sqliteSettings,
            Catalog = _emptyCatalog,
            Queries = { query },
            PluginOptions = ByteString.CopyFrom("{}", Encoding.UTF8)
        };

        // execute
        var response = CodeGenerator.Generate(request);

        // assert
        var fileGenerated = response.Result.Files.Any(f => f.Name == "Utils.cs");
        Assert.That(fileGenerated, Is.False);
    }

    [Test]
    public void TestSliceQueryGenerateUtilsMembers()
    {
        // data
        var query = new Query
        {
            Filename = "query.sql",
            Cmd = ":one",
            Name = "QueryWithSlice",
            Columns = { new Column { Name = "col_1", Type = new Identifier { Name = "text" } } },
            Params =
            {
                new Parameter
                {
                    Column = new Column
                    {
                        Name = "col_1", Type = new Identifier { Name = "text" }, IsSqlcSlice = true
                    }
                }
            },
            InsertIntoTable = new Identifier { Name = "tab_1" }
        };

        var request = new GenerateRequest
        {
            Settings = _sqliteSettings,
            Catalog = _emptyCatalog,
            Queries = { query },
            PluginOptions = ByteString.CopyFrom("{}", Encoding.UTF8)
        };

        // execute
        var response = CodeGenerator.Generate(request);

        // assertions
        var generatedFile = response.Result.Files.First(f => f.Name == "Utils.cs");
        Assert.That(generatedFile, Is.Not.Null);

        var generatedFileContents = generatedFile.Contents.ToStringUtf8();
        var utilsCode = ParseCompilationUnit(generatedFileContents);
        var members = utilsCode.DescendantNodes().OfType<MemberDeclarationSyntax>().ToList();

        var memberExists = TransformQueryForSliceMemberExists();
        Assert.That(memberExists, Is.True);
        return;

        bool TransformQueryForSliceMemberExists()
        {
            return members.Any(x =>
            {
                if (x is MethodDeclarationSyntax m)
                    return m.Identifier.Text is "TransformQueryForSliceArgs";
                return false;
            });
        }
    }

    [Test]
    public void TestMissingSliceQueryNotGenerateUtilsMembers()
    {
        // data
        var dummyColumn = new Column { Name = "col_1", Type = new Identifier { Name = "text" } };
        var query = new Query
        {
            Filename = "query.sql",
            Cmd = ":one",
            Name = "QueryWithSlice",
            Columns = { dummyColumn },
            Params =
            {
                new Parameter
                {
                    Column = dummyColumn
                }
            },
            InsertIntoTable = new Identifier { Name = "tab_1" }
        };

        var request = new GenerateRequest
        {
            Settings = _sqliteSettings,
            Catalog = _emptyCatalog,
            Queries = { query },
            PluginOptions = ByteString.CopyFrom("{}", Encoding.UTF8)
        };

        // execute
        var response = CodeGenerator.Generate(request);

        // assertions
        var fileGenerated = response.Result.Files.Any(f => f.Name == "Utils.cs");
        Assert.That(fileGenerated, Is.False);
    }
}