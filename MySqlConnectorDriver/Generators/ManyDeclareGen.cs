using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

internal static class ManyDeclareGen
{
    public static MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IEnumerable<Column> columns)
    {
        var methodDeclaration =
            MethodDeclaration(IdentifierName($"Task<List<{returnInterface}>>"), Identifier(funcName))
                .WithPublicAsync()
                .WithParameterList(ParseParameterList(Utils.GetParameterListAsString(argInterface, parameters)))
                .WithBody(GetMethodBody(queryTextConstant, returnInterface, columns, parameters));

        return methodDeclaration;
    }
    
    private static BlockSyntax GetMethodBody(string queryTextConstant, string returnInterface, 
        IEnumerable<Column> columns, IEnumerable<Parameter> parameters)
    {
        return Block(new[]
        {
            Utils.EstablishConnection(),
            Utils.PrepareSqlCommand(queryTextConstant, parameters),
            [
                Utils.UsingDataReader(),
                ParseStatement($"var {Variable.Rows.Name()} = new List<{returnInterface}>();"),
                GetWhileStatement(returnInterface, columns),
                ReturnStatement(IdentifierName(Variable.Rows.Name()))
            ]
        }.SelectMany(x => x));
    }

    private static StatementSyntax GetWhileStatement(string returnInterface, IEnumerable<Column> columns)
    {
        return WhileStatement(
            Utils.AwaitReaderRow(),
            Block(
                ExpressionStatement(
                    InvocationExpression(ParseExpression($"{Variable.Rows.Name()}.Add"))
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList(
                                    Argument(ObjectCreationExpression(IdentifierName(returnInterface))
                                        .WithInitializer(Utils.GetRecordInitExpression(columns))
                                    )
                                )
                            )
                        )
                )));
    }
}