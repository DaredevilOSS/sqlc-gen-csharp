namespace SqlcGenCsharp;

public static class NumberExtensions
{
    public static string StringifyLargeNumbers(this int value)
    {
        if (value < 1000) return value.ToString();

        var valueStr = value.ToString();
        if (valueStr.Length is >= 4 and <= 6) return $"{valueStr[0]}.{valueStr[1]}K";
        if (valueStr.Length is >= 7 and <= 9) return $"{valueStr[0]}.{valueStr[1]}M";
        if (valueStr.Length is >= 10 and <= 12) return $"{valueStr[0]}.{valueStr[1]}B";

        throw new ArgumentException($"Number {value} is too large to stringify");
    }
}