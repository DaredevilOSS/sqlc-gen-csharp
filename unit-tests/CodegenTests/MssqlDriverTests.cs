using NUnit.Framework;
using Plugin;
using SqlcGenCsharp;
using System.Text;

namespace CodegenTests;

public class MssqlDriverTests
{
    private static async Task<Dictionary<string, string>> GenerateFilesFor(string requestFile)
    {
        var request = GenerateRequest.Parser.ParseFrom(System.IO.File.ReadAllBytes(requestFile));
        var response = await new CodeGenerator().Generate(request);
        return response.Files.ToDictionary(f => f.Name, f => Encoding.UTF8.GetString(f.Contents.ToByteArray()));
    }

    [Test]
    public async Task TestParametersAreBoundWithAnExplicitSqlDbType()
    {
        var files = await GenerateFilesFor("MssqlExampleRequest.message");
        var queries = files["QuerySql.cs"];

        // a null sent as nvarchar cannot be converted to a binary type, and DateTime must not fall back to datetime
        Assert.That(queries, Does.Contain("""Parameters.Add("@c_varbinary", SqlDbType.VarBinary).Value = args.CVarbinary ?? (object)DBNull.Value;"""));
        Assert.That(queries, Does.Contain("""Parameters.Add("@c_datetime2", SqlDbType.DateTime2)"""));
        // decimals take their precision and scale from the value
        Assert.That(queries, Does.Contain("""Parameters.AddWithValue("@c_decimal", args.CDecimal ?? (object)DBNull.Value);"""));
    }

    [Test]
    public async Task TestCopyFromUsesSqlBulkCopy()
    {
        var files = await GenerateFilesFor("MssqlExampleRequest.message");
        var queries = files["QuerySql.cs"];

        Assert.That(queries, Does.Contain("new SqlBulkCopy(connection)"));
        Assert.That(queries, Does.Contain("""DestinationTableName = "[mssql_numeric_types]";"""));
    }

    [Test]
    public async Task TestSliceQueriesGenerateTheSliceTransformUtil()
    {
        var files = await GenerateFilesFor("MssqlExampleRequest.message");

        Assert.That(files["QuerySql.cs"], Does.Contain("""WHERE id IN (/*SLICE:ids*/@ids)"""));
        Assert.That(files["Utils.cs"], Does.Contain("TransformQueryForSliceArgs"));
    }

    [Test]
    public async Task TestDapperConfiguresDateTimeAndBinaryParameters()
    {
        var files = await GenerateFilesFor("MssqlDapperExampleRequest.message");

        Assert.That(files["Utils.cs"], Does.Contain("SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime2);"));
        Assert.That(files["Utils.cs"], Does.Contain("class BinaryParameter"));
        Assert.That(files["QuerySql.cs"], Does.Contain("new Utils.BinaryParameter(args.CBinary)"));
    }
}