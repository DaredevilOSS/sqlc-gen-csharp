using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace MssqlSyntaxMatrix;

public record OracleColumn(string Name, string Type, bool Nullable);

public record OracleParam(string Name, string Type);

public class OracleQuery
{
    public List<OracleParam>? Params { get; set; }
    public string? ParamsError { get; set; }
    public List<OracleColumn>? Columns { get; set; }
    public string? ColumnsError { get; set; }
}

public class OracleResult
{
    public string? SchemaError { get; set; }
    public Dictionary<string, OracleQuery> Queries { get; } = [];
}

// Asks SQL Server itself what a query takes and returns: sp_describe_undeclared_parameters for the parameters and
// sp_describe_first_result_set for the result columns. Every case gets a fresh database.
public partial class SqlServerOracle(string connectionString)
{
    private const string Database = "sqlc_syntax_matrix";

    public async Task<OracleResult> Describe(TestCase testCase)
    {
        var result = new OracleResult();
        var builder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };
        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();
        await Execute(connection, $"""
            IF DB_ID('{Database}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{Database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{Database}];
            END
            CREATE DATABASE [{Database}];
            """);
        await connection.ChangeDatabaseAsync(Database);

        try
        {
            foreach (var batch in GoRegex().Split(testCase.Schema).Where(b => b.Trim().Length > 0))
                await Execute(connection, batch);
        }
        catch (SqlException e)
        {
            result.SchemaError = e.Message;
            return result;
        }

        foreach (var query in testCase.Queries)
            result.Queries[query.Name] = await DescribeQuery(connection, ToNativeSql(query.Text));
        return result;
    }

    private static async Task<OracleQuery> DescribeQuery(SqlConnection connection, string sql)
    {
        var result = new OracleQuery();
        try
        {
            result.Params = await DescribeParams(connection, sql);
        }
        catch (SqlException e) when (e.Message.Contains("is used more than once"))
        {
            // SQL Server refuses a parameter used twice: describe every use under its own name and keep the first type
            try
            {
                var occurrence = 0;
                var unique = ParamRegex().Replace(sql, m => $"{m.Value}__{occurrence++}");
                result.Params = [.. (await DescribeParams(connection, unique))
                    .GroupBy(p => p.Name[..p.Name.LastIndexOf("__", StringComparison.Ordinal)], StringComparer.OrdinalIgnoreCase)
                    .Select(g => new OracleParam(g.Key, MergeTypes(g.Select(p => p.Type))))];
            }
            catch (SqlException inner)
            {
                result.Params = null;
                result.ParamsError = inner.Message;
            }
        }
        catch (SqlException e)
        {
            result.Params = null;
            result.ParamsError = e.Message;
        }

        try
        {
            result.Columns = [];
            await using var command = new SqlCommand("sp_describe_first_result_set", connection) { CommandType = System.Data.CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@tsql", sql);
            var declarations = result.Params?.Where(p => p.Type != "").Select(p => $"{p.Name} {p.Type}").ToList() ?? [];
            command.Parameters.AddWithValue("@params", declarations.Count > 0 ? string.Join(", ", declarations) : DBNull.Value);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.GetBoolean(reader.GetOrdinal("is_hidden")))
                    continue;
                result.Columns.Add(new OracleColumn(
                    reader.IsDBNull(reader.GetOrdinal("name")) ? "" : reader.GetString(reader.GetOrdinal("name")),
                    reader.IsDBNull(reader.GetOrdinal("system_type_name")) ? "" : reader.GetString(reader.GetOrdinal("system_type_name")),
                    reader.GetBoolean(reader.GetOrdinal("is_nullable"))));
            }
        }
        catch (SqlException e)
        {
            result.Columns = null;
            result.ColumnsError = e.Message;
        }
        return result;
    }

    // "@p IS NULL" makes SQL Server suggest int, its default, so a use that pins a real type wins over it
    private static string MergeTypes(IEnumerable<string> types)
    {
        var distinct = types.Where(t => t != "").Distinct().ToList();
        if (distinct.Count > 1)
            distinct.Remove("int");
        return distinct.FirstOrDefault() ?? "";
    }

    private static async Task<List<OracleParam>> DescribeParams(SqlConnection connection, string sql)
    {
        var result = new List<OracleParam>();
        await using var command = new SqlCommand("sp_describe_undeclared_parameters", connection) { CommandType = System.Data.CommandType.StoredProcedure };
        command.Parameters.AddWithValue("@tsql", sql);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(new OracleParam(
                reader.GetString(reader.GetOrdinal("name")),
                reader.IsDBNull(reader.GetOrdinal("suggested_system_type_name")) ? "" : reader.GetString(reader.GetOrdinal("suggested_system_type_name"))));
        return result;
    }

    // what sqlc turns its macros into, so SQL Server sees plain T-SQL
    public static string ToNativeSql(string sql)
    {
        sql = EmbedRegex().Replace(sql, m => m.Groups[1].Value + ".*");
        return MacroRegex().Replace(sql, m => "@" + m.Groups[1].Value);
    }

    private static async Task Execute(SqlConnection connection, string sql)
    {
        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    // @name, but not @@ROWCOUNT and not inside a string literal such as sql:variable("@status")
    [GeneratedRegex(@"(?<![@\w""])@\w+")]
    private static partial Regex ParamRegex();

    [GeneratedRegex(@"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)]
    private static partial Regex GoRegex();

    [GeneratedRegex(@"sqlc\.embed\(\s*['""]?(\w+)['""]?\s*\)", RegexOptions.IgnoreCase)]
    private static partial Regex EmbedRegex();

    [GeneratedRegex(@"sqlc\.(?:arg|narg|slice)\(\s*['""]?(\w+)['""]?\s*\)", RegexOptions.IgnoreCase)]
    private static partial Regex MacroRegex();
}