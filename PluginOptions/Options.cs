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
        var rawOptions = JsonSerializer.Deserialize<RawOptions>(text) ?? throw new InvalidOperationException();

        DriverName = EngineToDriverMapping[generateRequest.Settings.Engine];
        OverrideDriverVersion = rawOptions.OverrideDriverVersion;
        GenerateCsproj = rawOptions.GenerateCsproj;
        UseDapper = rawOptions.UseDapper;
        OverrideDapperVersion = rawOptions.OverrideDapperVersion;
        NamespaceName = rawOptions.NamespaceName;
        DotnetFramework = DotnetFrameworkExtensions.ParseName(rawOptions.TargetFramework);
        Overrides = rawOptions.Overrides ?? [];

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

    public bool NotNull { get; }

    public List<OverrideOption> Overrides { get; }

    public bool DebugRequest { get; }

    private static readonly Dictionary<string, DriverName> EngineToDriverMapping = new()
    {
        { "mysql", DriverName.MySqlConnector },
        { "postgresql", DriverName.Npgsql },
        { "sqlite", DriverName.Sqlite }
    };
}