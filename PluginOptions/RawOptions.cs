using System.Text.Json.Serialization;

namespace SqlcGenCsharp;

internal class RawOptions
{
    [JsonPropertyName("overrideDriverVersion")]
    public string OverrideDriverVersion { get; init; } = string.Empty;

    [JsonPropertyName("generateCsproj")]
    public bool GenerateCsproj { get; init; } = true; // generating .csproj files by default

    [JsonPropertyName("targetFramework")]
    public string TargetFramework { get; init; } = DotnetFramework.Dotnet80.ToName();

    [JsonPropertyName("namespaceName")]
    public string NamespaceName { get; init; } = string.Empty;

    [JsonPropertyName("useDapper")]
    public bool UseDapper { get; init; } = false;

    [JsonPropertyName("overrideDapperVersion")]
    public string OverrideDapperVersion { get; init; } = string.Empty;
}