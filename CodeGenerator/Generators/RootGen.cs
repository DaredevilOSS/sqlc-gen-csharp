using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Generators;

internal class RootGen(Options options)
{
    public CompilationUnitSyntax CompilationRootGen(IdentifierNameSyntax namespaceName,
        UsingDirectiveSyntax[] usingDirectives, MemberDeclarationSyntax[] classDeclarations)
    {
        return options.DotnetFramework.LatestDotnetSupported() ? GetFileScoped() : GetBLockScoped();

        CompilationUnitSyntax GetFileScoped()
        {
            return CompilationUnit()
                .AddUsings(usingDirectives)
                .AddMembers(FileScopedNamespaceDeclaration(namespaceName))
                .AddMembers(classDeclarations)
                .NormalizeWhitespace();
        }

        CompilationUnitSyntax GetBLockScoped()
        {
            return CompilationUnit()
                .AddMembers(
                    NamespaceDeclaration(namespaceName)
                        .AddUsings(usingDirectives)
                        .AddMembers(classDeclarations))
                .NormalizeWhitespace();
        }
    }
}