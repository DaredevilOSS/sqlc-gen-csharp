using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public record ConnectionGenCommands(string EstablishConnection, string ConnectionOpen);

public abstract class DbDriver
{
    public Options Options { get; }

    public Dictionary<string, Table> Tables { get; }

    private HashSet<string> NullableTypesInDotnetCore { get; } = ["string", "object", "byte[]"]; // TODO add arrays in here in a non hard-coded manner

    private HashSet<string> NullableTypes { get; } = ["bool", "byte", "short", "int", "long", "float", "double", "decimal", "DateTime"];

    protected abstract List<ColumnMapping> ColumnMappings { get; }

    protected DbDriver(Options options, Dictionary<string, Table> tables)
    {
        Options = options;
        Tables = tables;

        if (!Options.DotnetFramework.IsDotnetCore()) return; // `string?` is possible only in .Net Core
        foreach (var t in NullableTypesInDotnetCore)
        {
            NullableTypes.Add(t);
        }
    }

    public virtual UsingDirectiveSyntax[] GetUsingDirectives()
    {
        var usingDirectives = new List<UsingDirectiveSyntax>
        {
            UsingDirective(ParseName("System")),
            UsingDirective(ParseName("System.Collections.Generic")),
            UsingDirective(ParseName("System.Threading.Tasks"))
        };

        if (Options.UseDapper)
            usingDirectives.Add(UsingDirective(ParseName("Dapper")));
        return usingDirectives.ToArray();
    }

    public string AddNullableSuffixIfNeeded(string csharpType, bool notNull)
    {
        if (notNull) return csharpType;
        if (IsTypeNullable(csharpType)) return $"{csharpType}?";
        return Options.DotnetFramework.IsDotnetCore() ? $"{csharpType}?" : csharpType;
    }

    public string GetCsharpType(Column column)
    {
        if (column.EmbedTable != null)
            return column.EmbedTable.Name.ToModelName();

        var columnCsharpType = string.IsNullOrEmpty(column.Type.Name) ? "object" : GetTypeWithoutNullableSuffix();
        return AddNullableSuffixIfNeeded(columnCsharpType, column.NotNull);

        string GetTypeWithoutNullableSuffix()
        {
            foreach (var columnMapping in ColumnMappings
                         .Where(columnMapping => DoesColumnMappingApply(columnMapping, column)))
            {
                if (column.IsArray || column.IsSqlcSlice) return $"{columnMapping.CsharpType}[]";
                return columnMapping.CsharpType;
            }
            throw new NotSupportedException($"Column {column.Name} has unsupported column type: {column.Type.Name}");
        }
    }

    private static bool DoesColumnMappingApply(ColumnMapping columnMapping, Column column)
    {
        var columnType = column.Type.Name.ToLower();
        if (!columnMapping.DbTypes.TryGetValue(columnType, out var typeInfo))
            return false;
        if (typeInfo.Length is null)
            return true;
        return typeInfo.Length.Value == column.Length;
    }

    public string GetColumnReader(Column column, int ordinal)
    {
        foreach (var columnMapping in ColumnMappings
                     .Where(columnMapping => DoesColumnMappingApply(columnMapping, column)))
        {
            if (column.IsArray)
                return columnMapping.ReaderArrayFn?.Invoke(ordinal) ?? throw new InvalidOperationException("ReaderArrayFn is null");
            return columnMapping.ReaderFn(ordinal);
        }
        throw new NotSupportedException($"Column {column.Name} has unsupported column type: {column.Type.Name}");
    }

    protected string? GetColumnDbTypeOverride(Column column)
    {
        var columnType = column.Type.Name.ToLower();
        foreach (var columnMapping in ColumnMappings)
        {
            if (columnMapping.DbTypes.TryGetValue(columnType, out var dbTypeOverride))
                return dbTypeOverride.NpgsqlTypeOverride;
        }
        throw new NotSupportedException($"Column {column.Name} has unsupported column type: {column.Type.Name}");
    }

    public abstract string TransformQueryText(Query query);

    public abstract ConnectionGenCommands EstablishConnection(Query query);

    public abstract string CreateSqlCommand(string sqlTextConstant);

    // TODO move out from driver + rename
    public bool IsTypeNullable(string csharpType)
    {
        return NullableTypes.Contains(csharpType.Replace("?", ""));
    }

    /*
    Since there is no indication of the primary key column in SQLC protobuf (assuming it is a single column even),
    this method uses a few heuristics to assess the type of the id column
    */
    public string GetIdColumnType(Query query)
    {
        var tableColumns = Tables[query.InsertIntoTable.Name].Columns;
        var idColumn = tableColumns.First(c => c.Name.Equals("id", StringComparison.OrdinalIgnoreCase));
        if (idColumn is not null)
            return GetCsharpType(idColumn);

        idColumn = tableColumns.First(c => c.Name.Contains("id", StringComparison.CurrentCultureIgnoreCase));
        return GetCsharpType(idColumn ?? tableColumns[0]);
    }

    public virtual string[] GetLastIdStatement(Query query)
    {
        var convertFunc = GetIdColumnType(query) == "int" ? "ToInt32" : "ToInt64"; // TODO refactor
        return
        [
            $"var {Variable.Result.AsVarName()} = await {Variable.Command.AsVarName()}.ExecuteScalarAsync();",
            $"return Convert.{convertFunc}({Variable.Result.AsVarName()});"
        ];
    }

    public Column GetColumnFromParam(Parameter queryParam)
    {
        if (string.IsNullOrEmpty(queryParam.Column.Name))
            queryParam.Column.Name = $"{GetCsharpType(queryParam.Column).Replace("[]", "Arr")}_{queryParam.Number}";
        return queryParam.Column;
    }
}