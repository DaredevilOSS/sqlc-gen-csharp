using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static System.String;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp;

namespace SqlcGenCsharp.Drivers;

public static class CommonExpressions
{
    public static ExpressionSyntax AwaitReaderRow()
    {
        return ParseExpression($"await {Variable.Reader.Name()}.ReadAsync()");
    }
    
    public static string GetParameterListAsString(string argInterface, IEnumerable<Parameter> parameters)
    {
        return "(" + (IsNullOrEmpty(argInterface) || !parameters.Any() ? Empty : $"{argInterface} args") + ")";
    }
    
    public static StatementSyntax UsingDataReader()
    {
        return ParseStatement(
            $"await using var {Variable.Reader.Name()} = await {Variable.Command.Name()}.ExecuteReaderAsync();");
    }
    
    public static InitializerExpressionSyntax GetRecordInitExpression(IEnumerable<Column> columns, IDbDriver dbDriver)
    {
        return InitializerExpression(
            SyntaxKind.ObjectInitializerExpression,
            SeparatedList(columns
                .Select((column, ordinal) => 
                    GetColumnReadExpression(column, ordinal, dbDriver)
                        .AssignToVariable(column.Name.FirstCharToUpper())
            ))
        );
    }
    
    public static ExpressionSyntax GetColumnReadExpression(Column column, int ordinal, IDbDriver dbDriver)
    {
        if (!column.NotNull)
            return ConditionalExpression(
                GetReadNullCondition(ordinal),
                GetEmptyOrNullExpression(dbDriver.ColumnType(column)),
                dbDriver.ColumnReader(column, ordinal)
            );
        return dbDriver.ColumnReader(column, ordinal);
    }
    
    private static ExpressionSyntax GetEmptyOrNullExpression(string localType)
    {
        return localType == "string"
            ? GetEmptyValueForColumn(localType)
            : LiteralExpression(SyntaxKind.NullLiteralExpression);
    }
    
    private static ExpressionSyntax GetEmptyValueForColumn(string localColumnType)
    {
        return localColumnType.ToLower() switch
        {
            "string" => ParseExpression("string.Empty"),
            _ => throw new NotSupportedException($"Unsupported column type: {localColumnType}")
        };
    }
    
    private static ExpressionSyntax GetReadNullCondition(int ordinal)
    {
        return ParseExpression($"{Variable.Reader.Name()}.IsDBNull({ordinal})");
    }
}