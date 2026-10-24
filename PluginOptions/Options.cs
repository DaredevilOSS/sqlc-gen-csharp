using Plugin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SqlcGenCsharp;

public class Options
{
    public Options(GenerateRequest generateRequest)
    {
        var text = Encoding.UTF8.GetString(generateRequest.PluginOptions.ToByteArray());
        if (NoOptionsProvided(text))
            text = "{}";
        var rawOptions = JsonSerializer.Deserialize<RawOptions>(text) ?? throw new InvalidOperationException();

        DriverName = EngineToDriverMapping[generateRequest.Settings.Engine];
        DriverVersion = Resolve(rawOptions.DriverVersion, rawOptions.OverrideDriverVersion, "driverVersion", "overrideDriverVersion") ?? string.Empty;
        GenerateCsproj = rawOptions.GenerateCsproj;
        UseDapper = rawOptions.UseDapper;
        DapperVersion = Resolve(rawOptions.DapperVersion, rawOptions.OverrideDapperVersion, "dapperVersion", "overrideDapperVersion") ?? string.Empty;
        NamespaceName = rawOptions.NamespaceName;
        DotnetFramework = DotnetFrameworkExtensions.ParseName(rawOptions.TargetFramework);
        Overrides = rawOptions.Overrides ?? [];
        UseAsyncSuffix = Resolve(rawOptions.UseAsyncSuffix, rawOptions.WithAsyncSuffix, "useAsyncSuffix", "withAsyncSuffix") ?? true;
        UseCancellationToken = Resolve(rawOptions.UseCancellationToken, rawOptions.WithCancellationToken, "useCancellationToken", "withCancellationToken") ?? false;
        UseCentralPackageManagement = rawOptions.UseCentralPackageManagement;
        UseProperSingularization = rawOptions.UseProperSingularization;

        if (rawOptions.DebugRequest && generateRequest.Settings.Codegen.Wasm is not null)
            throw new ArgumentException("Debug request mode cannot be used with WASM plugin");
        DebugRequest = rawOptions.DebugRequest;
    }

    public DriverName DriverName { get; }

    public string DriverVersion { get; }

    public DotnetFramework DotnetFramework { get; }

    public bool GenerateCsproj { get; }

    public bool UseDapper { get; }

    public string DapperVersion { get; }

    public string NamespaceName { get; }

    public List<OverrideOption> Overrides { get; }

    public bool DebugRequest { get; }

    /// <summary>
    /// When true generated code will opt in to central package management.
    /// https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management
    /// </summary>
    public bool UseCentralPackageManagement { get; }

    public bool UseAsyncSuffix { get; }

    /// <summary>
    /// When true, generated methods take an optional CancellationToken threaded into every async DB call.
    /// </summary>
    public bool UseCancellationToken { get; }

    /// <summary>
    /// When true, model names are singularized with English grammar rules (policies -> Policy,
    /// statuses -> Status). Defaults to the legacy behavior of trimming trailing 's' characters,
    /// which mangles some names (policies -> Policie, access -> Acces). Opt-in because enabling
    /// it renames existing generated classes.
    /// </summary>
    public bool UseProperSingularization { get; }

    private static readonly Dictionary<string, DriverName> EngineToDriverMapping = new()
    {
        { "mysql", DriverName.MySqlConnector },
        { "postgresql", DriverName.Npgsql },
        { "sqlite", DriverName.Sqlite }
    };

    /// <summary>
    /// Resolves an option that was renamed: the new name wins, the deprecated name is still honored,
    /// and setting both to different values is rejected.
    /// </summary>
    private static T? Resolve<T>(T? current, T? deprecated, string currentName, string deprecatedName)
    {
        if (current is not null && deprecated is not null && !EqualityComparer<T>.Default.Equals(current, deprecated))
            throw new ArgumentException($"Options '{currentName}' and '{deprecatedName}' (deprecated) are both set with different values");
        return current ?? deprecated;
    }

    private static bool NoOptionsProvided(string optionsText)
    {
        return optionsText.Trim() == string.Empty;
    }
}