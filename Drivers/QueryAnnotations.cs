using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;

namespace SqlcGenCsharp.Drivers;

public interface IExecLastId
{
    MemberDeclarationSyntax ExecLastIdDeclare(string queryTextConstant, string argInterface, Query query);
}

public interface ICopyFrom
{
    MemberDeclarationSyntax CopyFromDeclare(string queryTextConstant, string argInterface, Query query);
}

public interface IExecRows
{
    MemberDeclarationSyntax ExecRowsDeclare(string queryTextConstant, string argInterface, Query query);
}