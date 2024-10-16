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
        var establishConnection = dbDriver.EstablishConnection(query);
        var createSqlCommand = dbDriver.CreateSqlCommand(queryTextConstant);
        var commandParameters = CommonGen.GetCommandParameters(query.Params);
        var initDataReader = CommonGen.InitDataReader();
        var awaitReaderRow = CommonGen.AwaitReaderRow();
        var dataclassInit = CommonGen.InstantiateDataclass(query.Columns, returnInterface);
        var readWhileExists = $$"""
                                while ({{awaitReaderRow}})
                                {
                                    {{Variable.Result.Name()}}.Add({{dataclassInit}});
                                }
                                """;
        return dbDriver.DotnetFramework.LatestDotnetSupported() ? Get() : GetAsLegacy();

        string Get()
        {
            return $$"""
                     {
                         {{establishConnection[0].Generate(dbDriver.DotnetFramework, establishConnection.Skip(1).ToArray())}}
                         await using {{createSqlCommand}};
                         {{commandParameters.JoinByNewLine()}}
                         {{initDataReader}};
                         var {{Variable.Result.Name()}} = new List<{{returnInterface}}>();
                         {{readWhileExists}}
                         return {{Variable.Result.Name()}};
                     }
                     """;
        }

        string GetAsLegacy()
        {
            return $$"""
                     {
                         using ({{establishConnection}})
                         {
                             using ({{createSqlCommand}})
                             {
                                 {{commandParameters.JoinByNewLine()}}
                                 using ({{initDataReader}})
                                 {
                                     var {{Variable.Result.Name()}} = new List<{{returnInterface}}>();
                                     {{readWhileExists}}
                                     return {{Variable.Result.Name()}};
                                 }
                             }
                         }
                     }
                     """;
        }
    }
}