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
        if (options.DotnetFramework.LatestDotnetSupported() && !options.UseDapper)
            return GenerateAsRecord(name, classMember, columns);
        return GenerateAsCLass(name, classMember, columns);
    }

    private RecordDeclarationSyntax GenerateAsRecord(string name, ClassMember classMember, IList<Column> columns)
    {
        return RecordDeclaration(
                Token(SyntaxKind.StructKeyword),
                $"{name}{classMember.Name()}")
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
                .Select(column => Parameter(Identifier(column.Name.ToPascalCase()))
                    .WithType(ParseTypeName(dbDriver.GetColumnType(column)))
                )));
        }
    }

    private ClassDeclarationSyntax GenerateAsCLass(string name, ClassMember classMember, IList<Column> columns)
    {
        var className = $"{name}{classMember.Name()}";
        return ClassDeclaration(className)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddMembers(ColumnsToProperties())
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        MemberDeclarationSyntax[] ColumnsToProperties()
        {
            return columns.Select(column =>
                {
                    var propertyType = dbDriver.GetColumnType(column);
                    return ParseMemberDeclaration(
                        $"public {propertyType} {column.Name.ToPascalCase()} {{ get; set; }}");
                })
                .Cast<MemberDeclarationSyntax>()
                .ToArray();
        }
    }
}