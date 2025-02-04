using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class ExecRowsDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string queryTextConstant, string argInterface, Query query)
    {
        var parametersStr = CommonGen.GetMethodParameterList(argInterface, query.Params);
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
        var sqlTextTrasformation = CommonGen.GetSqlTransformations(query, queryTextConstant);
        return dbDriver.Options.UseDapper ? GetAsDapper() : GetAsDriver();

        string GetAsDapper()
        {
            var dapperParamsSection = CommonGen.GetParameterListForDapper(query.Params);
            var dapperArgs = dapperParamsSection != string.Empty ? $", {Variable.DapperParams.AsVarName()}" : string.Empty;
            return $$"""
                        using ({{establishConnection}})
                        {{{sqlTextTrasformation}}{{dapperParamsSection}}
                            return await connection.ExecuteAsync({{queryTextConstant}}{{dapperArgs}});
                        }
                     """;
        }

        string GetAsDriver()
        {
            var createSqlCommand = dbDriver.CreateSqlCommand(sqlTextTrasformation != string.Empty ? Variable.TransformedSql.AsVarName() : queryTextConstant);
            var commandParameters = CommonGen.GetCommandParameters(query.Params);
            return $$"""
                     using ({{establishConnection}})
                     {
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}{{sqlTextTrasformation}}
                         using ({{createSqlCommand}})
                         {
                            {{commandParameters.JoinByNewLine()}}
                            return await {{Variable.Command.AsVarName()}}.ExecuteNonQueryAsync();
                         }
                     }
                     """;
        }
    }
}