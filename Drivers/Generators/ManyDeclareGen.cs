using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System.Collections.Generic;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace SqlcGenCsharp.Drivers.Generators;

public class ManyDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IEnumerable<Column> columns)
    {
        var returnType = $"Task<List<{returnInterface}>>";
        return ParseMemberDeclaration($$"""
                                        public async {{returnType}} {{funcName}}({{CommonGen.GetParameterListAsString(argInterface, parameters)}})
                                        {
                                            {{GetMethodBody(queryTextConstant, returnInterface, columns, parameters)}}
                                        }
                                        """)!;
    }

    private string GetMethodBody(string queryTextConstant, string returnInterface, IEnumerable<Column> columns,
        IEnumerable<Parameter> parameters)
    {
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection();
        var createSqlCommand = dbDriver.CreateSqlCommand(queryTextConstant);
        var commandParameters = CommonGen.GetCommandParameters(parameters);
        var initDataReader = CommonGen.InitDataReader();
        var awaitReaderRow = CommonGen.AwaitReaderRow();
        var readWhileExists = $$"""
                                while ({{awaitReaderRow}})
                                {
                                    {{Variable.Result.Name()}}.Add({{CommonGen.InstantiateDataclass(columns, returnInterface)}});
                                }
                                """;

        return dbDriver.DotnetFramework.LatestDotnetSupported()
            ? GetWithUsingAsStatement()
            : GetWithUsingAsBlock();

        string GetWithUsingAsStatement()
        {
            return $$"""
                     {
                         await using {{establishConnection}};
                         {{connectionOpen}};
                         await using {{createSqlCommand}};
                         {{commandParameters.JoinByNewLine()}}
                         {{initDataReader}};
                         var {{Variable.Result.Name()}} = new List<{{returnInterface}}>();
                         {{readWhileExists}}
                         return {{Variable.Result.Name()}};
                     }
                     """;
        }

        string GetWithUsingAsBlock()
        {
            return $$"""
                     {
                         using ({{establishConnection}})
                         {
                             {{connectionOpen}};
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