using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SqlcGenCsharp.Drivers.Common;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.MySqlConnector;

internal static class PreambleMembers
{
     public static UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return
        [
            UsingDirective(ParseName("System")),
            UsingDirective(ParseName("System.Threading.Tasks")),
            UsingDirective(ParseName("System.Data.Common")),
            UsingDirective(ParseName("MySqlConnector"))
        ];
    }
    
    public static MemberDeclarationSyntax[] GetClassMembers()
    {
        return
        [
            GetConnectionStringConstant(),
            GetGetBytesWrapperMethod()
        ];
    }

    private static MemberDeclarationSyntax GetConnectionStringConstant()
    {
        return FieldDeclaration(
                VariableDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)))
                    .WithVariables(SingletonSeparatedList(
                        VariableDeclarator(Identifier(Variables.ConnectionString.GetNameAsConst()))
                            .WithInitializer(EqualsValueClause(
                                LiteralExpression(SyntaxKind.StringLiteralExpression,
                                    Literal("server=localhost;user=root;database=mydb;port=3306;password=")))))
                    ))
            .WithModifiers(TokenList(
                Token(SyntaxKind.PrivateKeyword),
                Token(SyntaxKind.ConstKeyword)));
    }

    private static MemberDeclarationSyntax GetGetBytesWrapperMethod()
    {
        // TODO move to RESOURCES file
        // TODO fix function is nested within another block unnecessarily
        const string getBytesMethodCode = $$"""
                                            {
                                                const int bufferSize = 100000;
                                                ArgumentNullException.ThrowIfNull(reader);
                                                var buffer = new byte[bufferSize];
                                                
                                                var (bytesRead, offset) = (0, 0);
                                                while (bytesRead < bufferSize)
                                                {
                                                    var read = (int) reader.GetBytes(
                                                        ordinal,
                                                        bufferSize + bytesRead,
                                                        buffer,
                                                        offset,
                                                        bufferSize - bytesRead);
                                                    if (read == 0)
                                                        break;
                                                    bytesRead += read;
                                                    offset += read;
                                                }
                                            
                                                if (bytesRead < bufferSize)
                                                    Array.Resize(ref buffer, bytesRead);
                                                return buffer;
                                            }
                                            """;

        return MethodDeclaration(ParseTypeName("byte[]"), "GetBytes")
            .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword)))
            .AddParameterListParameters(
                Parameter(Identifier("reader")).WithType(ParseTypeName("DbDataReader")),
                Parameter(Identifier("ordinal")).WithType(PredefinedType(Token(SyntaxKind.IntKeyword))))
            .AddBodyStatements(ParseStatement(getBytesMethodCode).NormalizeWhitespace());
    }
}