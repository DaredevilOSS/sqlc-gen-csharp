using System.Text.Json.Serialization;

namespace SqlcGenCsharp;

internal class RawOptions
{
    [JsonPropertyName("driver")]
    public required string DriverName { get; init; }

    [JsonPropertyName("generateCsproj")]
    public bool GenerateCsproj { get; init; } = true; // generating .csproj files by default

    [JsonPropertyName("targetFramework")]
    public string TargetFramework { get; init; } = DotnetFramework.Dotnet80.ToName();

    [JsonPropertyName("namespaceName")]
    public string NamespaceName { get; init; } = string.Empty;
}