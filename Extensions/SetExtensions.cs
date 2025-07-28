namespace SqlcGenCsharp;

public static class SetExtensions
{
    private static ISet<T> AddIfNotNull<T>(this ISet<T> me, T? item)
    {
        if (item is not null)
            me.Add(item);
        return me;
    }

    public static ISet<T> AddRangeExcludeNulls<T>(this ISet<T> me, IEnumerable<T?> items)
    {
        foreach (var item in items)
            me.AddIfNotNull(item);
        return me;
    }

    public static ISet<T> AddRangeIf<T>(this ISet<T> me, IEnumerable<T?> items, bool condition)
    {
        if (condition)
            return me.AddRangeExcludeNulls(items);
        return me;
    }
}