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
        if (dbDriver is not EnumDbDriver enumDbDriver)
            return [];

        var enumValuesDef = possibleValues
            .Select((v, i) => $"{v.ToPascalCase()} = {i + 1}")
            .JoinByComma();

        var enumType = ParseMemberDeclaration($$"""
            public enum {{name}} 
            {
                Invalid = 0, // reserved for invalid enum value
                {{enumValuesDef}}
            }
            """)!;

        var classMembers = enumDbDriver.GetEnumExtensionsMembers(name, possibleValues);
        if (classMembers.Length == 0)
            return [enumType];

        var enumExtensionsClass = (ClassDeclarationSyntax)ParseMemberDeclaration(
            $$"""public static class {{name}}Extensions { }""")!;
        return [enumType, enumExtensionsClass.AddMembers(classMembers)];
    }
}