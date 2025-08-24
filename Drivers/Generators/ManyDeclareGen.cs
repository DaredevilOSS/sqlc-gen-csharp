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
        var sqlTextTransform = CommonGen.GetSqlTransformations(query, queryTextConstant);
        var anyEmbeddedTableExists = query.Columns.Any(c => c.EmbedTable is not null);
        var useDapper = dbDriver.Options.UseDapper && !anyEmbeddedTableExists;

        var dapperParams = useDapper ? CommonGen.ConstructDapperParamsDict(query) : string.Empty;
        var sqlVar = sqlTextTransform != string.Empty ? Variable.TransformedSql.AsVarName() : queryTextConstant;
        var transactionProperty = Variable.Transaction.AsPropertyName();

        var noTxBody = useDapper ? GetDapperNoTxBody(sqlVar, returnInterface, query) : GetDriverNoTxBody(sqlVar, returnInterface, query);
        var withTxBody = useDapper ? GetDapperWithTxBody(sqlVar, returnInterface, query) : GetDriverWithTxBody(sqlVar, returnInterface, query);

        return $$"""
                    {{sqlTextTransform}}
                    {{dapperParams}}
                    if (this.{{transactionProperty}} == null)
                    {
                        {{noTxBody}}
                    }
                    {{withTxBody}}
                 """;
    }

    private string GetDapperNoTxBody(string sqlVar, string returnInterface, Query query)
    {
        var (establishConnection, _) = dbDriver.EstablishConnection(query);
        var dapperArgs = query.Params.Any() ? $", {Variable.QueryParams.AsVarName()}" : string.Empty;
        var returnType = dbDriver.AddNullableSuffixIfNeeded(returnInterface, true);

        return $$"""
                    using ({{establishConnection}})
                    {
                        var {{Variable.Result.AsVarName()}} = await {{Variable.Connection.AsVarName()}}.QueryAsync<{{returnType}}>({{sqlVar}}{{dapperArgs}});
                        return {{Variable.Result.AsVarName()}}.AsList();
                    }
                 """;
    }

    private string GetDapperWithTxBody(string sqlVar, string returnInterface, Query query)
    {
        var transactionProperty = Variable.Transaction.AsPropertyName();
        var dapperArgs = query.Params.Any() ? $", {Variable.QueryParams.AsVarName()}" : string.Empty;
        var returnType = dbDriver.AddNullableSuffixIfNeeded(returnInterface, true);

        return $$"""
                    {{dbDriver.TransactionConnectionNullExcetionThrow}}
                    return (await this.{{transactionProperty}}.Connection.QueryAsync<{{returnType}}>(
                            {{sqlVar}}{{dapperArgs}},
                            transaction: this.{{transactionProperty}})).AsList();
                 """;
    }

    private string GetDriverNoTxBody(string sqlVar, string returnInterface, Query query)
    {
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(query);
        var createSqlCommand = dbDriver.CreateSqlCommand(sqlVar);
        var commandParameters = CommonGen.AddParametersToCommand(query);
        var initDataReader = CommonGen.InitDataReader();
        var awaitReaderRow = CommonGen.AwaitReaderRow();
        var dataclassInit = CommonGen.InstantiateDataclass(query.Columns.ToArray(), returnInterface, query);
        var resultVar = Variable.Result.AsVarName();
        var readWhileExists = $$"""
                                while ({{awaitReaderRow}})
                                    {{resultVar}}.Add({{dataclassInit}});
                                """;
        // TODO: add return null at end of code run so transaction code will not run?
        return $$"""
                     using ({{establishConnection}})
                     {
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}
                         using ({{createSqlCommand}})
                         {
                             {{commandParameters}}
                             using ({{initDataReader}})
                             {
                                 var {{resultVar}} = new List<{{returnInterface}}>();
                                 {{readWhileExists}}
                                 return {{resultVar}};
                             }
                         }
                     }
                     """;
    }

    private string GetDriverWithTxBody(string sqlVar, string returnInterface, Query query)
    {
        var transactionProperty = Variable.Transaction.AsPropertyName();
        var commandVar = Variable.Command.AsVarName();
        var commandParameters = CommonGen.AddParametersToCommand(query);
        var initDataReader = CommonGen.InitDataReader();
        var awaitReaderRow = CommonGen.AwaitReaderRow();
        var dataclassInit = CommonGen.InstantiateDataclass(query.Columns.ToArray(), returnInterface, query);
        var resultVar = Variable.Result.AsVarName();
        var readWhileExists = $$"""
                                while ({{awaitReaderRow}})
                                    {{resultVar}}.Add({{dataclassInit}});
                                """;

        return $$"""
                    {{dbDriver.TransactionConnectionNullExcetionThrow}}
                    using (var {{commandVar}} = this.{{transactionProperty}}.Connection.CreateCommand())
                    {
                        {{commandVar}}.CommandText = {{sqlVar}};
                        {{commandVar}}.Transaction = this.{{transactionProperty}};
                        {{commandParameters}}
                        using ({{initDataReader}})
                        {
                            var {{resultVar}} = new List<{{returnInterface}}>();
                            {{readWhileExists}}
                            return {{resultVar}};
                        }
                    }
                 """;
    }
}