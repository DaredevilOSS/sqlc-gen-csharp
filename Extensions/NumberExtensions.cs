namespace SqlcGenCsharp;

public static class NumberExtensions
{
    public static string StringifyLargeNumbers(this int value)
    {
        if (value < 1_000) return value.ToString();

        if (value < 1_000_000)
            return FormatWithSuffix(value / 1_000, "K"); // Thousands: 1000-999999

        if (value < 1_000_000_000)
            return FormatWithSuffix(value / 1_000_000, "M"); // Millions: 1000000-999999999

        return FormatWithSuffix(value / 1_000_000_000, "B"); // Billions: 1000000000+
    }

    private static string FormatWithSuffix(double value, string suffix)
    {
        if (value % 1 == 0)
            return $"{(int)value}{suffix}";

        var formatted = $"{value:F1}{suffix}";
        return formatted.EndsWith(".0" + suffix) ? $"{(int)value}{suffix}" : formatted;
    }
}