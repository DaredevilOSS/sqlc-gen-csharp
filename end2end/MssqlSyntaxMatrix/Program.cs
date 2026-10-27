using dotenv.net;
using System.Text.Json;

namespace MssqlSyntaxMatrix;

// Runs every T-SQL case under cases/ through sqlc (one or more builds), the C# plugin and SQL Server's own
// sp_describe_* procedures, and writes a support matrix. See README.md.
public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var root = FindRepoRoot();
        DotEnv.Load(options: new DotEnvOptions(envFilePaths: [Path.Combine(root, ".env")]));

        var options = MatrixOptions.Parse(args, root);
        var cases = TestCase.LoadAll(Path.Combine(options.MatrixDir, "cases"));
        if (options.Filter is not null)
            cases = [.. cases.Where(c => c.Id.Contains(options.Filter, StringComparison.OrdinalIgnoreCase))];
        Console.Error.WriteLine($"{cases.Count} cases, sqlc builds: {string.Join(", ", options.SqlcBuilds.Select(b => b.Name))}");

        var oracle = new SqlServerOracle(options.ConnectionString);
        var workDir = Directory.CreateTempSubdirectory("mssql-matrix-");
        var results = new List<CaseResult>();
        foreach (var testCase in cases)
        {
            Console.Error.WriteLine($"- {testCase.Id}");
            var caseDir = Path.Combine(workDir.FullName, testCase.Id.Replace('/', '_'));
            Directory.CreateDirectory(caseDir);
            var result = new CaseResult(testCase);
            foreach (var build in options.SqlcBuilds)
                result.Sqlc[build.Name] = await SqlcRunner.Run(build, testCase, Path.Combine(caseDir, build.Name), options);
            result.Oracle = await oracle.Describe(testCase);
            results.Add(result);
        }

        if (options.Compile)
            await PluginCompiler.CompileAll(results, options, Path.Combine(workDir.FullName, "compile"));

        var report = MarkdownReport.Render(results, options);
        await File.WriteAllTextAsync(Path.Combine(options.MatrixDir, "REPORT.md"), report);
        await File.WriteAllTextAsync(Path.Combine(options.MatrixDir, "results.json"),
            JsonSerializer.Serialize(results.Select(r => r.ToJson(options)), new JsonSerializerOptions { WriteIndented = true }) + "\n");
        Console.Error.WriteLine($"wrote {Path.Combine(options.MatrixDir, "REPORT.md")}");
        return 0;
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "sqlc-gen-csharp.sln")))
            dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("repository root not found");
    }
}