using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Common;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.MySqlConnector;

public class Driver : IDbDriver
{
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
    
    public MemberDeclarationSyntax OneDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        var funcParams = Common.FuncParamsDecl(argInterface, parameters);
        var returnType = GenericName(Identifier("Task"))
            .WithTypeArgumentList(IdentifierName(returnInterface).GetGenericListOf());
        var methodDeclaration = MethodDeclaration(returnType, Identifier(funcName))
            .AddModifiers(Generators.GetQueryMethodsTokens())
            .WithParameterList(funcParams)
            .WithBody(OneDeclareMembers.GetBlock(returnInterface, queryTextConstant, parameters, columns));

        return methodDeclaration;
    }
    
    public MemberDeclarationSyntax ExecDeclare(string funcName, string queryTextConstant, string argInterface,
        IList<Parameter> parameters)
    {
        var funcParams = Common.FuncParamsDecl(argInterface, parameters);

        // Creating the method declaration
        var methodDeclaration = MethodDeclaration(IdentifierName("Task"), Identifier(funcName))
            .AddModifiers(Generators.GetQueryMethodsTokens())
            .WithParameterList(funcParams)
            .WithBody(GetBlock());

        return methodDeclaration;

        BlockSyntax GetBlock()
        {
            var blockStatements = new StatementSyntax[]
                {
                    Common.UsingConnectionVar(),
                    Common.ConnectionOpen(),
                    Common.UsingCommandVar(queryTextConstant)
                }
                .Concat(Common.AddParametersToCommand(parameters))
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
        var funcParams = Common.FuncParamsDecl(argInterface, parameters);
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
            .AddModifiers(Generators.GetQueryMethodsTokens())
            .WithParameterList(funcParams)
            .WithBody(GetBlock());

        return methodDeclaration;

        BlockSyntax GetBlock()
        {
            var blockStatements = new StatementSyntax[]
                {
                    Common.UsingConnectionVar(),
                    Common.ConnectionOpen(),
                    Common.UsingCommandVar(queryTextConstant)
                }
                .Concat(Common.AddParametersToCommand(parameters))
                .Concat(new[]
                {
                    Common.UsingReaderVar(),
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
                Common.AwaitReaderRow(),
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
}