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
            public async {{returnType}} {{query.Name.ToMethodName(dbDriver.Options.WithAsyncSuffix)}}({{parametersStr}})
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
        var connectionCommands = dbDriver.EstablishConnection(query);
        var dapperArgs = CommonGen.GetDapperArgs(query);
        var returnType = dbDriver.AddNullableSuffixIfNeeded(returnInterface, false);
        var runStatement = $$"""var {{Variable.Result.AsVarName()}} = await {{Variable.Connection.AsVarName()}}.QueryFirstOrDefaultAsync<{{returnType}}>({{sqlVar}}{{dapperArgs}});""";
        var blockStatement = $$"""
            {{connectionCommands.ConnectionOpen.AppendSemicolonUnlessEmpty()}}
            {{runStatement}}
            return {{Variable.Result.AsVarName()}};
        """;

        return CommonGen.ConditionallyWrapAsUsing(
            connectionCommands.EstablishConnection, 
            blockStatement, 
            connectionCommands.WrapInUsing
        );
    }

    private string GetDapperWithTxBody(string sqlVar, string returnInterface, Query query)
    {
        var transactionProperty = Variable.Transaction.AsPropertyName();
        var dapperArgs = CommonGen.GetDapperArgs(query);
        var returnType = dbDriver.AddNullableSuffixIfNeeded(returnInterface, false);

        return $$"""
            {{dbDriver.TransactionConnectionNullExcetionThrow}}
            return await this.{{transactionProperty}}.Connection.QueryFirstOrDefaultAsync<{{returnType}}>(
                {{sqlVar}}{{dapperArgs}},
                transaction: this.{{transactionProperty}});
        """;
    }

    private string GetDriverNoTxBody(string sqlVar, string returnInterface, Query query)
    {
        var connectionCommands = dbDriver.EstablishConnection(query);
        var returnDataclass = CommonGen.InstantiateDataclass([.. query.Columns], returnInterface, query);
        var blockStatement = $$"""
            {{connectionCommands.ConnectionOpen.AppendSemicolonUnlessEmpty()}}
            using ({{dbDriver.CreateSqlCommand(sqlVar)}})
            {
                {{dbDriver.AddParametersToCommand(query)}}
                using ({{CommonGen.InitDataReader()}})
                {
                    if ({{CommonGen.AwaitReaderRow()}})
                    {
                        return {{returnDataclass}};
                    }
                }
            }
            return null;
        """;

        return CommonGen.ConditionallyWrapAsUsing(
            connectionCommands.EstablishConnection, 
            blockStatement, 
            connectionCommands.WrapInUsing
        );
    }

    private string GetDriverWithTxBody(string sqlVar, string returnInterface, Query query)
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
                using ({{CommonGen.InitDataReader()}})
                {
                    if ({{CommonGen.AwaitReaderRow()}})
                    {
                        return {{CommonGen.InstantiateDataclass([.. query.Columns], returnInterface, query)}};
                    }
                }
            }
            return null;
        """;
    }
}