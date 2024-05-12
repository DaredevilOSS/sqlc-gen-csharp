using System.Collections.Generic;
using System.Linq;

namespace SqlcGenCsharp;

public enum DotnetFramework
{
    DotnetStandard20,
    Dotnet80
}

public static class DotnetFrameworkExtensions
{
    private static readonly Dictionary<DotnetFramework, string> EnumToString = new()
    {
        { DotnetFramework.Dotnet80, "net8.0" },
        { DotnetFramework.DotnetStandard20, "netstandard2.0" }
    };

    public static string StandardName(this DotnetFramework me)
    {
        return EnumToString[me];
    }

    public static DotnetFramework ParseStandardName(string dotnetFramework)
    {
        return EnumToString
            .ToDictionary(x => x.Value, x => x.Key)
            [dotnetFramework];
    }

    public static bool NullableEnabled(this DotnetFramework me)
    {
        return me == DotnetFramework.Dotnet80;
    }
    
    public static bool UsingStatementEnabled(this DotnetFramework me)
    {
        return me == DotnetFramework.Dotnet80;
    }
}