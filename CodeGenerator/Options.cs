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

    [JsonPropertyName("minimalCsharp")] public double MinimalCsharp { get; init; } = -1.0;
}

public static class OptionsParser
{
    public static ValidOptions Parse(GenerateRequest generateRequest)
    {
        var text = Encoding.UTF8.GetString(generateRequest.PluginOptions.ToByteArray());
        var rawOptions = JsonSerializer.Deserialize<Options>(text) ?? throw new InvalidOperationException();
        return new ValidOptions(rawOptions.DriverName, rawOptions.MinimalCsharp);
    }
}

public enum DriverName
{
    MySqlConnector,
    Npgsql
}

public class ValidOptions
{
    public ValidOptions(string driverName, double minimalCsharp)
    {
        Enum.TryParse(driverName, true, out DriverName outDriverName);
        DriverName = outDriverName;
        MinimalCsharp = minimalCsharp <= 0 ? 10.0 : minimalCsharp;
    }

    private DriverName DriverName { get; }

    public double MinimalCsharp { get; }

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