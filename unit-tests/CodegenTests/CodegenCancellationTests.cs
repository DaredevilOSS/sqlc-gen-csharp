using Google.Protobuf;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Plugin;
using SqlcGenCsharp;
using System.Text;
using System.Text.Json.Nodes;

namespace CodegenTests;

public class CodegenCancellationTests
{
    private CodeGenerator CodeGenerator { get; } = new();

    private static readonly Catalog EmptyCatalog = new()
    {
        Schemas = { new Schema { Name = string.Empty, Tables = { Capacity = 0 }, Enums = { Capacity = 0 } } }
    };

    private static Query OneQuery() => new()
    {
        Filename = "query.sql",
        Cmd = ":one",
        Name = "GetAuthor",
        Columns = { new Column { Name = "name", Type = new Identifier { Name = "text" }, NotNull = true } },
        Params = { new Parameter { Number = 1, Column = new Column { Name = "id", Type = new Identifier { Name = "integer" }, NotNull = true } } },
        Text = "SELECT name FROM authors WHERE id = $1"
    };

    private static Query CopyFromQuery() => new()
    {
        Filename = "query.sql",
        Cmd = ":copyfrom",
        Name = "CopyFromAuthors",
        Columns = { new Column { Name = "name", Type = new Identifier { Name = "text" } } },
        Params = { new Parameter { Column = new Column { Name = "name", Type = new Identifier { Name = "text" } } } },
        InsertIntoTable = new Identifier { Name = "authors" }
    };

    private static Query ExecQuery() => new()
    {
        Filename = "query.sql",
        Cmd = ":exec",
        Name = "DeleteAuthor",
        Params = { new Parameter { Number = 1, Column = new Column { Name = "id", Type = new Identifier { Name = "integer" }, NotNull = true } } },
        Text = "DELETE FROM authors WHERE id = $1"
    };

    // Returns the full source of the generated method, found by name across all generated files.
    private string GenerateMethod(string engine, bool withCancellationToken, Query query, string methodName, bool useDapper = false)
    {
        var flags = new List<string>();
        if (withCancellationToken) flags.Add("\"withCancellationToken\":true");
        if (useDapper) flags.Add("\"useDapper\":true");
        var options = "{" + string.Join(",", flags) + "}";
        var request = new GenerateRequest
        {
            Settings = new Settings { Engine = engine, Codegen = new Codegen { Out = "DummyProject" } },
            Catalog = EmptyCatalog,
            Queries = { query },
            PluginOptions = ByteString.CopyFrom(options, Encoding.UTF8)
        };

        var response = CodeGenerator.Generate(request);
        foreach (var file in response.Result.Files)
        {
            var unit = CSharpSyntaxTree.ParseText(file.Contents.ToStringUtf8()).GetCompilationUnitRoot();
            var method = unit.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(m => m.Identifier.Text == methodName);
            if (method is not null)
                return method.ToFullString();
        }

        throw new AssertionException($"method {methodName} not found in generated files");
    }

    [Test]
    public void EnabledAddsOptionalTokenToOneQuerySignature()
    {
        var method = GenerateMethod("postgresql", withCancellationToken: true, OneQuery(), "GetAuthorAsync");
        Assert.That(method, Does.Contain("CancellationToken cancellationToken = default"));
    }

    [Test]
    public void DisabledLeavesOneQuerySignatureWithoutToken()
    {
        var method = GenerateMethod("postgresql", withCancellationToken: false, OneQuery(), "GetAuthorAsync");
        Assert.That(method, Does.Not.Contain("CancellationToken"));
    }

    [Test]
    public void EnabledThreadsTokenIntoReaderCalls()
    {
        var method = GenerateMethod("postgresql", withCancellationToken: true, OneQuery(), "GetAuthorAsync");
        Assert.That(method, Does.Contain("ExecuteReaderAsync(cancellationToken)"));
        Assert.That(method, Does.Contain("ReadAsync(cancellationToken)"));
        Assert.That(method, Does.Contain("OpenConnectionAsync(cancellationToken)"));
    }

    [Test]
    public void EnabledAddsOptionalTokenToCopyFromSignature()
    {
        var method = GenerateMethod("postgresql", withCancellationToken: true, CopyFromQuery(), "CopyFromAuthorsAsync");
        Assert.That(method, Does.Contain("CancellationToken cancellationToken = default"));
    }

