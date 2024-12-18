using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static System.String;

namespace SqlcGenCsharp.Drivers;

public record ConnectionGenCommands(string EstablishConnection, string ConnectionOpen);

public abstract class DbDriver(DotnetFramework dotnetFramework, bool useDapper)
{
    public bool UseDapper { get; } = useDapper;
    public DotnetFramework DotnetFramework { get; } = dotnetFramework;

    private HashSet<string> CsharpPrimitives { get; } = ["long", "double", "int", "float", "bool", "DateTime"];

    public virtual UsingDirectiveSyntax[] GetUsingDirectives()
    {
        var usingDirectives = new List<UsingDirectiveSyntax>
        {
            UsingDirective(ParseName("System")),
            UsingDirective(ParseName("System.Collections.Generic")),
            UsingDirective(ParseName("System.Threading.Tasks"))
        };

        if (UseDapper)
            usingDirectives.Add(UsingDirective(ParseName("Dapper")));

        return usingDirectives.ToArray();
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

    public abstract ConnectionGenCommands EstablishConnection(Query query, bool UseDapper = false);

    public abstract string CreateSqlCommand(string sqlTextConstant);

    public abstract MemberDeclarationSyntax OneDeclare(string sqlTextConstant, string argInterface, string returnInterface, Query query);

    public abstract MemberDeclarationSyntax ManyDeclare(string sqlTextConstant, string argInterface, string returnInterface, Query query);

    public abstract MemberDeclarationSyntax ExecDeclare(string text, string argInterface, Query query);

    public bool IsCsharpPrimitive(string csharpType)
    {
        return CsharpPrimitives.Contains(csharpType.Replace("?", ""));
    }
}