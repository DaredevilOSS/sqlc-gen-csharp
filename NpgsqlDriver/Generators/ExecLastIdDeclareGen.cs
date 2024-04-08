using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

internal static class ExecLastIdDeclareGen
{
    public static MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        var methodDeclaration = MethodDeclaration(IdentifierName("Task<long>"), Identifier(funcName))
            .WithPublicAsync()
            .WithParameterList(ParseParameterList(NpgsqlDriver.Utils.GetParameterListAsString(argInterface, parameters)))
            .WithBody(Block(
                Array.Empty<StatementSyntax>()
                    .Concat(NpgsqlDriver.Utils.EstablishConnection())
                    .Concat(NpgsqlDriver.Utils.PrepareSqlCommand(queryTextConstant, parameters))
                    .Append(ParseStatement($"await {Variable.Command.Name()}.ExecuteNonQueryAsync();"))
                    .Append(ParseStatement($"return {Variable.Command.Name()}.LastInsertedId;"))));
        return methodDeclaration;
    }
}