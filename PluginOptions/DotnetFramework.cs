using System.Collections.Generic;
using System.Linq;

namespace SqlcGenCsharp;

public enum DotnetFramework
{
    Dotnet80,
    DotnetStandard20,
    DotnetStandard21
}

public static class DotnetFrameworkExtensions
{
    private static readonly Dictionary<DotnetFramework, string> EnumToString = new()
    {
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
        return EnumToString
            .ToDictionary(x => x.Value, x => x.Key)
            [dotnetFramework];
    }

    public static bool IsDotnetCore(this DotnetFramework me)
    {
        return me == DotnetFramework.Dotnet80;
    }
}