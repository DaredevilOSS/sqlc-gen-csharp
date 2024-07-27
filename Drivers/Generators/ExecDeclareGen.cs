using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class ExecDeclareGen(DbDriver dbDriver)
{
    public MemberDeclarationSyntax Generate(string queryTextConstant, string argInterface, Query query)
    {
        var parametersStr = CommonGen.GetParameterListAsString(argInterface, query.Params);
        return ParseMemberDeclaration($$"""
                                        public async Task {{query.Name}}({{parametersStr}})
                                        {
                                            {{GetMethodBody(queryTextConstant, query)}}
                                        }
                                        """)!;
    }

    private string GetMethodBody(string queryTextConstant, Query query)
    {
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(query);
        var createSqlCommand = dbDriver.CreateSqlCommand(queryTextConstant);
        var commandParameters = CommonGen.GetCommandParameters(query.Params);
        var executeScalar = $"await {Variable.Command.Name()}.ExecuteScalarAsync();";
        return dbDriver.DotnetFramework.LatestDotnetSupported() ? Get() : GetAsLegacy();

        string Get()
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
                                 {{executeScalar}}
                             }
                         }
                     }
                     """;
        }
    }
}