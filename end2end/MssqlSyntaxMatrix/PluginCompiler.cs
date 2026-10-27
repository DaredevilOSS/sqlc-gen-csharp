using System.Text.RegularExpressions;

namespace MssqlSyntaxMatrix;

// Builds the plugin output of every case in one project, so a case whose generated C# does not compile is spotted
// without paying for one build per case.
public static partial class PluginCompiler
{
    public static async Task CompileAll(List<CaseResult> results, MatrixOptions options, string dir)
    {
        Directory.CreateDirectory(dir);
        var byNamespace = new Dictionary<string, CaseResult>();
        foreach (var result in results)
        {
            var sqlc = result.Sqlc[options.PluginBuild.Name];
            if (sqlc.PluginOutputDir is null)
                continue;
            var ns = SqlcRunner.CaseNamespace(result.Case);
            byNamespace[ns] = result;
            result.CompileStatus = RunStatus.Ok;
            foreach (var file in Directory.GetFiles(sqlc.PluginOutputDir, "*.cs", SearchOption.AllDirectories))
            {
                var target = Path.Combine(dir, ns, Path.GetRelativePath(sqlc.PluginOutputDir, file));
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                File.Copy(file, target, true);
            }
        }

        await File.WriteAllTextAsync(Path.Combine(dir, "Compile.csproj"), """
            <Project Sdk="Microsoft.NET.Sdk">
                <PropertyGroup>
                    <TargetFramework>net10.0</TargetFramework>
                    <Nullable>enable</Nullable>
                    <OutputType>Library</OutputType>
                    <NoWarn>$(NoWarn);CS8618;CS8604;CS8600;CS8601;CS8602;CS8603;NU1902;NU1903</NoWarn>
                </PropertyGroup>
                <ItemGroup>
                    <PackageReference Include="Dapper" Version="2.1.66" />
                    <PackageReference Include="Microsoft.Data.SqlClient" Version="7.1.1" />
                    <PackageReference Include="Microsoft.SqlServer.Types" Version="160.1000.6" />
                    <PackageReference Include="NodaTime" Version="3.2.0" />
                </ItemGroup>
            </Project>
            """);

        var (_, output) = await SqlcRunner.Exec("dotnet", "build -nologo -clp:NoSummary", dir);
        foreach (Match m in ErrorRegex().Matches(output))
        {
            var relative = Path.GetRelativePath(dir, m.Groups["file"].Value);
            var ns = relative.Split(Path.DirectorySeparatorChar)[0];
            if (!byNamespace.TryGetValue(ns, out var result))
                continue;
            result.CompileStatus = RunStatus.Error;
            var error = $"{string.Join('/', relative.Split(Path.DirectorySeparatorChar).Skip(1))}: {m.Groups["code"].Value} {m.Groups["message"].Value}";
            if (!result.CompileError.Contains(error) && result.CompileError.Split('\n').Length < 5)
                result.CompileError = (result.CompileError + "\n" + error).Trim();
        }
    }

    [GeneratedRegex(@"^(?<file>[^\r\n]*?\.cs)\(\d+,\d+\): error (?<code>CS\d+): (?<message>[^\r\n]*?) \[", RegexOptions.Multiline)]
    private static partial Regex ErrorRegex();
}