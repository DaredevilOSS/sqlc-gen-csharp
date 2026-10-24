using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlcGenCsharp;

public enum DotnetFramework
{
    Dotnet80,
    Dotnet90,
    Dotnet100,
    DotnetStandard20,
    DotnetStandard21
}

public static class DotnetFrameworkExtensions
{
    private static readonly Dictionary<DotnetFramework, string> EnumToString = new()
    {
        { DotnetFramework.Dotnet80, "net8.0" },
        { DotnetFramework.Dotnet90, "net9.0" },
        { DotnetFramework.Dotnet100, "net10.0" },
        { DotnetFramework.DotnetStandard21, "netstandard2.1" },
        { DotnetFramework.DotnetStandard20, "netstandard2.0" }
    };

    public static string ToName(this DotnetFramework me)
    {
        return EnumToString[me];
    }

    public static DotnetFramework ParseName(string dotnetFramework)
    {
        var nameToEnum = EnumToString.ToDictionary(x => x.Value, x => x.Key);
        if (nameToEnum.TryGetValue(dotnetFramework, out var parsed))
            return parsed;
        throw new ArgumentException(
            $"Unsupported targetFramework '{dotnetFramework}', supported values: {string.Join(", ", nameToEnum.Keys)}");
    }

    public static bool IsDotnetCore(this DotnetFramework me)
    {
        return me is DotnetFramework.Dotnet80 or DotnetFramework.Dotnet90 or DotnetFramework.Dotnet100;
    }

    public static bool IsDotnetLegacy(this DotnetFramework me)
    {
        return !IsDotnetCore(me);
    }
}