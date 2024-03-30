using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;

namespace SqlcGenCsharp.Drivers;

public interface IDbDriver
{
    TypeSyntax ColumnType(string columnType, bool notNull);

    (UsingDirectiveSyntax[], MemberDeclarationSyntax[]) Preamble(Query[] queries);

    MemberDeclarationSyntax OneDeclare(string name, string text, string argInterface, string returnInterface,
        IList<Parameter> parameters, IList<Column> columns);

    MemberDeclarationSyntax ManyDeclare(string name, string text, string argInterface, string returnInterface,
        IList<Parameter> parameters, IList<Column> columns);

    MemberDeclarationSyntax ExecDeclare(string name, string text, string argInterface,
        IList<Parameter> parameters);
}