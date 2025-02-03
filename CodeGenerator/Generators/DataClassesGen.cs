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
        var className = classMember == ClassMember.Model ? $"{name.ToModelName()}" : $"{name}{classMember.Name()}";
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
            return ParameterList(SeparatedList(columns
                .Select(column =>
                    {
                        var fieldName = column.EmbedTable == null
                            ? column.Name.ToPascalCase()
                            : column.Name.ToModelName();
                        return Parameter(Identifier(fieldName))
                            .WithType(ParseTypeName(dbDriver.GetColumnType(column)));
                    }
                )));
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
            return columns.Select(column =>
                {
                    var propertyType = dbDriver.GetColumnType(column);
                    var propertyName = column.EmbedTable == null
                        ? column.Name.ToPascalCase()
                        : column.Name.ToModelName();
                    return ParseMemberDeclaration(
                        $"public {propertyType} {propertyName} {{ get; set; }}");
                })
                .Cast<MemberDeclarationSyntax>()
                .ToArray();
        }
    }
}