using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class ExecLastIdDeclareGen(DbDriver dbDriver)
{
    public MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        IList<Parameter> parameters)
    {
        return ParseMemberDeclaration($$"""
                                        public async Task<long> {{funcName}}({{CommonGen.GetParameterListAsString(argInterface, parameters)}})
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
        var executeScalarAndReturnCreated = ExecuteScalarAndReturnCreated();

        return dbDriver.DotnetFramework.LatestDotnetSupported()
            ? GetWithUsingAsStatement()
            : GetWithUsingAsBlock();

        string GetWithUsingAsStatement()
        {
            return $$"""
                     {
                         await using {{establishConnection}};
                         {{connectionOpen}};
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
                             {{connectionOpen}};
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
}