using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SqlcGenCsharp;

public record RawOptions
{
    [JsonPropertyName("driverVersion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DriverVersion { get; init; }

    /// <summary>Deprecated alias of <see cref="DriverVersion"/>.</summary>
    [JsonPropertyName("overrideDriverVersion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OverrideDriverVersion { get; init; }

    [JsonPropertyName("generateCsproj")]
    public bool GenerateCsproj { get; init; } = true;

    [JsonPropertyName("targetFramework")]
    public string TargetFramework { get; init; } = DotnetFramework.Dotnet80.ToName();

    [JsonPropertyName("namespaceName")]
    public string NamespaceName { get; init; } = string.Empty;

    [JsonPropertyName("useDapper")]
    public bool UseDapper { get; init; }

    [JsonPropertyName("dapperVersion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DapperVersion { get; init; }

    /// <summary>Deprecated alias of <see cref="DapperVersion"/>.</summary>
    [JsonPropertyName("overrideDapperVersion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OverrideDapperVersion { get; init; }

    [JsonPropertyName("overrides")]
    public List<OverrideOption>? Overrides { get; init; }

    [JsonPropertyName("debugRequest")]
    public bool DebugRequest { get; init; }

    [JsonPropertyName("useCentralPackageManagement")]
    public bool UseCentralPackageManagement { get; init; }

    [JsonPropertyName("useAsyncSuffix")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? UseAsyncSuffix { get; init; }

    /// <summary>Deprecated alias of <see cref="UseAsyncSuffix"/>.</summary>
    [JsonPropertyName("withAsyncSuffix")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? WithAsyncSuffix { get; init; }

    [JsonPropertyName("useCancellationToken")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? UseCancellationToken { get; init; }

    /// <summary>Deprecated alias of <see cref="UseCancellationToken"/>.</summary>
    [JsonPropertyName("withCancellationToken")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? WithCancellationToken { get; init; }

    [JsonPropertyName("useProperSingularization")]
    public bool UseProperSingularization { get; init; }
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