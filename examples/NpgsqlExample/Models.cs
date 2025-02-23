// auto-generated by sqlc - do not edit
using System;

namespace NpgsqlExampleGen;
public readonly record struct Author(long Id, string Name, string? Bio);
public readonly record struct Book(long Id, string Name, long AuthorId, string? Description);
public readonly record struct PostgresType(byte[]? CBit, int? CSmallint, bool? CBoolean, int? CInteger, long? CBigint, float? CDecimal, float? CNumeric, float? CReal, float? CDoublePrecision, DateTime? CDate, string? CTime, DateTime? CTimestamp, string? CChar, string? CVarchar, string? CCharacterVarying, byte[]? CBytea, string? CText, object? CJson, string[]? CTextArray, int[]? CIntegerArray);