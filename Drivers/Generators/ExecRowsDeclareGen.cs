using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System.Collections.Generic;
using System.Linq;
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
        return dbDriver.Options.UseDapper ? GetAsDapper() : GetAsDriver();

        string GetAsDapper()
        {
            var dapperParamsSection = CommonGen.GetParameterListForDapper(query.Params);
            return $$"""
                        using ({{establishConnection}})
                        {
                            {{dapperParamsSection}}
                            return await connection.ExecuteAsync({{queryTextConstant}}, {{Variable.DapperParams.AsVarName()}});
                        }
                     """;
        }

        string GetAsDriver()
        {
            var sqlcSliceSection = CommonGen.GetSqlTransformations(query, queryTextConstant);
            var createSqlCommand = dbDriver.CreateSqlCommand(sqlcSliceSection != string.Empty ? Variable.TransformedSql.AsVarName() : queryTextConstant);
            var commandParameters = CommonGen.GetCommandParameters(query.Params);
            return $$"""
                     using ({{establishConnection}})
                     {
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}{{sqlcSliceSection}}
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