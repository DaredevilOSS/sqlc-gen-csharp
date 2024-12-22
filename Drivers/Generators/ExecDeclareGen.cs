using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class ExecDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string queryTextConstant, string argInterface, Query query)
    {
        var parametersStr = CommonGen.GetParameterListAsString(argInterface, query.Params);
        return ParseMemberDeclaration($$"""
                                        public async Task {{query.Name}}({{parametersStr}})
                                        {
                                            {{GetMethodBody(queryTextConstant, query)}}
                                        }
                                        """)!;
    }

    private string GetMethodBody(string queryTextConstant, Query query)
    {
        var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(query);
        var createSqlCommand = dbDriver.CreateSqlCommand(queryTextConstant);
        var commandParameters = CommonGen.GetCommandParameters(query.Params);
        var executeScalar = $"await {Variable.Command.AsVarName()}.ExecuteScalarAsync();";

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
                            await connection.ExecuteAsync({{queryTextConstant}}{{argsParams}});
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
                         {{executeScalar}}
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
                                 {{executeScalar}}
                             }
                         }
                     }
                     """;
        }
    }
}