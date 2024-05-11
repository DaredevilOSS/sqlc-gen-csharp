using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class CommonGen(DbDriver dbDriver)
{
    public string AddAwaitIfSupported()
    {
        return dbDriver.DotnetFramework == DotnetFramework.Dotnet80 ? "await " : string.Empty;
    }

    public static ExpressionSyntax AwaitReaderRow()
    {
        return ParseExpression($"await {Variable.Reader.Name()}.ReadAsync()");
    }

    public static string GetParameterListAsString(string argInterface, IEnumerable<Parameter> parameters)
    {
        return "(" + (string.IsNullOrEmpty(argInterface) || !parameters.Any() ? string.Empty : $"{argInterface} args") +
               ")";
    }

    public StatementSyntax UsingDataReader()
    {
        return ParseStatement(
            $"{AddAwaitIfSupported()}using var {Variable.Reader.Name()} = await {Variable.Command.Name()}.ExecuteReaderAsync();");
    }

    public ExpressionSyntax InstantiateDataclass(IEnumerable<Column> columns, string returnInterface)
    {
        var columnsInit = columns
            .Select((column, ordinal) =>
            {
                var readExpression = column.NotNull
                    ? dbDriver.GetColumnReader(column, ordinal)
                    : GetNullableReadExpression(column, ordinal);
                return $"{column.Name.FirstCharToUpper()} = {readExpression}";
            });

        return ParseExpression($$"""
                                 new {{returnInterface}}
                                 {
                                     {{string.Join(",\n", columnsInit)}}
                                 }
                                 """);

        string GetNullableReadExpression(Column column, int ordinal)
        {
            return
                $"{CheckNullExpression(ordinal)} ? {GetNullExpression(column)} : {dbDriver.GetColumnReader(column, ordinal)}";
        }

        string CheckNullExpression(int ordinal)
        {
            return $"{Variable.Reader.Name()}.IsDBNull({ordinal})";
        }

        string GetNullExpression(Column column)
        {
            var csharpType = dbDriver.GetColumnType(column);
            if (csharpType == "string")
                return "string.Empty";
            return !dbDriver.DotnetFramework.NullableEnabled() && Utils.IsCsharpPrimitive(csharpType)
                ? $"({csharpType}) null"
                : "null";
        }
    }
}