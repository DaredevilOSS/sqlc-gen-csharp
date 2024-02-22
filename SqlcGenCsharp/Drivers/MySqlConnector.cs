using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Type = System.Type;

namespace sqlc_gen_csharp.Drivers;

public class MySqlConnector : IDbDriver
{
    public TypeSyntax ColumnType(string columnType, bool notNull)
    {
        var nullableSuffix = notNull ? string.Empty : "?";

        if (string.IsNullOrEmpty(columnType))
            return SyntaxFactory.ParseTypeName("object" + nullableSuffix);

        switch (columnType.ToLower())
        {
            case "bigint":
                return SyntaxFactory.ParseTypeName("long" + nullableSuffix);
            case "binary":
            case "bit":
            case "blob":
            case "longblob":
            case "mediumblob":
            case "tinyblob":
            case "varbinary":
                return SyntaxFactory.ParseTypeName("byte[]" + nullableSuffix);
            case "char":
            case "date":
            case "datetime":
            case "decimal":
            case "longtext":
            case "mediumtext":
            case "text":
            case "time":
            case "timestamp":
            case "tinytext":
            case "varchar":
                return SyntaxFactory.ParseTypeName("string");
            case "double":
            case "float":
                return SyntaxFactory.ParseTypeName("double" + nullableSuffix);
            case "int":
            case "mediumint":
            case "smallint":
            case "tinyint":
            case "year":
                return SyntaxFactory.ParseTypeName("int" + nullableSuffix);
            case "json":
                // Assuming JSON is represented as a string or a specific class
                return SyntaxFactory.ParseTypeName("object" + nullableSuffix);
            default:
                throw new NotSupportedException($"Unsupported column type: {columnType}");
        }
    }
    
