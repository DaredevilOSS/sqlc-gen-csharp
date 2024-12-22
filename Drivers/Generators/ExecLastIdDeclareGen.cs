using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class ExecLastIdDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string queryTextConstant, string argInterface, Query query)
    {
        var parametersStr = CommonGen.GetParameterListAsString(argInterface, query.Params);


        return ParseMemberDeclaration($$"""
                                        public async Task<long> {{query.Name}}({{parametersStr}})
                                        {
                                            {{GetMethodBody(queryTextConstant, query)}}
                                        }
                                        """)!;
    }


    private string GetMethodBody(string queryTextConstant, Query query)
    {
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(query);
        var createSqlCommand = dbDriver.CreateSqlCommand(queryTextConstant);
        var commandParameters = CommonGen.GetCommandParameters(query.Params).ToList();
        var executeScalarAndReturnCreated = ExecuteScalarAndReturnCreated();

        if (dbDriver.Options.DotnetFramework.LatestDotnetSupported())
            return GetAsLatest();
        return GetAsLegacy();

        string GetAsLatest()
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

        string GetAsLegacy()
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
            return
            [
                $"await {Variable.Command.Name()}.ExecuteNonQueryAsync();",
                $"return {Variable.Command.Name()}.LastInsertedId;"
            ];
        }
    }
}