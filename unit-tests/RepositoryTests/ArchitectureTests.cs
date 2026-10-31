using System.Xml.Linq;

namespace RepositoryTests;

/// <summary>
/// Guards the plugin's layering (see AGENTS.md, "Project dependency graph").
/// Adding a project reference requires updating this map on purpose.
/// </summary>
public class ArchitectureTests
{
    private static readonly Dictionary<string, string[]> AllowedProjectReferences = new()
    {
        ["GeneratedProtobuf/GeneratedProtobuf.csproj"] = [],
        ["Extensions/Extensions.csproj"] = [],
        ["PluginOptions/PluginOptions.csproj"] = ["GeneratedProtobuf"],
        ["Drivers/Drivers.csproj"] = ["Extensions", "PluginOptions"],
        ["CodeGenerator/CodeGenerator.csproj"] = ["Drivers", "GeneratedProtobuf", "PluginOptions"],
        ["SqlcGenCsharp/SqlcGenCsharp.csproj"] = ["CodeGenerator"],
        ["LocalRunner/LocalRunner.csproj"] = ["SqlcGenCsharp"],
        ["WasmRunner/WasmRunner.csproj"] = ["SqlcGenCsharp"]
    };

    [Test]
    [TestCaseSource(nameof(PluginProjects))]
    public void TestPluginProjectReferencesFollowLayering(string csprojPath)
    {
        var fullPath = Path.Combine(FindRepositoryRoot(), csprojPath);
        Assert.That(File.Exists(fullPath), $"{csprojPath} not found");

        var actual = XDocument.Load(fullPath)
            .Descendants("ProjectReference")
            .Select(r => Path.GetFileNameWithoutExtension(r.Attribute("Include")!.Value.Replace('\\', '/')))
            .Order()
            .ToArray();

        Assert.That(actual, Is.EqualTo(AllowedProjectReferences[csprojPath].Order().ToArray()),
            $"{csprojPath} references {string.Join(", ", actual)} - update AGENTS.md and this test if the layering should change");
    }

    private static readonly string[] ConcreteDrivers = ["NpgsqlDriver", "MySqlConnectorDriver", "SqliteDriver", "MssqlDriver"];

    // Only CodeGenerator.InstantiateDriver may know the concrete drivers; everything else goes through DbDriver.
    [Test]
    [TestCase("CodeGenerator", "CodeGenerator.cs")]
    [TestCase("Drivers/Generators", null)]
    public void TestConcreteDriversAreNotReferenced(string directory, string? allowedFile)
    {
        var offenders = Directory
            .EnumerateFiles(Path.Combine(FindRepositoryRoot(), directory), "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}") &&
                        !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
            .Where(f => Path.GetFileName(f) != allowedFile)
            .Where(f => ConcreteDrivers.Any(File.ReadAllText(f).Contains))
            .Select(Path.GetFileName)
            .ToArray();

        Assert.That(offenders, Is.Empty, $"{directory} must use DbDriver hooks, not concrete drivers");
    }

    private static IEnumerable<string> PluginProjects() => AllowedProjectReferences.Keys;

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "sqlc-gen-csharp.sln")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("repository root (sqlc-gen-csharp.sln) not found");
    }
}