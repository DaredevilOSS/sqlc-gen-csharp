namespace MssqlSyntaxMatrix;

// Ordered from best to worst; a case gets the worst level of its queries.
public enum Level { Ok, Minor, Mismatch, Dropped, Error, Panic }

public record Finding(Level Level, string Message);

public class QueryVerdict
{
    public Level Level => Findings.Count == 0 ? Level.Ok : Findings.Max(f => f.Level);
    public List<Finding> Findings { get; } = [];
    public void Add(Level level, string message) => Findings.Add(new Finding(level, message));
}

// Compares what sqlc reports for a query with what SQL Server reports for it.
public static class Comparison
{
    private static readonly Dictionary<string, string> Synonyms = new()
    {
        ["numeric"] = "decimal",
        ["integer"] = "int",
        ["double precision"] = "float",
        ["rowversion"] = "timestamp",
        ["sysname"] = "nvarchar",
        ["character varying"] = "varchar",
        ["national character varying"] = "nvarchar"
    };

    private static readonly string[][] Families =
    [
        ["bit"],
        ["tinyint", "smallint", "int", "bigint"],
        ["real", "float"],
        ["decimal", "money", "smallmoney"],
        ["char", "nchar", "varchar", "nvarchar", "text", "ntext"],
        ["binary", "varbinary", "image", "timestamp"],
        ["date", "datetime", "datetime2", "smalldatetime"]
    ];

    public static string Normalize(string type)
    {
        var t = type.Trim().ToLowerInvariant();
        var paren = t.IndexOf('(');
        if (paren >= 0)
            t = t[..paren].Trim();
        if (t.StartsWith("sys.") || t.StartsWith("dbo."))
            t = t[4..];
        return Synonyms.GetValueOrDefault(t, t);
    }

    private static bool IsUntyped(string type) => Normalize(type) is "" or "any" or "unknown";

    private static bool SameFamily(string a, string b) => Families.Any(f => f.Contains(a) && f.Contains(b));

    public static QueryVerdict Compare(SqlcQuery query, OracleQuery? oracle)
    {
        var verdict = new QueryVerdict();
        CompareParams(query, oracle, verdict);
        CompareColumns(query, oracle, verdict);
        return verdict;
    }

    private static void CompareParams(SqlcQuery query, OracleQuery? oracle, QueryVerdict verdict)
    {
        foreach (var p in query.Params.Where(p => IsUntyped(p.Type)))
        {
            var oracleType = oracle?.Params?.FirstOrDefault(o => Same(o.Name, p.Name))?.Type;
            verdict.Add(Level.Mismatch, oracleType is { Length: > 0 }
                ? $"parameter @{p.Name} has no type (SQL Server: {oracleType})"
                : $"parameter @{p.Name} has no type (SQL Server cannot deduce it either)");
        }

        if (oracle?.Params is null)
            return;

        foreach (var o in oracle.Params)
        {
            var p = query.Params.FirstOrDefault(p => Same(o.Name, p.Name));
            if (p is null)
            {
                verdict.Add(Level.Mismatch, $"parameter {o.Name} ({o.Type}) is missing");
                continue;
            }
            if (IsUntyped(p.Type) || o.Type == "")
                continue;
            var (sqlcType, oracleType) = (Normalize(p.Type), Normalize(o.Type));
            if (sqlcType == oracleType)
                continue;
            verdict.Add(SameFamily(sqlcType, oracleType) ? Level.Minor : Level.Mismatch,
                $"parameter @{p.Name}: sqlc {p.Type}, SQL Server {o.Type}");
        }

        foreach (var p in query.Params.Where(p => !oracle.Params.Any(o => Same(o.Name, p.Name))))
            verdict.Add(Level.Mismatch, $"parameter @{p.Name} is not a parameter for SQL Server");
    }

    private static void CompareColumns(SqlcQuery query, OracleQuery? oracle, QueryVerdict verdict)
    {
        foreach (var c in query.Columns.Where(c => c.EmbedTable is null && IsUntyped(c.Type)))
            verdict.Add(Level.Mismatch, $"column {Display(c.Name)} has no type");

        if (oracle?.Columns is null || query.Columns.Any(c => c.EmbedTable is not null))
            return;

        if (query.Columns.Count != oracle.Columns.Count)
        {
            verdict.Add(Level.Mismatch, $"{query.Columns.Count} result columns, SQL Server returns {oracle.Columns.Count}" +
                                        $" ({string.Join(", ", oracle.Columns.Select(c => Display(c.Name)))})");
            return;
        }

        for (var i = 0; i < query.Columns.Count; i++)
        {
            var (c, o) = (query.Columns[i], oracle.Columns[i]);
            if (o.Name == "")
                verdict.Add(Level.Minor, $"column {i + 1} has no name in SQL Server (sqlc calls it {Display(c.Name)})");
            else if (!Same(c.Name, o.Name))
                verdict.Add(Level.Mismatch, $"column {i + 1} is named {Display(c.Name)}, SQL Server names it {o.Name}");
            else if (c.Name != o.Name)
                verdict.Add(Level.Minor, $"column {i + 1} is named {c.Name}, SQL Server keeps the case: {o.Name}");

            if (!IsUntyped(c.Type) && Normalize(c.Type) != Normalize(o.Type))
                verdict.Add(Level.Mismatch, $"column {Display(c.Name)}: sqlc {c.Type}, SQL Server {o.Type}");

            // SQL Server reports most expressions (COUNT(*), CASE, ...) as nullable, so only a table column is a sure miss
            if (c.NotNull && o.Nullable)
                verdict.Add(c.FromTable ? Level.Mismatch : Level.Minor,
                    $"column {Display(c.Name)}: sqlc NOT NULL, SQL Server nullable" + (c.FromTable ? "" : " (SQL Server is conservative for expressions)"));
            else if (!c.NotNull && !o.Nullable)
                verdict.Add(Level.Minor, $"column {Display(c.Name)}: sqlc nullable, SQL Server NOT NULL");
        }
    }

    private static bool Same(string a, string b) =>
        string.Equals(a.TrimStart('@'), b.TrimStart('@'), StringComparison.OrdinalIgnoreCase);

    private static string Display(string name) => name == "" ? "(unnamed)" : name;
}