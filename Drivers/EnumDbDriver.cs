using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp;
using SqlcGenCsharp.Drivers;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

public abstract class EnumDbDriver : DbDriver
{
    public Dictionary<string, Dictionary<string, Enum>> Enums { get; }

    public EnumDbDriver(Options options, Catalog catalog, IList<Query> queries) : base(options, catalog, queries)
    {
        Enums = ConstructEnumsLookup(catalog);

        foreach (var schemaEnums in Enums)
            foreach (var e in schemaEnums.Value)
                NullableTypes.Add(e.Key.ToModelName(schemaEnums.Key, DefaultSchema));
    }

    public virtual MemberDeclarationSyntax[] GetEnumExtensionsMembers(string name, IList<string> possibleValues)
    {
        return [.. new List<MemberDeclarationSyntax>()
        {
            ParseMemberDeclaration($$"""
                private static readonly Dictionary<string, {{name}}> StringToEnum = new Dictionary<string, {{name}}>()
                {
                    [string.Empty] = {{name}}.Invalid,
                    {{possibleValues
                        .Select(v => $"[\"{v}\"] = {name}.{v.ToPascalCase()}")
                        .JoinByComma()}}
                };
                """)!
        }.AddRangeIf(
            [
                ParseMemberDeclaration($$"""
                    public static {{name}} To{{name}}(this string me)
                    {
                        return StringToEnum[me];
                    }
                """)!
            ],
            !Options.UseDapper)];
    }

    protected abstract Enum? GetEnumType(Column column);

    protected abstract string EnumToCsharpDataType(Column column);

    public abstract string EnumToModelName(string schemaName, Enum enumType);

    protected abstract string EnumToModelName(Column column);

    protected abstract string GetEnumReader(Column column, int ordinal);

    public override string GetColumnReader(Column column, int ordinal, Query? query)
    {
        if (GetEnumType(column) is not null)
            return GetEnumReader(column, ordinal);
        return base.GetColumnReader(column, ordinal, query);
    }

    protected override string GetCsharpTypeWithoutNullableSuffix(Column column, Query? query)
    {
        if (GetEnumType(column) is not null)
            return EnumToCsharpDataType(column);
        return base.GetCsharpTypeWithoutNullableSuffix(column, query);
    }

    private static Dictionary<string, Dictionary<string, Enum>> ConstructEnumsLookup(Catalog catalog)
    {
        return catalog
            .Schemas
            .SelectMany(s => s.Enums.Select(e => new { EnumItem = e, Schema = s.Name }))
            .GroupBy(x => x.Schema == catalog.DefaultSchema ? string.Empty : x.Schema)
            .ToDictionary(
                group => group.Key,
                group => group.ToDictionary(
                    x => x.EnumItem.Name,
                    x => x.EnumItem
                )
            );
    }
}