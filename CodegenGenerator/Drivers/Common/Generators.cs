using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Common;

public static class Generators
{
    public static ExpressionSyntax NullExpression()
    {
        return LiteralExpression(SyntaxKind.NullLiteralExpression);
    }

    public static SyntaxToken[] GetSyntaxTokens()
    {
        return 
        [
            Token(SyntaxKind.PublicKeyword),
            Token(SyntaxKind.StaticKeyword),
            Token(SyntaxKind.AsyncKeyword)
        ];
    }

    public static StatementSyntax DeclareResultRowsVar(IdentifierNameSyntax identifier)
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
                                    ObjectCreationExpression(
                                        GenericName(Identifier("List"))
                                            .WithTypeArgumentList(
                                                identifier.GetGenericListOf()
                                            )
                                    ).WithArgumentList(ArgumentList())
                                )
                            )
                    )
                )
        );
    }

    public static ReturnStatementSyntax Return(this IdentifierNameSyntax me)
    {
        return ReturnStatement(me);
    }

    public static LocalDeclarationStatementSyntax WithAwaitUsing(this LocalDeclarationStatementSyntax me)
    {
        return me
            .WithAwaitKeyword(Token(SyntaxKind.AwaitKeyword))
            .WithUsingKeyword(Token(SyntaxKind.UsingKeyword));
    }

    public static TypeArgumentListSyntax GetGenericListOf(this IdentifierNameSyntax me)
    {
        return TypeArgumentList(
            SingletonSeparatedList<TypeSyntax>(
                NullableType(me)));
    }
    
    public static ExpressionSyntax ColumnAssignment(ExpressionSyntax assignmentValue, Column column)
    {
        return AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            IdentifierName(column.Name.FirstCharToUpper()),
            assignmentValue
        );
    }
}