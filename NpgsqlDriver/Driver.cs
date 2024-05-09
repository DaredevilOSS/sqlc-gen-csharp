using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static System.String;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using SqlcGenCsharp.NpgsqlDriver.Generators;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.NpgsqlDriver;

public class Driver : IDbDriver
{
    public string ColumnType(Column column)
    {
        var nullableSuffix = column.NotNull ? string.Empty : "?";
        if (IsNullOrEmpty(column.Type.Name))
            return "object" + nullableSuffix;

        switch (column.Type.Name.ToLower())
        {
            case "serial":
            case "bigserial":
                return "long" + nullableSuffix;
            case "bit":
            case "bytea":
                return "byte[]" + nullableSuffix;
            case "char":
            case "bpchar":
            case "varchar":
            case "text":
            case "date":
            case "time":
            case "timestamp":
                return "string";
            case "decimal":
                return "decimal" + nullableSuffix;
            case "numeric":
            case "float4":
            case "float8":
                return "float" + nullableSuffix;
            case "int2":
            case "int4":
            case "int8":
                return "int" + nullableSuffix;
            case "json":
                return "object" + nullableSuffix;
            case "bool":
            case "boolean":
                return "bool" + nullableSuffix;
            default:
                throw new NotSupportedException($"Unsupported column type: {column.Type.Name}");
        }
    }

    public ExpressionSyntax ColumnReader(Column column, int ordinal)
    {
        switch (column.Type.Name.ToLower())
        {
            case "serial":
            case "bigserial":
                return ParseExpression($"reader.GetInt64({ordinal})");
            case "binary":
            case "bit":
            case "bytea":
            case "blob":
            case "longblob":
            case "mediumblob":
            case "tinyblob":
            case "varbinary":
                return ParseExpression($"Utils.GetBytes(reader, {ordinal})");
            case "char":
            case "date":
            case "datetime":
            case "longtext":
            case "mediumtext":
            case "text":
            case "bpchar":
            case "time":
            case "timestamp":
            case "tinytext":
            case "varchar":
            case "json":
                return ParseExpression($"reader.GetString({ordinal})");
            case "double":
                return ParseExpression($"reader.GetDouble({ordinal})");
            case "numeric":
            case "float4":
            case "float8":
                return ParseExpression($"reader.GetFloat({ordinal})");
            case "decimal":
                return ParseExpression($"reader.GetDecimal({ordinal})");
            case "bool":
            case "boolean":
                return ParseExpression($"reader.GetBoolean({ordinal})");
            case "int":
            case "int2":
            case "int4":
            case "int8":
            case "mediumint":
            case "smallint":
            case "tinyint":
            case "year":
                return ParseExpression($"reader.GetInt32({ordinal})");
            default:
                throw new NotSupportedException($"Unsupported column type: {column.Type.Name}");
        }
    }

    public string TransformQuery(Query query)
    {
        var queryText = query.Text;
        for (var i = 0; i < query.Params.Count; i++)
        {
            var currentParameter = query.Params[i];
            queryText = Regex.Replace(queryText, $@"\$\s*{i + 1}",
                $"@{currentParameter.Column.Name.FirstCharToLower()}");
        }

        return queryText;
    }

    public (UsingDirectiveSyntax[], MemberDeclarationSyntax[]) Preamble()
    {
        return (
            PreambleGen.GetUsingDirectives(),
            []
        );
    }

    public MemberDeclarationSyntax OneDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        return OneDeclareGen.Generate(funcName, queryTextConstant, argInterface, returnInterface, parameters, columns, this);
    }

    public MemberDeclarationSyntax ExecDeclare(string funcName, string queryTextConstant, string argInterface,
        IList<Parameter> parameters)
    {
        return ExecDeclareGen.Generate(funcName, queryTextConstant, argInterface, parameters);
    }

    public MemberDeclarationSyntax ExecLastIdDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        return ExecLastIdDeclareGen.Generate(funcName, queryTextConstant, argInterface, returnInterface, parameters,
            columns);
    }

    public MemberDeclarationSyntax ManyDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IEnumerable<Column> columns)
    {
        return ManyDeclareGen.Generate(funcName, queryTextConstant, argInterface, returnInterface, parameters, columns,
            this);
    }
}