using Plugin;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SqlcGenCsharp.Drivers.Generators;

public class CommonGen(DbDriver dbDriver)
{
    public static string AwaitReaderRow()
    {
        return $"await {Variable.Reader.AsVarName()}.ReadAsync()";
    }

    public static string GetMethodParameterList(string argInterface, IEnumerable<Parameter> parameters)
    {
        return $"{(string.IsNullOrEmpty(argInterface) || !parameters.Any()
            ? string.Empty
            : $"{argInterface} {Variable.Args.AsVarName()}")}";
    }

    public static string GetParameterListForDapper(IList<Parameter> parameters)
    {
        var parametersStr = parameters
            .Select(p => p.Column.Name + "=args." + p.Column.Name.ToPascalCase() + "");
        return parameters.Count > 0
            ? ", new { " + string.Join(", ", parametersStr) + "}"
            : string.Empty;
    }

    public static string InitDataReader()
    {
        return $"var {Variable.Reader.AsVarName()} = await {Variable.Command.AsVarName()}.ExecuteReaderAsync()";
    }

    public string InstantiateDataclass(IEnumerable<Column> columns, string returnInterface)
    {
        var columnsInit = columns
            .Select((column, ordinal) =>
            {
                var readExpression = column.NotNull
                    ? dbDriver.GetColumnReader(column, ordinal)
                    : GetNullableReadExpression(column, ordinal);
                return $"{column.Name.ToPascalCase()} = {readExpression}";
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
            return $"{Variable.Reader.AsVarName()}.IsDBNull({ordinal})";
        }

        string GetNullExpression(Column column)
        {
            var csharpType = dbDriver.GetColumnType(column);
            if (csharpType == "string")
                return "string.Empty";
            return !dbDriver.Options.DotnetFramework.LatestDotnetSupported() && dbDriver.IsTypeNullableForAllRuntimes(csharpType)
                ? $"({csharpType}) null"
                : "null";
        }
    }

    public IList<string> GetCommandParameters(IEnumerable<Parameter> parameters)
    {
        return parameters.Select(p =>
        {
            var varName = Variable.Command.AsVarName();
            var columnName = p.Column.Name;
            var param = p.Column.Name.ToPascalCase();
            var nullCheck = dbDriver.Options.DotnetFramework.LatestDotnetSupported() && !p.Column.NotNull ? "!" : "";
            if (p.Column.IsSqlcSlice == true)
            {
                return $$"""
                         foreach (var (value, i) in args.Ids.Select((v, i) => (v, i)))
                         {
                            {{varName}}.Parameters.AddWithValue($"@{nameof(args.{{param}}{{nullCheck}})}Arg{i}", value);
                         }
                         """;
            }
            return $"{varName}.Parameters.AddWithValue(\"@{columnName}\", args.{param}{nullCheck});";
        }).ToList();
    }
}