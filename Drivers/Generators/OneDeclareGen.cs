using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace SqlcGenCsharp.Drivers.Generators;

public class OneDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string queryTextConstant, string argInterface,
        string returnInterface, Query query)
    {
        var returnType = $"Task<{dbDriver.AddNullableSuffix(returnInterface, false)}>";
        var parametersStr = CommonGen.GetParameterListAsString(argInterface, query.Params);
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
        var returnDataclass = CommonGen.InstantiateDataclass(query.Columns, returnInterface);

        return dbDriver.DotnetFramework.LatestDotnetSupported()
            ? GetWithUsingAsStatement()
            : GetWithUsingAsBlock();

        string GetWithUsingAsStatement()
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

        string GetWithUsingAsBlock()
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