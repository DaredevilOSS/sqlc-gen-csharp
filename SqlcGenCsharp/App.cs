using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProtoBuf;
using sqlc_gen_csharp.Drivers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace sqlc_gen_csharp;

public class App
{
    public static IDbDriver CreateNodeGenerator(string driver)
    {
        switch (driver)
        {
            case "MySqlConnector":
                return new MySqlConnector();
            default:
                throw new ArgumentException($"unknown driver: {driver}", nameof(driver));
        }
    }

    public static InterfaceDeclarationSyntax RowDecl(string name, Func<Column, TypeSyntax> ctype,
        IEnumerable<Column> columns)
    {
        // Create a list of property signatures based on the columns
        var properties = columns.Select((column, i) =>
            PropertyDeclaration(ctype(column), Identifier(Utils.ColName(i, column)))
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                )
        ).ToArray();

        // Create the interface declaration
        var interfaceDeclaration = InterfaceDeclaration(name)
            .AddModifiers(Token(SyntaxKind.PublicKeyword)) // Making the interface public
            .AddMembers(properties); // Adding the properties

        return interfaceDeclaration;
    }

    public static CompilationUnitSyntax QueryDecl(string name, string sql)
    {
        // Create the constant field declaration
        var fieldDeclaration = FieldDeclaration(
                VariableDeclaration(
                        PredefinedType(
                            Token(SyntaxKind.StringKeyword)
                        )
                    )
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(
                                    Identifier(name)
                                )
                                .WithInitializer(
                                    EqualsValueClause(
                                        LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(sql)
                                        )
                                    )
                                )
                        )
                    )
            )
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.ConstKeyword));

        // Create a class to contain the constant
        var classDeclaration = ClassDeclaration("Queries")
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
            .AddMembers(fieldDeclaration);

        // Create a namespace
        var namespaceDeclaration = NamespaceDeclaration(IdentifierName("YourNamespace"))
            .AddMembers(classDeclaration);

        // Create the compilation unit (root of the syntax tree) and add the namespace
        var compilationUnit = CompilationUnit()
            .AddUsings(UsingDirective(IdentifierName("System")))
            .AddMembers(namespaceDeclaration)
            .NormalizeWhitespace(); // Format the code for readability

        return compilationUnit;
    }

    public static CompilationUnitSyntax ArgsDecl(string name, Func<Column, TypeSyntax> ctype,
        IEnumerable<Parameter> parameters)
    {
        // Create a list of property signatures based on the parameters
        var properties = parameters.Select((param, i) =>
            PropertyDeclaration(ctype(param.Column), Identifier(Utils.ArgName(i, param.Column)))
                .AddModifiers(Token(SyntaxKind
                    .PublicKeyword)) // Assuming properties in interfaces cannot have accessors
                .WithAccessorList(AccessorList(List(new[]
                {
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                })))
        ).ToArray();

        // Create the interface declaration
        var interfaceDeclaration = InterfaceDeclaration(name)
            .AddModifiers(Token(SyntaxKind.PublicKeyword)) // Making the interface public
            .AddMembers(properties); // Adding the properties

        // Optionally, wrap the interface in a namespace
        var namespaceDeclaration = NamespaceDeclaration(IdentifierName("YourNamespace"))
            .AddMembers(interfaceDeclaration);

        // Create the compilation unit (root of the syntax tree) and add the namespace
        var compilationUnit = CompilationUnit()
            .AddUsings(UsingDirective(IdentifierName("System")))
            .AddMembers(namespaceDeclaration)
            .NormalizeWhitespace(); // Format the code for readability

        return compilationUnit;
    }

    public static GenerateRequest ReadInput()
    {
        // Reading from standard input stream
        using (var memoryStream = new MemoryStream())
        {
            Console.OpenStandardInput().CopyTo(memoryStream);
            memoryStream.Position = 0; // Resetting position to the beginning of the stream

            // Deserializing from binary data
            return Serializer.Deserialize<GenerateRequest>(memoryStream);
        }
    }

    public static void WriteOutput(GenerateResponse output)
    {
        // Assuming GenerateResponse is a Protobuf-generated class
        var encodedOutput = output.ToByteArray(); // Convert the Protobuf message to a byte array
        using (var stdout = Console.OpenStandardOutput())
        {
            stdout.Write(encodedOutput, 0, encodedOutput.Length);
        }
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
            var property = PropertyDeclaration(IdentifierName(param.Type), Identifier(param.Name))
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
        // The static call below is generated at build time, and will list the syntax trees used in the compilation
    }
}