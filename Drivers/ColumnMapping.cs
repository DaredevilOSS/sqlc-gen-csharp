using System;
using System.Collections.Generic;

namespace SqlcGenCsharp.Drivers;

public record DbTypeInfo(int? Length = null, string? NpgsqlTypeOverride = null);

public delegate string ReaderFn(int ordinal);

public delegate string WriterFn(string el, bool notNull, bool isDapper);

public delegate string ConvertFunc(string el);

public delegate string SqlMapperImplFunc(bool isDotnetCore);

public class ColumnMapping(
    Dictionary<string, DbTypeInfo> dbTypes,
    ReaderFn readerFn,
    ReaderFn? readerArrayFn = null,
    string? usingDirective = null,
    WriterFn? writerFn = null,
    ConvertFunc? convertFunc = null,
    string? sqlMapper = null,
    SqlMapperImplFunc? sqlMapperImpl = null)
{
    public Dictionary<string, DbTypeInfo> DbTypes { get; } = dbTypes;
    public ReaderFn ReaderFn { get; } = readerFn;
    public ReaderFn? ReaderArrayFn { get; } = readerArrayFn;
    public string? UsingDirective { get; } = usingDirective;
    public WriterFn? WriterFn { get; } = writerFn;
    public ConvertFunc? ConvertFunc { get; } = convertFunc;
    public string? SqlMapper { get; } = sqlMapper;
    public SqlMapperImplFunc? SqlMapperImpl { get; } = sqlMapperImpl;
}