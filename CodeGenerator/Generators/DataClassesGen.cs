using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Generators;

internal class DataClassesGen(DbDriver dbDriver)
{
    public MemberDeclarationSyntax Generate(string name, ClassMember classMember, IList<Column> columns, Options options)
    {
        var className = classMember.Name(name);
        if (options.DotnetFramework.LatestDotnetSupported() && !options.UseDapper)
            return GenerateAsRecord(className, columns);
        return GenerateAsCLass(className, columns);
    }

    private RecordDeclarationSyntax GenerateAsRecord(string className, IList<Column> columns)
    {
        return RecordDeclaration(Token(SyntaxKind.StructKeyword), className)
            .AddModifiers(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.ReadOnlyKeyword),
                Token(SyntaxKind.RecordKeyword)
            )
            .WithParameterList(ColumnsToParameterList())
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        ParameterListSyntax ColumnsToParameterList()
        {
            var seenEmbed = new Dictionary<string, int>();
            return ParameterList(SeparatedList(columns
                .Select(column => Parameter(Identifier(GetFieldName(column, seenEmbed)))
                    .WithType(ParseTypeName(dbDriver.GetCsharpType(column))))));
        }
    }

    private ClassDeclarationSyntax GenerateAsCLass(string className, IList<Column> columns)
    {
        return ClassDeclaration(className)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddMembers(ColumnsToProperties())
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        MemberDeclarationSyntax[] ColumnsToProperties()
        {
            var seenEmbed = new Dictionary<string, int>();
            return columns.Select(column => ParseMemberDeclaration(
                    $"public {dbDriver.GetCsharpType(column)} {GetFieldName(column, seenEmbed)} {{ get; set; }}"))
                .Cast<MemberDeclarationSyntax>()
                .ToArray();
        }
    }

    private static string GetFieldName(Column column, Dictionary<string, int> seenEmbed)
    {
        if (column.EmbedTable is null)
            return column.Name.ToPascalCase();

        var fieldName = column.Name.ToModelName();
        fieldName = seenEmbed.TryGetValue(fieldName, out var value)
            ? $"{fieldName}{value}" : fieldName;
        seenEmbed.TryAdd(fieldName, 1);
        seenEmbed[fieldName]++;
        return fieldName;
    }
}