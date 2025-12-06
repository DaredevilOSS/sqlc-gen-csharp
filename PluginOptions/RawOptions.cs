using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SqlcGenCsharp;

public record RawOptions
{
    [JsonPropertyName("overrideDriverVersion")]
    public string OverrideDriverVersion { get; init; } = string.Empty;

    [JsonPropertyName("generateCsproj")]
    public bool GenerateCsproj { get; init; } = true;

    [JsonPropertyName("targetFramework")]
    public string TargetFramework { get; init; } = DotnetFramework.Dotnet80.ToName();

    [JsonPropertyName("namespaceName")]
    public string NamespaceName { get; init; } = string.Empty;

    [JsonPropertyName("useDapper")]
    public bool UseDapper { get; init; }

    [JsonPropertyName("overrideDapperVersion")]
    public string OverrideDapperVersion { get; init; } = string.Empty;

    [JsonPropertyName("overrides")]
    public List<OverrideOption>? Overrides { get; init; }

    [JsonPropertyName("debugRequest")]
    public bool DebugRequest { get; init; }

    [JsonPropertyName("useCentralPackageManagement")]
    public bool UseCentralPackageManagement { get; init; }

    [JsonPropertyName("withAsyncSuffix")]
    public bool WithAsyncSuffix { get; init; } = true;

    [JsonPropertyName("mySqlConnectionReset")]
    public bool? MySqlConnectionReset { get; init; }
}

public class OverrideOption
{
    [JsonPropertyName("column")]
    public string Column { get; init; } = string.Empty;

    [JsonPropertyName("csharp_type")]
    public CsharpTypeOption CsharpType { get; init; } = new();
}

public class CsharpTypeOption
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("notNull")]
    public bool NotNull { get; init; } = false;
}