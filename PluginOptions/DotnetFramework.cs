using System.Collections.Generic;
using System.Linq;

namespace SqlcGenCsharp;

public enum DotnetFramework
{
    Dotnet80,
    DotnetStandard20,
    DotnetStandard21,
    Dotnet90
}

public static class DotnetFrameworkExtensions
{
    private static readonly Dictionary<DotnetFramework, string> EnumToString = new()
    {
        { DotnetFramework.Dotnet90, "net9.0" },
        { DotnetFramework.Dotnet80, "net8.0" },
        { DotnetFramework.DotnetStandard21, "netstandard2.1" },
        { DotnetFramework.DotnetStandard20, "netstandard2.0" }
    };

    public static string ToName(this DotnetFramework me)
    {
        return EnumToString[me];
    }

    public static DotnetFramework ParseName(string dotnetFramework)
    {
        return EnumToString.ToDictionary(x => x.Value, x => x.Key)[dotnetFramework];
    }

    public static bool IsDotnetCore(this DotnetFramework me)
    {
        return new List<DotnetFramework>
        {
            DotnetFramework.Dotnet80,
            DotnetFramework.Dotnet90,
        }.Contains(me);
    }

    public static bool IsDotnetLegacy(this DotnetFramework me)
    {
        return !IsDotnetCore(me);
    }
}