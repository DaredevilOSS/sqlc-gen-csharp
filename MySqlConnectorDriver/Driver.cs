using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Plugin;
using static System.String;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using SqlcGenCsharp.Drivers;
using SqlcGenCsharp.MySqlConnectorDriver.Generators;

namespace SqlcGenCsharp.MySqlConnectorDriver;

public partial class Driver : IDbDriver
{
    private PreambleGen PreambleGen { get;  }

    private OneDeclareGen OneDeclareGen { get;  }

    private ManyDeclareGen ManyDeclareGen { get;  }
    
    private ExecDeclareGen ExecDeclareGen { get;  }
    
    private ExecLastIdDeclareGen ExecLastIdDeclareGen { get; }

    public Driver()
    {
        PreambleGen = new PreambleGen(this);
        OneDeclareGen = new OneDeclareGen(this);
        ManyDeclareGen = new ManyDeclareGen(this);
        ExecDeclareGen = new ExecDeclareGen(this);
        ExecLastIdDeclareGen = new ExecLastIdDeclareGen(this);
    }
    
    public string GetColumnType(Column column)
    {
        var nullableSuffix = column.NotNull ? Empty : "?";
        if (IsNullOrEmpty(column.Type.Name))
            return "object" + nullableSuffix;

        switch (column.Type.Name.ToLower())
        {
            case "bigint":
                return "long" + nullableSuffix;
            case "binary":
            case "bit":
            case "blob":
            case "longblob":
            case "mediumblob":
            case "tinyblob":
            case "varbinary":
                return "byte[]" + nullableSuffix;
            case "char":
            case "date":
            case "datetime":
            case "decimal":
            case "longtext":
            case "mediumtext":
            case "text":
            case "time":
            case "timestamp":
            case "tinytext":
            case "varchar":
                return "string";
            case "double":
            case "float":
                return "double" + nullableSuffix;
            case "int":
            case "mediumint":
            case "smallint":
            case "tinyint":
            case "year":
                return "int" + nullableSuffix;
            case "json":
                // Assuming JSON is represented as a string or a specific class
                return "object" + nullableSuffix;
            default:
                throw new NotSupportedException($"Unsupported column type: {column.Type.Name}");
        }
    }

    public ExpressionSyntax GetColumnReader(Column column, int ordinal)
    {
        switch (column.Type.Name.ToLower())
        {
            case "bigint":
                return ParseExpression($"reader.GetInt64({ordinal})");
            case "binary":
            case "bit":
            case "blob":
            case "longblob":
            case "mediumblob":
            case "tinyblob":
            case "varbinary":
                return ParseExpression($"Utils.GetBytes(reader, {ordinal})");
            case "char":
            case "date":
            case "datetime":
            case "decimal":
            case "longtext":
            case "mediumtext":
            case "text":
            case "time":
            case "timestamp":
            case "tinytext":
            case "varchar":
            case "json":
                return ParseExpression($"reader.GetString({ordinal})");
            case "double":
            case "float":
                return ParseExpression($"reader.GetDouble({ordinal})");
            case "int":
            case "mediumint":
            case "smallint":
            case "tinyint":
            case "year":
                return ParseExpression($"reader.GetInt32({ordinal})");
            default:
                throw new NotSupportedException($"Unsupported column type: {column.Type.Name}");
        }
    }
    
    public string TransformQueryText(Query query)
    {
        var counter = 0;
        return MyRegex().Replace(query.Text, _ => "@" + query.Params[counter++].Column.Name);
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
        return OneDeclareGen.Generate(funcName, queryTextConstant, argInterface, returnInterface, parameters, columns);
    }

    public MemberDeclarationSyntax ExecDeclare(string funcName, string queryTextConstant, string argInterface,
        IList<Parameter> parameters)
    {
        return ExecDeclareGen.Generate(funcName, queryTextConstant, argInterface, parameters);
    }

    public MemberDeclarationSyntax ExecLastIdDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        return ExecLastIdDeclareGen.Generate(funcName, queryTextConstant, argInterface, parameters);
    }

    public MemberDeclarationSyntax ManyDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IEnumerable<Column> columns)
    {
        return ManyDeclareGen.Generate(funcName, queryTextConstant, argInterface, returnInterface, parameters, columns);
    }

    [GeneratedRegex(@"\?")]
    private static partial Regex MyRegex();
}