using Microsoft.CodeAnalysis.CSharp.Syntax;
using SqlcGenCsharp.Drivers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.NpgsqlDriver.Generators;

internal static class PreambleGen
{
    public static UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return
        [
            UsingDirective(ParseName("System.Data")),
            UsingDirective(ParseName("Npgsql"))
        ];
    }

    public static MemberDeclarationSyntax[] GetClassMembers()
    {
        return
        [
            GetGetBytesWrapperMethod()
        ];
    }

    private static MemberDeclarationSyntax GetGetBytesWrapperMethod()
    {
        // TODO move to RESOURCES file
        // TODO fix function is nested within another block unnecessarily
        const string getBytesMethod = """
                                          private static byte[] GetBytes(IDataRecord reader, int ordinal)
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

        return ParseMemberDeclaration(getBytesMethod)!.AppendNewLine();
    }
}