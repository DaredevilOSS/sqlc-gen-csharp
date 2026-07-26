using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlcGenCsharp;

public static partial class StringExtensions
{
    [GeneratedRegex(@"[A-Za-z0-9]+")]
    private static partial Regex WordRegex();

    [GeneratedRegex(@"[^A-Za-z0-9]+")]
    private static partial Regex NoWordRegex();

    public static string ToPascalCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;
        string cleaned = NoWordRegex().Replace(value, " ");

        var sb = new StringBuilder();
        foreach (Match match in WordRegex().Matches(cleaned))
        {
            var word = match.Value;
            if (word.Length == 0)
                continue;

            if (word.Length == 1)
            {
                sb.Append(CultureInfo.InvariantCulture.TextInfo.ToUpper(word));
                continue;
            }

            sb.Append(char.ToUpperInvariant(word[0]));
            if (word.Length > 1)
                sb.Append(word[1..]);
        }

        return sb.ToString();
    }

    public static string ToCamelCase(this string value)
    {
        var newValue = value.ToPascalCase();
        return string.Concat(newValue[0].ToString().ToLower(), newValue.AsSpan(1));
    }

    public static string ToModelName(this string value, string schema, string defaultSchema,
        bool properSingularization = false)
    {
        var schemaName = schema == defaultSchema ? string.Empty : schema;
        var singular = properSingularization ? value.Singularize() : value.TrimEnd('s');
        return $"{schemaName}_{singular}".ToPascalCase();
    }

    private static readonly string[] DropEsSuffixes = ["sses", "uses", "xes", "ches", "shes"];

    // Rule-based English singularization for identifiers, e.g. policies -> policy,
    // statuses -> status, devices -> device, responses -> response. Words not ending
    // in a plural-looking "s" (address, people) pass through unchanged.
    private static string Singularize(this string value)
    {
        if (!value.EndsWith('s') || value.EndsWith("ss", StringComparison.OrdinalIgnoreCase))
            return value;
        if (value.EndsWith("ies", StringComparison.OrdinalIgnoreCase) && value.Length > 3)
            return $"{value[..^3]}y";
        if (DropEsSuffixes.Any(suffix => value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
            return value[..^2];
        return value[..^1];
    }

    public static string ToMethodName(this string value, bool withAsyncSuffix)
    {
        var methodName = value.ToPascalCase();
        return withAsyncSuffix ? $"{methodName}Async" : methodName;
    }

    public static string AppendSemicolonUnlessEmpty(this string input)
    {
        return input == string.Empty ? "" : $"{input};";
    }
}