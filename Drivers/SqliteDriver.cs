using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public partial class SqliteDriver(DotnetFramework dotnetFramework) : DbDriver(dotnetFramework)
{
    protected override List<ColumnMapping> ColumnMappings { get; } = [
        new("byte[]", ordinal => $"Utils.GetBytes(reader, {ordinal})",
            new Dictionary<string, string?>
            {
                {"blob", null}
            }),
        new("string", ordinal => $"reader.GetString({ordinal})",
            new Dictionary<string, string?>
            {
                {"text", null}
            }),
        new("int", ordinal => $"reader.GetInt32({ordinal})",
            new Dictionary<string, string?>{
            {
                "integer", null
            }}),
        new("float", ordinal => $"reader.GetFloat({ordinal})",
            new Dictionary<string, string?>
            {
                {"real", null}
            }),
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
            $"var {Variable.Connection.Name()} = new SqliteConnection({Variable.ConnectionString.Name()})",
            $"{Variable.Connection.Name()}.Open()"
        );
    }

    public override string CreateSqlCommand(string sqlTextConstant)
    {
        return $"var {Variable.Command.Name()} = new SqliteCommand({sqlTextConstant}, {Variable.Connection.Name()})";
    }

    public override string TransformQueryText(Query query)
    {
        return query.Params
            .Aggregate(query.Text, (current, currentParameter) => BindParameterRegex()
            .Replace(current, $"@{currentParameter.Column.Name.FirstCharToLower()}", 1));
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
}