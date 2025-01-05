using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using OneDeclareGen = SqlcGenCsharp.Drivers.Generators.OneDeclareGen;

namespace SqlcGenCsharp.Drivers;

public partial class MySqlConnectorDriver(Options options) : DbDriver(options), IOne, IMany, IExec, IExecRows, IExecLastId
{
    protected override List<ColumnMapping> ColumnMappings { get; } =
    [
        new("long", ordinal => $"reader.GetInt64({ordinal})",
            new Dictionary<string, string?> { { "bigint", null } }),
        new("byte[]", ordinal => $"Utils.GetBytes(reader, {ordinal})",
            new Dictionary<string, string?>
            {
                { "binary", null },
                { "bit", null },
                { "blob", null },
                { "longblob", null },
                { "mediumblob", null },
                { "tinyblob", null },
                { "varbinary", null }
            }),
        new("string", ordinal => $"reader.GetString({ordinal})",
            new Dictionary<string, string?>
            {
                { "char", null },
                { "decimal", null },
                { "longtext", null },
                { "mediumtext", null },
                { "text", null },
                { "time", null },
                { "tinytext", null },
                { "varchar", null }
            }),
        new("DateTime", ordinal => $"reader.GetDateTime({ordinal})",
            new Dictionary<string, string?> { { "date", null }, { "datetime", null }, { "timestamp", null } }),
        new("int", ordinal => $"reader.GetInt32({ordinal})",
            new Dictionary<string, string?>
            {
                { "int", null },
                { "mediumint", null },
                { "smallint", null },
                { "tinyint", null },
                { "year", null }
            }),
        new("double", ordinal => $"reader.GetDouble({ordinal})",
            new Dictionary<string, string?> { { "double", null }, { "float", null } }),
        new("object", ordinal => $"reader.GetString({ordinal})",
            new Dictionary<string, string?> { { "json", null } })
    ];

    public override UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return base.GetUsingDirectives()
            .Append(UsingDirective(ParseName("MySqlConnector")))
            .ToArray();
    }

    public override ConnectionGenCommands EstablishConnection(Query query)
    {
        return new ConnectionGenCommands(
            $"var {Variable.Connection.AsVarName()} = new MySqlConnection({GetConnectionStringField()})",
            $"{Variable.Connection.AsVarName()}.Open()"
        );
    }

    public override string CreateSqlCommand(string sqlTextConstant)
    {
        return $"var {Variable.Command.AsVarName()} = new MySqlCommand({sqlTextConstant}, {Variable.Connection.AsVarName()})";
    }

    public override string TransformQueryText(Query query)
    {
        var counter = 0;
        var queryText = options.UseDapper ? $"{query.Text}; SELECT LAST_INSERT_ID()" : query.Text;
        return QueryParamRegex().Replace(queryText, _ => "@" + query.Params[counter++].Column.Name);
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

    public MemberDeclarationSyntax ExecLastIdDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new ExecLastIdDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }

    public string[] GetLastIdStatement()
    {
        return
        [
            $"await {Variable.Command.AsVarName()}.ExecuteNonQueryAsync();",
            $"return {Variable.Command.AsVarName()}.LastInsertedId;"
        ];
    }


    public override MemberDeclarationSyntax ManyDeclare(string queryTextConstant, string argInterface,
        string returnInterface, Query query)
    {
        return new ManyDeclareGen(this).Generate(queryTextConstant, argInterface, returnInterface, query);
    }

    [GeneratedRegex(@"\?")]
    private static partial Regex QueryParamRegex();

    public MemberDeclarationSyntax ExecRowsDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new ExecRowsDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }
}