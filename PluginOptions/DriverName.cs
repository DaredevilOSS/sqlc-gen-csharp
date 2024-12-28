using System.Collections.Generic;

namespace SqlcGenCsharp;

public enum DriverName
{
    MySqlConnector,
    Npgsql,
    Sqlite
}


public static class DriverNameExtensions
{
    private static readonly Dictionary<DriverName, string> EnumToString = new()
    {
        { DriverName.MySqlConnector, "MySqlConnector" },
        { DriverName.Npgsql, "Npgsql" },
        { DriverName.Sqlite, "Microsoft.Data.Sqlite" }
    };

    public static string ToName(this DriverName me)
    {
        return EnumToString[me];
    }
}