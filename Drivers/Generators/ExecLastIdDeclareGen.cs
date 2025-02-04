using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class ExecLastIdDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string queryTextConstant, string argInterface, Query query)
    {
        var parametersStr = CommonGen.GetMethodParameterList(argInterface, query.Params);
        return ParseMemberDeclaration($$"""
            public async Task<{{dbDriver.GetIdColumnType()}}> {{query.Name}}({{parametersStr}})
            {
                {{GetMethodBody(queryTextConstant, query)}}
            }
            """)!;
    }

    private string GetMethodBody(string queryTextConstant, Query query)
    {
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(query);
        var sqlTextTransform = CommonGen.GetSqlTransformations(query, queryTextConstant);
        connectionOpen = connectionOpen.AppendSemicolonUnlessEmpty();
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
                        return await {{Variable.Connection.AsVarName()}}.QuerySingleAsync<{{dbDriver.GetIdColumnType()}}>({{queryTextConstant}}{{dapperArgs}});
                     }
                     """;
        }

        string GetAsDriver()
        {
            var createSqlCommand = dbDriver.CreateSqlCommand(sqlTextTransform != string.Empty ? Variable.SqlText.AsVarName() : queryTextConstant);
            var commandParameters = CommonGen.AddParametersToCommand(query.Params);
            var returnLastId = ((IExecLastId)dbDriver).GetLastIdStatement().JoinByNewLine();
            return $$"""
                     using ({{establishConnection}})
                     {
                         {{connectionOpen}}{{sqlTextTransform}}
                         using ({{createSqlCommand}})
                         {
                            {{commandParameters}}
                            {{returnLastId}}
                         }
                     }
                     """;
        }
    }
}