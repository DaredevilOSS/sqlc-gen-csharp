using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Plugin;
using SqlcGenCsharp.Drivers;
using SqlcGenCsharp.MySqlConnectorDriver;
using Enum = System.Enum;

namespace SqlcGenCsharp;

internal class Options
{
    [JsonPropertyName("driver")] public required string DriverName { get; init; }

    [JsonPropertyName("generateCsproj")]
    public bool GenerateCsproj { get; init; } = true; // generating .csproj files by default

    [JsonPropertyName("minimalCsharp")] public double MinimalCsharp { get; init; } = 10.0;

    [JsonPropertyName("filePerQuery")] public bool FilePerQuery { get; init; } // generating single file by default
}

public static class OptionsParser
{
    public static ValidOptions Parse(GenerateRequest generateRequest)
    {
        var text = Encoding.UTF8.GetString(generateRequest.PluginOptions.ToByteArray());
        var rawOptions = JsonSerializer.Deserialize<Options>(text) ?? throw new InvalidOperationException();
        return new ValidOptions(rawOptions.DriverName, rawOptions.MinimalCsharp, rawOptions.FilePerQuery,
            rawOptions.GenerateCsproj);
    }
}

public enum DriverName
{
    MySqlConnector,
    Npgsql
}

public class ValidOptions
{
    public ValidOptions(string driverName, double minimalCsharp, bool filePerQuery, bool generateCsproj)
    {
        Enum.TryParse(driverName, true, out DriverName outDriverName);
        DriverName = outDriverName;
        MinimalCsharp = minimalCsharp;
        FilePerQuery = filePerQuery;
        GenerateCsproj = generateCsproj;
    }

    private DriverName DriverName { get; }

    public double MinimalCsharp { get; }
    public bool FilePerQuery { get; }
    
    public bool GenerateCsproj { get; }

    public IDbDriver InstantiateDriver()
    {
        return DriverName switch
        {
            DriverName.MySqlConnector => new Driver(),
            DriverName.Npgsql => new NpgsqlDriver.Driver(),
            _ => throw new ArgumentException($"unknown driver: {DriverName}")
        };
    }
}