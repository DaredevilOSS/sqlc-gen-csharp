using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static System.String;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public class MySqlConnector : IDbDriver
{
    public TypeSyntax ColumnType(string columnType, bool notNull)
    {
        var nullableSuffix = notNull ? Empty : "?";

        if (IsNullOrEmpty(columnType))
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

    public (UsingDirectiveSyntax[], MemberDeclarationSyntax[]) Preamble(Query[] queries)
    {
        return (
            GetUsingDirectives(),
            GetClassProperties()
        );
        
        UsingDirectiveSyntax[] GetUsingDirectives()
        {
            return
            [
                UsingDirective(ParseName("System")),
                UsingDirective(ParseName("System.Threading.Tasks")),
                UsingDirective(ParseName("MySqlConnector"))
            ];
        }

        MemberDeclarationSyntax[] GetClassProperties()
        {
            return
            [
                FieldDeclaration(
                        VariableDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)))
                            .WithVariables(SingletonSeparatedList(
                                VariableDeclarator(Identifier(GeneratedMember.ConnectionString.GetNameAsConst()))
                                    .WithInitializer(EqualsValueClause(
                                        LiteralExpression(SyntaxKind.StringLiteralExpression,
                                            Literal("server=localhost;user=root;database=mydb;port=3306;password=")))))
                            ))
                    .WithModifiers(TokenList(
                        Token(SyntaxKind.PrivateKeyword),
                        Token(SyntaxKind.ConstKeyword)))
            ];
        }

    }

    public MemberDeclarationSyntax OneDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        var funcParams = FuncParamsDecl(argInterface, parameters);
        var returnType = GenericName(Identifier("Task"))
            .WithTypeArgumentList(CommonGenerators.GetGenericListOfInputType(returnInterface));
        var methodDeclaration = MethodDeclaration(returnType, Identifier(funcName))
            .AddModifiers(CommonGenerators.GetQueryMethodsTokens())
            .WithParameterList(funcParams)
            .WithBody(GetBlock());

        return methodDeclaration;

        BlockSyntax GetBlock()
        {
            var blockStatements = new StatementSyntax[]
                {
                    UsingConnectionVar(),
                    CommonGenerators.ConnectionOpen(),
                    UsingCommandVar(queryTextConstant)
                }
                .Concat(AddParametersToCommand(parameters))
                .Concat(new[]
                {
                    UsingReaderVar(),
                    GetIfRowExistsStatement(),
                    CommonGenerators.ReturnNull()
                })
                .ToArray();
            return Block(blockStatements);
        }

        IfStatementSyntax GetIfRowExistsStatement()
        {
            return IfStatement(
                AwaitReaderRow(),
                Block(
                    SingletonList<StatementSyntax>(
                        ReturnStatement(
                            ObjectCreationExpression(IdentifierName(returnInterface))
                                .WithInitializer(
                                    InitializerExpression(
                                        SyntaxKind.ObjectInitializerExpression,
                                        SeparatedList(columns.Select((column, idx) =>
                                                AssignmentExpression(
                                                    SyntaxKind.SimpleAssignmentExpression,
                                                    IdentifierName(column.Name.FirstCharToUpper()),
                                                    InvocationExpression(
                                                        GetReaderType(column.Type.Name, column.NotNull)
                                                    ).AddArgumentListArguments(Argument(
                                                        LiteralExpression(
                                                            SyntaxKind.NumericLiteralExpression,
                                                            Literal(idx))))
                                                ))
                                            .Cast<ExpressionSyntax>()
                                            .ToArray())
                                    )
                                )
                        )
                    )
                )
            );
        }
    }

    public MemberDeclarationSyntax ExecDeclare(string funcName, string queryTextConstant, string argInterface,
        IList<Parameter> parameters)
    {
        var funcParams = FuncParamsDecl(argInterface, parameters);

        // Creating the method declaration
        var methodDeclaration = MethodDeclaration(IdentifierName("Task"), Identifier(funcName))
            .AddModifiers(CommonGenerators.GetQueryMethodsTokens())
            .WithParameterList(funcParams)
            .WithBody(GetBlock());

        return methodDeclaration;

        BlockSyntax GetBlock()
        {
            var blockStatements = new StatementSyntax[]
                {
                    UsingConnectionVar(),
                    CommonGenerators.ConnectionOpen(),
                    UsingCommandVar(queryTextConstant)
                }
                .Concat(AddParametersToCommand(parameters))
                .Concat(new[]
                {
                    UsingExecuteScalarVar()
                })
                .ToArray();
            return Block(blockStatements);
        }
        
        StatementSyntax UsingExecuteScalarVar()
        {
            return ExpressionStatement(
                AwaitExpression(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("command"),
                            IdentifierName("ExecuteScalarAsync")
                        )
                    )
                )
            );
        }
    }

    public MemberDeclarationSyntax ManyDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        var funcParams = FuncParamsDecl(argInterface, parameters);
        var returnType = GenericName(Identifier("Task"))
            .WithTypeArgumentList(
                TypeArgumentList(
                    SingletonSeparatedList<TypeSyntax>(
                        GenericName(Identifier("List"))
                            .WithTypeArgumentList(
                                TypeArgumentList(
                                    SingletonSeparatedList<TypeSyntax>(IdentifierName(returnInterface))
                                )
                            ))));

        // Method declaration
        var methodDeclaration = MethodDeclaration(returnType, Identifier(funcName))
            .AddModifiers(CommonGenerators.GetQueryMethodsTokens())
            .WithParameterList(funcParams)
            .WithBody(GetBlock());

        return methodDeclaration;

        BlockSyntax GetBlock()
        {
            var blockStatements = new StatementSyntax[]
                {
                    UsingConnectionVar(),
                    CommonGenerators.ConnectionOpen(),
                    UsingCommandVar(queryTextConstant)
                }
                .Concat(AddParametersToCommand(parameters))
                .Concat(new[]
                {
                    UsingReaderVar(),
                    CommonGenerators.DeclareResultRowsVar(returnInterface),
                    GetWhileRowExistsStatement(),
                    CommonGenerators.ReturnRowsVar()
                })
                .ToArray();
            return Block(blockStatements);
        }

        WhileStatementSyntax GetWhileRowExistsStatement()
        {
            var initializerExpressions = columns.Select((column, idx) =>
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(column.Name.FirstCharToUpper()),
                    InvocationExpression(
                            GetReaderType(column.Type.Name, column.NotNull)
                        )
                        .AddArgumentListArguments(
                            Argument(
                                LiteralExpression(
                                    SyntaxKind.NumericLiteralExpression,
                                    Literal(idx)
                                )
                            )
                        )
                )
            ).Cast<ExpressionSyntax>().ToArray();

            return WhileStatement(
                AwaitReaderRow(),
                Block(
                    ExpressionStatement(
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("rows"),
                                    IdentifierName("Add")
                                )
                            )
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList(
                                        Argument(
                                            ObjectCreationExpression(
                                                    IdentifierName("ListAuthorsRow")
                                                )
                                                .WithInitializer(
                                                    InitializerExpression(
                                                        SyntaxKind.ObjectInitializerExpression,
                                                        SeparatedList(initializerExpressions)
                                                    )
                                                )
                                        )
                                    )
                                )
                            )
                    )));
        }
    }

    private static MemberAccessExpressionSyntax GetReaderType(string columnType, bool notNull)
    {
        switch (columnType.ToLower())
        {
            case "bigint":
                return MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(GeneratedMember.Reader.GetNameAsVar()),
                    IdentifierName("GetInt64")
                );
            case "binary":
            case "bit":
            case "blob":
            case "longblob":
            case "mediumblob":
            case "tinyblob":
            case "varbinary":
                return MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(GeneratedMember.Reader.GetNameAsVar()),
                    IdentifierName("GetBytes")
                );
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
            case "json":
                return MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(GeneratedMember.Reader.GetNameAsVar()),
                    IdentifierName("GetString")
                );
            case "double":
            case "float":
                return MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(GeneratedMember.Reader.GetNameAsVar()),
                    IdentifierName("GetDouble")
                );
            case "int":
            case "mediumint":
            case "smallint":
            case "tinyint":
            case "year":
                return MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(GeneratedMember.Reader.GetNameAsVar()),
                    IdentifierName("GetInt32")
                );
            default:
                throw new NotSupportedException($"Unsupported column type: {columnType}");
        }
    }
    
    private AwaitExpressionSyntax AwaitReaderRow()
    {
        return AwaitExpression(
            InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(GeneratedMember.Reader.GetNameAsVar()),
                    IdentifierName("ReadAsync")
                )
            )
        );
    }

    private static LocalDeclarationStatementSyntax UsingConnectionVar()
    {
        return LocalDeclarationStatement(
                VariableDeclaration(IdentifierName("var"))
                    .WithVariables(SingletonSeparatedList(
                        VariableDeclarator(Identifier(GeneratedMember.Connection.GetNameAsVar()))
                            .WithInitializer(
                                EqualsValueClause(ObjectCreationExpression(IdentifierName("MySqlConnection"))
                                    .AddArgumentListArguments(
                                        Argument(IdentifierName(GeneratedMember.ConnectionString
                                            .GetNameAsConst()))))))))
            .WithAwaitUsing();
    }

    private LocalDeclarationStatementSyntax UsingCommandVar(string sqlTextConstant)
    {
        return LocalDeclarationStatement(
            VariableDeclaration(IdentifierName("var"))
                .WithVariables(
                    SingletonSeparatedList(
                        VariableDeclarator(Identifier("command"))
                            .WithInitializer(EqualsValueClause(
                                ObjectCreationExpression(IdentifierName("MySqlCommand"))
                                    .AddArgumentListArguments(
                                        Argument(IdentifierName(sqlTextConstant)),
                                        Argument(IdentifierName(GeneratedMember.Connection.GetNameAsVar()))
                                    )
                            ))
                    )
                )
        ).WithAwaitUsing();
    }

    private StatementSyntax UsingReaderVar()
    {
        return LocalDeclarationStatement(
                VariableDeclaration(IdentifierName("var"))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(Identifier("reader"))
                                .WithInitializer(EqualsValueClause(
                                        AwaitExpression(
                                            InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("command"),
                                                    IdentifierName("ExecuteReaderAsync")
                                                )
                                            )
                                        )
                                    )
                                ))))
            .WithAwaitUsing();
    }

    private static ExpressionStatementSyntax[] AddParametersToCommand(IEnumerable<Parameter> parameters)
    {
        return parameters.Select(
            param => ExpressionStatement(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("command"),
                                IdentifierName("Parameters")),
                            IdentifierName("AddWithValue")))
                    .AddArgumentListArguments(
                        Argument(
                            LiteralExpression(
                                SyntaxKind.StringLiteralExpression, Literal($"@{param.Column.Name}"))),
                        Argument(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression, IdentifierName("args"),
                            IdentifierName($"{param.Column.Name.FirstCharToUpper()}")))
                    )
            )
        ).ToArray();
    }

    private static ParameterListSyntax FuncParamsDecl(string argInterface, IList<Parameter> parameters)
    {
        return ParameterList(
            SeparatedList(
                new List<ParameterSyntax>(
                    !IsNullOrEmpty(argInterface) && parameters.Any()
                        ? new[] { Parameter(Identifier("args")).WithType(IdentifierName(argInterface)) }
                        : Enumerable.Empty<ParameterSyntax>())));
    }
}