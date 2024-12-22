using System.Text.RegularExpressions;

namespace SqlcGenCsharp;

public static partial class StringExtensions
{
    public static string ToPascalCase(this string value)
    {
        var invalidCharsRgx = InvalidCharsRegex();
        var whiteSpace = WhiteSpaceRegex();
        var startsWithLowerCaseChar = StartsWithLowerCaseCharRegex();
        var firstCharFollowedByUpperCasesOnly = FirstCharFollowedByUpperCasesOnlyRegex();
        var lowerCaseNextToNumber = LowerCaseNextToNumberRegex();
        var upperCaseInside = UpperCaseInsideRegex();

        // replace white spaces with undescore, then replace all invalid chars with empty string
        var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(value, "_"), string.Empty)
            // split by underscores
            .Split(["_"], StringSplitOptions.RemoveEmptyEntries)
            // set first letter to uppercase
            .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
            // replace second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
            .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
            // set upper case the first lower case following a number (Ab9cd -> Ab9Cd)
            .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
            // lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
            .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

        return string.Concat(pascalCase);
    }

    public static string ToCamelCase(this string value)
    {
        var newValue = value.ToPascalCase();
        return string.Concat(newValue[0].ToString().ToLower(), newValue.AsSpan(1));
    }

    public static string AppendSemicolonUnlessEmpty(this string input)
    {
        return input == string.Empty ? "" : $"{input};";
    }

    [GeneratedRegex("[^_a-zA-Z0-9]")]
    private static partial Regex InvalidCharsRegex();
    [GeneratedRegex(@"(?<=\s)")]
    private static partial Regex WhiteSpaceRegex();
    [GeneratedRegex("^[a-z]")]
    private static partial Regex StartsWithLowerCaseCharRegex();
    [GeneratedRegex("(?<=[A-Z])[A-Z0-9]+$")]
    private static partial Regex FirstCharFollowedByUpperCasesOnlyRegex();
    [GeneratedRegex("(?<=[0-9])[a-z]")]
    private static partial Regex LowerCaseNextToNumberRegex();
    [GeneratedRegex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))")]
    private static partial Regex UpperCaseInsideRegex();
}