using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public static class Utils
{
    public static MethodDeclarationSyntax WithPublicStaticAsync(this MethodDeclarationSyntax me)
    {
        return me.AddModifiers(
        [
            Token(SyntaxKind.PublicKeyword),
            Token(SyntaxKind.StaticKeyword),
            Token(SyntaxKind.AsyncKeyword)
        ]);
    }

    public static LocalDeclarationStatementSyntax WithAwaitUsing(this LocalDeclarationStatementSyntax me)
    {
        return me
            .WithAwaitKeyword(Token(SyntaxKind.AwaitKeyword))
            .WithUsingKeyword(Token(SyntaxKind.UsingKeyword));
    }
    
    public static ExpressionSyntax AssignTo(this ExpressionSyntax assignmentValue, string assignmentVar)
    {
        return AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            IdentifierName(assignmentVar),
            assignmentValue
        );
    }
}