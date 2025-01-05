using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;

namespace SqlcGenCsharp.Drivers;

public interface IOne
{
    MemberDeclarationSyntax OneDeclare(string queryTextConstant, string argInterface, string returnInterface, Query query);
}
public interface IMany
{
    MemberDeclarationSyntax ManyDeclare(string queryTextConstant, string argInterface, string returnInterface, Query query);
}
public interface IExec
{
    MemberDeclarationSyntax ExecDeclare(string queryTextConstant, string argInterface, Query query);
}

public interface IExecLastId
{
    MemberDeclarationSyntax ExecLastIdDeclare(string queryTextConstant, string argInterface, Query query);

    string[] GetLastIdStatement();
}

public interface ICopyFrom
{
    MemberDeclarationSyntax CopyFromDeclare(string queryTextConstant, string argInterface, Query query);
}

public interface IExecRows
{
    MemberDeclarationSyntax ExecRowsDeclare(string queryTextConstant, string argInterface, Query query);
}