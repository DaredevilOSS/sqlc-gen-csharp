using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class CopyFromDeclareGen(DbDriver dbDriver)
{
    public MemberDeclarationSyntax Generate(string queryTextConstant, string argInterface, Query query)
    {
        return ParseMemberDeclaration($$"""
                                        public async Task {{query.Name}}(List<{{argInterface}}> args)
                                        {
                                            {{GetMethodBody(queryTextConstant, query)}}
                                        }
                                        """)!;
    }


    private string GetMethodBody(string queryTextConstant, Query query)
    {
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(query);
        var beginBinaryImport = $"{Variable.Connection.Name()}.BeginBinaryImportAsync({queryTextConstant}";
        var addRowsToCopyCommand = AddRowsToCopyCommand(query);

        if (dbDriver.Options.DotnetFramework.LatestDotnetSupported())
            return GetAsLatest();
        return GetAsLegacy();

        string GetAsLatest()
        {
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

        string GetAsLegacy()
        {
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
    }

    private string AddRowsToCopyCommand(Query query)
    {
        var constructRow = new List<string>()
            .Append($"await {Variable.Writer.Name()}.StartRowAsync();")
            .Concat(query.Params
                .Select(p =>
                {
                    var typeOverride = dbDriver.GetColumnDbTypeOverride(p.Column);
                    var partialStmt =
                        $"await {Variable.Writer.Name()}.WriteAsync({Variable.Row.Name()}.{p.Column.Name.FirstCharToUpper()}";
                    return typeOverride is null
                        ? $"{partialStmt});"
                        : $"{partialStmt}, {typeOverride});";
                }))
            .JoinByNewLine();
        return $$"""
                 foreach (var {{Variable.Row.Name()}} in args) 
                 {
                      {{constructRow}}
                 }
                 """;
    }
}