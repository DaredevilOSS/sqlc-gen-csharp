using System;
using System.Collections.Generic;

namespace SqlcGenCsharp.Drivers;

public class ColumnMapping(string csharpType, Dictionary<string, string?> dbTypes, Func<int, string> readerFn, Func<int, string>? readerArrayFn = null)
{
    public string CsharpType { get; } = csharpType;
    public Func<int, string> ReaderFn { get; } = readerFn;
    public Func<int, string>? ReaderArrayFn { get; } = readerArrayFn;
    public Dictionary<string, string?> DbTypes { get; } = dbTypes;
}