using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp;

public static class Utils
{
    public static MethodDeclarationSyntax WithPublicAsync(this MethodDeclarationSyntax me)
    {
        return me.AddModifiers(
        [
            Token(SyntaxKind.PublicKeyword),
            Token(SyntaxKind.AsyncKeyword)
        ]);
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