using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

internal static class ExecDeclareGen
{
    public static MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        IList<Parameter> parameters)
    {
        var methodDeclaration = MethodDeclaration(IdentifierName("Task"), Identifier(funcName))
            .WithPublicAsync()
            .WithParameterList(ParseParameterList(Utils.GetParameterListAsString(argInterface, parameters)))
            .WithBody(Block(
                Array.Empty<StatementSyntax>()
                    .Concat(Utils.EstablishConnection())
                    .Concat(Utils.PrepareSqlCommand(queryTextConstant, parameters))
                    .Append(ParseStatement($"await {Variable.Command.Name()}.ExecuteScalarAsync();"))));
        return methodDeclaration;
    }
}