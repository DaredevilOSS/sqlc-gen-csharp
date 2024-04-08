using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SqlcGenCsharp;

public static class MemberDeclarationExtensions
{
    public static MemberDeclarationSyntax AppendNewLine(this MemberDeclarationSyntax member)
    {
        var trailingTrivia = member.GetTrailingTrivia();
        trailingTrivia = trailingTrivia.Add(SyntaxFactory.CarriageReturnLineFeed);
        return member.WithTrailingTrivia(trailingTrivia);
    }
}