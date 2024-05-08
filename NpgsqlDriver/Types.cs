using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using static System.String;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.NpgsqlDriver;

public static class Types
{
    public static string PostgreSqlTypeToCsharpType(this string me, bool notNull)
    {
        var nullableSuffix = notNull ? Empty : "?";

        if (IsNullOrEmpty(me))
            return "object" + nullableSuffix;

        switch (me.ToLower())
        {
            case "serial":
            case "bigserial":
                return "long" + nullableSuffix;
            case "bit":
            case "bytea":
                return "byte[]" + nullableSuffix;
            case "char":
            case "bpchar":
            case "varchar":
            case "text":
            case "date":
            case "time":
            case "timestamp":
                return "string";
            case "decimal":
                return "decimal" + nullableSuffix;
            case "numeric":
            case "float4":
            case "float8":
                return "float" + nullableSuffix;
            case "int2":
            case "int4":
            case "int8":
                return "int" + nullableSuffix;
            case "json":
                return "object" + nullableSuffix;
            case "bool":
            case "boolean":
                return "bool" + nullableSuffix;
            default:
                throw new NotSupportedException($"Unsupported column type: {me}");
        }
    }

    public static ExpressionSyntax GetReadExpression(this Column me, int ordinal)
    {
        if (!me.NotNull)
            return ConditionalExpression(
                GetReadNullCondition(ordinal),
                GetEmptyOrNullExpression(me.Type.Name.PostgreSqlTypeToCsharpType(me.NotNull)),
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
            case "serial":
            case "bigserial":
                return ParseExpression($"reader.GetInt64({ordinal})");
            case "binary":
            case "bit":
            case "bytea":
            case "blob":
            case "longblob":
            case "mediumblob":
            case "tinyblob":
            case "varbinary":
                return ParseExpression($"Utils.GetBytes(reader, {ordinal})");
            case "char":
            case "date":
            case "datetime":
            case "longtext":
            case "mediumtext":
            case "text":
            case "bpchar":
            case "time":
            case "timestamp":
            case "tinytext":
            case "varchar":
            case "json":
                return ParseExpression($"reader.GetString({ordinal})");
            case "double":
                return ParseExpression($"reader.GetDouble({ordinal})");
            case "numeric":
            case "float4":
            case "float8":
                return ParseExpression($"reader.GetFloat({ordinal})");
            case "decimal":
                return ParseExpression($"reader.GetDecimal({ordinal})");
            case "bool":
            case "boolean":
                return ParseExpression($"reader.GetBoolean({ordinal})");
            case "int":
            case "int2":
            case "int4":
            case "int8":
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