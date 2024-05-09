using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Plugin;
using SqlcGenCsharp.Drivers;
using Enum = System.Enum;

namespace SqlcGenCsharp;

internal class RawOptions
{
    [JsonPropertyName("driver")] 
    public required string DriverName { get; init; }

    [JsonPropertyName("generateCsproj")] 
    public bool GenerateCsproj { get; init; } = true; // generating .csproj files by default

    [JsonPropertyName("targetFramework")] 
    public string TargetFramework { get; init; } = DotnetFramework.Dotnet80.StandardName();

    [JsonPropertyName("filePerQuery")] 
    public bool FilePerQuery { get; init; } // generating single file by default
}

public static class OptionsParser
{
    public static ValidOptions Parse(GenerateRequest generateRequest)
    {
        var text = Encoding.UTF8.GetString(generateRequest.PluginOptions.ToByteArray());
        var options = JsonSerializer.Deserialize<RawOptions>(text) ?? throw new InvalidOperationException();
        return new ValidOptions(options.DriverName, options.TargetFramework, options.FilePerQuery, options.GenerateCsproj);
    }
}

public enum DriverName
{
    MySqlConnector,
    Npgsql
}

public enum DotnetFramework
{
    DotnetStandard20,
    Dotnet80
}

internal static class DotnetFrameworkExtensions
{
    private static readonly Dictionary<DotnetFramework, string> EnumToString = new()
    {
        { DotnetFramework.Dotnet80, "net8.0" },
        { DotnetFramework.DotnetStandard20, "netstandard2.0" },
    };
    
    public static string StandardName(this DotnetFramework me)
    {
        return EnumToString[me];
    }
    
    public static DotnetFramework ParseStandardName(string dotnetFramework)
    {
        return DotnetFrameworkExtensions.EnumToString
            .ToDictionary(x => x.Value, x => x.Key)
            [dotnetFramework];
    }
}

public class ValidOptions
{
    public ValidOptions(string driverName, string dotnetFramework, bool filePerQuery, bool generateCsproj)
    {
        Enum.TryParse(driverName, true, out DriverName outDriverName);
        DriverName = outDriverName;
        FilePerQuery = filePerQuery;
        GenerateCsproj = generateCsproj;
        DotnetFramework = DotnetFrameworkExtensions.ParseStandardName(dotnetFramework);
    }

    public DriverName DriverName { get; }

    public DotnetFramework DotnetFramework { get; }
    
    public bool FilePerQuery { get; }

    public bool GenerateCsproj { get; }

    public IDbDriver InstantiateDriver()
    {
        return DriverName switch
        {
            DriverName.MySqlConnector => new MySqlConnectorDriver.Driver(),
            DriverName.Npgsql => new NpgsqlDriver.Driver(),
            _ => throw new ArgumentException($"unknown driver: {DriverName}")
        };
    }
}