using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace SqlcGenCsharp.Drivers.Generators;

public class ManyDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string queryTextConstant, string argInterface, string returnInterface, Query query)
    {
        var parametersStr = CommonGen.GetMethodParameterList(argInterface, query.Params);
        var returnType = $"Task<List<{returnInterface}>>";
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
            var sqlcSliceSection = CommonGen.GetSqlTransformations(query, queryTextConstant);
            var sqlArgs = CommonGen.GetParameterListForDapper(query.Params);
            var returnType = dbDriver.AddNullableSuffix(returnInterface, true);
            var sqlQuery = sqlcSliceSection != string.Empty ? Variable.TransformedSql.AsVarName() : queryTextConstant;
            return $$"""
                        using ({{establishConnection}})
                        {{{sqlcSliceSection}}
                            {{sqlArgs}}
                            var results = await connection.QueryAsync<{{returnType}}>({{sqlQuery}}, {{Variable.SqlParams.AsVarName()}});
                            return results.AsList();
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
            var dataclassInit = CommonGen.InstantiateDataclass(query.Columns, returnInterface);
            var readWhileExists = $$"""
                                    while ({{awaitReaderRow}})
                                    {
                                        {{Variable.Result.AsVarName()}}.Add({{dataclassInit}});
                                    }
                                    """;
            return $$"""
                     using ({{establishConnection}})
                     {
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}{{sqlcSliceSection}}
                         using ({{createSqlCommand}})
                         {
                             {{commandParameters.JoinByNewLine()}}
                             using ({{initDataReader}})
                             {
                                 var {{Variable.Result.AsVarName()}} = new List<{{returnInterface}}>();
                                 {{readWhileExists}}
                                 return {{Variable.Result.AsVarName()}};
                             }
                         }
                     }
                     """;
        }
    }
}