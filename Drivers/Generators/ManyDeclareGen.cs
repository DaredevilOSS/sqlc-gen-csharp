using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System;
using System.Collections.Generic;
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
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(query);
        return dbDriver.Options.UseDapper ? GetAsDapper() : GetAsDriver();

        string GetAsDapper()
        {
            var args = CommonGen.GetParameterListForDapper(query.Params);
            var returnType = dbDriver.AddNullableSuffix(returnInterface, true);
            return $$"""
                        using ({{establishConnection}})
                        {
                            var results = await connection.QueryAsync<{{returnType}}>({{queryTextConstant}}{{args}});
                            return results.AsList();
                        }
                     """;
        }

        string GetAsDriver()
        {
            var isSliceInQuery = query.Params.Any(p => p.Column.IsSqlcSlice);
            var sqlcSliceSection = "";
            if (isSliceInQuery)
            {
                var sqlcSliceCommands = new List<string>();
                var initVariable = $"var {Variable.TransformSql.AsVarName()} = {queryTextConstant};";
                sqlcSliceCommands.Add(initVariable);
                var sqlcSliceParams = query.Params.Where(p => p.Column.IsSqlcSlice);
                foreach (var sqlcSliceParam in sqlcSliceParams)
                {
                    var paramName = sqlcSliceParam.Column.Name;
                    var createArgsName = $"var {paramName.ToPascalCase()}Args = Enumerable.Range(0, args.{paramName.ToPascalCase()}.Length).Select(i => $\"@{{nameof(args.Ids)}}Arg{{i}}\").ToList();";
                    var sqlReplace = $"{Variable.TransformSql.AsVarName()} = {Variable.TransformSql.AsVarName()}.Replace(\"/*SLICE:{paramName}*/@{paramName}\", string.Join(\",\", {paramName.ToPascalCase()}Args));";
                    sqlcSliceCommands.Add(createArgsName);
                    sqlcSliceCommands.Add(sqlReplace);
                }
                sqlcSliceSection = Environment.NewLine + sqlcSliceCommands.JoinByNewLine();
            }
            var createSqlCommand = dbDriver.CreateSqlCommand(isSliceInQuery ? Variable.TransformSql.AsVarName() : queryTextConstant);
            var commandParameters = CommonGen.GetCommandParameters(query.Params);
            var initDataReader = CommonGen.InitDataReader();
            var awaitReaderRow = CommonGen.AwaitReaderRow();
            var dataclassInit = CommonGen.InstantiateDataclass(query.Columns, returnInterface);
            var readWhileExists = $$"""
                                    while ({{awaitReaderRow}})
                                    {
                                        {{Variable.Result.AsVarName()}}.Add({{dataclassInit}});
                                    }
                                    """;
            return $$"""
                     using ({{establishConnection}})
                     {
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}{{sqlcSliceSection}}
                         using ({{createSqlCommand}})
                         {
                             {{commandParameters.JoinByNewLine()}}
                             using ({{initDataReader}})
                             {
                                 var {{Variable.Result.AsVarName()}} = new List<{{returnInterface}}>();
                                 {{readWhileExists}}
                                 return {{Variable.Result.AsVarName()}};
                             }
                         }
                     }
                     """;
        }
    }
}