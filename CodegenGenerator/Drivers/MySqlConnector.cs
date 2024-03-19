using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public class MySqlConnector : IDbDriver
{
    public TypeSyntax ColumnType(string columnType, bool notNull)
    {
        var nullableSuffix = notNull ? string.Empty : "?";

        if (string.IsNullOrEmpty(columnType))
            return ParseTypeName("object" + nullableSuffix);

        switch (columnType.ToLower())
        {
            case "bigint":
                return ParseTypeName("long" + nullableSuffix);
            case "binary":
            case "bit":
            case "blob":
            case "longblob":
            case "mediumblob":
            case "tinyblob":
            case "varbinary":
                return ParseTypeName("byte[]" + nullableSuffix);
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
                return ParseTypeName("string");
            case "double":
            case "float":
                return ParseTypeName("double" + nullableSuffix);
            case "int":
            case "mediumint":
            case "smallint":
            case "tinyint":
            case "year":
                return ParseTypeName("int" + nullableSuffix);
            case "json":
                // Assuming JSON is represented as a string or a specific class
                return ParseTypeName("object" + nullableSuffix);
            default:
                throw new NotSupportedException($"Unsupported column type: {columnType}");
        }
    }

    private static MethodDeclarationSyntax _getMethodForQuery(Query query)
    {
        return MethodDeclaration(
                PredefinedType(Token(ParseTypeName("Task").Kind())), query.Name)
            .AddModifiers(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.AsyncKeyword))
            .WithBody(Block(ParseStatement("var connection = new MySqlConnection();")));
    }
    
    public (UsingDirectiveSyntax, IEnumerable<MethodDeclarationSyntax>) Preamble(IEnumerable<Query> queries)
    {
        var usingDirective = UsingDirective(ParseName("MySql.Data.MySqlClient"));
        var methodDeclarations = queries.Select(selector: _getMethodForQuery);
        return (usingDirective, methodDeclarations);
    }

    public MethodDeclarationSyntax OneDeclare(string funcName, string queryName, string argInterface,
        string returnInterface, IEnumerable<Parameter> parameters, IEnumerable<Column> columns)
    {
        // Generating function parameters, potentially including 'args'
        var funcParams = FuncParamsDecl(argInterface, parameters); // FuncParamsDecl should be implemented as before

        // Return type is Task<ReturnType?>
        var returnType = GenericName(Identifier("Task"))
            .WithTypeArgumentList(
                TypeArgumentList(
                    SingletonSeparatedList<TypeSyntax>(
                        NullableType(IdentifierName(returnInterface)))));

        // Method declaration
        var methodDeclaration = MethodDeclaration(returnType, Identifier(funcName))
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword))
            .WithParameterList(ParameterList(SeparatedList(funcParams)))
            .WithBody(Block(
                // Placeholder for method body: In a real scenario, you'd include logic to execute the query
                // and return a single result or null. The following statement is a simplification
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

    public MethodDeclarationSyntax ExecDeclare(string funcName, string queryName, string argInterface,
        IEnumerable<Parameter> parameters)
    {
        // Generating the parameters for the method, potentially including 'args' if specified
        var funcParams =
            FuncParamsDecl(argInterface, parameters);

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

    public MethodDeclarationSyntax ManyDeclare(string funcName, string queryName, string argInterface,
        string returnInterface,
        IEnumerable<Parameter> parameters, IEnumerable<Column> columns)
    {
        // Assuming FuncParamsDecl is implemented as shown previously
        var funcParams = FuncParamsDecl(argInterface, parameters);

        // Return type is Task<List<ReturnType>>
        var returnType = GenericName(Identifier("Task"))
            .WithTypeArgumentList(
                TypeArgumentList(
                    SingletonSeparatedList<TypeSyntax>(
                        GenericName(Identifier("List"))
                            .WithTypeArgumentList(
                                TypeArgumentList(
                                    SingletonSeparatedList<TypeSyntax>(
                                        IdentifierName(returnInterface)))))));

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

    private static IEnumerable<ParameterSyntax> FuncParamsDecl(string argInterface, IEnumerable<Parameter> parameters)
    {
        var funcParams = new List<ParameterSyntax>
        {
            Parameter(Identifier("client")).WithType(IdentifierName("Client"))
        };

        using var enumerator = parameters.GetEnumerator();
        if (!string.IsNullOrEmpty(argInterface) && enumerator.MoveNext())
            funcParams.Add(
                Parameter(Identifier("args")).WithType(IdentifierName(argInterface))
            );

        return funcParams;
    }

    public static InterfaceDeclarationSyntax RowDeclare(string name, Func<Column, TypeSyntax> columnToType,
        IEnumerable<Column> columns)
    {
        var properties = columns.Select((column, i) =>
            PropertyDeclaration(columnToType(column), Identifier(Utils.ColName(i, column)))
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .WithAccessorList(
                    AccessorList(
                        List(new[]
                        {
                            AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                            AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
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