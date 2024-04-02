using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Common;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static System.String;

namespace SqlcGenCsharp.Drivers.MySqlConnector;

public class Driver : IDbDriver
{
    public string ColumnType(string mysqlColumnType, bool notNull)
    {
        var nullableSuffix = notNull ? Empty : "?";

        if (IsNullOrEmpty(mysqlColumnType))
            return "object" + nullableSuffix;

        switch (mysqlColumnType.ToLower())
        {
            case "bigint":
                return "long" + nullableSuffix;
            case "binary":
            case "bit":
            case "blob":
            case "longblob":
            case "mediumblob":
            case "tinyblob":
            case "varbinary":
                return "byte[]" + nullableSuffix;
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
                return "string";
            case "double":
            case "float":
                return "double" + nullableSuffix;
            case "int":
            case "mediumint":
            case "smallint":
            case "tinyint":
            case "year":
                return "int" + nullableSuffix;
            case "json":
                // Assuming JSON is represented as a string or a specific class
                return "object" + nullableSuffix;
            default:
                throw new NotSupportedException($"Unsupported column type: {mysqlColumnType}");
        }
    }


    // TODO add ExecLastId handling
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
        return MethodDeclaration(IdentifierName($"Task<{returnInterface}?>"), funcName)
            .WithPublicStaticAsyncModifiers()
            .WithParameterList(ParseParameterList(GetParameterListAsString(argInterface, parameters)))
            .WithBody(Block(
                Array.Empty<StatementSyntax>()
                    .Concat(EstablishConnection())
                    .Concat(PrepareSqlCommand(queryTextConstant, parameters))
                    .Concat(ExecuteAndReturnOne(returnInterface, columns))));
    }

    private ExpressionSyntax GetReader(Column column, int ordinal)
    {
        if (!column.NotNull)
            return ConditionalExpression(
                GetReadNullCondition(ordinal),
                GetEmptyOrNullExpression(ColumnType(column.Type.Name, column.NotNull)),
                GetNullSafeColumnReader(column, ordinal)
            );
        return GetNullSafeColumnReader(column, ordinal);
    }

    private static ExpressionSyntax GetEmptyValueForColumn(string localColumnType)
    {
        switch (localColumnType.ToLower())
        {
            case "string":
                return ParseExpression("String.Empty");
            default:
                throw new NotSupportedException($"Unsupported column type: {localColumnType}");
        }
    }

    private static ExpressionSyntax GetReadNullCondition(int ordinal)
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
    
    private static ExpressionSyntax GetEmptyOrNullExpression(string localType)
    {
        return localType == "string" 
            ? GetEmptyValueForColumn(localType)
            : Utils.NullExpression();
    }
    
    private static ExpressionSyntax GetNullSafeColumnReader(Column column, int ordinal)
    {
        switch (column.Type.Name.ToLower())
        {
            case "bigint":
                return ParseExpression($"reader.GetInt64({ordinal})");
            case "binary":
            case "bit":
            case "blob":
            case "longblob":
            case "mediumblob":
            case "tinyblob":
            case "varbinary":
                return ParseExpression($"GetBytes(reader, {ordinal})");
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
                return ParseExpression($"reader.GetString({ordinal})");
            case "double":
            case "float":
                return ParseExpression($"reader.GetDouble({ordinal})");
            case "int":
            case "mediumint":
            case "smallint":
            case "tinyint":
            case "year":
                return ParseExpression($"reader.GetInt32({ordinal})");
            default:
                throw new NotSupportedException($"Unsupported column type: {column.Type.Name}");
        }
    }
    
    public MemberDeclarationSyntax ExecDeclare(string funcName, string queryTextConstant, string argInterface,
        IList<Parameter> parameters)
    {
        var methodDeclaration = MethodDeclaration(IdentifierName("Task"), Identifier(funcName))
            .WithPublicStaticAsyncModifiers()
            .WithParameterList(ParseParameterList(GetParameterListAsString(argInterface, parameters)))
            .WithBody(Block(
                Array.Empty<StatementSyntax>()
                    .Concat(EstablishConnection())
                    .Concat(PrepareSqlCommand(queryTextConstant, parameters))
                    .Concat(ExecuteScalarAndReturn())));
        return methodDeclaration;
    }

    public MemberDeclarationSyntax ManyDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        var methodDeclaration = MethodDeclaration(IdentifierName($"Task<List<{returnInterface}>>"), Identifier(funcName))
            .WithPublicStaticAsyncModifiers()
            .WithParameterList(ParseParameterList(GetParameterListAsString(argInterface, parameters)))
            .WithBody(GetBlock());

        return methodDeclaration;

        BlockSyntax GetBlock()
        {
            var blockStatements = new StatementSyntax[] {}
                .Concat(EstablishConnection())
                .Concat(PrepareSqlCommand(queryTextConstant, parameters))
                .Concat(new[]
                {
                    UsingDataReader(),
                    Utils.DeclareResultRowsVar(returnInterface),
                    GetWhileRowExistsStatement(),
                    ReturnStatement(IdentifierName(Variables.Rows.GetNameAsVar()))
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
                                                        SeparatedList(columns.Select((column, idx) =>
                                                            Utils.AssignToColumn(GetReader(column, idx), column)
                                                        ))
                                                    )
                                                )
                                        )
                                    )
                                )
                            )
                    )));
        }
    }

    private static string GetParameterListAsString(string argInterface, IList<Parameter> parameters)
    {
        return "(" + (IsNullOrEmpty(argInterface) || !parameters.Any() ? Empty : $"{argInterface} args") + ")";
    }

    private static IEnumerable<StatementSyntax> ExecuteScalarAndReturn()
    {
        return 
        [
            ExpressionStatement(
                AwaitExpression(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("command"),
                            IdentifierName("ExecuteScalarAsync")
                        )
                    )
                )
            )
        ];
    }

    private static IEnumerable<StatementSyntax> EstablishConnection()
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

    private static IEnumerable<StatementSyntax> PrepareSqlCommand(string sqlTextConstant, IEnumerable<Parameter> parameters)
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

    private IEnumerable<StatementSyntax> ExecuteAndReturnOne(string returnInterface, IList<Column> columns)
    {
        return
        [
            UsingDataReader(),
            IfStatement(
                AwaitReaderRow(),
                ReturnSingleRow(returnInterface, columns)
            ),
            ReturnStatement(Utils.NullExpression())
        ];
    }

    private StatementSyntax ReturnSingleRow(string returnInterface, IEnumerable<Column> columns)
    {
        return ReturnStatement(
            ObjectCreationExpression(IdentifierName(returnInterface))
                .WithInitializer(
                    InitializerExpression(
                        SyntaxKind.ObjectInitializerExpression,
                        SeparatedList(columns.Select((column, idx) =>
                                Utils.AssignToColumn(GetReader(column, idx), column))
                            .ToArray())
                    )
                )
        );
    }

    private static StatementSyntax UsingDataReader()
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


    private static AwaitExpressionSyntax AwaitReaderRow()
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