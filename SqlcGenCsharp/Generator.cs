using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using sqlc_gen_csharp.Drivers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace sqlc_gen_csharp;

public static class CodeGenerator
{
    public static GenerateResponse? Generate(GenerateRequest generateRequest)
    {
        if (generateRequest.PluginOptions.Length <= 0) 
            return null;
        var text = Encoding.UTF8.GetString(generateRequest.PluginOptions.ToByteArray());
        var options = JsonSerializer.Deserialize<Options>(text);
        if (options?.Driver is null)
            return null;
        
        var dbDriver = CreateNodeGenerator(options.Driver);
        var queryMap = generateRequest.Queries
            .GroupBy(query => query.Filename)
            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (var fileQueries in queryMap)
        {
            var nodes = dbDriver.Preamble(fileQueries.Value);
            foreach (var query in fileQueries.Value)
            {
                var colMap = new Dictionary<string, int>();
                var updatedColumns = query.Columns
                    .Where(column => !string.IsNullOrEmpty(column.Name)) // Filter out columns without a name
                    .Select(column => {
                        var count = colMap.GetValueOrDefault(column.Name, 0);
                        var updatedName = count > 0 ? $"{column.Name}_{count + 1}" : column.Name;
                        colMap[column.Name] = count + 1; // Update the count for the current name
                        return new Column { Name = updatedName };
                    })
                    .ToList();
                
                var lowerName = char.ToLower(query.Name[0]) + query.Name.Substring(1);
                var textName = $"{lowerName}Query";
                var queryDeclaration = QueryDecl(
                    textName,
                    $"-- name: {query.Name} {query.Cmd}\n{query.Text}"
                );
                
                string? argIface = null;
                string? returnIface = null;

                if (query.Params.Count > 0)
                {
                    argIface = $"{query.Name}Args";
                    // Assuming 'nodes' is a List of some type and 'argsDecl' returns an instance of that type
                    // 'ctype' and 'query.Params' need to be appropriately defined and passed
                    nodes.Add(ArgsDecl(argIface, dbDriver.ColumnType, query.Params));
                }

                if (query.Columns.Count > 0)
                {
                    returnIface = $"{query.Name}Row";
                    // Similarly, assuming 'rowDecl' returns an instance of the type contained in 'nodes'
                    // 'ctype' and 'query.Columns' need to be appropriately defined and passed
                    nodes.Add(RowDecl(returnIface, dbDriver.ColumnType, query.Columns));
                }
            }
        }
    }
    
    private static IDbDriver CreateNodeGenerator(string driver)
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

    static CompilationUnitSyntax ArgsDecl(string name, Func<Column, TypeSyntax> ctype,
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
}