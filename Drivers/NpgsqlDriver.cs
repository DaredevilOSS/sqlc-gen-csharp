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

    public override (string, string) EstablishConnection(bool isCopyCommand = false)
    {
        if (isCopyCommand)
            return (
                $"var ds = NpgsqlDataSource.Create({Variable.ConnectionString.Name()})",
                $"var {Variable.Connection.Name()} = ds.CreateConnection()"
            );
        return ($"var {Variable.Connection.Name()} = NpgsqlDataSource.Create({Variable.ConnectionString.Name()})", "");
    }

    public override string CreateSqlCommand(string sqlTextConstant)
    {
        return $"var {Variable.Command.Name()} = {Variable.Connection.Name()}.CreateCommand({sqlTextConstant})";
    }

    public override string TransformQueryText(Query query)
    {
        if (query.Cmd == ":copyfrom")
            return GetCopyCommand();

        var queryText = query.Text;
        for (var i = 0; i < query.Params.Count; i++)
        {
            var currentParameter = query.Params[i];
            queryText = Regex.Replace(queryText, $@"\$\s*{i + 1}",
                $"@{currentParameter.Column.Name.FirstCharToLower()}");
        }
        return queryText;

        string GetCopyCommand()
        {
            var copyParams = query.Params.Select(p => p.Column.Name).JoinByComma();
            return $"COPY {query.InsertIntoTable.Name} ({copyParams}) FROM STDIN (FORMAT BINARY)";
        }
    }

    public override MemberDeclarationSyntax OneDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, Query query)
    {
        return new OneDeclareGen(this).Generate(funcName, queryTextConstant, argInterface, returnInterface, query);
    }

    public override MemberDeclarationSyntax ExecDeclare(string funcName, string queryTextConstant, string argInterface,
        Query query)
    {
        return new ExecDeclareGen(this).Generate(funcName, queryTextConstant, argInterface, query);
    }

    public override MemberDeclarationSyntax ManyDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, Query query)
    {
        return new ManyDeclareGen(this).Generate(funcName, queryTextConstant, argInterface, returnInterface, query);
    }

    public MemberDeclarationSyntax CopyFromDeclare(string funcName, string queryTextConstant, string argInterface, Query query)
    {
        return new CopyFromDeclareGen(this).Generate(funcName, queryTextConstant, argInterface, query);
    }
}