using Plugin;
using SqlcGenCsharp;
using SqlcGenCsharp.Drivers;
using System.Collections.Generic;

public abstract class EnumDbDriver(Options options, Catalog catalog, IList<Query> queries) : DbDriver(options, catalog, queries)
{
    protected abstract Enum? GetEnumType(Column column);

    protected abstract string EnumToCsharpDataType(Column column);

    public abstract string EnumToModelName(string schemaName, Enum enumType);

    protected abstract string EnumToModelName(Column column);

    protected abstract string GetEnumReader(Column column, int ordinal);

    public override string GetColumnReader(Column column, int ordinal, Query? query)
    {
        if (GetEnumType(column) is not null)
            return GetEnumReader(column, ordinal);
        return base.GetColumnReader(column, ordinal, query);
    }

    protected override string GetCsharpTypeWithoutNullableSuffix(Column column, Query? query)
    {
        if (GetEnumType(column) is not null)
            return EnumToCsharpDataType(column);
        return base.GetCsharpTypeWithoutNullableSuffix(column, query);
    }
}