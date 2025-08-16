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

    public static string ToModelName(this string value, string schema, string defaultSchema)
    {
        var schemaName = schema == defaultSchema ? string.Empty : schema;
        return $"{schemaName}_{value.TrimEnd('s')}".ToPascalCase(); // TODO implement better way to turn words to singular
    }

    public static string AppendSemicolonUnlessEmpty(this string input)
    {
        return input == string.Empty ? "" : $"{input};";
    }
}