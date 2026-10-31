namespace MssqlSyntaxMatrix;

public class CaseResult(TestCase testCase)
{
    public TestCase Case { get; } = testCase;
    public Dictionary<string, SqlcResult> Sqlc { get; } = [];
    public OracleResult Oracle { get; set; } = new();
    public RunStatus CompileStatus { get; set; } = RunStatus.Skipped;
    public string CompileError { get; set; } = "";

    // the verdict of every query for one sqlc build, in case order
    public List<(CaseQuery Query, QueryVerdict Verdict)> Verdicts(string build)
    {
        var sqlc = Sqlc[build];
        return [.. Case.Queries.Select(q =>
        {
            var verdict = new QueryVerdict();
            if (sqlc.Status == RunStatus.Panic)
                verdict.Add(Level.Panic, $"sqlc panics: {FirstLine(sqlc.Error)}");
            else if (sqlc.Status == RunStatus.Error && sqlc.Error.Contains("no queries contained in paths"))
                verdict.Add(Level.Dropped, "every statement is silently dropped (sqlc: no queries contained in paths)");
            else if (sqlc.Status == RunStatus.Error)
                verdict.Add(Level.Error, sqlc.Error);
            else if (!sqlc.Queries.TryGetValue(q.Name, out var query))
                verdict.Add(Level.Dropped, "the query is silently missing from sqlc's output");
            else
                verdict = Comparison.Compare(query, Oracle.Queries.GetValueOrDefault(q.Name));
            return (q, verdict);
        })];
    }

    public Level CaseLevel(string build) =>
        Verdicts(build).Select(v => v.Verdict.Level).DefaultIfEmpty(Level.Ok).Max();

    public static string FirstLine(string text) => text.Split('\n').FirstOrDefault(l => l.Trim().Length > 0)?.Trim() ?? "";

    public object ToJson(MatrixOptions options) => new
    {
        id = Case.Id,
        description = Case.Description,
        schemaError = Oracle.SchemaError,
        builds = options.SqlcBuilds.ToDictionary(b => b.Name, b => new
        {
            status = Sqlc[b.Name].Status.ToString(),
            level = CaseLevel(b.Name).ToString(),
            error = Sqlc[b.Name].Error,
            queries = Verdicts(b.Name).Select(v => new
            {
                name = v.Query.Name,
                level = v.Verdict.Level.ToString(),
                findings = v.Verdict.Findings.Select(f => $"{f.Level}: {f.Message}")
            })
        }),
        plugin = Sqlc[options.PluginBuild.Name].PluginStatus.ToString(),
        pluginError = Sqlc[options.PluginBuild.Name].PluginError,
        compile = CompileStatus.ToString(),
        compileError = CompileError,
        oracle = Oracle.Queries.ToDictionary(q => q.Key, q => new
        {
            paramsError = q.Value.ParamsError,
            columnsError = q.Value.ColumnsError,
            @params = q.Value.Params?.Select(p => $"{p.Name} {p.Type}"),
            columns = q.Value.Columns?.Select(c => $"{c.Name} {c.Type}{(c.Nullable ? "" : " NOT NULL")}")
        })
    };
}