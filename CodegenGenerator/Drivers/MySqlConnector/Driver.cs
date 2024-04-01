using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static System.String;
using SqlcGenCsharp.Drivers.Common;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.MySqlConnector;

public class Driver : IDbDriver
{
    public static ParameterListSyntax FuncParamsDecl(string argInterface, IList<Parameter> parameters)
    {
        return ParameterList(
            SeparatedList(
                new List<ParameterSyntax>(
                    !IsNullOrEmpty(argInterface) && parameters.Any()
                        ? new[] { Parameter(Identifier("args")).WithType(IdentifierName(argInterface)) }
                        : Enumerable.Empty<ParameterSyntax>())));
    }
    
    public string ColumnType(string columnType, bool notNull)
    {
        return Types.GetLocalType(columnType, notNull);
    }

    public (UsingDirectiveSyntax[], MemberDeclarationSyntax[]) Preamble(Query[] queries)
    {
        return (
            PreambleMembers.GetUsingDirectives(),
            PreambleMembers.GetClassMembers()
        );
    }

    public ParameterListSyntax Params(string argInterface, IEnumerable<Parameter> parameters)
    {
        return ParameterList(
            SeparatedList(
                new List<ParameterSyntax>(
                    !IsNullOrEmpty(argInterface) && parameters.Any()
                        ? new[] { Parameter(Identifier("args")).WithType(IdentifierName(argInterface)) }
                        : Enumerable.Empty<ParameterSyntax>())));
    }
    
    public MemberDeclarationSyntax OneDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        return MethodDeclaration(
                GenericName(Identifier("Task"))
                    .WithTypeArgumentList(IdentifierName(returnInterface).GetGenericListOf()),
                Identifier(funcName))
            .AddModifiers(Generators.GetSyntaxTokens())
            .WithParameterList(Params(argInterface, parameters))
            .WithBody(Block(
                Array.Empty<StatementSyntax>()
                    .Concat(EstablishConnection())
                    .Concat(PrepareSqlCommand(queryTextConstant, parameters))
                    .Concat(ExecuteAndReturn(returnInterface, columns))));
    }
    
    public MemberDeclarationSyntax ExecDeclare(string funcName, string queryTextConstant, string argInterface,
        IList<Parameter> parameters)
    {
        var methodDeclaration = MethodDeclaration(IdentifierName("Task"), Identifier(funcName))
            .AddModifiers(Generators.GetSyntaxTokens())
            .WithParameterList(FuncParamsDecl(argInterface, parameters))
            .WithBody(GetBlock());

        return methodDeclaration;

        BlockSyntax GetBlock()
        {
            var blockStatements = new StatementSyntax[]{}
                .Concat(EstablishConnection())
                .Concat(PrepareSqlCommand(queryTextConstant, parameters))
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
            .AddModifiers(Generators.GetSyntaxTokens())
            .WithParameterList(funcParams)
            .WithBody(GetBlock());

        return methodDeclaration;

        BlockSyntax GetBlock()
        {
            var blockStatements = new StatementSyntax[] {}
                .Concat(EstablishConnection())
                .Concat(PrepareSqlCommand(queryTextConstant, parameters))
                .Concat(new[]
                {
                    UsingReaderVar(),
                    Generators.DeclareResultRowsVar(IdentifierName(returnInterface)),
                    GetWhileRowExistsStatement(),
                    IdentifierName(Variables.Rows.GetNameAsVar()).Return()
                })
                .ToArray();
            return Block(blockStatements);
        }

        WhileStatementSyntax GetWhileRowExistsStatement()
        {
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
                                                        SeparatedList(Types.GetColumnsAssignments(columns))
                                                    )
                                                )
                                        )
                                    )
                                )
                            )
                    )));
        }
    }
    
    
    public IEnumerable<StatementSyntax> EstablishConnection()
    {
        return
        [
            LocalDeclarationStatement(
                    VariableDeclaration(IdentifierName("var"))
                        .WithVariables(SingletonSeparatedList(
                            VariableDeclarator(Identifier(Variables.Connection.GetNameAsVar()))
                                .WithInitializer(
                                    EqualsValueClause(ObjectCreationExpression(IdentifierName("MySqlConnection"))
                                        .AddArgumentListArguments(
                                            Argument(IdentifierName(Variables.ConnectionString
                                                .GetNameAsConst()))))))))
                .WithAwaitUsing(),
            ExpressionStatement(
                InvocationExpression(MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("connection"),
                    IdentifierName("Open")))),
        ];
    }


    public IEnumerable<StatementSyntax> PrepareSqlCommand(string sqlTextConstant, IEnumerable<Parameter> parameters)
    {
        return new StatementSyntax[]
            {
                LocalDeclarationStatement(
                    VariableDeclaration(IdentifierName("var"))
                        .WithVariables(
                            SingletonSeparatedList(
                                VariableDeclarator(Identifier("command"))
                                    .WithInitializer(EqualsValueClause(
                                        ObjectCreationExpression(IdentifierName("MySqlCommand"))
                                            .AddArgumentListArguments(
                                                Argument(IdentifierName(sqlTextConstant)),
                                                Argument(IdentifierName(Variables.Connection.GetNameAsVar()))
                                            )
                                    ))
                            )
                        )
                ).WithAwaitUsing()
            }.Concat(
                parameters.Select(
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
                )
            )
            .ToArray();
    }

    IEnumerable<StatementSyntax> IDbDriver.ExecuteAndReturn(string returnInterface, IList<Column> columns)
    {
        return ExecuteAndReturn(returnInterface, columns);
    }
    
    
    private static IfStatementSyntax GetIfRowExistsStatement(string returnInterface, IList<Column> columns)
    {
        return IfStatement(
            Common.AwaitReaderRow(),
            Block(
                SingletonList<StatementSyntax>(
                    ReturnStatement(
                        ObjectCreationExpression(IdentifierName(returnInterface))
                            .WithInitializer(
                                InitializerExpression(
                                    SyntaxKind.ObjectInitializerExpression,
                                    SeparatedList(Types.GetColumnsAssignments(columns)
                                        .ToArray())
                                )
                            )
                    )
                )
            )
        );
    }

    public IEnumerable<StatementSyntax> ExecuteAndReturn(string returnInterface, IList<Column> columns)
    {
        return
        [
            UsingReaderVar(),
            GetIfRowExistsStatement(returnInterface, columns),
            ReturnStatement(Generators.NullExpression())
        ];
    }

    
    private static IfStatementSyntax GetIfRowExistsStatement(string returnInterface, IList<Column> columns)
    {
        return IfStatement(
            Common.AwaitReaderRow(),
            Block(
                SingletonList<StatementSyntax>(
                    ReturnStatement(
                        ObjectCreationExpression(IdentifierName(returnInterface))
                            .WithInitializer(
                                InitializerExpression(
                                    SyntaxKind.ObjectInitializerExpression,
                                    SeparatedList(Types.GetColumnsAssignments(columns)
                                        .ToArray())
                                )
                            )
                    )
                )
            )
        );
    }
    
    protected static StatementSyntax UsingReaderVar()
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

    AwaitExpressionSyntax AwaitReaderRow()
    {
        return AwaitExpression(
            InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(Variables.Reader.GetNameAsVar()),
                    IdentifierName("ReadAsync")
                )
            )
        );
    }
}