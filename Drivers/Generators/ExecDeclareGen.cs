using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class ExecDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string queryTextConstant, string argInterface, Query query)
    {
        var parametersStr = CommonGen.GetMethodParameterList(argInterface, query.Params);
        return ParseMemberDeclaration($$"""
            public async Task {{query.Name}}({{parametersStr}})
            {
                {{GetMethodBody(queryTextConstant, query)}}
            }
            """)!;
    }

    private string GetMethodBody(string queryTextConstant, Query query)
    {
        var sqlTextTransform = CommonGen.GetSqlTransformations(query, queryTextConstant);
        var useDapper = dbDriver.Options.UseDapper;

        var dapperParams = useDapper ? CommonGen.ConstructDapperParamsDict(query.Params) : string.Empty;
        var sqlVar = sqlTextTransform != string.Empty ? Variable.TransformedSql.AsVarName() : queryTextConstant;
        var transactionProperty = Variable.Transaction.AsPropertyName();

        var noTxBody = useDapper ? GetDapperNoTxBody(sqlVar, query) : GetDriverNoTxBody(sqlVar, query);
        var withTxBody = useDapper ? GetDapperWithTxBody(sqlVar, query) : GetDriverWithTxBody(sqlVar, query);

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

    private string GetDapperNoTxBody(string sqlVar, Query query)
    {
        var (establishConnection, _) = dbDriver.EstablishConnection(query);
        var dapperArgs = query.Params.Any() ? $", {Variable.QueryParams.AsVarName()}" : string.Empty;
        return $$"""
                    using ({{establishConnection}})
                    {
                        await {{Variable.Connection.AsVarName()}}.ExecuteAsync({{sqlVar}}{{dapperArgs}});
                    }
                    return;
                 """;
    }

    private string GetDapperWithTxBody(string sqlVar, Query query)
    {
        var transactionProperty = Variable.Transaction.AsPropertyName();
        var dapperArgs = query.Params.Any() ? $", {Variable.QueryParams.AsVarName()}" : string.Empty;
        return $$"""
                    if (this.{{transactionProperty}}?.Connection == null || this.{{transactionProperty}}?.Connection.State != System.Data.ConnectionState.Open)
                    {
                        throw new System.InvalidOperationException("Transaction is provided, but its connection is null.");
                    }
                    
                    await this.{{transactionProperty}}.Connection.ExecuteAsync(
                            {{sqlVar}}{{dapperArgs}},
                            transaction: this.{{transactionProperty}});
                 """;
    }

    private string GetDriverNoTxBody(string sqlVar, Query query)
    {
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(query);
        var createSqlCommand = dbDriver.CreateSqlCommand(sqlVar);
        var commandParameters = CommonGen.AddParametersToCommand(query.Params);
        return $$"""
                    using ({{establishConnection}})
                    {
                        {{connectionOpen.AppendSemicolonUnlessEmpty()}}
                        using ({{createSqlCommand}})
                        {
                            {{commandParameters}}
                            await {{Variable.Command.AsVarName()}}.ExecuteNonQueryAsync();
                        }
                    }
                    return;
                 """;
    }

    private string GetDriverWithTxBody(string sqlVar, Query query)
    {
        var transactionProperty = Variable.Transaction.AsPropertyName();
        var commandVar = Variable.Command.AsVarName();
        var commandParameters = CommonGen.AddParametersToCommand(query.Params);

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
                        await {{commandVar}}.ExecuteNonQueryAsync();
                    }
                 """;
    }
}