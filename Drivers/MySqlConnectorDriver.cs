using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using OneDeclareGen = SqlcGenCsharp.Drivers.Generators.OneDeclareGen;

namespace SqlcGenCsharp.Drivers;

public partial class MySqlConnectorDriver(DotnetFramework dotnetFramework) : DbDriver(dotnetFramework)
{
    protected override List<(string, Func<int, string>, HashSet<string>)> GetColumnMapping()
    {
        return
        [
            ("long", ordinal => $"reader.GetInt64({ordinal})", ["bigint"]),
            ("byte[]", ordinal => $"Utils.GetBytes(reader, {ordinal})", [
                "binary",
                "bit",
                "blob",
                "longblob",
                "mediumblob",
                "tinyblob",
                "varbinary"
            ]),
            ("string", ordinal => $"reader.GetString({ordinal})", [
                "char",
                "date",
                "datetime",
                "decimal",
                "longtext",
                "mediumtext",
                "text",
                "time",
                "timestamp",
                "tinytext",
                "varchar"
            ]),
            ("int", ordinal => $"reader.GetInt32({ordinal})", [
                "int",
                "mediumint",
                "smallint",
                "tinyint",
                "year"
            ]),
            ("double", ordinal => $"reader.GetDouble({ordinal})", ["double", "float"]),
            ("object", ordinal => $"reader.GetString({ordinal})", ["json"])
        ];
    }

    public override UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return base.GetUsingDirectives()
            .Append(UsingDirective(ParseName("MySqlConnector")))
            .ToArray();
    }

    public override (string, string) EstablishConnection()
    {
        return (
            $"var {Variable.Connection.Name()} = new MySqlConnection({Variable.ConnectionString.Name()})",
            $"{Variable.Connection.Name()}.Open()");
    }

    public override string CreateSqlCommand(string sqlTextConstant)
    {
        return $"var {Variable.Command.Name()} = new MySqlCommand({sqlTextConstant}, {Variable.Connection.Name()})";
    }

    public override string TransformQueryText(Query query)
    {
        var counter = 0;
        return QueryParamRegex().Replace(query.Text, _ => "@" + query.Params[counter++].Column.Name);
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
    private static partial Regex QueryParamRegex();
}