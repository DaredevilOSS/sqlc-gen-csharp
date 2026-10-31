namespace MssqlSyntaxMatrix;

public record SqlcBuild(string Name, string Path, string Label);

public class MatrixOptions
{
    public required string RepoRoot { get; init; }
    public required string MatrixDir { get; init; }
    public required List<SqlcBuild> SqlcBuilds { get; init; }
    public required string PluginPath { get; init; }
    public required string ConnectionString { get; init; }
    public bool Compile { get; init; }
    public string? Filter { get; init; }

    // the build whose output is fed to the plugin and the compiler: the last one given
    public SqlcBuild PluginBuild => SqlcBuilds[^1];

    // --sqlc name=path[@label] (repeatable, in report column order), --plugin path, --compile, --filter text
    public static MatrixOptions Parse(string[] args, string repoRoot)
    {
        var builds = new List<SqlcBuild>();
        var pluginPath = Path.Combine(repoRoot, "dist", "LocalRunner");
        var compile = false;
        string? filter = null;
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--sqlc":
                    var spec = args[++i];
                    var (nameAndPath, label) = spec.Contains('@') ? (spec[..spec.LastIndexOf('@')], spec[(spec.LastIndexOf('@') + 1)..]) : (spec, "");
                    var parts = nameAndPath.Split('=', 2);
                    builds.Add(new SqlcBuild(parts[0], parts[1], label));
                    break;
                case "--plugin":
                    pluginPath = Path.GetFullPath(args[++i]);
                    break;
                case "--compile":
                    compile = true;
                    break;
                case "--filter":
                    filter = args[++i];
                    break;
                default:
                    throw new ArgumentException($"unknown argument {args[i]}");
            }
        }
        if (builds.Count == 0)
            builds.Add(new SqlcBuild("sqlc", "sqlc", ""));

        return new MatrixOptions
        {
            RepoRoot = repoRoot,
            MatrixDir = Path.Combine(repoRoot, "end2end", "MssqlSyntaxMatrix"),
            SqlcBuilds = builds,
            PluginPath = pluginPath,
            ConnectionString = Environment.GetEnvironmentVariable("MSSQL_CONNECTION_STRING")
                               ?? throw new InvalidOperationException("MSSQL_CONNECTION_STRING is not set"),
            Compile = compile,
            Filter = filter
        };
    }
}