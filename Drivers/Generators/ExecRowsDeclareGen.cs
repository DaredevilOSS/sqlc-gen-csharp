using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System.Collections.Generic;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class ExecRowsDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string queryTextConstant, string argInterface, Query query)
    {
        var parametersStr = CommonGen.GetParameterListAsString(argInterface, query.Params);
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(query);
        var createSqlCommand = dbDriver.CreateSqlCommand(queryTextConstant);
        var commandParameters = CommonGen.GetCommandParameters(query.Params);
        var executeScalarAndReturnCreated = ExecuteScalarAndReturnCreated();
        var methodBody = dbDriver.DotnetFramework.LatestDotnetSupported()
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
            return [$"return await {Variable.Command.Name()}.ExecuteNonQueryAsync();"];
        }
    }
}