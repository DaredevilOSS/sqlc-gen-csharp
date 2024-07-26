using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class CopyFromDeclareGen(DbDriver dbDriver)
{
    public MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface, Query query)
    {
        return ParseMemberDeclaration($$"""
                                        public async Task {{funcName}}(List<{{argInterface}}> args)
                                        {
                                            {{GetMethodBody(queryTextConstant, query)}}
                                        }
                                        """)!;
    }

    private string GetMethodBody(string queryTextConstant, Query query)
    {
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(true);
        var beginBinaryImport = $"{Variable.Connection.Name()}.BeginBinaryImportAsync({queryTextConstant}";

        return dbDriver.DotnetFramework.LatestDotnetSupported()
            ? GetAsModernDotnet()
            : GetAsLegacyDotnet();

        string GetAsModernDotnet()
        {
            var addRowsToCopyCommand = AddRowsToCopyCommand();
            return $$"""
                     {
                         await using {{establishConnection}};
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}
                         await {{Variable.Connection.Name()}}.OpenAsync();
                         await using var {{Variable.Writer.Name()}} = await {{beginBinaryImport}});
                         {{addRowsToCopyCommand}}
                         await {{Variable.Writer.Name()}}.CompleteAsync();
                         await {{Variable.Connection.Name()}}.CloseAsync();
                     }
                     """;
        }

        string GetAsLegacyDotnet()
        {
            var addRowsToCopyCommand = AddRowsToCopyCommand();
            return $$"""
                     {
                         using ({{establishConnection}})
                         {
                             {{connectionOpen.AppendSemicolonUnlessEmpty()}}
                             await {{Variable.Connection.Name()}}.OpenAsync();
                             using (var {{Variable.Writer.Name()}} = await {{beginBinaryImport}}))
                             {
                                {{addRowsToCopyCommand}}
                                await {{Variable.Writer.Name()}}.CompleteAsync();
                             }
                             await {{Variable.Connection.Name()}}.CloseAsync();
                         }
                     }
                     """;
        }

        string AddRowsToCopyCommand()
        {
            var constructRow = new List<string>()
                    .Append($"await {Variable.Writer.Name()}.StartRowAsync();")
                    .Concat(query.Params.Select(p =>
                            $"await {Variable.Writer.Name()}.WriteAsync({Variable.Row.Name()}.{p.Column.Name.FirstCharToUpper()});"))
                    .JoinByNewLine();
            return $$"""
                   foreach (var {{Variable.Row.Name()}} in args) 
                   {
                        {{constructRow}}
                   }
                   """;
        }
    }
}