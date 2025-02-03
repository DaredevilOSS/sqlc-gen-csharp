using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System;
using System.Collections.Generic;
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
                                        public async Task<{{dbDriver.GetIdColumnType()}}> {{query.Name}}({{parametersStr}})
                                        {
                                            {{GetMethodBody(queryTextConstant, query)}}
                                        }
                                        """)!;
    }

    private string GetMethodBody(string queryTextConstant, Query query)
    {
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(query);
        connectionOpen = connectionOpen.AppendSemicolonUnlessEmpty();
        return dbDriver.Options.UseDapper ? GetAsDapper() : GetAsDriver();

        string GetAsDapper()
        {
            var args = CommonGen.GetParameterListForDapper(query.Params);
            return $$"""
                     using ({{establishConnection}})
                     {
                        return await connection.QuerySingleAsync<{{dbDriver.GetIdColumnType()}}>({{queryTextConstant}}{{args}});
                     }
                     """;
        }

        string GetAsDriver()
        {
            var sqlcSliceSection = CommonGen.GetSqlSliceSection(query, queryTextConstant);
            var createSqlCommand = dbDriver.CreateSqlCommand(sqlcSliceSection!=string.Empty ? Variable.TransformSql.AsVarName() : queryTextConstant);
            var commandParameters = CommonGen.GetCommandParameters(query.Params).JoinByNewLine();
            var returnLastId = ((IExecLastId)dbDriver).GetLastIdStatement().JoinByNewLine();
            return $$"""
                     using ({{establishConnection}})
                     {
                         {{connectionOpen}}{{sqlcSliceSection}}
                         using ({{createSqlCommand}})
                         {
                            {{commandParameters}}
                            {{returnLastId}}
                         }
                     }
                     """;
        }
    }
}