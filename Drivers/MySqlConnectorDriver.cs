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

public partial class MySqlConnectorDriver(DotnetFramework dotnetFramework) : DbDriver(dotnetFramework), IExecLastId
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

    public override (string, string) EstablishConnection(Query query)
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
        var parametersStr = CommonGen.GetParameterListAsString(argInterface, query.Params);
        var (establishConnection, connectionOpen) = EstablishConnection(query);
        var createSqlCommand = CreateSqlCommand(queryTextConstant);
        var commandParameters = CommonGen.GetCommandParameters(query.Params);
        var executeScalarAndReturnCreated = ExecuteScalarAndReturnCreated();
        var methodBody = DotnetFramework.LatestDotnetSupported()
            ? GetWithUsingAsStatement()
            : GetWithUsingAsBlock();

        return ParseMemberDeclaration($$"""
                                        public async Task<long> {{query.Name}}({{parametersStr}})
                                        {
                                            {{methodBody}}
                                        }
                                        """)!;

        string GetWithUsingAsStatement()
        {
            return $$"""
                     {
                         await using {{establishConnection}};
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}
                         await using {{createSqlCommand}};
                         {{commandParameters.JoinByNewLine()}}
                         {{executeScalarAndReturnCreated.JoinByNewLine()}}
                     }
                     """;
        }

        string GetWithUsingAsBlock()
        {
            return $$"""
                     {
                         using ({{establishConnection}})
                         {
                             {{connectionOpen.AppendSemicolonUnlessEmpty()}}
                             using ({{createSqlCommand}})
                             {
                                {{commandParameters.JoinByNewLine()}}
                                {{executeScalarAndReturnCreated.JoinByNewLine()}}
                             }
                         }
                     }
                     """;
        }

        IEnumerable<string> ExecuteScalarAndReturnCreated()
        {
            return new[]
            {
                $"await {Variable.Command.Name()}.ExecuteNonQueryAsync();",
                $"return {Variable.Command.Name()}.LastInsertedId;"
            };
        }
    }


    public override MemberDeclarationSyntax ManyDeclare(string queryTextConstant, string argInterface,
        string returnInterface, Query query)
    {
        return new ManyDeclareGen(this).Generate(queryTextConstant, argInterface, returnInterface, query);
    }

    [GeneratedRegex(@"\?")]
    private static partial Regex QueryParamRegex();
}