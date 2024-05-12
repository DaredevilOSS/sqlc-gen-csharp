using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using SqlcGenCsharp.Drivers.Generators;
using static System.String;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.NpgsqlDriver;

public class Driver(DotnetFramework dotnetFramework) : DbDriver(dotnetFramework)
{
    public override UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return
        [
            UsingDirective(ParseName("System.Collections.Generic")),
            UsingDirective(ParseName("System.Threading.Tasks")),
            UsingDirective(ParseName("Npgsql"))
        ];
    }

    public override string GetColumnType(Column column)
    {
        var csharpType = IsNullOrEmpty(column.Type.Name) ? "object" : GetTypeWithoutNullableSuffix();
        return AddNullableSuffix(csharpType, column.NotNull);

        string GetTypeWithoutNullableSuffix()
        {
            switch (column.Type.Name.ToLower())
            {
                case "serial":
                case "bigserial":
                    return "long";
                case "bit":
                case "bytea":
                    return "byte[]";
                case "char":
                case "bpchar":
                case "varchar":
                case "text":
                case "date":
                case "time":
                case "timestamp":
                    return "string";
                case "decimal":
                    return "decimal";
                case "numeric":
                case "float4":
                case "float8":
                    return "float";
                case "int2":
                case "int4":
                case "int8":
                    return "int";
                case "json":
                    return "object";
                case "bool":
                case "boolean":
                    return "bool";
                default:
                    throw new NotSupportedException($"Unsupported column type: {column.Type.Name}");
            }
        }
    }

    public override string[] EstablishConnection()
    {
        return
        [
            $"var {Variable.Connection.Name()} = NpgsqlDataSource.Create({Variable.ConnectionString.Name()})"
        ];
    }

    public override string CreateSqlCommand(string sqlTextConstant)
    {
        return $"var {Variable.Command.Name()} = {Variable.Connection.Name()}.CreateCommand({sqlTextConstant})";
    }

    public override string GetColumnReader(Column column, int ordinal)
    {
        switch (column.Type.Name.ToLower())
        {
            case "serial":
            case "bigserial":
                return $"reader.GetInt64({ordinal})";
            case "binary":
            case "bit":
            case "bytea":
            case "blob":
            case "longblob":
            case "mediumblob":
            case "tinyblob":
            case "varbinary":
                return $"Utils.GetBytes({Variable.Reader.Name()}, {ordinal})";
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
                return $"reader.GetString({ordinal})";
            case "double":
                return $"reader.GetDouble({ordinal})";
            case "numeric":
            case "float4":
            case "float8":
                return $"reader.GetFloat({ordinal})";
            case "decimal":
                return $"reader.GetDecimal({ordinal})";
            case "bool":
            case "boolean":
                return $"reader.GetBoolean({ordinal})";
            case "int":
            case "int2":
            case "int4":
            case "int8":
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
        var queryText = query.Text;
        for (var i = 0; i < query.Params.Count; i++)
        {
            var currentParameter = query.Params[i];
            queryText = Regex.Replace(queryText, $@"\$\s*{i + 1}",
                $"@{currentParameter.Column.Name.FirstCharToLower()}");
        }

        return queryText;
    }

    public override MemberDeclarationSyntax OneDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        return new OneDeclareGen(this).Generate(funcName, queryTextConstant, argInterface, returnInterface, parameters,
            columns);
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
}