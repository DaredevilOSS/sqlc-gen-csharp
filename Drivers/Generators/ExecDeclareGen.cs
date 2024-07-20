using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System.Collections.Generic;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class ExecDeclareGen(DbDriver dbDriver)
{
    public MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        IList<Parameter> parameters)
    {
        var parametersStr = CommonGen.GetParameterListAsString(argInterface, parameters);
        return ParseMemberDeclaration($$"""
                                        public async Task {{funcName}}({{parametersStr}})
                                        {
                                            {{GetMethodBody(queryTextConstant, parameters)}}
                                        }
                                        """)!;
    }

    private string GetMethodBody(string queryTextConstant, IEnumerable<Parameter> parameters)
    {
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection();
        var createSqlCommand = dbDriver.CreateSqlCommand(queryTextConstant);
        var commandParameters = CommonGen.GetCommandParameters(parameters);
        var executeScalar = $"await {Variable.Command.Name()}.ExecuteScalarAsync();";

        return dbDriver.DotnetFramework.LatestDotnetSupported()
            ? GetWithUsingAsStatement()
            : GetWithUsingAsBlock();

        string GetWithUsingAsStatement()
        {
            return $$"""
                     {
                         await using {{establishConnection}};
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}
                         await using {{createSqlCommand}};
                         {{commandParameters.JoinByNewLine()}}
                         {{executeScalar}}
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
                                 {{executeScalar}}
                             }
                         }
                     }
                     """;
        }
    }
}