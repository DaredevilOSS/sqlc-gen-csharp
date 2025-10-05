namespace SqlcGenCsharp;

public static class DictionaryExtensions
{
    public static IDictionary<K, V> Merge<K, V>(this IDictionary<K, V> me, IDictionary<K, V>? items)
    {
        if (items is null)
            return me;
        foreach (var item in items)
            me[item.Key] = item.Value;
        return me;
    }

    public static IDictionary<K, V> MergeIf<K, V>(this IDictionary<K, V> me, IDictionary<K, V>? items, bool condition)
    {
        if (!condition)
            return me;
        return me.Merge(items);
    }
}