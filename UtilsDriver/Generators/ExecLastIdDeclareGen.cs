using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class ExecLastIdDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);
    
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
        var establishConnection = dbDriver.EstablishConnection();
        var connectionOpen = establishConnection.Length == 2 ? establishConnection[1] + ";" : string.Empty;
        var createSqlCommand = dbDriver.CreateSqlCommand(queryTextConstant);
        var commandParameters = CommonGen.GetCommandParameters(parameters);
        var executeScalarAndReturnCreated = ExecuteScalarAndReturnCreated();

        return dbDriver.DotnetFramework.UsingStatementEnabled()
            ? GetWithUsingAsStatement()
            : GetWithUsingAsBlock();

        string GetWithUsingAsStatement()
        {
            return $$"""
             {
                 await using {{establishConnection[0]}};
                 {{connectionOpen}}
                 await using {{createSqlCommand}};
                 {{string.Join("\n", commandParameters)}}
                 {{string.Join("\n", executeScalarAndReturnCreated)}}
             }
             """;
        }

        string GetWithUsingAsBlock()
        {
            return $$"""
             {
                 using ({{establishConnection[0]}})
                 {
                     {{connectionOpen}}
                     using ({{createSqlCommand}})
                     {
                         {{string.Join("\n", commandParameters)}}
                         {{string.Join("\n", executeScalarAndReturnCreated)}}
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