using Plugin;
using System.Collections.Generic;
using System.Linq;

namespace SqlcGenCsharp.Drivers.Generators;

public class CommonGen(DbDriver dbDriver)
{
    public static string GetMethodParameterList(string argInterface, IEnumerable<Parameter> parameters)
    {
        return $"{(string.IsNullOrEmpty(argInterface) || !parameters.Any()
            ? string.Empty
            : $"{argInterface} {Variable.Args.AsVarName()}")}";
    }

    public string AddParametersToCommand(IEnumerable<Parameter> parameters)
    {
        return parameters.Select(p =>
        {
            var commandVar = Variable.Command.AsVarName();
            var param = p.Column.Name.ToPascalCase();
            var nullCheck = dbDriver.Options.DotnetFramework.LatestDotnetSupported() && !p.Column.NotNull ? "!" : "";
            return p.Column.IsSqlcSlice
                ? $$"""
                    for (int i = 0; i < {{Variable.Args.AsVarName()}}.{{param}}.Length; i++)
                       {{commandVar}}.Parameters.AddWithValue($"@{{param}}Arg{i}", {{Variable.Args.AsVarName()}}.{{param}}[i]);
                    """
                : $"{commandVar}.Parameters.AddWithValue(\"@{p.Column.Name}\", args.{param}{nullCheck});";
        }).JoinByNewLine();
    }

    public static string ConstructDapperParamsDict(IList<Parameter> parameters)
    {
        if (!parameters.Any()) return string.Empty;
        var initParamsDict = $"var {Variable.QueryParams.AsVarName()} = new Dictionary<string, object>();";
        var dapperParamsCommands = parameters.Select(p =>
        {
            var param = p.Column.Name.ToPascalCase();
            return p.Column.IsSqlcSlice
                ? $$"""
                    for (int i = 0; i < {{Variable.Args.AsVarName()}}.{{param}}.Length; i++)
                       {{Variable.QueryParams.AsVarName()}}.Add($"@{{param}}Arg{i}", {{Variable.Args.AsVarName()}}.{{param}}[i]);
                    """
                : $"{Variable.QueryParams.AsVarName()}.Add(\"{p.Column.Name}\", {Variable.Args.AsVarName()}.{param});";
        });

        return $$"""
                 {{initParamsDict}}
                 {{dapperParamsCommands.JoinByNewLine()}}
                 """;
    }

    public static string AwaitReaderRow()
    {
        return $"await {Variable.Reader.AsVarName()}.ReadAsync()";
    }

    public static string InitDataReader()
    {
        return $"var {Variable.Reader.AsVarName()} = await {Variable.Command.AsVarName()}.ExecuteReaderAsync()";
    }

    public static string GetSqlTransformations(Query query, string queryTextConstant)
    {
        if (!query.Params.Any(p => p.Column.IsSqlcSlice)) return string.Empty;
        var initVariable = $"var {Variable.SqlText.AsVarName()} = {queryTextConstant};";

        var sqlcSliceCommands = query.Params
            .Where(p => p.Column.IsSqlcSlice)
            .Select(c =>
            {
                var sqlTextVar = Variable.SqlText.AsVarName();
                var paramName = c.Column.Name.ToPascalCase();
                return $"""
                         {sqlTextVar} = Utils.GetTransformedString({sqlTextVar}, {Variable.Args.AsVarName()}.{paramName}, "{paramName}", "{c.Column.Name}");
                         """;
            });

        return $"""
                 {initVariable}
                 {sqlcSliceCommands.JoinByNewLine()}
                 """;
    }

    public string InstantiateDataclass(Column[] columns, string returnInterface)
    {
        var columnsInit = new List<string>();
        var actualOrdinal = 0;
        var seenEmbed = new Dictionary<string, int>();

        foreach (var column in columns)
        {
            if (column.EmbedTable is null)
            {
                columnsInit.Add(GetAsSimpleAssignment(column, actualOrdinal));
                actualOrdinal++;
                continue;
            }

            var tableFieldType = column.EmbedTable.Name.ToModelName();
            var tableFieldName = seenEmbed.TryGetValue(tableFieldType, out var value)
                ? $"{tableFieldType}{value}" : tableFieldType;
            seenEmbed.TryAdd(tableFieldType, 1);
            seenEmbed[tableFieldType]++;
            var tableColumnsInit = GetAsEmbeddedTableColumnAssignment(column, actualOrdinal);
            columnsInit.Add($"{tableFieldName} = {InstantiateDataclassInternal(tableFieldType, tableColumnsInit)}");
            actualOrdinal += tableColumnsInit.Length;
        }

        return InstantiateDataclassInternal(returnInterface, columnsInit);

        string[] GetAsEmbeddedTableColumnAssignment(Column tableColumn, int ordinal)
        {
            var tableColumns = dbDriver.Tables[tableColumn.EmbedTable.Name].Columns;
            return tableColumns
                .Select((c, o) => GetAsSimpleAssignment(c, o + ordinal))
                .ToArray();
        }

        string GetAsSimpleAssignment(Column column, int ordinal)
        {
            var readExpression = column.NotNull
                ? dbDriver.GetColumnReader(column, ordinal)
                : $"{CheckNullExpression(ordinal)} ? {GetNullExpression(column)} : {dbDriver.GetColumnReader(column, ordinal)}";
            return $"{column.Name.ToPascalCase()} = {readExpression}";
        }

        string GetNullExpression(Column column)
        {
            var csharpType = dbDriver.GetCsharpType(column);
            if (csharpType == "string")
                return "string.Empty";
            return !dbDriver.Options.DotnetFramework.LatestDotnetSupported() && dbDriver.IsTypeNullableForAllRuntimes(csharpType)
                ? $"({csharpType}) null"
                : "null";
        }

        string CheckNullExpression(int ordinal)
        {
            return $"{Variable.Reader.AsVarName()}.IsDBNull({ordinal})";
        }

        string InstantiateDataclassInternal(string name, IEnumerable<string> fieldsInit)
        {
            return $$"""
                     new {{name}}
                     {
                         {{string.Join(",\n", fieldsInit)}}
                     }
                     """;
        }
    }
}