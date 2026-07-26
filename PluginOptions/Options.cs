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
        OverrideDriverVersion = rawOptions.OverrideDriverVersion;
        GenerateCsproj = rawOptions.GenerateCsproj;
        UseDapper = rawOptions.UseDapper;
        OverrideDapperVersion = rawOptions.OverrideDapperVersion;
        NamespaceName = rawOptions.NamespaceName;
        DotnetFramework = DotnetFrameworkExtensions.ParseName(rawOptions.TargetFramework);
        Overrides = rawOptions.Overrides ?? [];
        WithAsyncSuffix = rawOptions.WithAsyncSuffix;
        WithCancellationToken = rawOptions.WithCancellationToken;
        UseCentralPackageManagement = rawOptions.UseCentralPackageManagement;
        UseProperSingularization = rawOptions.UseProperSingularization;

        if (rawOptions.DebugRequest && generateRequest.Settings.Codegen.Wasm is not null)
            throw new ArgumentException("Debug request mode cannot be used with WASM plugin");
        DebugRequest = rawOptions.DebugRequest;
    }

    public DriverName DriverName { get; }

    public string OverrideDriverVersion { get; }

    public DotnetFramework DotnetFramework { get; }

    public bool GenerateCsproj { get; }

    public bool UseDapper { get; }

    public string OverrideDapperVersion { get; }

    public string NamespaceName { get; }

    public List<OverrideOption> Overrides { get; }

    public bool DebugRequest { get; }

    /// <summary>
    /// When true generated code will opt in to central package management.
    /// https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management
    /// </summary>
    public bool UseCentralPackageManagement { get; }

    public bool WithAsyncSuffix { get; }

    /// <summary>
    /// When true, generated methods take an optional CancellationToken threaded into every async DB call.
    /// </summary>
    public bool WithCancellationToken { get; }

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

    private static bool NoOptionsProvided(string optionsText)
    {
        return optionsText.Trim() == string.Empty;
    }
}