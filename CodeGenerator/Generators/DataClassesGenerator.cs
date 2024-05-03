using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Generators;

public class DataClassesGenerator(IDbDriver dbDriver)
{
    private IDbDriver DbDriver { get; } = dbDriver;

    public MemberDeclarationSyntax Generate(string name, ClassMember classMember, IEnumerable<Column> columns, 
        ValidOptions validOptions)
    {
        // TODO logic should be part of a feature matrix
        if (validOptions.MinimalCsharp >= 9.0)
            return GenerateAsRecord(name, classMember, columns);
        return GenerateAsCLass(name, classMember, columns);
    }
    
    private RecordDeclarationSyntax GenerateAsRecord(string name, ClassMember classMember, 
        IEnumerable<Column> columns)
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
                .Select(column => Parameter(Identifier(column.Name.FirstCharToUpper()))
                    .WithType(ParseTypeName(DbDriver.ColumnType(column.Type.Name, column.NotNull)))
                )));
        }
    }

    private ClassDeclarationSyntax GenerateAsCLass(string name, ClassMember classMember,
        IEnumerable<Column> columns)
    {
        return ClassDeclaration($"{name}{classMember.Name()}")
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddMembers(ColumnsToProperties())
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        
        MemberDeclarationSyntax[] ColumnsToProperties()
        {
            return columns.Select(c =>
                {
                    var propertyType = DbDriver.ColumnType(c.Type.Name, c.NotNull);
                    return ParseMemberDeclaration(
                        $"public required {propertyType} {c.Name.FirstCharToUpper()} {{ get; init; }}");
                })
                .Cast<MemberDeclarationSyntax>()
                .ToArray();
        }
    }
}