    [Test]
    public void EnabledThreadsTokenIntoNonQueryExecution()
    {
        var method = GenerateMethod("postgresql", withCancellationToken: true, ExecQuery(), "DeleteAuthorAsync");
        Assert.That(method, Does.Contain("ExecuteNonQueryAsync(cancellationToken)"));
    }

    [Test]
    public void DisabledLeavesAsyncCallsWithoutToken()
    {
        var method = GenerateMethod("postgresql", withCancellationToken: false, OneQuery(), "GetAuthorAsync");
        Assert.That(method, Does.Contain("ExecuteReaderAsync()"));
        Assert.That(method, Does.Contain("ReadAsync()"));
    }

    [Test]
    public void EnabledWrapsDapperCallsInCommandDefinitionWithToken()
    {
        var method = GenerateMethod("postgresql", withCancellationToken: true, OneQuery(), "GetAuthorAsync", useDapper: true);
        Assert.That(method, Does.Contain("new CommandDefinition("));
        Assert.That(method, Does.Contain("cancellationToken: cancellationToken"));
    }

    [Test]
    public void DisabledLeavesDapperCallsWithoutCommandDefinition()
    {
        var method = GenerateMethod("postgresql", withCancellationToken: false, OneQuery(), "GetAuthorAsync", useDapper: true);
        Assert.That(method, Does.Not.Contain("CommandDefinition"));
    }

    // The example requests cover every annotation (:one/:many/:exec/:execrows/:execlastid/:copyfrom)
    // across all engines and both the driver and Dapper paths.
    private static readonly string[] ExampleRequests =
    [
        "NpgsqlExampleRequest.message",
        "NpgsqlDapperExampleRequest.message",
        "MySqlConnectorExampleRequest.message",
        "MySqlConnectorDapperExampleRequest.message",
        "SqliteExampleRequest.message",
        "SqliteDapperExampleRequest.message"
    ];

    private static GenerateRequest ExampleRequestWith(string requestFile, bool withCancellationToken)
    {
        var path = Path.Combine(AppContext.BaseDirectory, requestFile);
        var request = GenerateRequest.Parser.ParseFrom(System.IO.File.ReadAllBytes(path));
        var raw = request.PluginOptions.ToStringUtf8();
        var options = (JsonObject)(JsonNode.Parse(string.IsNullOrWhiteSpace(raw) ? "{}" : raw) ?? new JsonObject());
        options["withCancellationToken"] = withCancellationToken;
        request.PluginOptions = ByteString.CopyFrom(options.ToJsonString(), Encoding.UTF8);
        return request;
    }

    private static IEnumerable<Plugin.File> GeneratedCsharpFiles(GenerateRequest request)
    {
        return new CodeGenerator().Generate(request).Result.Files.Where(f => f.Name.EndsWith(".cs"));
    }

    [TestCaseSource(nameof(ExampleRequests))]
    public void EnabledGeneratesSyntacticallyValidCodeForEveryAnnotation(string requestFile)
    {
        var files = GeneratedCsharpFiles(ExampleRequestWith(requestFile, withCancellationToken: true)).ToList();

        foreach (var file in files)
        {
            var errors = CSharpSyntaxTree.ParseText(file.Contents.ToStringUtf8())
                .GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .ToList();
            Assert.That(errors, Is.Empty, $"syntax errors in {file.Name}: {string.Join("; ", errors)}");
        }

        Assert.That(files.Any(f => f.Contents.ToStringUtf8().Contains("cancellationToken")),
            "expected the generated queries to mention cancellationToken");
    }

    [TestCaseSource(nameof(ExampleRequests))]
    public void DisabledMentionsNoCancellationConstructs(string requestFile)
    {
        foreach (var file in GeneratedCsharpFiles(ExampleRequestWith(requestFile, withCancellationToken: false)))
        {
            var contents = file.Contents.ToStringUtf8();
            Assert.That(contents, Does.Not.Contain("cancellationToken"), file.Name);
            Assert.That(contents, Does.Not.Contain("CommandDefinition"), file.Name);
        }
    }
}