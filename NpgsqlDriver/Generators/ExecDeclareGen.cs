using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

internal static class ExecDeclareGen
{
    public static MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        IList<Parameter> parameters)
    {
        var methodDeclaration = MethodDeclaration(IdentifierName("Task"), Identifier(funcName))
            .WithPublicAsync()
            .WithParameterList(ParseParameterList(NpgsqlDriver.Utils.GetParameterListAsString(argInterface, parameters)))
            .WithBody(Block(
                Array.Empty<StatementSyntax>()
                    .Concat(NpgsqlDriver.Utils.EstablishConnection())
                    .Concat(NpgsqlDriver.Utils.PrepareSqlCommand(queryTextConstant, parameters))
                    .Append(ParseStatement($"await {Variable.Command.Name()}.ExecuteScalarAsync();"))));
        return methodDeclaration;
    }
}