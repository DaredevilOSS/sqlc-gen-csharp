using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public class NpgsqlDriver(DotnetFramework dotnetFramework) : DbDriver(dotnetFramework)
{
    protected override List<(string, Func<int, string>, HashSet<string>)> GetColumnMapping()
    {
        return
        [
            ("long", ordinal => $"reader.GetInt64({ordinal})", ["serial", "bigserial"]),
            ("byte[]", ordinal => $"Utils.GetBytes(reader, {ordinal})",
                ["binary", "bit", "bytea", "blob", "longblob", "mediumblob", "tinyblob", "varbinary"]),
            ("string", ordinal => $"reader.GetString({ordinal})",
            [
                "char",
                "date",
                "datetime",
                "longtext",
                "mediumtext",
                "text",
                "bpchar",
                "time",
                "timestamp",
                "tinytext",
                "varchar"
            ]),
            ("object", ordinal => $"reader.GetString({ordinal})", ["json"]),
            ("int", ordinal => $"reader.GetInt32({ordinal})", ["int2", "int4", "int8"]),
            ("float", ordinal => $"reader.GetFloat({ordinal})", ["numeric", "float4", "float8"]),
            ("decimal", ordinal => $"reader.GetDecimal({ordinal})", ["decimal"]),
            ("bool", ordinal => $"reader.GetBoolean({ordinal})", ["bool", "boolean"])
        ];
    }

    public override UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return base.GetUsingDirectives()
            .Append(UsingDirective(ParseName("Npgsql")))
            .ToArray();
    }

    public override (string, string) EstablishConnection()
    {
        return (
            $"var {Variable.Connection.Name()} = NpgsqlDataSource.Create({Variable.ConnectionString.Name()})",
            string.Empty);
    }

    public override string CreateSqlCommand(string sqlTextConstant)
    {
        return $"var {Variable.Command.Name()} = {Variable.Connection.Name()}.CreateCommand({sqlTextConstant})";
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