using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.NpgsqlDriver.Generators;

internal static class PreambleGen
{
    public static UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return
        [
            UsingDirective(ParseName("System.Collections.Generic")),
            UsingDirective(ParseName("System.Threading.Tasks")),
            UsingDirective(ParseName("Npgsql"))
        ];
    }
}