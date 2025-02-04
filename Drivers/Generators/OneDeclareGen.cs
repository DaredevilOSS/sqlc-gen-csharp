using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace SqlcGenCsharp.Drivers.Generators;

public class OneDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string queryTextConstant, string argInterface, string returnInterface, Query query)
    {
        var returnType = $"Task<{dbDriver.AddNullableSuffix(returnInterface, false)}>";
        var parametersStr = CommonGen.GetMethodParameterList(argInterface, query.Params);
        return ParseMemberDeclaration($$"""
                                        public async {{returnType}} {{query.Name}}({{parametersStr}})
                                        {
                                            {{GetMethodBody(queryTextConstant, returnInterface, query)}}
                                        }
                                        """)!;
    }

    private string GetMethodBody(string queryTextConstant, string returnInterface, Query query)
    {
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(query);
        var sqlTextTrasformation = CommonGen.GetSqlTransformations(query, queryTextConstant);
        return dbDriver.Options.UseDapper ? GetAsDapper() : GetAsDriver();

        string GetAsDapper()
        {
            var dapperParamsSection = CommonGen.GetParameterListForDapper(query.Params);
            var dapperArgs = dapperParamsSection != string.Empty ? $", {Variable.DapperParams.AsVarName()}" : string.Empty;
            var returnType = dbDriver.AddNullableSuffix(returnInterface, false);
            return $$"""
                        using ({{establishConnection}})
                        {{{sqlTextTrasformation}}{{dapperParamsSection}}
                            var result = await connection.QueryFirstOrDefaultAsync<{{returnType}}>({{queryTextConstant}}{{dapperArgs}});
                            return result;
                        }
                     """;
        }

        string GetAsDriver()
        {
            var createSqlCommand = dbDriver.CreateSqlCommand(sqlTextTrasformation != string.Empty ? Variable.TransformedSql.AsVarName() : queryTextConstant);
            var commandParameters = CommonGen.GetCommandParameters(query.Params);
            var initDataReader = CommonGen.InitDataReader();
            var awaitReaderRow = CommonGen.AwaitReaderRow();
            var returnDataclass = dbDriver.InstantiateDataclass(query.Columns.ToArray(), returnInterface);
            return $$"""
                     using ({{establishConnection}})
                     {
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}{{sqlTextTrasformation}}
                         using ({{createSqlCommand}})
                         {
                            {{commandParameters.JoinByNewLine()}}
                             using ({{initDataReader}})
                             {
                                 if ({{awaitReaderRow}})
                                 {
                                     return {{returnDataclass}};
                                 }
                             }
                         }
                     }
                     return null;
                     """;
        }
    }
}