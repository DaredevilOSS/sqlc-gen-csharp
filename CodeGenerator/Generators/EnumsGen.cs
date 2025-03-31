using Microsoft.CodeAnalysis.CSharp.Syntax;
using SqlcGenCsharp.Drivers;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Generators;

internal class EnumsGen(DbDriver dbDriver)
{
    public MemberDeclarationSyntax[] Generate(string name, IList<string> possibleValues)
    {
        var enumName = name.ToModelName();
        var enumValuesDef = possibleValues
            .Select((v, i) => $"{v.ToPascalCase()} = {i + 1}")
            .JoinByComma();

        var enumType = ParseMemberDeclaration($$"""
               public enum {{name.ToModelName()}} 
               {
                   Invalid = 0, // reserved for invalid enum value
                   {{enumValuesDef}}
               }
               """)!;

        var dictDefinitionAndLookup = dbDriver.Options.UseDapper
            ? $$"""
                private static readonly Dictionary<{{enumName}}, string> EnumToString = new Dictionary<{{enumName}}, string>()
                {
                    [{{enumName}}.Invalid] = string.Empty,
                    {{possibleValues
                        .Select(v => $"[{enumName}.{v.ToPascalCase()}] = \"{v}\"")
                        .JoinByComma()}}
                };
                
                public static string ToEnumString(this {{enumName}} me)
                {
                    return EnumToString[me];
                }
                """
            : $$"""
                private static readonly Dictionary<string, {{enumName}}> StringToEnum = new Dictionary<string, {{enumName}}>()
                {
                    [string.Empty] = {{enumName}}.Invalid,
                    {{possibleValues
                        .Select(v => $"[\"{v}\"] = {enumName}.{v.ToPascalCase()}")
                        .JoinByComma()}}
                };
                
                public static {{enumName}} To{{enumName}}(this string me)
                {
                    return StringToEnum[me];
                }
                """;

        var enumExtensions = ParseMemberDeclaration($$"""
               public static class {{enumName}}Extensions 
               {
                   {{dictDefinitionAndLookup}}
               }
               """)!;
        return [enumType, enumExtensions];
    }
}