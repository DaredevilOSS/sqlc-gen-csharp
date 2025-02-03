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
        return dbDriver.Options.UseDapper ? GetAsDapper() : GetAsDriver();

        string GetAsDapper()
        {
            var sqlArgs = CommonGen.GetParameterListForDapper(query.Params);
            var returnType = dbDriver.AddNullableSuffix(returnInterface, false);
            return $$"""
                        using ({{establishConnection}})
                        {
                            {{sqlArgs}}
                            var result = await connection.QueryFirstOrDefaultAsync<{{returnType}}>({{queryTextConstant}}, {{Variable.DapperParams.AsVarName()}});
                            return result;
                        }
                     """;
        }

        string GetAsDriver()
        {
            var sqlcSliceSection = CommonGen.GetSqlTransformations(query, queryTextConstant);
            var createSqlCommand = dbDriver.CreateSqlCommand(sqlcSliceSection != string.Empty ? Variable.TransformedSql.AsVarName() : queryTextConstant);
            var commandParameters = CommonGen.GetCommandParameters(query.Params);
            var initDataReader = CommonGen.InitDataReader();
            var awaitReaderRow = CommonGen.AwaitReaderRow();
            var returnDataclass = CommonGen.InstantiateDataclass(query.Columns, returnInterface);
            return $$"""
                     using ({{establishConnection}})
                     {
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}{{sqlcSliceSection}}
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