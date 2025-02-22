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
            public async Task<{{dbDriver.GetIdColumnType(query)}}> {{query.Name}}({{parametersStr}})
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
            var dapperArgs = dapperParamsSection == string.Empty
                ? string.Empty
                : $", {Variable.QueryParams.AsVarName()}";
            return $$"""
                     using ({{establishConnection}})
                     {{{sqlTextTransform}}{{dapperParamsSection}}
                        return await {{Variable.Connection.AsVarName()}}.QuerySingleAsync<{{dbDriver.GetIdColumnType(query)}}>({{queryTextConstant}}{{dapperArgs}});
                     }
                     """;
        }

        string GetAsDriver()
        {
            var sqlTextVar = sqlTextTransform == string.Empty ? queryTextConstant : Variable.TransformedSql.AsVarName();
            var createSqlCommand = dbDriver.CreateSqlCommand(sqlTextVar);
            var commandParameters = CommonGen.AddParametersToCommand(query.Params);
            var returnLastId = ((IExecLastId)dbDriver).GetLastIdStatement(query).JoinByNewLine();
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