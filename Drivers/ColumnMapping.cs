using System;
using System.Collections.Generic;

namespace SqlcGenCsharp.Drivers;

public class ColumnMapping(string csharpType, Func<int, string> readerFn, Dictionary<string, string?> dbTypes)
{
    public string CsharpType { get; } = csharpType;

    public Func<int, string> ReaderFn { get; } = readerFn;

    public Dictionary<string, string?> DbTypes { get; } = dbTypes;
}