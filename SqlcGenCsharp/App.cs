using System;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using ProtoBuf;
using sqlc_gen_csharp.Drivers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using File = System.IO.File;

namespace sqlc_gen_csharp;

public class App
{
    private static GenerateRequest ReadInput()
    {
        // Reading from standard input stream
        using var memoryStream = new MemoryStream();
        Console.OpenStandardInput().CopyTo(memoryStream);
        memoryStream.Position = 0; // Resetting position to the beginning of the stream

        // Deserializing from binary data
        return Serializer.Deserialize<GenerateRequest>(memoryStream);
    }

    private static void WriteOutput(GenerateResponse output)
    {
        var encodedOutput = output.ToByteArray(); // Convert the Protobuf message to a byte array
        using var stdout = Console.OpenStandardOutput();
        stdout.Write(encodedOutput, 0, encodedOutput.Length);
    }

    public void GenerateCodeAndSave(IEnumerable<Parameter> parameters, string outputPath)
    {
        var compilationUnit = CompilationUnit().AddMembers(GenerateClass("GeneratedClass", parameters))
            .NormalizeWhitespace();

        var code = compilationUnit.ToFullString();
        File.WriteAllText(outputPath, code);
    }

    private ClassDeclarationSyntax GenerateClass(string className, IEnumerable<Parameter> parameters)
    {
        var classDeclaration = ClassDeclaration(className)
            .AddModifiers(Token(SyntaxKind.PublicKeyword));

        foreach (var param in parameters)
        {
            var property = PropertyDeclaration(IdentifierName(param.Column.Type.Name), Identifier(param.Column.Name))
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

            classDeclaration = classDeclaration.AddMembers(property);
        }

        return classDeclaration;
    }
    
    public static void Main()
    {
        System.Console.WriteLine("working plugin");
        // The static call below is generated at build time, and will list the syntax trees used in the compilation
        // System.Environment.Exit(1);
        var generateRequest = ReadInput();
        var generateResponse = CodeGenerator.Generate(generateRequest);
        if (generateResponse != null) WriteOutput(generateResponse);
    }
}