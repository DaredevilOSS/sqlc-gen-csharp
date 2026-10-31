using System.Collections.Generic;

namespace SqlcGenCsharp;

public enum DriverName
{
    MySqlConnector,
    Npgsql,
    Sqlite,
    Mssql
}


public static class DriverNameExtensions
{
    private static readonly Dictionary<DriverName, string> EnumToString = new()
    {
        { DriverName.MySqlConnector, "MySqlConnector" },
        { DriverName.Npgsql, "Npgsql" },
        { DriverName.Sqlite, "Microsoft.Data.Sqlite" },
        { DriverName.Mssql, "Microsoft.Data.SqlClient" }
    };

    public static string ToName(this DriverName me)
    {
        return EnumToString[me];
    }
}