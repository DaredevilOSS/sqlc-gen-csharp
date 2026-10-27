using Google.Protobuf;
using NUnit.Framework;
using Plugin;
using SqlcGenCsharp;
using System.Text;

namespace CodegenTests;

// Query shapes every engine can produce, found by the SQL Server syntax matrix
public class CodegenQueryShapeTests
{
    private static GenerateRequest RequestFor(Query query, string options = "{}") => new()
    {
        Settings = new Settings { Engine = "postgresql", Codegen = new Codegen { Out = "DummyProject" } },
        Catalog = new Catalog { DefaultSchema = "public", Schemas = { new Schema { Name = "public" } } },
        Queries = { query },
        PluginOptions = ByteString.CopyFrom(options, Encoding.UTF8)
    };

    private static Column IntColumn(string name) => new() { Name = name, NotNull = true, Type = new Identifier { Name = "integer" } };

    private static async Task<string> GenerateQueries(GenerateRequest request)
    {
        var response = await new CodeGenerator().Generate(request);
        return response.Files.Single(f => f.Name == "QuerySql.cs").Contents.ToStringUtf8();
    }

    [Test]
    public async Task TestDuplicateColumnNamesGetASuffix()
    {
        var query = new Query
        {
            Name = "DuplicateIds",
            Cmd = ":many",
            Filename = "query.sql",
            Text = "SELECT a.id, b.id FROM a JOIN b ON b.a_id = a.id",
            Columns = { IntColumn("id"), IntColumn("id") }
        };

        var queries = await GenerateQueries(RequestFor(query));

        Assert.That(queries, Does.Contain("DuplicateIdsRow(int Id, int Id2)"));
    }

    [Test]
    public async Task TestDapperReadsDuplicateColumnNamesByOrdinal()
    {
        var query = new Query
        {
            Name = "DuplicateIds",
            Cmd = ":many",
            Filename = "query.sql",
            Text = "SELECT a.id, b.id FROM a JOIN b ON b.a_id = a.id",
            Columns = { IntColumn("id"), IntColumn("id") }
        };

        var queries = await GenerateQueries(RequestFor(query, """{"useDapper": true}"""));

        // Dapper maps by name and could not fill Id2
        Assert.That(queries, Does.Contain("Id2 = reader.GetInt32(1)"));
    }

    [Test]
    public async Task TestSingleLineQueryWithQuotesAndBackslashesIsEscaped()
    {
        var query = new Query
        {
            Name = "Quoted",
            Cmd = ":many",
            Filename = "query.sql",
            Text = """SELECT id AS "Author Id" FROM a WHERE name LIKE 'a\_%' ESCAPE '\'""",
            Columns = { IntColumn("Author Id") }
        };

        var queries = await GenerateQueries(RequestFor(query));

        Assert.That(queries, Does.Contain("""private const string QuotedSql = "SELECT id AS \"Author Id\" FROM a WHERE name LIKE 'a\\_%' ESCAPE '\\'";"""));
    }
}
