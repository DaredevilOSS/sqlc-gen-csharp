using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using static System.String;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.NpgsqlDriver;

public static class Types
{
    public static string MySqlTypeToCsharpType(this string me, bool notNull)
    {
        var nullableSuffix = notNull ? Empty : "?";

        if (IsNullOrEmpty(me))
            return "object" + nullableSuffix;

        switch (me.ToLower())
        {
            case "bigserial":
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
    
    public static ExpressionSyntax GetReadExpression(this Column me, int ordinal)
    {
        if (!me.NotNull)
            return ConditionalExpression(
                GetReadNullCondition(ordinal),
                GetEmptyOrNullExpression(me.Type.Name.MySqlTypeToCsharpType(me.NotNull)),
                GetNullSafeColumnReader(me, ordinal)
            );
        return GetNullSafeColumnReader(me, ordinal);
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
            case "bigserial":
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
}