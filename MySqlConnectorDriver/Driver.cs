using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using SqlcGenCsharp.Drivers.Generators;
using static System.String;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using OneDeclareGen = SqlcGenCsharp.Drivers.Generators.OneDeclareGen;

namespace SqlcGenCsharp.MySqlConnectorDriver;

public partial class Driver(DotnetFramework dotnetFramework) : DbDriver(dotnetFramework)
{
    public override string[] EstablishConnection()
    {
        return
        [
            $"var {Variable.Connection.Name()} = new MySqlConnection({Variable.ConnectionString.Name()})",
            $"{Variable.Connection.Name()}.Open()"
        ];
    }

    public override UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return
        [
            UsingDirective(ParseName("System.Collections.Generic")),
            UsingDirective(ParseName("System.Threading.Tasks")),
            UsingDirective(ParseName("MySqlConnector"))
        ];
    }

    public override string CreateSqlCommand(string sqlTextConstant)
    {
        return $"var {Variable.Command.Name()} = new MySqlCommand({sqlTextConstant}, {Variable.Connection.Name()})";
    }

    public override string GetColumnType(Column column)
    {
        var csharpType = IsNullOrEmpty(column.Type.Name) ? "object" : GetTypeWithoutNullableSuffix();
        return AddNullableSuffix(csharpType, column.NotNull);

        string GetTypeWithoutNullableSuffix()
        {
            switch (column.Type.Name.ToLower())
            {
                case "bigint":
                    return "long";
                case "binary":
                case "bit":
                case "blob":
                case "longblob":
                case "mediumblob":
                case "tinyblob":
                case "varbinary":
                    return "byte[]";
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
                    return "double";
                case "int":
                case "mediumint":
                case "smallint":
                case "tinyint":
                case "year":
                    return "int";
                case "json":
                    // Assuming JSON is represented as a string or a specific class
                    return "object";
                default:
                    throw new NotSupportedException($"Unsupported column type: {column.Type.Name}");
            }
        }
    }

    public override string GetColumnReader(Column column, int ordinal)
    {
        switch (column.Type.Name.ToLower())
        {
            case "bigint":
                return $"reader.GetInt64({ordinal})";
            case "binary":
            case "bit":
            case "blob":
            case "longblob":
            case "mediumblob":
            case "tinyblob":
            case "varbinary":
                return $"Utils.GetBytes(reader, {ordinal})";
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
                return $"reader.GetString({ordinal})";
            case "double":
            case "float":
                return $"reader.GetDouble({ordinal})";
            case "int":
            case "mediumint":
            case "smallint":
            case "tinyint":
            case "year":
                return $"reader.GetInt32({ordinal})";
            default:
                throw new NotSupportedException($"Unsupported column type: {column.Type.Name}");
        }
    }

    public override string TransformQueryText(Query query)
    {
        var counter = 0;
        return MyRegex().Replace(query.Text, _ => "@" + query.Params[counter++].Column.Name);
    }

    public override MemberDeclarationSyntax OneDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        return new OneDeclareGen(this).Generate(funcName, queryTextConstant, argInterface, returnInterface,
            parameters, columns);
    }

    public override MemberDeclarationSyntax ExecDeclare(string funcName, string queryTextConstant, string argInterface,
        IList<Parameter> parameters)
    {
        return new ExecDeclareGen(this).Generate(funcName, queryTextConstant, argInterface, parameters);
    }

    public override MemberDeclarationSyntax ExecLastIdDeclare(string funcName, string queryTextConstant,
        string argInterface, IList<Parameter> parameters)
    {
        return new ExecLastIdDeclareGen(this).Generate(funcName, queryTextConstant, argInterface, parameters);
    }

    public override MemberDeclarationSyntax ManyDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IEnumerable<Column> columns)
    {
        return new ManyDeclareGen(this).Generate(funcName, queryTextConstant, argInterface, returnInterface, parameters,
            columns);
    }

    [GeneratedRegex(@"\?")]
    private static partial Regex MyRegex();
}