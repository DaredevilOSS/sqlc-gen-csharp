using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using File = Plugin.File;


namespace SqlcGenCsharp.Generators;

internal class UtilsGen(string namespaceName, Options options)
{
    private const string ClassName = "Utils";

    private string NamespaceName { get; } = namespaceName;

    private RootGen RootGen { get; } = new(options);

    public File GenerateFile()
    {
        var root = RootGen.CompilationRootGen(
            IdentifierName(NamespaceName), GetUsingDirectives(), GetUtilsClass());
        root = root.AddCommentOnTop(Consts.AutoGeneratedComment);

        return new File
        {
            Name = $"{ClassName}.cs",
            Contents = root.ToByteString()
        };
    }

    private static UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return
        [
            UsingDirective(ParseName("System")),
            UsingDirective(ParseName("System.Data"))
        ];
    }

    private static MemberDeclarationSyntax GetUtilsClass()
    {
        const string utilsClassDeclaration = $$"""
                                               public static class {{ClassName}}
                                               {
                                                   public static byte[] GetBytes(IDataRecord reader, int ordinal)
                                                   {
                                                       const int bufferSize = 100000;
                                                       if (reader is null) throw new ArgumentNullException(nameof(reader));
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
                                               }
                                               """;
        return ParseMemberDeclaration(utilsClassDeclaration)!.AppendNewLine();
    }
}