using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static System.String;
namespace SqlcGenCsharp.Drivers;

public static class Utils
{
    public static ExpressionSyntax NullExpression()
    {
        return LiteralExpression(SyntaxKind.NullLiteralExpression);
    }

    public static MethodDeclarationSyntax WithPublicStaticAsyncModifiers(this MethodDeclarationSyntax me)
    {
        return me.AddModifiers(
        [
            Token(SyntaxKind.PublicKeyword),
            Token(SyntaxKind.StaticKeyword),
            Token(SyntaxKind.AsyncKeyword)
        ]);
    }

    public static StatementSyntax DeclareResultRowsVar(string identifier)
    {
        return LocalDeclarationStatement(
            VariableDeclaration(
                    IdentifierName("var")
                )
                .WithVariables(
                    SingletonSeparatedList(
                        VariableDeclarator(Identifier("rows"))
                            .WithInitializer(
                                EqualsValueClause(
                                    ObjectCreationExpression(ParseTypeName($"List<{identifier}>")
                                    ).WithArgumentList(ArgumentList())
                                )
                            )
                    )
                )
        );
    }

    public static LocalDeclarationStatementSyntax WithAwaitUsing(this LocalDeclarationStatementSyntax me)
    {
        return me
            .WithAwaitKeyword(Token(SyntaxKind.AwaitKeyword))
            .WithUsingKeyword(Token(SyntaxKind.UsingKeyword));
    }
    
    public static ExpressionSyntax AssignToColumn(ExpressionSyntax me, Column column)
    {
        return AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            IdentifierName(column.Name.FirstCharToUpper()),
            me
        );
    }
}