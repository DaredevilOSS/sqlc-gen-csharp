using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public partial class SqliteDriver(Options options) : DbDriver(options), IOne, IMany, IExec, IExecRows, IExecLastId
{
    protected override List<ColumnMapping> ColumnMappings { get; } = [
        new("byte[]", new Dictionary<string, string?>
            {
                {"blob", null}
            }, ordinal => $"Utils.GetBytes(reader, {ordinal})"),
        new("string",
            new Dictionary<string, string?>
            {
                {"text", null}
            }, ordinal => $"reader.GetString({ordinal})"),
        new("int",
            new Dictionary<string, string?>{
            {
                "integer", null
            }}, ordinal => $"reader.GetInt32({ordinal})"),
        new("float",
            new Dictionary<string, string?>
            {
                {"real", null}
            }, ordinal => $"reader.GetFloat({ordinal})"),
    ];

    public override UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return base.GetUsingDirectives()
            .Append(UsingDirective(ParseName("Microsoft.Data.Sqlite")))
            .ToArray();
    }

    public override ConnectionGenCommands EstablishConnection(Query query)
    {
        return new ConnectionGenCommands(
            $"var {Variable.Connection.AsVarName()} = new SqliteConnection({GetConnectionStringField()})",
            $"{Variable.Connection.AsVarName()}.Open()"
        );
    }

    public override string CreateSqlCommand(string sqlTextConstant)
    {
        return $"var {Variable.Command.AsVarName()} = new SqliteCommand({sqlTextConstant}, {Variable.Connection.AsVarName()})";
    }

    public override string TransformQueryText(Query query)
    {
        return query.Params
            .Aggregate(query.Text, (current, currentParameter) => BindParameterRegex()
            .Replace(current, $"@{currentParameter.Column.Name.ToCamelCase()}", 1));
    }

    public override MemberDeclarationSyntax OneDeclare(string queryTextConstant, string argInterface,
        string returnInterface, Query query)
    {
        return new OneDeclareGen(this).Generate(queryTextConstant, argInterface, returnInterface, query);
    }

    public override MemberDeclarationSyntax ExecDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new ExecDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }

    public override MemberDeclarationSyntax ManyDeclare(string queryTextConstant, string argInterface,
        string returnInterface, Query query)
    {
        return new ManyDeclareGen(this).Generate(queryTextConstant, argInterface, returnInterface, query);
    }

    [GeneratedRegex(@"\?")]
    private static partial Regex BindParameterRegex();

    public MemberDeclarationSyntax ExecRowsDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new ExecRowsDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }

    public MemberDeclarationSyntax ExecLastIdDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new ExecLastIdDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }
}