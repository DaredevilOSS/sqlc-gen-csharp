using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public class SqliteDriver(DotnetFramework dotnetFramework) : DbDriver(dotnetFramework), IExecLastId
{
    protected override List<(string, Func<int, string>, HashSet<string>)> GetColumnMapping()
    {
        return
        [
            ("byte[]", ordinal => $"Utils.GetBytes(reader, {ordinal})", ["blob"]),
            ("string", ordinal => $"reader.GetString({ordinal})", ["text"]),
            ("int", ordinal => $"reader.GetInt32({ordinal})", ["integer"]),
            ("float", ordinal => $"reader.GetFloat({ordinal})", ["real"]),
        ];
    }

    public override UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return base.GetUsingDirectives()
            .Append(UsingDirective(ParseName("Microsoft.Data.Sqlite")))
            .ToArray();
    }

    public override GenExpression[] EstablishConnection(Query query)
    {
        return [
            new GenExpression(
                $"var {Variable.Connection.Name()} = new MySqlConnection({Variable.ConnectionString.Name()})",
                true, true),
            new GenExpression($"{Variable.Connection.Name()}.Open()", false, false)
        ];
    }

    public override string CreateSqlCommand(string sqlTextConstant)
    {
        return $"var {Variable.Command.Name()} = new SQLiteCommand({sqlTextConstant}, {Variable.Connection.Name()})";
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

    public MemberDeclarationSyntax ExecLastIdDeclare(string queryTextConstant, string argInterface, Query query)
    {
        var parametersStr = CommonGen.GetParameterListAsString(argInterface, query.Params);
        var establishConnection = EstablishConnection(query);
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
                $"return {Variable.Connection.Name()}.LastInsertRowId;"
            };
        }
    }
}