    public CompilationUnitSyntax Preamble(List<Query> queries)
    {
        // Using directive for MySQL (or similar)
        var usingDirective = UsingDirective(ParseName("MySql.Data.MySqlClient"));

        // Class declaration
        var classDeclaration = ClassDeclaration("DatabaseClient")
            .AddModifiers(Token(SyntaxKind.PublicKeyword)) // Making the class public
            .AddMembers(
                // Example method that represents using a MySQL connection
                MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), "ConnectToDatabase")
                    .AddModifiers(Token(SyntaxKind.PublicKeyword)) // Making the method public
                    .WithBody(Block(ParseStatement("var connection = new MySqlConnection();")))
            );

        // Namespace declaration
        var namespaceDeclaration = NamespaceDeclaration(ParseName("GeneratedNamespace"))
            .AddMembers(classDeclaration);

        // Compilation unit (root of the syntax tree) with using directives and namespace
        var compilationUnit = CompilationUnit()
            .AddUsings(usingDirective)
            .AddMembers(namespaceDeclaration)
            .NormalizeWhitespace(); // Format the code for readability

        return compilationUnit;
    }

    public MethodDeclarationSyntax OneDeclare(string funcName, string queryName, string argIface, string returnIface,
        IList<Parameter> parameters, IList<Column> columns)
    {
        // Generating function parameters, potentially including 'args'
        var funcParams = FuncParamsDecl(argIface, parameters); // FuncParamsDecl should be implemented as before

        // Return type is Task<ReturnType?>
        var returnType = GenericName(Identifier("Task"))
            .WithTypeArgumentList(
                TypeArgumentList(
                    SingletonSeparatedList<TypeSyntax>(
                        NullableType(IdentifierName(returnIface)))));

        // Method declaration
        var methodDeclaration = MethodDeclaration(returnType, Identifier(funcName))
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword))
            .WithParameterList(ParameterList(SeparatedList(funcParams)))
            .WithBody(Block(
                // Placeholder for method body: In a real scenario, you'd include logic to execute the query
                // and return a single result or null.
                // The following statement is a simplification.
                SingletonList<StatementSyntax>(
                    ReturnStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("Task"),
                                IdentifierName("FromResult")),
                            ArgumentList(SeparatedList(new[]
                            {
                                Argument(
                                    // Assuming a method to convert a database row to returnIface
                                    InvocationExpression(IdentifierName("ConvertToReturnType"),
                                            ArgumentList(SingletonSeparatedList(
                                                Argument(IdentifierName("row")))))
                                        .WithLeadingTrivia(Comment("// Convert row to ReturnType instance"))
                                )
                            }))
                        ).WithLeadingTrivia(Comment("// Placeholder for actual database query execution"))
                    )
                )
            ));

        return methodDeclaration;
    }

    private IEnumerable<ParameterSyntax> FuncParamsDecl(string iface, IList<Parameter> parameters)
    {
        var funcParams = new List<ParameterSyntax>
        {
            Parameter(Identifier("client")).WithType(IdentifierName("Client"))
        };

        using var enumerator = parameters.GetEnumerator();
        if (!string.IsNullOrEmpty(iface) && enumerator.MoveNext())
        {
            funcParams.Add(
                Parameter(Identifier("args")).WithType(IdentifierName(iface))
            );
        }

        return funcParams;
    }
    
    public MethodDeclarationSyntax ExecDeclare(string funcName, string queryName, string argIface,
        IList<Parameter> parameters)
    {
        // Generating the parameters for the method, potentially including 'args' if specified
        var funcParams =
            FuncParamsDecl(argIface, parameters);

        // Creating the method declaration
        var methodDeclaration = MethodDeclaration(
                GenericName(Identifier("Task"))
                    .WithTypeArgumentList(TypeArgumentList(
                        SingletonSeparatedList<TypeSyntax>(PredefinedType(Token(SyntaxKind.VoidKeyword))))),
                Identifier(funcName))
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword))
            .WithParameterList(ParameterList(SeparatedList(funcParams)))
            .WithBody(Block(
                // Simplified body; assuming a method call to 'client.query' with parameters.
                // The actual implementation would involve more detailed Roslyn syntax generation,
                // depending on how the 'client.query' method is defined and used.
                SingletonList<StatementSyntax>(
                    ExpressionStatement(
                        AwaitExpression(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("client"),
                                    IdentifierName(
                                        "QueryAsync") // Assuming a method QueryAsync exists for simplification
                                ),
                                ArgumentList(SingletonSeparatedList(
                                    Argument(
                                        // Simplified argument; actual implementation would dynamically build this based on 'params'
                                        IdentifierName("queryParameters") // Placeholder for actual query parameters
                                    )
                                ))
                            )
                        )
                    )
                )
            ));

        return methodDeclaration;
    }

    public MethodDeclarationSyntax ManyDeclare(string funcName, string queryName, string argIface, string returnIface,
        IList<Parameter> parameters, IList<Column> columns)
    {
        // Assuming FuncParamsDecl is implemented as shown previously
        var funcParams = FuncParamsDecl(argIface, parameters);

        // Return type is Task<List<ReturnType>>
        var returnType = GenericName(Identifier("Task"))
            .WithTypeArgumentList(
                TypeArgumentList(
                    SingletonSeparatedList<TypeSyntax>(
                        GenericName(Identifier("List"))
                            .WithTypeArgumentList(
                                TypeArgumentList(
                                    SingletonSeparatedList<TypeSyntax>(
                                        IdentifierName(returnIface)))))));

        // Method declaration
        var methodDeclaration = MethodDeclaration(returnType, Identifier(funcName))
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword))
            .WithParameterList(ParameterList(SeparatedList(funcParams)))
            .WithBody(Block(
                // Simplified body: In a real scenario, you'd construct the logic to execute the query,
                // map the results to a list of `returnIface` objects, and return it.
                // The following is a placeholder to illustrate structure.
                SingletonList<StatementSyntax>(
                    ReturnStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("Task"),
                                GenericName(Identifier("FromResult"))
                                    .WithTypeArgumentList(
                                        TypeArgumentList(
                                            SingletonSeparatedList<TypeSyntax>(
                                                PredefinedType(Token(SyntaxKind.StringKeyword)))))),
                            ArgumentList(SingletonSeparatedList(
                                Argument(
                                    LiteralExpression(SyntaxKind.StringLiteralExpression,
                                        Literal("Placeholder for actual implementation"))
                                )
                            ))
                        )
                    )
                )
            ));

        return methodDeclaration;
    }
    
    public static InterfaceDeclarationSyntax RowDeclare(string name, Func<Column, TypeSyntax> ctype,
        IList<Column?> columns)
    {
        var properties = columns.Select((column, i) =>
            PropertyDeclaration(ctype(column), Identifier(Utils.ColName(i, column)))
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .WithAccessorList(
                    AccessorList(
                        List(new AccessorDeclarationSyntax[]
                        {
                            AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                            AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                        })
                    )
                )
        ).ToArray();

        var interfaceDeclaration = InterfaceDeclaration(Identifier(name))
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddMembers(properties);

        return interfaceDeclaration;
    }
    
    public static string PrintNodes(IEnumerable<SyntaxNode> nodes)
    {
        var stringBuilder = new StringBuilder();
        foreach (var node in nodes)
        {
            stringBuilder.AppendLine(node.NormalizeWhitespace().ToFullString());
            stringBuilder.AppendLine(); // Adding an extra line for separation, similar to "\n\n"
        }
        return stringBuilder.ToString();
    }
}