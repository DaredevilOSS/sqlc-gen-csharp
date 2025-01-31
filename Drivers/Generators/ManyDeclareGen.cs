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
            var args = CommonGen.GetParameterListForDapper(query.Params);
            var returnType = dbDriver.AddNullableSuffix(returnInterface, true);
            return $$"""
                        using ({{establishConnection}})
                        {
                            var results = await connection.QueryAsync<{{returnType}}>({{queryTextConstant}}{{args}});
                            return results.AsList();
                        }
                     """;
        }

        string GetAsDriver()
        {
            var createSqlCommand = dbDriver.CreateSqlCommand(queryTextConstant);
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
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}
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