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
        var sqlTextTransform = CommonGen.GetSqlTransformations(query, queryTextConstant);
        var anyEmbeddedTableExists = query.Columns.Any(c => c.EmbedTable is not null);
        var useDapper = dbDriver.Options.UseDapper && !anyEmbeddedTableExists;

        var dapperParams = useDapper ? CommonGen.ConstructDapperParamsDict(query.Params) : string.Empty;
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
        var returnType = dbDriver.AddNullableSuffixIfNeeded(returnInterface, false);

        return $$"""
                    using ({{establishConnection}})
                    {
                        var {{Variable.Result.AsVarName()}} = await {{Variable.Connection.AsVarName()}}.QueryFirstOrDefaultAsync<{{returnType}}>({{sqlVar}}{{dapperArgs}});
                        return {{Variable.Result.AsVarName()}};
                    }
                 """;
    }

    private string GetDapperWithTxBody(string sqlVar, string returnInterface, Query query)
    {
        var transactionProperty = Variable.Transaction.AsPropertyName();
        var dapperArgs = query.Params.Any() ? $", {Variable.QueryParams.AsVarName()}" : string.Empty;
        var returnType = dbDriver.AddNullableSuffixIfNeeded(returnInterface, false);

        return $$"""
                    if (this.{{transactionProperty}}?.Connection == null || this.{{transactionProperty}}?.Connection.State != System.Data.ConnectionState.Open)
                    {
                        throw new System.InvalidOperationException("Transaction is provided, but its connection is null.");
                    }
                    
                    return await this.{{transactionProperty}}.Connection.QueryFirstOrDefaultAsync<{{returnType}}>(
                            {{sqlVar}}{{dapperArgs}},
                            transaction: this.{{transactionProperty}});
                 """;
    }

    private string GetDriverNoTxBody(string sqlVar, string returnInterface, Query query)
    {
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(query);
        var createSqlCommand = dbDriver.CreateSqlCommand(sqlVar);
        var commandParameters = CommonGen.AddParametersToCommand(query.Params);
        var initDataReader = CommonGen.InitDataReader();
        var awaitReaderRow = CommonGen.AwaitReaderRow();
        var returnDataclass = CommonGen.InstantiateDataclass(query.Columns.ToArray(), returnInterface, query);

        return $$"""
                    using ({{establishConnection}})
                    {
                        {{connectionOpen.AppendSemicolonUnlessEmpty()}}
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

    private string GetDriverWithTxBody(string sqlVar, string returnInterface, Query query)
    {
        var transactionProperty = Variable.Transaction.AsPropertyName();
        var commandVar = Variable.Command.AsVarName();
        var commandParameters = CommonGen.AddParametersToCommand(query.Params);
        var initDataReader = CommonGen.InitDataReader();
        var awaitReaderRow = CommonGen.AwaitReaderRow();
        var returnDataclass = CommonGen.InstantiateDataclass(query.Columns.ToArray(), returnInterface, query);

        return $$"""
                    if (this.{{transactionProperty}}?.Connection == null || this.{{transactionProperty}}?.Connection.State != System.Data.ConnectionState.Open)
                    {
                        throw new System.InvalidOperationException("Transaction is provided, but its connection is null.");
                    }

                    using (var {{commandVar}} = this.{{transactionProperty}}.Connection.CreateCommand())
                    {
                        {{commandVar}}.CommandText = {{sqlVar}};
                        {{commandVar}}.Transaction = this.{{transactionProperty}};
                        {{commandParameters}}
                        using ({{initDataReader}})
                        {
                            if ({{awaitReaderRow}})
                            {
                                return {{returnDataclass}};
                            }
                        }
                    }
                    return null;
                 """;
    }
}