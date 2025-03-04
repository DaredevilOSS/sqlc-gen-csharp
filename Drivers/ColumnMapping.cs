using System;
using System.Collections.Generic;

namespace SqlcGenCsharp.Drivers;

public record DbTypeInfo(int? Length = null, string? NpgsqlTypeOverride = null);

public class ColumnMapping(
    string csharpType,
    Dictionary<string, DbTypeInfo> dbTypes, Func<int, string> readerFn, Func<int, string>? readerArrayFn = null)
{
    public string CsharpType { get; } = csharpType;
    public Func<int, string> ReaderFn { get; } = readerFn;
    public Func<int, string>? ReaderArrayFn { get; } = readerArrayFn;
    public Dictionary<string, DbTypeInfo> DbTypes { get; } = dbTypes;
}