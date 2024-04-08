using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;

namespace SqlcGenCsharp.Drivers;

public interface IDbDriver
{
    string ColumnType(string columnType, bool notNull);

    (UsingDirectiveSyntax[], MemberDeclarationSyntax[]) Preamble();

    MemberDeclarationSyntax OneDeclare(string name, string sqlTextConstant,
        string argInterface, string returnInterface,
        IList<Parameter> parameters, IList<Column> columns);

    MemberDeclarationSyntax ManyDeclare(string funcName, string sqlTextConstant,
        string argInterface, string returnInterface,
        IList<Parameter> parameters, IEnumerable<Column> columns);

    MemberDeclarationSyntax ExecDeclare(string funcName, string text, string argInterface,
        IList<Parameter> parameters);

    MemberDeclarationSyntax ExecLastIdDeclare(string funcName, string queryTextConstant,
        string argInterface, string returnInterface, IList<Parameter> parameters, IList<Column> columns);
}