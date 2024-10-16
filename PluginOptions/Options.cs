using Plugin;
using System;
using System.Text;
using System.Text.Json;
using Enum = System.Enum;

namespace SqlcGenCsharp;

public enum DriverName
{
    MySqlConnector,
    Npgsql,
    Sqlite
}

public static class DriverNameExtensions
{
    public static (string, string) GetNameAndVersion(this DriverName me)
    {
        return me switch
        {
            DriverName.Npgsql => ("Npgsql", "8.0.3"),
            DriverName.MySqlConnector => ("MySqlConnector", "2.3.7"),
            DriverName.Sqlite => ("Microsoft.Data.Sqlite", "8.0.8"),
            _ => throw new NotSupportedException($"unsupported driver: {me}")
        };
    }
}

public class Options
{
    public Options(GenerateRequest generateRequest)
    {
        var text = Encoding.UTF8.GetString(generateRequest.PluginOptions.ToByteArray());
        var rawOptions = JsonSerializer.Deserialize<RawOptions>(text) ?? throw new InvalidOperationException();

        Enum.TryParse(rawOptions.DriverName, true, out DriverName outDriverName);
        DriverName = outDriverName;
        GenerateCsproj = rawOptions.GenerateCsproj;
        DotnetFramework = DotnetFrameworkExtensions.ParseName(rawOptions.TargetFramework);
    }

    public DriverName DriverName { get; }

    public DotnetFramework DotnetFramework { get; }

    public bool GenerateCsproj { get; }
}