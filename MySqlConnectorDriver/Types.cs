using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using static System.String;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.MySqlConnectorDriver;

public static class Types
{
    public static string MySqlTypeToCsharpType(this string me, bool notNull)
    {
        var nullableSuffix = notNull ? Empty : "?";

        if (IsNullOrEmpty(me))
            return "object" + nullableSuffix;

        switch (me.ToLower())
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
                throw new NotSupportedException($"Unsupported column type: {me}");
        }
    }

    public static ExpressionSyntax GetColumnReadExpression(Column column, int ordinal)
    {
        if (!column.NotNull)
            return ConditionalExpression(
                GetReadNullCondition(ordinal),
                GetEmptyOrNullExpression(column.Type.Name.MySqlTypeToCsharpType(column.NotNull)),
                GetNullSafeColumnReader(column, ordinal)
            );
        return GetNullSafeColumnReader(column, ordinal);
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
                return ParseExpression($"Utils.GetBytes(reader, {ordinal})");
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

    private static ExpressionSyntax GetEmptyValueForColumn(string localColumnType)
    {
        return localColumnType.ToLower() switch
        {
            "string" => ParseExpression("string.Empty"),
            _ => throw new NotSupportedException($"Unsupported column type: {localColumnType}")
        };
    }
}