using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;

namespace SqlcGenCsharp.Drivers;

public interface IExecLastId
{
    MemberDeclarationSyntax ExecLastIdDeclare(string queryTextConstant, string argInterface, Query query);
}