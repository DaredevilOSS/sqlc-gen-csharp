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
        var returnType = $"Task<{dbDriver.AddNullableSuffixIfNeeded(returnInterface, false)}>";
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
        var sqlTextTransform = CommonGen.GetSqlTransformations(query, queryTextConstant);
        var connectionVar = Variable.Connection.AsVarName();
        var resultVar = Variable.Result.AsVarName();
        var anyEmbeddedTableExists = query.Columns.Any(c => c.EmbedTable is not null);
        return dbDriver.Options.UseDapper && !anyEmbeddedTableExists
            ? GetAsDapper()
            : GetAsDriver();

        string GetAsDapper()
        {
            var dapperParamsSection = CommonGen.ConstructDapperParamsDict(query.Params);
            var dapperArgs = dapperParamsSection != string.Empty ? $", {Variable.QueryParams.AsVarName()}" : string.Empty;
            var returnType = dbDriver.AddNullableSuffixIfNeeded(returnInterface, false);

            return $$"""
                        using ({{establishConnection}})
                        {{{sqlTextTransform}}{{dapperParamsSection}}
                            var {{resultVar}} = await {{connectionVar}}.QueryFirstOrDefaultAsync<{{returnType}}>({{queryTextConstant}}{{dapperArgs}});
                            return {{resultVar}};
                        }
                     """;
        }

        string GetAsDriver()
        {
            var createSqlCommand = dbDriver.CreateSqlCommand(sqlTextTransform != string.Empty ? Variable.SqlText.AsVarName() : queryTextConstant);
            var commandParameters = CommonGen.AddParametersToCommand(query.Params);
            var initDataReader = CommonGen.InitDataReader();
            var awaitReaderRow = CommonGen.AwaitReaderRow();
            var returnDataclass = CommonGen.InstantiateDataclass(query.Columns.ToArray(), returnInterface);
            return $$"""
                     using ({{establishConnection}})
                     {
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}{{sqlTextTransform}}
                         using ({{createSqlCommand}})
                         {
                            {{commandParameters}}
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