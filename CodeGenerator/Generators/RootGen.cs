using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp;

internal class RootGen(Options options)
{
    public CompilationUnitSyntax CompilationRootGen(IdentifierNameSyntax namespaceName,
        UsingDirectiveSyntax[] usingDirectives, MemberDeclarationSyntax classDeclaration)
    {
        return options.DotnetFramework == DotnetFramework.Dotnet80 ? GetFileScoped() : GetBLockScoped();

        CompilationUnitSyntax GetFileScoped()
        {
            return CompilationUnit()
                .AddUsings(usingDirectives)
                .AddMembers(FileScopedNamespaceDeclaration(namespaceName), classDeclaration)
                .NormalizeWhitespace();
        }

        CompilationUnitSyntax GetBLockScoped()
        {
            return CompilationUnit()
                .AddMembers(
                    NamespaceDeclaration(namespaceName)
                        .AddUsings(usingDirectives)
                        .AddMembers(classDeclaration))
                .NormalizeWhitespace();
        }
    }
}