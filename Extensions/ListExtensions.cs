namespace SqlcGenCsharp;

public static class ListExtensions
{
    private static IEnumerable<T> AddIfNotNull<T>(this IEnumerable<T> me, T? item)
    {
        return item is not null ? me.Append(item) : me;
    }
    private static ISet<T> AddIfNotNull<T>(this ISet<T> me, T? item)
    {
        if (item is not null)
            me.Add(item);
        return me;
    }

    public static IEnumerable<T> AddRangeExcludeNulls<T>(this IEnumerable<T> me, IEnumerable<T?> items)
    {
        foreach (var item in items)
            me = me.AddIfNotNull(item);
        return me;
    }

    public static ISet<T> AddRangeExcludeNulls<T>(this ISet<T> me, IEnumerable<T?> items)
    {
        foreach (var item in items)
            me.AddIfNotNull(item);
        return me;
    }

    public static IEnumerable<T> AddRangeIf<T>(this IEnumerable<T> me, IEnumerable<T?> items, bool condition)
    {
        if (condition)
            return me.AddRangeExcludeNulls(items);
        return me;
    }

    public static ISet<T> AddRangeIf<T>(this ISet<T> me, IEnumerable<T?> items, bool condition)
    {
        if (condition)
            return me.AddRangeExcludeNulls(items);
        return me;
    }

    public static string JoinByNewLine(this IEnumerable<string> me)
    {
        return string.Join("\n", me);
    }

    public static string JoinByComma(this IEnumerable<string> me)
    {
        return string.Join(", ", me);
    }
}