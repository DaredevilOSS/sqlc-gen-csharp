using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public record ConnectionGenCommands(string EstablishConnection, string ConnectionOpen);

public abstract class DbDriver(Options options, Dictionary<string, Table> tables)
{
    public Options Options { get; } = options;

    private HashSet<string> NullableTypesInAllRuntimes { get; } = ["long", "double", "int", "float", "bool", "DateTime"];

    protected abstract List<ColumnMapping> ColumnMappings { get; }

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

    public string AddNullableSuffix(string csharpType, bool notNull)
    {
        if (notNull) return csharpType;
        if (IsTypeNullableForAllRuntimes(csharpType)) return $"{csharpType}?";
        return Options.DotnetFramework.LatestDotnetSupported() ? $"{csharpType}?" : csharpType;
    }

    // TODO rename to GetCsharpFieldType
    public string GetColumnType(Column column)
    {
        if (column.EmbedTable != null)
            return column.EmbedTable.Name.ToModelName();

        var columnCsharpType = string.IsNullOrEmpty(column.Type.Name) ? "object" : GetTypeWithoutNullableSuffix();
        return AddNullableSuffix(columnCsharpType, column.NotNull);

        string GetTypeWithoutNullableSuffix()
        {
            var columnType = column.Type.Name.ToLower();
            foreach (var columnMapping in ColumnMappings
                         .Where(columnMapping => columnMapping.DbTypes.ContainsKey(columnType)))
            {
                if (column.IsArray || column.IsSqlcSlice) return $"{columnMapping.CsharpType}[]";
                return columnMapping.CsharpType;
            }
            throw new NotSupportedException($"Column {column.Name} has unsupported column type: {column.Type.Name}");
        }
    }

    private string GetColumnReader(Column column, int ordinal)
    {
        var columnType = column.Type.Name.ToLower();
        foreach (var columnMapping in ColumnMappings
                     .Where(columnMapping => columnMapping.DbTypes.ContainsKey(columnType)))
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
                return dbTypeOverride;
        }

        throw new NotSupportedException($"Unsupported column type: {column.Type.Name}");
    }

    public abstract string TransformQueryText(Query query);

    public abstract ConnectionGenCommands EstablishConnection(Query query);

    public abstract string CreateSqlCommand(string sqlTextConstant);

    public string InstantiateDataclass(Column[] columns, string returnInterface)
    {
        var columnsInit = new List<string>();
        var actualOrdinal = 0;

        foreach (var column in columns)
        {
            if (column.EmbedTable == null)
            {
                columnsInit.Add(GetAsSimpleAssignment(column, actualOrdinal));
                actualOrdinal++;
                continue;
            }

            (actualOrdinal, var embeddedColumnInit) = GetAsEmbeddedTableAssignment(column, actualOrdinal);
            columnsInit.Add(embeddedColumnInit);
        }

        return $$"""
                 new {{returnInterface}}
                 {
                     {{string.Join(",\n", columnsInit)}}
                 }
                 """;

        (int, string) GetAsEmbeddedTableAssignment(Column tableColumn, int ordinal)
        {
            var tableName = tableColumn.EmbedTable.Name.ToModelName();
            var tableColumns = tables[tableColumn.EmbedTable.Name].Columns;
            var tableColumnsInit = tableColumns
                .Select((c, o) => GetAsSimpleAssignment(c, o + ordinal));

            return (
                ordinal + tableColumns.Count,
                $$"""
                  {{tableName}} = new {{tableName}}
                  {
                      {{string.Join(",\n", tableColumnsInit)}}
                  }
                  """
                );
        }

        string GetAsSimpleAssignment(Column column, int ordinal)
        {
            var readExpression = column.NotNull
                ? GetColumnReader(column, ordinal)
                : $"{CheckNullExpression(ordinal)} ? {GetNullExpression(column)} : {GetColumnReader(column, ordinal)}";
            return $"{column.Name.ToPascalCase()} = {readExpression}";
        }

        string GetNullExpression(Column column)
        {
            var csharpType = GetColumnType(column);
            if (csharpType == "string")
                return "string.Empty";
            return !Options.DotnetFramework.LatestDotnetSupported() && IsTypeNullableForAllRuntimes(csharpType)
                ? $"({csharpType}) null"
                : "null";
        }

        string CheckNullExpression(int ordinal)
        {
            return $"{Variable.Reader.AsVarName()}.IsDBNull({ordinal})";
        }
    }

    private bool IsTypeNullableForAllRuntimes(string csharpType)
    {
        return NullableTypesInAllRuntimes.Contains(csharpType.Replace("?", ""));
    }

    protected static string GetConnectionStringField()
    {
        return Variable.ConnectionString.AsPropertyName();
    }

    public string GetIdColumnType()
    {
        return Options.DriverName switch
        {
            DriverName.Sqlite => "int",
            _ => "long"
        };
    }

    public virtual string[] GetLastIdStatement()
    {
        var convertFunc = GetIdColumnType() == "int" ? "ToInt32" : "ToInt64";
        return
        [
            $"var {Variable.Result.AsVarName()} = await {Variable.Command.AsVarName()}.ExecuteScalarAsync();",
            $"return Convert.{convertFunc}({Variable.Result.AsVarName()});"
        ];
    }

    public Column GetColumnFromParam(Parameter queryParam)
    {
        if (string.IsNullOrEmpty(queryParam.Column.Name))
            queryParam.Column.Name = $"{GetColumnType(queryParam.Column).Replace("[]", "Arr")}_{queryParam.Number}";
        return queryParam.Column;
    }
}