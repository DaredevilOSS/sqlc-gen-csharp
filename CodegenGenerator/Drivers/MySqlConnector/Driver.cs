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
    
    public (UsingDirectiveSyntax[], MemberDeclarationSyntax[]) Preamble(string className, Query[] queries)
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
            .WithPublicAsync()
            .WithParameterList(ParseParameterList(GetParameterListAsString(argInterface, parameters)))
            .WithBody(GetMethodBody());

        BlockSyntax GetMethodBody()
        {
            return Block(new[]
            {
                EstablishConnection(),
                PrepareSqlCommand(queryTextConstant, parameters),
                ExecuteAndReturnOne(returnInterface, columns)
            }.SelectMany(x => x));
        }
    }

    // TODO should be cleaner as an extension method of Column?
    private ExpressionSyntax GetReadExpression(Column column, int ordinal)
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
                return ParseExpression("string.Empty");
            default:
                throw new NotSupportedException($"Unsupported column type: {localColumnType}");
        }
    }

    private static ExpressionSyntax GetReadNullCondition(int ordinal)
    {
        return ParseExpression($"{Variable.Reader.Name()}.IsDBNull({ordinal})");
    }
    
    private static ExpressionSyntax GetEmptyOrNullExpression(string localType)
    {
        return localType == "string" 
            ? GetEmptyValueForColumn(localType)
            : LiteralExpression(SyntaxKind.NullLiteralExpression);
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
            .WithPublicAsync()
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
            .WithPublicAsync()
            .WithParameterList(ParseParameterList(GetParameterListAsString(argInterface, parameters)))
            .WithBody(GetMethodBody());

        return methodDeclaration;
        
        BlockSyntax GetMethodBody()
        {
            return Block(new[]
            {
                EstablishConnection(),
                PrepareSqlCommand(queryTextConstant, parameters),
                [
                    UsingDataReader(),
                    ParseStatement($"var {Variable.Rows.Name()} = new List<{returnInterface}>();"),
                    GetWhileStatement(),
                    ReturnStatement(IdentifierName(Variable.Rows.Name()))
                ]
            }.SelectMany(x => x));
        }
        
        StatementSyntax GetWhileStatement()
        {
            return WhileStatement(
                AwaitReaderRow(),
                Block(
                    ExpressionStatement(
                        InvocationExpression(ParseExpression($"{Variable.Rows.Name()}.Add"))
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList(
                                        Argument(ObjectCreationExpression(IdentifierName(returnInterface))
                                            .WithInitializer(GetRecordInitExpression(columns))
                                        )
                                    )
                                )
                            )
                    )));
        }
    }

    
    private InitializerExpressionSyntax GetRecordInitExpression(IEnumerable<Column> columns)
    {
        return InitializerExpression(
            SyntaxKind.ObjectInitializerExpression,
            SeparatedList(columns.Select((column, idx) =>
                GetReadExpression(column, idx).AssignTo(column.Name.FirstCharToUpper())
            ))
        );
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
            ParseStatement(
                $"await using var {Variable.Connection.Name()} = " +
                $"new MySqlConnection({Variable.ConnectionString.Name()});"),
            ParseStatement($"{Variable.Connection.Name()}.Open();")
        ];
    }

    private static IEnumerable<StatementSyntax> PrepareSqlCommand(string sqlTextConstant, IEnumerable<Parameter> parameters)
    {
        return new[]
        {
            ParseStatement(
                $"await using var {Variable.Command.Name()} = " +
                $"new MySqlCommand({sqlTextConstant}, {Variable.Connection.Name()});")
        }.Concat(
            parameters.Select(param => ParseStatement(
                $"command.Parameters.AddWithValue(\"@{param.Column.Name}\", " +
                $"args.{param.Column.Name.FirstCharToUpper()});"))
        );
    }

    private IEnumerable<StatementSyntax> ExecuteAndReturnOne(string returnInterface, IList<Column> columns)
    {
        return
        [
            UsingDataReader(),
            IfStatement(
                AwaitReaderRow(),
                ReturnSingleRow()
            ),
            ReturnStatement(LiteralExpression(SyntaxKind.NullLiteralExpression))
        ];
        
        StatementSyntax ReturnSingleRow()
        {
            return ReturnStatement(
                ObjectCreationExpression(IdentifierName(returnInterface))
                .WithInitializer(GetRecordInitExpression(columns)));
        }
    }
    
    private static StatementSyntax UsingDataReader()
    {
        return ParseStatement(
            $"await using var {Variable.Reader.Name()} = await {Variable.Command.Name()}.ExecuteReaderAsync();");
    }
    
    private static ExpressionSyntax AwaitReaderRow()
    {
        return ParseExpression($"await {Variable.Reader.Name()}.ReadAsync()");
    }
}