using System.Collections.Generic;
using System.Linq;
using Plugin;

namespace SqlcGenCsharp.Drivers.Generators;

public class CommonGen(DbDriver dbDriver)
{
    public static string AwaitReaderRow()
    {
        return $"await {Variable.Reader.Name()}.ReadAsync()";
    }

    public static string GetParameterListAsString(string argInterface, IEnumerable<Parameter> parameters)
    {
        return $"{(string.IsNullOrEmpty(argInterface) || !parameters.Any() ? string.Empty : $"{argInterface} args")}";
    }

    public static string InitDataReader()
    {
        return $"var {Variable.Reader.Name()} = await {Variable.Command.Name()}.ExecuteReaderAsync()";
    }

    public string InstantiateDataclass(IEnumerable<Column> columns, string returnInterface)
    {
        var columnsInit = columns
            .Select((column, ordinal) =>
            {
                var readExpression = column.NotNull
                    ? dbDriver.GetColumnReader(column, ordinal)
                    : GetNullableReadExpression(column, ordinal);
                return $"{column.Name.FirstCharToUpper()} = {readExpression}";
            });

        return $$"""
            new {{returnInterface}}
            {
                {{string.Join(",\n", columnsInit)}}
            }
            """;

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

    public IEnumerable<string> GetCommandParameters(IEnumerable<Parameter> parameters)
    {
        return parameters.Select(param =>
            $"{Variable.Command.Name()}.Parameters.AddWithValue(\"@{param.Column.Name}\", " +
            $"args.{param.Column.Name.FirstCharToUpper()});");
    }
}