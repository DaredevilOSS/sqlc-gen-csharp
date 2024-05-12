using System.Collections.Generic;

namespace SqlcGenCsharp.Drivers;

public static class Utils
{
    public static bool IsCsharpPrimitive(string csharpType)
    {
        var csharpPrimitives = new HashSet<string> { "long", "double", "int", "float", "bool" };
        return csharpPrimitives.Contains(csharpType.Replace("?", ""));
    }
}