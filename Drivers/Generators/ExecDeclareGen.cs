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
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(query);
        return dbDriver.Options.UseDapper ? GetAsDapper() : GetAsDriver();

        string GetAsDapper()
        {
            var args = CommonGen.GetParameterListForDapper(query.Params);
            return $$"""
                        using ({{establishConnection}})
                        {
                            await connection.ExecuteAsync({{queryTextConstant}}{{args}});
                        }
                     """;
        }

        string GetAsDriver()
        {
            var commandParameters = CommonGen.GetCommandParameters(query.Params);
            var sqlcSliceSection = CommonGen.GetSqlSliceSection(query, queryTextConstant);
            var createSqlCommand = dbDriver.CreateSqlCommand(sqlcSliceSection!=string.Empty ? Variable.TransformSql.AsVarName() : queryTextConstant);
            var executeScalar = $"await {Variable.Command.AsVarName()}.ExecuteScalarAsync();";
            return $$"""
                     using ({{establishConnection}})
                     {
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}{{sqlcSliceSection}}
                         using ({{createSqlCommand}})
                         {
                             {{commandParameters.JoinByNewLine()}}
                             {{executeScalar}}
                         }
                     }
                     """;
        }
    }
}