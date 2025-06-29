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
    public MemberDeclarationSyntax Generate(string name, ClassMember? classMember, IList<Column> columns, Options options, Query? query)
    {
        var className = classMember is null ? name : classMember.Value.Name(name);
        if (options.DotnetFramework.IsDotnetCore() && !options.UseDapper)
            return GenerateAsRecord(className, columns, query);
        return GenerateAsCLass(className, columns, query);
    }

    private MemberDeclarationSyntax GenerateAsRecord(string className, IList<Column> columns, Query? query)
    {
        var seenEmbed = new Dictionary<string, int>();
        var recordParameters = columns
            .Select(column => $"{dbDriver.GetCsharpType(column, query)} {GetFieldName(column, seenEmbed)}")
            .JoinByComma();
        return ParseMemberDeclaration($"public readonly record struct {className} ({recordParameters});")!;
    }

    private ClassDeclarationSyntax GenerateAsCLass(string className, IList<Column> columns, Query? query)
    {
        var modernDotnetSupported = dbDriver.Options.DotnetFramework.IsDotnetCore();
        return ClassDeclaration(className)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddMembers(ColumnsToProperties())
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        MemberDeclarationSyntax[] ColumnsToProperties()
        {
            var seenEmbed = new Dictionary<string, int>();
            return columns.Select(column =>
                {
                    var csharpType = dbDriver.GetCsharpType(column, query);
                    var optionalRequiredModifier = RequiredModifierNeeded(column) ? "required" : string.Empty;
                    var setterMethod = modernDotnetSupported ? "init" : "set";
                    return ParseMemberDeclaration(
                        $$"""
                          public {{optionalRequiredModifier}} {{csharpType}} {{GetFieldName(column, seenEmbed)}} { get; {{setterMethod}}; }
                          """);
                })
                .Cast<MemberDeclarationSyntax>()
                .ToArray();
        }

        bool RequiredModifierNeeded(Column column)
        {
            if (!dbDriver.Options.DotnetFramework.IsDotnetCore())
                return false;
            if (column.EmbedTable != null)
                return true;
            return dbDriver.IsColumnNotNull(column, query);
        }
    }

    private string GetFieldName(Column column, Dictionary<string, int> seenEmbed)
    {
        if (column.EmbedTable is null)
            return column.Name.ToPascalCase();

        var fieldName = column.Name.ToModelName(column.EmbedTable.Schema, dbDriver.DefaultSchema);
        fieldName = seenEmbed.TryGetValue(fieldName, out var value)
            ? $"{fieldName}{value}" : fieldName;
        seenEmbed.TryAdd(fieldName, 1);
        seenEmbed[fieldName]++;
        return fieldName;
    }
}