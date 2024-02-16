using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using sqlc_gen_csharp.drivers.abstractions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Type = System.Type;

namespace sqlc_gen_csharp.drivers;

public class MySqlConnector : IDbDriver
{
    public Type ColumnType(string columnType, bool notNull)
    {
        if (string.IsNullOrEmpty(columnType))
            return typeof(object);

        switch (columnType.ToLower())
        {
            case "bigint":
                return typeof(long);
            case "binary":
            case "bit":
            case "blob":
            case "longblob":
            case "mediumblob":
            case "tinyblob":
            case "varbinary":
                return typeof(byte[]);
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
                return typeof(string);
            case "double":
            case "float":
                return typeof(double);
            case "int":
            case "mediumint":
            case "smallint":
            case "tinyint":
            case "year":
                return typeof(int);
            case "json":
                return typeof(object); // or a specific class if JSON structure is known
            default:
                throw new NotSupportedException($"Unsupported column type: {columnType}");
        }
    }

    public CompilationUnitSyntax Preamble(Query[] queries)
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

    public IEnumerable<ParameterSyntax> FuncParamsDecl(string iface, IEnumerable<Parameter> parameters)
    {
        var funcParams = new List<ParameterSyntax>
        {
            Parameter(Identifier("client"))
                .WithType(IdentifierName("Client"))
        };

        using var enumerator = parameters.GetEnumerator();
        if (!string.IsNullOrEmpty(iface) && enumerator.MoveNext())
            funcParams.Add(
                Parameter(Identifier("args"))
                    .WithType(IdentifierName(iface))
            );

        return funcParams;
    }

    public MethodDeclarationSyntax ExecDecl(string funcName, string queryName, string argIface,
        IEnumerable<Parameter> parameters)
    {
        // Generating the parameters for the method, potentially including 'args' if specified
        var funcParams =
            FuncParamsDecl(argIface, parameters); // Assuming FuncParamsDecl is implemented as shown previously

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
}