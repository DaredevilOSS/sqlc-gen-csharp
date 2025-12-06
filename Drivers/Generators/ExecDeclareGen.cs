using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class ExecDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string queryTextConstant, string argInterface, Query query)
    {
        var parametersStr = CommonGen.GetMethodParameterList(argInterface, query.Params);
        return ParseMemberDeclaration($$"""
            public async Task {{query.Name.ToMethodName(dbDriver.Options.WithAsyncSuffix)}}({{parametersStr}})
            {
                {{GetMethodBody(queryTextConstant, query)}}
            }
            """)!;
    }

    private string GetMethodBody(string queryTextConstant, Query query)
    {
        var sqlTextTransform = CommonGen.GetSqlTransformations(query, queryTextConstant);
        var useDapper = dbDriver.Options.UseDapper;

        var dapperParams = useDapper ? CommonGen.ConstructDapperParamsDict(query) : string.Empty;
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
        var connectionCommands = dbDriver.EstablishConnection(query);
        var dapperArgs = CommonGen.GetDapperArgs(query);
        return connectionCommands.GetConnectionOrDataSource.WrapBlock(
            $"""
            await {Variable.Connection.AsVarName()}.ExecuteAsync({sqlVar}{dapperArgs});
            return;
            """
        );
    }

    private string GetDapperWithTxBody(string sqlVar, Query query)
    {
        var transactionProperty = Variable.Transaction.AsPropertyName();
        var dapperArgs = CommonGen.GetDapperArgs(query);
        return $$"""
                    {{dbDriver.TransactionConnectionNullExcetionThrow}}
                    await this.{{transactionProperty}}.Connection.ExecuteAsync(
                            {{sqlVar}}{{dapperArgs}},
                            transaction: this.{{transactionProperty}});
                 """;
    }

    private string GetDriverNoTxBody(string sqlVar, Query query)
    {
        var connectionCommands = dbDriver.EstablishConnection(query);
        var sqlCommands = dbDriver.CreateSqlCommand(sqlVar);
        var commandBlock = sqlCommands.CommandCreation.WrapBlock(
            $"""
            {sqlCommands.SetCommandText.AppendSemicolonUnlessEmpty()}
            {dbDriver.AddParametersToCommand(query)}
            {sqlCommands.PrepareCommand.AppendSemicolonUnlessEmpty()}
            await {Variable.Command.AsVarName()}.ExecuteNonQueryAsync();
            """
        );
        return connectionCommands.GetConnectionOrDataSource.WrapBlock(
            $$"""
            {{connectionCommands.ConnectionOpen.AppendSemicolonUnlessEmpty()}}
            {{commandBlock}}
            return;
            """
        );
    }

    private string GetDriverWithTxBody(string sqlVar, Query query)
    {
        var transactionProperty = Variable.Transaction.AsPropertyName();
        var commandVar = Variable.Command.AsVarName();

        return $$"""
            {{dbDriver.TransactionConnectionNullExcetionThrow}}
            using (var {{commandVar}} = this.{{transactionProperty}}.Connection.CreateCommand())
            {
                {{commandVar}}.CommandText = {{sqlVar}};
                {{commandVar}}.Transaction = this.{{transactionProperty}};
                {{dbDriver.AddParametersToCommand(query)}}
                await {{commandVar}}.ExecuteNonQueryAsync();
            }
        """;
    }
}