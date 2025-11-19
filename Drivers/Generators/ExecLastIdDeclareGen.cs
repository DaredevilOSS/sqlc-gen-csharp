using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class ExecLastIdDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string queryTextConstant, string argInterface, Query query)
    {
        var parametersStr = CommonGen.GetMethodParameterList(argInterface, query.Params);
        return ParseMemberDeclaration($$"""
            public async Task<{{dbDriver.GetIdColumnType(query)}}> {{query.Name.ToMethodName(dbDriver.Options.WithAsyncSuffix)}}({{parametersStr}})
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
        var blockStatement = $$"""
            {{connectionCommands.ConnectionOpen.AppendSemicolonUnlessEmpty()}}
            return await {{Variable.Connection.AsVarName()}}.QuerySingleAsync<{{dbDriver.GetIdColumnType(query)}}>({{sqlVar}}{{dapperArgs}});
        """;
        return CommonGen.ConditionallyWrapAsUsing(
            connectionCommands.EstablishConnection, 
            blockStatement, 
            connectionCommands.WrapInUsing
        );
    }

    private string GetDapperWithTxBody(string sqlVar, Query query)
    {
        var transactionProperty = Variable.Transaction.AsPropertyName();
        var dapperArgs = query.Params.Any() ? $", {Variable.QueryParams.AsVarName()}" : string.Empty;
        return $$"""
                {{dbDriver.TransactionConnectionNullExcetionThrow}}
                return await this.{{transactionProperty}}.Connection.QuerySingleAsync<{{dbDriver.GetIdColumnType(query)}}>({{sqlVar}}{{dapperArgs}}, transaction: this.{{transactionProperty}});
                """;
    }

    private string GetDriverNoTxBody(string sqlVar, Query query)
    {
        var connectionCommands = dbDriver.EstablishConnection(query);
        var returnLastId = ((IExecLastId)dbDriver).GetLastIdStatement(query).JoinByNewLine();
        var blockStatement = $$"""
                        {{connectionCommands.ConnectionOpen.AppendSemicolonUnlessEmpty()}}
                        using ({{dbDriver.CreateSqlCommand(sqlVar)}})
                        {
                            {{dbDriver.AddParametersToCommand(query)}}
                            {{returnLastId}}
                        }
                        """;
        
        return CommonGen.ConditionallyWrapAsUsing(
            connectionCommands.EstablishConnection, 
            blockStatement, 
            connectionCommands.WrapInUsing
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
                {{((IExecLastId)dbDriver).GetLastIdStatement(query).JoinByNewLine()}}
            }
        """;
    }
}