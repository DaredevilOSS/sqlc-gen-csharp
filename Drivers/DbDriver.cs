using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System;
using System.Collections.Generic;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static System.String;

namespace SqlcGenCsharp.Drivers;

public abstract class DbDriver(DotnetFramework dotnetFramework)
{
    public DotnetFramework DotnetFramework { get; } = dotnetFramework;

    public virtual UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return
        [
            UsingDirective(ParseName("System.Collections.Generic")),
            UsingDirective(ParseName("System.Threading.Tasks"))
        ];
    }

    protected abstract List<(string, Func<int, string>, HashSet<string>)> GetColumnMapping();

    public string AddNullableSuffix(string csharpType, bool notNull)
    {
        if (notNull) return csharpType;
        if (IsCsharpPrimitive(csharpType)) return $"{csharpType}?";
        return DotnetFramework.LatestDotnetSupported() ? $"{csharpType}?" : csharpType;
    }

    public string GetColumnType(Column column)
    {
        var columnCsharpType = IsNullOrEmpty(column.Type.Name) ? "object" : GetTypeWithoutNullableSuffix();
        return AddNullableSuffix(columnCsharpType, column.NotNull);

        string GetTypeWithoutNullableSuffix()
        {
            var columnType = column.Type.Name.ToLower();
            foreach (var (csharpType, _, dbTypes) in GetColumnMapping())
            {
                if (dbTypes.Contains(columnType))
                    return csharpType;
            }
            throw new NotSupportedException($"Unsupported column type: {column.Type.Name}");
        }
    }

    public string GetColumnReader(Column column, int ordinal)
    {
        var columnType = column.Type.Name.ToLower();
        foreach (var (_, getDataReader, dbTypes) in GetColumnMapping())
        {
            if (dbTypes.Contains(columnType))
                return getDataReader(ordinal);
        }
        throw new NotSupportedException($"Unsupported column type: {column.Type.Name}");
    }

    public abstract string TransformQueryText(Query query);

    public abstract (string, string) EstablishConnection(Query query);

    public abstract string CreateSqlCommand(string sqlTextConstant);

    public abstract MemberDeclarationSyntax OneDeclare(string sqlTextConstant, string argInterface,
        string returnInterface, Query query);

    public abstract MemberDeclarationSyntax ManyDeclare(string sqlTextConstant, string argInterface,
        string returnInterface, Query query);

    public abstract MemberDeclarationSyntax ExecDeclare(string text, string argInterface, Query query);

    public static bool IsCsharpPrimitive(string csharpType)
    {
        var csharpPrimitives = new HashSet<string> { "long", "double", "int", "float", "bool" };
        return csharpPrimitives.Contains(csharpType.Replace("?", ""));
    }
}