using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
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
        var sqlTextTransform = CommonGen.GetSqlTransformations(query, queryTextConstant);
        return dbDriver.Options.UseDapper ? GetAsDapper() : GetAsDriver();

        string GetAsDapper()
        {
            var dapperParamsSection = CommonGen.ConstructDapperParamsDict(query.Params);
            var dapperArgs = dapperParamsSection != string.Empty
                ? $", {Variable.QueryParams.AsVarName()}"
                : string.Empty;
            return $$"""
                        using ({{establishConnection}})
                        {{{sqlTextTransform}}{{dapperParamsSection}}
                            await {{Variable.Connection.AsVarName()}}.ExecuteAsync({{queryTextConstant}}{{dapperArgs}});
                        }
                     """;
        }

        string GetAsDriver()
        {
            var commandParameters = CommonGen.AddParametersToCommand(query.Params);
            var createSqlCommand = dbDriver.CreateSqlCommand(sqlTextTransform != string.Empty ? Variable.SqlText.AsVarName() : queryTextConstant);
            var executeScalar = $"await {Variable.Command.AsVarName()}.ExecuteScalarAsync();";
            return $$"""
                     using ({{establishConnection}})
                     {
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}{{sqlTextTransform}}
                         using ({{createSqlCommand}})
                         {
                             {{commandParameters}}
                             {{executeScalar}}
                         }
                     }
                     """;
        }
    }
}