using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Common;
using static System.String;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace SqlcGenCsharp.Drivers.MySqlConnector;

public static class Types
{
    public static string GetLocalType(string mysqlColumnType, bool notNull)
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

    public static ExpressionSyntax GetEmptyValueForColumn(string localColumnType)
    {
        return localColumnType.ToLower() switch
        {
            "string" => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                PredefinedType(Token(SyntaxKind.StringKeyword)), IdentifierName("Empty")),
            _ => throw new NotSupportedException($"Unsupported column type: {localColumnType}")
        };
    }
    
    public static ExpressionSyntax GetColumnReader(Column column, int ordinal)
    {
        if (!column.NotNull)
            return ConditionalExpression(
                Common.GetNullCondition(ordinal),
                GetEmptyOrNullExpression(column.Type.Name),
                GetNullSafeColumnReader(column, ordinal)
            );
        return GetNullSafeColumnReader(column, ordinal);
    }
    
    public static IEnumerable<ExpressionSyntax> GetColumnsAssignments(IEnumerable<Column> columns)
    {
        return columns.Select((column, idx) =>
            Generators.ColumnAssignment(Types.GetColumnReader(column, idx), column)
        );
    }
    
    private static ExpressionSyntax GetEmptyOrNullExpression(string localType)
    {
        return localType == "string" 
            ? Types.GetEmptyValueForColumn(localType)
            : Generators.NullExpression();
    }
    
        private static ExpressionSyntax GetNullSafeColumnReader(Column column, int ordinal)
    {
        switch (column.Type.Name.ToLower())
        {
            case "bigint":
                return InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(Variables.Reader.GetNameAsVar()),
                            IdentifierName("GetInt64")
                        )
                    )
                    .AddArgumentListArguments(
                        Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(ordinal))));
            case "binary":
            case "bit":
            case "blob":
            case "longblob":
            case "mediumblob":
            case "tinyblob":
            case "varbinary":
                return InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(Variables.Reader.GetNameAsVar()),
                            IdentifierName("GetBytes")
                        )
                    )
                    .AddArgumentListArguments(
                        Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(ordinal))),
                        Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0))),
                        Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal("bytes[]"))),
                        Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(ordinal))),
                        Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(ordinal))));
            case "char":
            case "date":
            case "datetime":
            case "longtext":
            case "mediumtext":
            case "text":
            case "time":
            case "timestamp":
            case "tinytext":
            case "varchar":
            case "json":
                return InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(Variables.Reader.GetNameAsVar()),
                            IdentifierName("GetString")
                        )
                    )
                    .AddArgumentListArguments(
                        Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(ordinal))));
            case "decimal":
                return InvocationExpression(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(Variables.Reader.GetNameAsVar()),
                            IdentifierName("GetDecimal")
                        )
                    )
                    .AddArgumentListArguments(
                        Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(ordinal))));
            case "double":
            case "float":
                return InvocationExpression(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(Variables.Reader.GetNameAsVar()),
                            IdentifierName("GetDouble")
                        )
                    )
                    .AddArgumentListArguments(
                        Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(ordinal))));
            case "int":
            case "mediumint":
            case "smallint":
            case "tinyint":
            case "year":
                return InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(Variables.Reader.GetNameAsVar()),
                            IdentifierName("GetInt32")
                        )
                    )
                    .AddArgumentListArguments(
                        Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(ordinal))));
                ;
            default:
                throw new NotSupportedException($"Unsupported column type: {column.Type.Name}");
        }
    }
    
}