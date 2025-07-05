namespace SqlcGenCsharp;

public static class ListExtensions
{
    public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> me, T item, bool condition)
    {
        return condition ? me.Append(item) : me;
    }

    public static ISet<T> AddIf<T>(this ISet<T> me, T item, bool condition)
    {
        if (condition)
            me.Add(item);
        return me;
    }

    public static IEnumerable<T> AppendIfNotNull<T>(this IEnumerable<T> me, T? item)
    {
        return item is not null ? me.Append(item) : me;
    }

    public static ISet<T> AddIfNotNull<T>(this ISet<T> me, T? item)
    {
        if (item is not null)
            me.Add(item);
        return me;
    }

    public static ISet<T> AddRange<T>(this ISet<T> me, IEnumerable<T?> items)
    {
        foreach (var item in items)
            me.AddIfNotNull(item);
        return me;
    }

    public static ISet<T> AddRangeIf<T>(this ISet<T> me, IEnumerable<T?> items, bool condition)
    {
        if (condition)
            return me.AddRange(items);
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