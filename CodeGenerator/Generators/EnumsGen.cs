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
                private static readonly Dictionary<{{name}}, string> EnumToString = new Dictionary<{{name}}, string>()
                {
                    [{{name}}.Invalid] = string.Empty,
                    {{possibleValues
                        .Select(v => $"[{name}.{v.ToPascalCase()}] = \"{v}\"")
                        .JoinByComma()}}
                };
                
                public static string ToEnumString(this {{name}} me)
                {
                    return EnumToString[me];
                }
                """
            : $$"""
                private static readonly Dictionary<string, {{name}}> StringToEnum = new Dictionary<string, {{name}}>()
                {
                    [string.Empty] = {{name}}.Invalid,
                    {{possibleValues
                        .Select(v => $"[\"{v}\"] = {name}.{v.ToPascalCase()}")
                        .JoinByComma()}}
                };
                
                public static {{name}} To{{name}}(this string me)
                {
                    return StringToEnum[me];
                }
                """;

        var enumExtensions = ParseMemberDeclaration($$"""
               public static class {{name}}Extensions 
               {
                   {{dictDefinitionAndLookup}}
               }
               """)!;
        return [enumType, enumExtensions];
    }
}