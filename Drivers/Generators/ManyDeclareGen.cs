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
        var asyncEnumerable = dbDriver.Options.UseAsyncEnumerable;
        var cancellationParam = dbDriver.Cancellation.MethodParameter();
        if (asyncEnumerable && cancellationParam != string.Empty)
            cancellationParam = $"[EnumeratorCancellation] {cancellationParam}";
        var parametersStr = CommonGen.GetMethodParameterList(argInterface, query.Params, cancellationParam);
        var returnType = asyncEnumerable ? $"IAsyncEnumerable<{returnInterface}>" : $"Task<List<{returnInterface}>>";
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

        if (dbDriver.Options.UseAsyncEnumerable)
            return GetAsyncEnumerableBody(sqlTextTransform, dapperParams, sqlVar, returnInterface, query, useDapper);

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

    // Streaming variant: rows are yielded as they are read, no intermediate List is built.
    private string GetAsyncEnumerableBody(string sqlTextTransform, string dapperParams, string sqlVar,
        string returnInterface, Query query, bool useDapper)
    {
        var transactionProperty = Variable.Transaction.AsPropertyName();
        var noTxBody = useDapper ? GetDapperStreamNoTxBody(sqlVar, returnInterface, query) : GetDriverStreamNoTxBody(sqlVar, returnInterface, query);
        var withTxBody = useDapper ? GetDapperStreamWithTxBody(sqlVar, returnInterface, query) : GetDriverStreamWithTxBody(sqlVar, returnInterface, query);
        return $$"""
            {{sqlTextTransform}}
            {{dapperParams}}
            if (this.{{transactionProperty}} == null)
            {
                {{noTxBody}}
                yield break;
            }
            {{withTxBody}}
        """;
    }

    private string GetDapperStreamNoTxBody(string sqlVar, string returnInterface, Query query)
    {
        var connectionCommands = dbDriver.EstablishConnection(query);
        var dapperArgs = CommonGen.GetDapperArgs(query);
        var returnType = dbDriver.AddNullableSuffixIfNeeded(returnInterface, true);
        return connectionCommands.GetConnectionOrDataSource.WrapBlock(
            $"""
            await foreach (var row in {Variable.Connection.AsVarName()}.QueryUnbufferedAsync<{returnType}>({sqlVar}{dapperArgs}){dbDriver.Cancellation.EnumerationSuffix()})
                yield return row;
            """
        );
    }

    private string GetDapperStreamWithTxBody(string sqlVar, string returnInterface, Query query)
    {
        var transactionProperty = Variable.Transaction.AsPropertyName();
        var returnType = dbDriver.AddNullableSuffixIfNeeded(returnInterface, true);
        var queryParams = query.Params.Count == 0 ? "null" : Variable.QueryParams.AsVarName();
        return $$"""
            {{dbDriver.TransactionConnectionNullExcetionThrow}}
            await foreach (var row in this.{{transactionProperty}}.Connection.QueryUnbufferedAsync<{{returnType}}>({{sqlVar}}, {{queryParams}}, transaction: this.{{transactionProperty}}){{dbDriver.Cancellation.EnumerationSuffix()}})
                yield return row;
        """;
    }

    private string GetDriverStreamNoTxBody(string sqlVar, string returnInterface, Query query)
    {
        var connectionCommands = dbDriver.EstablishConnection(query);
        var dataclassInit = CommonGen.InstantiateDataclass([.. query.Columns], returnInterface, query);
        var sqlCommands = dbDriver.CreateSqlCommand(sqlVar);
        var commandBlock = sqlCommands.CommandCreation.WrapBlock(
            $$"""
            {{sqlCommands.SetCommandText.AppendSemicolonUnlessEmpty()}}
            {{dbDriver.AddParametersToCommand(query)}}
            {{sqlCommands.PrepareCommand.AppendSemicolonUnlessEmpty()}}
            using ({{CommonGen.InitDataReader(dbDriver.Cancellation.Argument())}})
            {
                while ({{CommonGen.AwaitReaderRow(dbDriver.Cancellation.Argument())}})
                    yield return {{dataclassInit}};
            }
            """
        );
        return connectionCommands.GetConnectionOrDataSource.WrapBlock(
            $$"""
            {{connectionCommands.ConnectionOpen.AppendSemicolonUnlessEmpty()}}
            {{commandBlock}}
            """
        );
    }

    private string GetDriverStreamWithTxBody(string sqlVar, string returnInterface, Query query)
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
                using ({{CommonGen.InitDataReader(dbDriver.Cancellation.Argument())}})
                {
                    while ({{CommonGen.AwaitReaderRow(dbDriver.Cancellation.Argument())}})
                        yield return {{CommonGen.InstantiateDataclass([.. query.Columns], returnInterface, query)}};
                }
            }
        """;
    }

    private string GetDapperNoTxBody(string sqlVar, string returnInterface, Query query)
    {
        var connectionCommands = dbDriver.EstablishConnection(query);
        var dapperArgs = CommonGen.GetDapperArgs(query);
        var returnType = dbDriver.AddNullableSuffixIfNeeded(returnInterface, true);
        var resultVar = Variable.Result.AsVarName();
        var callArgs = dbDriver.Cancellation.WrapDapperArgs($"{sqlVar}{dapperArgs}");
        return connectionCommands.GetConnectionOrDataSource.WrapBlock(
            $"""
            var {resultVar} = await {Variable.Connection.AsVarName()}.QueryAsync<{returnType}>({callArgs});
            return {resultVar}.AsList();
            """
        );
    }

    private string GetDapperWithTxBody(string sqlVar, string returnInterface, Query query)
    {
        var transactionProperty = Variable.Transaction.AsPropertyName();
        var dapperArgs = CommonGen.GetDapperArgs(query);
        var returnType = dbDriver.AddNullableSuffixIfNeeded(returnInterface, true);

        var callArgs = dbDriver.Cancellation.WrapDapperArgs($"{sqlVar}{dapperArgs}, transaction: this.{transactionProperty}");
        return $$"""
            {{dbDriver.TransactionConnectionNullExcetionThrow}}
            return (await this.{{transactionProperty}}.Connection.QueryAsync<{{returnType}}>({{callArgs}})).AsList();
        """;
    }

    private string GetDriverNoTxBody(string sqlVar, string returnInterface, Query query)
    {
        var connectionCommands = dbDriver.EstablishConnection(query);
        var dataclassInit = CommonGen.InstantiateDataclass([.. query.Columns], returnInterface, query);
        var resultVar = Variable.Result.AsVarName();
        var readWhileExists = $$"""
            while ({{CommonGen.AwaitReaderRow(dbDriver.Cancellation.Argument())}})
                {{resultVar}}.Add({{dataclassInit}});
        """;
        var sqlCommands = dbDriver.CreateSqlCommand(sqlVar);
        var commandBlock = sqlCommands.CommandCreation.WrapBlock(
            $$"""
            {{sqlCommands.SetCommandText.AppendSemicolonUnlessEmpty()}}
            {{dbDriver.AddParametersToCommand(query)}}
            {{sqlCommands.PrepareCommand.AppendSemicolonUnlessEmpty()}}
            using ({{CommonGen.InitDataReader(dbDriver.Cancellation.Argument())}})
            {
                var {{resultVar}} = new List<{{returnInterface}}>();
                {{readWhileExists}}
                return {{resultVar}};
            }
            """
        );
        return connectionCommands.GetConnectionOrDataSource.WrapBlock(
            $$"""
            {{connectionCommands.ConnectionOpen.AppendSemicolonUnlessEmpty()}}
            {{commandBlock}}
            """
        );
    }

    private string GetDriverWithTxBody(string sqlVar, string returnInterface, Query query)
    {
        var transactionProperty = Variable.Transaction.AsPropertyName();
        var commandVar = Variable.Command.AsVarName();
        var resultVar = Variable.Result.AsVarName();

        return $$"""
            {{dbDriver.TransactionConnectionNullExcetionThrow}}
            using (var {{commandVar}} = this.{{transactionProperty}}.Connection.CreateCommand())
            {
                {{commandVar}}.CommandText = {{sqlVar}};
                {{commandVar}}.Transaction = this.{{transactionProperty}};
                {{dbDriver.AddParametersToCommand(query)}}
                using ({{CommonGen.InitDataReader(dbDriver.Cancellation.Argument())}})
                {
                    var {{resultVar}} = new List<{{returnInterface}}>();
                    while ({{CommonGen.AwaitReaderRow(dbDriver.Cancellation.Argument())}})
                        {{resultVar}}.Add({{CommonGen.InstantiateDataclass([.. query.Columns], returnInterface, query)}});
                    return {{resultVar}};
                }
            }
        """;
    }
}