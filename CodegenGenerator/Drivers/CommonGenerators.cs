using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public static class CommonGenerators
{
    public static ExpressionStatementSyntax ConnectionOpen()
    {
        return ExpressionStatement(
            InvocationExpression(MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName("connection"),
                IdentifierName("Open"))));
    }

    public static ReturnStatementSyntax ReturnNull()
    {
        return ReturnStatement(LiteralExpression(SyntaxKind.NullLiteralExpression));
    }

    public static SyntaxToken[] GetQueryMethodsTokens()
    {
        return
        [
            Token(SyntaxKind.PublicKeyword),
            Token(SyntaxKind.StaticKeyword),
            Token(SyntaxKind.AsyncKeyword)
        ];
    }

    public static StatementSyntax DeclareResultRowsVar(string returnInterface)
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
                                                TypeArgumentList(
                                                    SingletonSeparatedList<TypeSyntax>(
                                                        IdentifierName(returnInterface)
                                                    )
                                                )
                                            )
                                    ).WithArgumentList(ArgumentList())
                                )
                            )
                    )
                )
        );
    }

    public static StatementSyntax ReturnRowsVar()
    {
        return ReturnStatement(IdentifierName("rows"));
    }

    public static LocalDeclarationStatementSyntax WithAwaitUsing(this LocalDeclarationStatementSyntax me)
    {
        return me
            .WithAwaitKeyword(Token(SyntaxKind.AwaitKeyword))
            .WithUsingKeyword(Token(SyntaxKind.UsingKeyword));
    }

    public static TypeArgumentListSyntax GetGenericListOfInputType(string inputInterface)
    {
        return TypeArgumentList(
            SingletonSeparatedList<TypeSyntax>(
                NullableType(IdentifierName(inputInterface))));
    }
}