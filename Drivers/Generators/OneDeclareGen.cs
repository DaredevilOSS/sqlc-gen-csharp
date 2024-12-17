using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace SqlcGenCsharp.Drivers.Generators;

public class OneDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string queryTextConstant, string argInterface,
        string returnInterface, Query query, bool UseDapper = false)
    {
        var returnType = $"Task<{dbDriver.AddNullableSuffix(returnInterface, false)}>";
        var parametersStr = CommonGen.GetParameterListAsString(argInterface, query.Params);
        return ParseMemberDeclaration($$"""
                                        public async {{returnType}} {{query.Name}}({{parametersStr}})
                                        {
                                            {{GetMethodBody(queryTextConstant, returnInterface, query, UseDapper)}}
                                        }
                                        """)!;
    }

    private string GetMethodBody(string queryTextConstant, string returnInterface, Query query, bool useDapper)
    {
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(query, useDapper);
        var createSqlCommand = dbDriver.CreateSqlCommand(queryTextConstant);
        var commandParameters = CommonGen.GetCommandParameters(query.Params);
        var initDataReader = CommonGen.InitDataReader();
        var awaitReaderRow = CommonGen.AwaitReaderRow();
        var returnDataclass = CommonGen.InstantiateDataclass(query.Columns, returnInterface);

        if (useDapper)
        {
            return GetAsDapper();
        }

        return dbDriver.DotnetFramework.LatestDotnetSupported() ? Get() : GetAsLegacy();
        string GetAsDapper()
        {
            var argsParams = query.Params.Count > 0 ? $", args" : "";
            return $$"""
                        using ({{establishConnection}})
                        {
                            var result = await connection.QueryFirstOrDefaultAsync<{{dbDriver.AddNullableSuffix(returnInterface, false)}}>(
                            {{queryTextConstant}}{{argsParams}});
                            return result;
                        }
                     """;
        }
        string Get()
        {
            return $$"""
                     {
                         await using {{establishConnection}};
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}
                         await using {{createSqlCommand}};
                         {{commandParameters.JoinByNewLine()}}
                         {{initDataReader}};
                         if ({{awaitReaderRow}})
                         {
                             return {{returnDataclass}};
                         }
                         return null;
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
                                     if ({{awaitReaderRow}})
                                     {
                                         return {{returnDataclass}};
                                     }
                                 }
                             }
                         }
                         return null;
                     }
                     """;
        }
    }
}