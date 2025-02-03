using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class ExecDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string queryTextConstant, string argInterface, Query query)
    {
        var parametersStr = CommonGen.GetMethodParameterList(argInterface, query.Params);
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
        var sqlTextTrasformation = CommonGen.GetSqlTransformations(query, queryTextConstant);
        return dbDriver.Options.UseDapper ? GetAsDapper() : GetAsDriver();

        string GetAsDapper()
        {
            var dapperParamsSection = CommonGen.GetParameterListForDapper(query.Params);
            var dapperArgs = dapperParamsSection != string.Empty ? $", {Variable.DapperParams.AsVarName()}" : string.Empty;
            return $$"""
                        using ({{establishConnection}})
                        {{{sqlTextTrasformation}}{{dapperParamsSection}}
                            await connection.ExecuteAsync({{queryTextConstant}}{{dapperArgs}});
                        }
                     """;
        }

        string GetAsDriver()
        {
            var commandParameters = CommonGen.GetCommandParameters(query.Params);
            var createSqlCommand = dbDriver.CreateSqlCommand(sqlTextTrasformation != string.Empty ? Variable.TransformedSql.AsVarName() : queryTextConstant);
            var executeScalar = $"await {Variable.Command.AsVarName()}.ExecuteScalarAsync();";
            return $$"""
                     using ({{establishConnection}})
                     {
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}{{sqlTextTrasformation}}
                         using ({{createSqlCommand}})
                         {
                             {{commandParameters.JoinByNewLine()}}
                             {{executeScalar}}
                         }
                     }
                     """;
        }
    }
}