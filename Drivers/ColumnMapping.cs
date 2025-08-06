using System;
using System.Collections.Generic;

namespace SqlcGenCsharp.Drivers;

public record DbTypeInfo(int? Length = null, string? NpgsqlTypeOverride = null);

public class ColumnMapping(
    Dictionary<string, DbTypeInfo> dbTypes,
    Func<int, string> readerFn,
    Func<int, string>? readerArrayFn = null,
    string? usingDirective = null,
    Func<string, bool, bool, string>? writerFn = null,
    Func<string, string>? convertFunc = null,
    string? sqlMapper = null,
    string? sqlMapperImpl = null)
{
    public Dictionary<string, DbTypeInfo> DbTypes { get; } = dbTypes;
    public Func<int, string> ReaderFn { get; } = readerFn;
    public Func<int, string>? ReaderArrayFn { get; } = readerArrayFn;
    public string? UsingDirective { get; } = usingDirective;
    public Func<string, bool, bool, string>? WriterFn { get; } = writerFn;
    public Func<string, string>? ConvertFunc { get; } = convertFunc;
    public string? SqlMapper { get; } = sqlMapper;
    public string? SqlMapperImpl { get; } = sqlMapperImpl;
}