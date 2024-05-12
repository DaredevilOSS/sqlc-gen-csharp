using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;

namespace SqlcGenCsharp.Drivers;

public abstract class DbDriver(DotnetFramework dotnetFramework)
{
    public DotnetFramework DotnetFramework { get; } = dotnetFramework;

    public string AddNullableSuffix(string csharpType, bool notNull)
    {
        if (notNull) return csharpType;
        if (Utils.IsCsharpPrimitive(csharpType)) return $"{csharpType}?";
        return DotnetFramework.NullableEnabled() ? $"{csharpType}?" : csharpType;
    }

    public abstract UsingDirectiveSyntax[] GetUsingDirectives();

    public abstract string GetColumnType(Column column);

    public abstract string GetColumnReader(Column column, int ordinal);

    public abstract string TransformQueryText(Query query);

    public abstract string[] EstablishConnection();

    public abstract string CreateSqlCommand(string sqlTextConstant);

    public abstract MemberDeclarationSyntax OneDeclare(string name, string sqlTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns);

    public abstract MemberDeclarationSyntax ManyDeclare(string funcName, string sqlTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IEnumerable<Column> columns);

    public abstract MemberDeclarationSyntax ExecDeclare(string funcName, string text, string argInterface,
        IList<Parameter> parameters);

    public abstract MemberDeclarationSyntax ExecLastIdDeclare(string funcName, string queryTextConstant,
        string argInterface, IList<Parameter> parameters);
}