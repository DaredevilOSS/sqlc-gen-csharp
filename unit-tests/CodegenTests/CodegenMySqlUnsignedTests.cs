using Google.Protobuf;
using NUnit.Framework;
using Plugin;
using SqlcGenCsharp;
using System.Text;

namespace CodegenTests;

public class CodegenMySqlUnsignedTests
{
    private CodeGenerator CodeGenerator { get; } = new();

    private static Column CreateColumn(string name, string type, int length = -1, bool unsigned = false) =>
        new() { Name = name, Type = new Identifier { Name = type }, Length = length, Unsigned = unsigned };

    private static GenerateRequest CreateRequest(string cmd, params Column[] columns)
    {
        var table = new Table { Rel = new Identifier { Name = "dummy_table" } };
        table.Columns.AddRange(columns);
        var query = new Query
        {
            Name = "DummyQuery",
            Cmd = cmd,
            Text = "SELECT 1",
            Filename = "query.sql",
            InsertIntoTable = new Identifier { Name = "dummy_table" }
        };
        if (cmd == ":copyfrom")
            query.Params.AddRange(columns.Select((c, i) => new Parameter { Number = i + 1, Column = c }));
        else
            query.Columns.AddRange(columns);

        var schema = new Schema { Name = string.Empty };
        schema.Tables.Add(table);
        var catalog = new Catalog { DefaultSchema = string.Empty };
        catalog.Schemas.Add(schema);
        var request = new GenerateRequest
        {
            Settings = new Settings { Engine = "mysql", Codegen = new Codegen { Out = "DummyProject" } },
            Catalog = catalog,
            PluginOptions = ByteString.CopyFrom("{}", Encoding.UTF8)
        };
        request.Queries.Add(query);
        return request;
    }

    private string GenerateQueriesCode(GenerateRequest request)
    {
        var response = CodeGenerator.Generate(request);
        return response.Result.Files.First(f => f.Name == "QuerySql.cs").Contents.ToStringUtf8();
    }

    [Test]
    public void TestUnsignedIntegersAreMappedToUnsignedTypes()
    {
        var request = CreateRequest(
            ":one",
            CreateColumn("c_tinyint", "tinyint", 3, unsigned: true),
            CreateColumn("c_smallint", "smallint", unsigned: true),
            CreateColumn("c_int", "int", unsigned: true),
            CreateColumn("c_bigint", "bigint", unsigned: true));

        var code = GenerateQueriesCode(request);

        Assert.That(code, Does.Contain("GetFieldValue<byte>"));
        Assert.That(code, Does.Contain("GetUInt16"));
        Assert.That(code, Does.Contain("GetUInt32"));
        Assert.That(code, Does.Contain("GetUInt64"));
        Assert.That(code, Does.Not.Contain("GetInt16"));
        Assert.That(code, Does.Not.Contain("GetInt32"));
        Assert.That(code, Does.Not.Contain("GetInt64"));
    }

    [Test]
    public void TestSignedIntegersKeepSignedTypes()
    {
        var request = CreateRequest(
            ":one",
            CreateColumn("c_tinyint", "tinyint", 3),
            CreateColumn("c_smallint", "smallint"),
            CreateColumn("c_int", "int"),
            CreateColumn("c_bigint", "bigint"));

        var code = GenerateQueriesCode(request);

        Assert.That(code, Does.Contain("GetInt16"));
        Assert.That(code, Does.Contain("GetInt32"));
        Assert.That(code, Does.Contain("GetInt64"));
        Assert.That(code, Does.Not.Contain("GetUInt"));
    }

    [Test]
    public void TestUnsignedTinyIntWithLengthOneRemainsBool()
    {
        var request = CreateRequest(":one", CreateColumn("c_bool", "tinyint", 1, unsigned: true));

        Assert.That(GenerateQueriesCode(request), Does.Contain("GetBoolean"));
    }

    [Test]
    public void TestBitMappingDependsOnLength()
    {
        var request = CreateRequest(
            ":one",
            CreateColumn("c_bit_narrow", "bit", 8),
            CreateColumn("c_bit_wide", "bit", 32));

        var code = GenerateQueriesCode(request);

        Assert.That(code, Does.Contain("GetFieldValue<byte>(0)"));
        Assert.That(code, Does.Contain("GetUInt64(1)"));
    }

    [Test]
    public void TestCopyFromWithWideBitIsNotSupported()
    {
        var request = CreateRequest(":copyfrom", CreateColumn("c_bit_wide", "bit", 32));

        Assert.That(() => GenerateQueriesCode(request), Throws.Exception);
    }

    [Test]
    public void TestCopyFromWithBitAndUnsignedTinyIntIsNotSupported()
    {
        var request = CreateRequest(
            ":copyfrom",
            CreateColumn("c_bit", "bit", 8),
            CreateColumn("c_tinyint", "tinyint", 3, unsigned: true));

        Assert.That(() => GenerateQueriesCode(request), Throws.Exception);
    }
}