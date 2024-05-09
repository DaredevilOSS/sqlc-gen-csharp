using Microsoft.CodeAnalysis.CSharp.Syntax;
using SqlcGenCsharp.Drivers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.NpgsqlDriver.Generators;

public class PreambleGen(IDbDriver dbDriver)
{
    public UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return
        [
            UsingDirective(ParseName("System.Collections.Generic")),
            UsingDirective(ParseName("System.Threading.Tasks")),
            UsingDirective(ParseName("Npgsql"))
        ];
    }
}