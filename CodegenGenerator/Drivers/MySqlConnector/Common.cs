using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Common;
using static System.String;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.MySqlConnector;

public static class Common
{
    public static LocalDeclarationStatementSyntax UsingConnectionVar()
    {
        return LocalDeclarationStatement(
                VariableDeclaration(IdentifierName("var"))
                    .WithVariables(SingletonSeparatedList(
                        VariableDeclarator(Identifier(Variables.Connection.GetNameAsVar()))
                            .WithInitializer(
                                EqualsValueClause(ObjectCreationExpression(IdentifierName("MySqlConnection"))
                                    .AddArgumentListArguments(
                                        Argument(IdentifierName(Variables.ConnectionString
                                            .GetNameAsConst()))))))))
            .WithAwaitUsing();
    }
    
    public static ExpressionStatementSyntax ConnectionOpen()
    {
        return ExpressionStatement(
            InvocationExpression(MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName("connection"),
                IdentifierName("Open"))));
    }
    
    public static StatementSyntax UsingReaderVar()
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

    public static ParameterListSyntax FuncParamsDecl(string argInterface, IList<Parameter> parameters)
    {
        return ParameterList(
            SeparatedList(
                new List<ParameterSyntax>(
                    !IsNullOrEmpty(argInterface) && parameters.Any()
                        ? new[] { Parameter(Identifier("args")).WithType(IdentifierName(argInterface)) }
                        : Enumerable.Empty<ParameterSyntax>())));
    }
    
    public static ExpressionStatementSyntax[] AddParametersToCommand(IEnumerable<Parameter> parameters)
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
    
    public static ExpressionSyntax GetNullCondition(int ordinal)
    {
        return InvocationExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName("reader"),
                IdentifierName("IsDBNull")
            )
        ).AddArgumentListArguments(
            Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(ordinal))));
    }
    
    public static LocalDeclarationStatementSyntax UsingCommandVar(string sqlTextConstant)
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
                                        Argument(IdentifierName(Variables.Connection.GetNameAsVar()))
                                    )
                            ))
                    )
                )
        ).WithAwaitUsing();
    }
    
    public static AwaitExpressionSyntax AwaitReaderRow()
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