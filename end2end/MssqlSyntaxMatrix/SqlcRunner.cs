using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MssqlSyntaxMatrix;

public record SqlcColumn(string Name, string Type, bool NotNull, string? EmbedTable, bool IsArray, bool IsSlice, bool FromTable);

public record SqlcQuery(string Name, string Cmd, string Text, List<SqlcColumn> Columns, List<SqlcColumn> Params);

public enum RunStatus { Ok, Error, Panic, Skipped }

public class SqlcResult
{
    public RunStatus Status { get; set; }
    public string Error { get; set; } = "";
    public Dictionary<string, SqlcQuery> Queries { get; } = [];
    public RunStatus PluginStatus { get; set; } = RunStatus.Skipped;
    public string PluginError { get; set; } = "";
    public string? PluginOutputDir { get; set; }
}

public static partial class SqlcRunner
{
    public static async Task<SqlcResult> Run(SqlcBuild build, TestCase testCase, string dir, MatrixOptions options)
    {
        Directory.CreateDirectory(dir);
        await File.WriteAllTextAsync(Path.Combine(dir, "schema.sql"), testCase.Schema);
        await File.WriteAllTextAsync(Path.Combine(dir, "query.sql"), testCase.QueriesText);
        await File.WriteAllTextAsync(Path.Combine(dir, "sqlc.json.yaml"), """
            version: "2"
            sql:
              - engine: mssql
                schema: schema.sql
                queries: query.sql
                gen:
                  json:
                    out: json
            """);

        var result = new SqlcResult();
        var (exitCode, output) = await Exec(build.Path, "-f sqlc.json.yaml generate", dir);
        if (exitCode != 0)
        {
            result.Status = output.Contains("panic:") ? RunStatus.Panic : RunStatus.Error;
            result.Error = Summarize(output.Replace(dir + Path.DirectorySeparatorChar, ""));
            return result;
        }

        result.Status = RunStatus.Ok;
        using var request = JsonDocument.Parse(await File.ReadAllTextAsync(Path.Combine(dir, "json", "codegen_request.json")));
        foreach (var q in request.RootElement.GetProperty("queries").EnumerateArray())
        {
            var query = new SqlcQuery(
                q.GetProperty("name").GetString()!,
                q.GetProperty("cmd").GetString()!,
                q.GetProperty("text").GetString()!,
                [.. q.GetProperty("columns").EnumerateArray().Select(ReadColumn)],
                [.. q.GetProperty("params").EnumerateArray().Select(p =>
                {
                    var column = ReadColumn(p.GetProperty("column"));
                    var name = p.TryGetProperty("name", out var n) && n.GetString() is { Length: > 0 } s ? s : column.Name;
                    return column with { Name = name };
                })]);
            result.Queries[query.Name] = query;
        }

        if (build == options.PluginBuild)
            await RunPlugin(dir, testCase, options, result);
        return result;
    }

    private static SqlcColumn ReadColumn(JsonElement c)
    {
        var embed = c.TryGetProperty("embed_table", out var e) && e.ValueKind == JsonValueKind.Object
            ? e.GetProperty("name").GetString()
            : null;
        return new SqlcColumn(
            c.GetProperty("name").GetString() ?? "",
            c.GetProperty("type").GetProperty("name").GetString() ?? "",
            c.GetProperty("not_null").GetBoolean(),
            embed,
            c.GetProperty("is_array").GetBoolean(),
            c.TryGetProperty("is_sqlc_slice", out var s) && s.GetBoolean(),
            c.TryGetProperty("table", out var t) && t.ValueKind == JsonValueKind.Object && t.GetProperty("name").GetString() is { Length: > 0 });
    }

    // the plugin generates both the driver and the Dapper flavour, as namespaces the compile step can tell apart
    private static async Task RunPlugin(string dir, TestCase testCase, MatrixOptions options, SqlcResult result)
    {
        var ns = CaseNamespace(testCase);
        await File.WriteAllTextAsync(Path.Combine(dir, "sqlc.plugin.yaml"), $"""
            version: "2"
            plugins:
              - name: csharp
                process:
                  cmd: {options.PluginPath}
            sql:
              - engine: mssql
                schema: schema.sql
                queries: query.sql
                codegen:
                  - plugin: csharp
                    out: csharp/driver
                    options:
                      namespaceName: {ns}.Driver
                      generateCsproj: false
                      targetFramework: net10.0
                  - plugin: csharp
                    out: csharp/dapper
                    options:
                      namespaceName: {ns}.Dapper
                      generateCsproj: false
                      useDapper: true
                      targetFramework: net10.0
            """);
        var (exitCode, output) = await Exec(options.PluginBuild.Path, "-f sqlc.plugin.yaml generate", dir);
        result.PluginStatus = exitCode == 0 ? RunStatus.Ok : RunStatus.Error;
        result.PluginError = exitCode == 0 ? "" : Summarize(output.Replace(dir + Path.DirectorySeparatorChar, ""));
        result.PluginOutputDir = exitCode == 0 ? Path.Combine(dir, "csharp") : null;
    }

    public static string CaseNamespace(TestCase testCase) =>
        "Case_" + string.Concat(testCase.Id.Select(ch => char.IsLetterOrDigit(ch) ? ch : '_'));

    // keeps the lines that explain a failure and drops stack traces
    private static string Summarize(string output)
    {
        var lines = output.Split('\n')
            .Select(l => l.TrimEnd())
            .Where(l => l.Length > 0 && !l.StartsWith('\t') && !l.StartsWith("   at ") && !l.StartsWith("goroutine ")
                        && !l.StartsWith("# package") && !l.StartsWith("[signal"))
            .Take(6);
        // goroutine traces carry addresses that change on every run
        return HexRegex().Replace(string.Join("\n", lines), "0x?");
    }

    [GeneratedRegex(@"0x[0-9a-fA-F]+\??")]
    private static partial Regex HexRegex();

    public static async Task<(int ExitCode, string Output)> Exec(string file, string arguments, string workingDir)
    {
        var psi = new ProcessStartInfo(file, arguments)
        {
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        using var process = Process.Start(psi)!;
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        await process.WaitForExitAsync(cts.Token);
        return (process.ExitCode, (await stdout) + (await stderr));
    }
}