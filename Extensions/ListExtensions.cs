namespace SqlcGenCsharp;

public static class ListExtensions
{
    public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> me, T item, bool condition)
    {
        return condition ? me.Append(item) : me;
    }

    public static string JoinByNewLine(this IEnumerable<string> me)
    {
        return string.Join("\n", me);
    }
}