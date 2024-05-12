using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace SqlcGenCsharp.Drivers.Generators;

public class OneDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IEnumerable<Column> columns)
    {
        var returnType = $"Task<{dbDriver.AddNullableSuffix(returnInterface, false)}>";
        return ParseMemberDeclaration($$"""
            public async {{returnType}} {{funcName}}({{CommonGen.GetParameterListAsString(argInterface, parameters)}})
            {
                {{GetMethodBody(queryTextConstant, returnInterface, columns, parameters)}}
            }
            """)!;
    }

    private string GetMethodBody(string queryTextConstant, string returnInterface,
        IEnumerable<Column> columns, IEnumerable<Parameter> parameters)
    {
        var establishConnection = dbDriver.EstablishConnection();
        var connectionOpen = establishConnection.Length == 2 ? establishConnection[1] + ";" : string.Empty;
        var createSqlCommand = dbDriver.CreateSqlCommand(queryTextConstant);
        var commandParameters = CommonGen.GetCommandParameters(parameters);
        var initDataReader = CommonGen.InitDataReader();
        var awaitReaderRow = CommonGen.AwaitReaderRow();
        var returnDataclass = CommonGen.InstantiateDataclass(columns, returnInterface);

        return dbDriver.DotnetFramework.UsingStatementEnabled()
            ? GetWithUsingAsStatement()
            : GetWithUsingAsBlock();

        string GetWithUsingAsStatement()
        {
            return $$"""
             {
                 await using {{establishConnection[0]}};
                 {{connectionOpen}}
                 await using {{createSqlCommand}};
                 {{string.Join("\n", commandParameters)}}
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
                 using ({{establishConnection[0]}})
                 {
                     {{connectionOpen}}
                     using ({{createSqlCommand}})
                     {
                        {{string.Join("\n", commandParameters)}}
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