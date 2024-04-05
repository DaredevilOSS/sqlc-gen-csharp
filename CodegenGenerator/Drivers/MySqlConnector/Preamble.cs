using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.MySqlConnector;

internal static class PreambleMembers
{
     public static UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return
        [
            UsingDirective(ParseName("System.Data")),
            UsingDirective(ParseName("MySqlConnector"))
        ];
    }
    
    public static MemberDeclarationSyntax[] GetClassMembers()
    {
        // TODO in TypeScript plugin a special handling for ExecLastId in this corresponding code - figure out why)
        return
        [
            GetGetBytesWrapperMethod()
        ];
    }

    private static MemberDeclarationSyntax GetGetBytesWrapperMethod()
    {
        // TODO move to RESOURCES file
        // TODO fix function is nested within another block unnecessarily
        const string getBytesMethodCode = """
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
            .WithParameterList(ParseParameterList("(IDataRecord reader, int ordinal)"))
            .AddBodyStatements(ParseStatement(getBytesMethodCode).NormalizeWhitespace());
    }
}