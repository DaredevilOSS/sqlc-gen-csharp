using System.Collections.Generic;

namespace SqlcGenCsharp.Drivers;

public record DbTypeInfo(int? Length = null, string? NpgsqlTypeOverride = null);

public delegate string ReaderFn(int ordinal, string dbType);

public delegate string WriterFn(string el, string dbType, bool notNull, bool isDapper, bool isLegacy);

public delegate string ConvertFunc(string el);

public delegate string SqlMapperImplFunc(bool isDotnetCore, string? dbType);

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