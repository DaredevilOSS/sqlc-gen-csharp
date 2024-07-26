using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;

namespace SqlcGenCsharp.Drivers;

public interface ICopyFrom
{
    MemberDeclarationSyntax CopyFromDeclare(string queryTextConstant, string argInterface, Query query);
}