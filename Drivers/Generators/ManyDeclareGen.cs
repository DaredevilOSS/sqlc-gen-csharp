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
        var parametersStr = CommonGen.GetParameterListAsString(argInterface, query.Params);
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
        var createSqlCommand = dbDriver.CreateSqlCommand(queryTextConstant);
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

        if (dbDriver.Options.UseDapper)
            return GetAsDapper();
        if (dbDriver.Options.DotnetFramework.LatestDotnetSupported())
            return GetAsLatest();
        return GetAsLegacy();

        string GetAsDapper()
        {
            var argsParams = query.Params.Count > 0 ? ", new { " + string.Join(", ", query.Params.Select(p => p.Column.Name + "=args." + p.Column.Name.ToPascalCase() + "")) + "}" : "";
            return $$"""
                        using ({{establishConnection}})
                        {
                            var results = await connection.QueryAsync<{{dbDriver.AddNullableSuffix(returnInterface, true)}}>(
                            {{queryTextConstant}}{{argsParams}});
                            return results.AsList();
                        }
                     """;
        }

        string GetAsLatest()
        {
            return $$"""
                     {
                         await using {{establishConnection}};
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}
                         await using {{createSqlCommand}};
                         {{commandParameters.JoinByNewLine()}}
                         {{initDataReader}};
                         var {{Variable.Result.AsVarName()}} = new List<{{returnInterface}}>();
                         {{readWhileExists}}
                         return {{Variable.Result.AsVarName()}};
                     }
                     """;
        }

        string GetAsLegacy()
        {
            return $$"""
                     {
                         using ({{establishConnection}})
                         {
                             {{connectionOpen.AppendSemicolonUnlessEmpty()}}
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
                     }
                     """;
        }
    }
}