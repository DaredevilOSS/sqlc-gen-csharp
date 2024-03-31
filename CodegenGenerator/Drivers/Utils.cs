using System.Data.Common;
using System.Text.RegularExpressions;
using Plugin;

namespace SqlcGenCsharp.Drivers;

public static partial class Utils
{
    private static string FieldName(string prefix, int index, Column? column = null)
    {
        var name = $"{prefix}_{index}";
        if (column != null) name = column.Name;

        return MyRegex().Replace(
            name.ToLower(), m => m.Value.ToUpper().Replace("_", ""));
    }

    public static string ArgName(int index, Column? column = null)
    {
        return FieldName("arg", index, column);
    }

    public static string ColName(int index, Column? column = null)
    {
        return FieldName("col", index, column);
    }

    [GeneratedRegex("(_[a-z])")]
    private static partial Regex MyRegex();
}