using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static System.String;

namespace SqlcGenCsharp.Drivers;

public record ConnectionGenCommands(string EstablishConnection, string ConnectionOpen);

public abstract class DbDriver(Options options)
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

    public string GetColumnType(Column column)
    {
        var columnCsharpType = IsNullOrEmpty(column.Type.Name) ? "object" : GetTypeWithoutNullableSuffix();
        return AddNullableSuffix(columnCsharpType, column.NotNull);

        string GetTypeWithoutNullableSuffix()
        {
            var columnType = column.Type.Name.ToLower();
            foreach (var columnMapping in ColumnMappings
                         .Where(columnMapping => columnMapping.DbTypes.ContainsKey(columnType)))
            {
                if (column.IsArray)
                {
                    return $"{columnMapping.CsharpType}[]";
                }
                return columnMapping.CsharpType;
            }
            throw new NotSupportedException($"Unsupported column type: {column.Type.Name}");
        }
    }

    public string GetColumnReader(Column column, int ordinal)
    {
        var columnType = column.Type.Name.ToLower();
        foreach (var columnMapping in ColumnMappings
                     .Where(columnMapping => columnMapping.DbTypes.ContainsKey(columnType)))
        {
            if (column.IsArray)
            {
                return columnMapping.ReaderArrayFn?.Invoke(ordinal) ?? throw new InvalidOperationException("ReaderArrayFn is null");
            }

            return columnMapping.ReaderFn(ordinal);
        }
        throw new NotSupportedException($"Unsupported column type: {column.Type.Name}");
    }

    public string? GetColumnDbTypeOverride(Column column)
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

    public abstract MemberDeclarationSyntax OneDeclare(string sqlTextConstant, string argInterface,
        string returnInterface, Query query);

    public abstract MemberDeclarationSyntax ManyDeclare(string sqlTextConstant, string argInterface,
        string returnInterface, Query query);

    public abstract MemberDeclarationSyntax ExecDeclare(string text, string argInterface, Query query);

    public bool IsTypeNullableForAllRuntimes(string csharpType)
    {
        return NullableTypesInAllRuntimes.Contains(csharpType.Replace("?", ""));
    }

    protected string GetConnectionStringField()
    {
        return Variable.ConnectionString.AsPropertyName();
    }

    public string GetIdColumnType()
    {
        return options.DriverName switch
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

    public Column GetColumnFromParam(Plugin.Parameter queryParam)
    {
        if (IsNullOrEmpty(queryParam.Column.Name))
        {
            queryParam.Column.Name = $"{GetColumnType(queryParam.Column).Replace("[]", "Arr")}_{queryParam.Number}";
        }
        return queryParam.Column;
    }
}