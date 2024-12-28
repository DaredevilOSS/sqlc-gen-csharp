using Plugin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Enum = System.Enum;

namespace SqlcGenCsharp;

public class Options
{
    public Options(GenerateRequest generateRequest)
    {
        var text = Encoding.UTF8.GetString(generateRequest.PluginOptions.ToByteArray());
        var rawOptions = JsonSerializer.Deserialize<RawOptions>(text) ?? throw new InvalidOperationException();

        var driverName = EngineMapping[generateRequest.Settings.Engine];
        Enum.TryParse(driverName, true, out DriverName outDriverName);
        DriverName = outDriverName;
        GenerateCsproj = rawOptions.GenerateCsproj;
        UseDapper = rawOptions.UseDapper;
        NamespaceName = rawOptions.NamespaceName;
        DotnetFramework = DotnetFrameworkExtensions.ParseName(rawOptions.TargetFramework);
    }

    public DriverName DriverName { get; }

    public DotnetFramework DotnetFramework { get; }

    public bool GenerateCsproj { get; }

    public bool UseDapper { get; }

    public string NamespaceName { get; }

    private static readonly Dictionary<string, string> EngineMapping = new()
    {
        { "mysql", DriverName.MySqlConnector.ToString() },
        { "postgresql", DriverName.Npgsql.ToString() },
        { "sqlite", DriverName.Sqlite.ToString() }
    };
}