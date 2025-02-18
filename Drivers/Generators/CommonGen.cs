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
            var argsVar = Variable.Args.AsVarName();
            if (p.Column.IsSqlcSlice)
                return $$"""
                         for (int i = 0; i < {{argsVar}}.{{param}}.Length; i++)
                             {{commandVar}}.Parameters.AddWithValue($"@{{param}}Arg{i}", {{argsVar}}.{{param}}[i]);
                         """;

            var nullableParamCasting = p.Column.NotNull ? "" : " ?? (object)DBNull.Value";
            var addParamToCommand = $"""{commandVar}.Parameters.AddWithValue("@{p.Column.Name}", {argsVar}.{param}{nullableParamCasting});""";
            return addParamToCommand;
        }).JoinByNewLine();
    }

    public string ConstructDapperParamsDict(IList<Parameter> parameters)
    {
        if (!parameters.Any()) return string.Empty;
        var objectType = dbDriver.AddNullableSuffixIfNeeded("object", false);
        var initParamsDict = $"var {Variable.QueryParams.AsVarName()} = new Dictionary<string, {objectType}>();";
        var argsVar = Variable.Args.AsVarName();
        var queryParamsVar = Variable.QueryParams.AsVarName();

        var dapperParamsCommands = parameters.Select(p =>
        {
            var param = p.Column.Name.ToPascalCase();
            if (p.Column.IsSqlcSlice)
                return $$"""
                        for (int i = 0; i < {{argsVar}}.{{param}}.Length; i++)
                            {{queryParamsVar}}.Add($"@{{param}}Arg{i}", {{argsVar}}.{{param}}[i]);
                        """;

            var addParamToDict = $"{queryParamsVar}.Add(\"{p.Column.Name}\", {argsVar}.{param});";
            return addParamToDict;
        });

        return $"""
                 {initParamsDict}
                 {dapperParamsCommands.JoinByNewLine()}
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
            var readExpression = GetReadExpression(column, ordinal);
            return $"{column.Name.ToPascalCase()} = {readExpression}";
        }

        string GetReadExpression(Column column, int ordinal)
        {
            return column.NotNull
                ? dbDriver.GetColumnReader(column, ordinal)
                : $"{CheckNullExpression(ordinal)} ? {GetNullExpression(column)} : {dbDriver.GetColumnReader(column, ordinal)}";
            ;
        }

        string GetNullExpression(Column column)
        {
            var csharpType = dbDriver.GetCsharpType(column);
            if (csharpType == "string")
                return "string.Empty";
            return !dbDriver.Options.DotnetFramework.IsDotnetCore() && dbDriver.IsTypeNullable(csharpType)
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
                         {{fieldsInit.JoinByComma()}}
                     }
                     """;
        }
    }
    private bool ShouldCheckParameterForNull(Parameter parameter)
    {
        if (parameter.Column.IsArray || parameter.Column.NotNull)
            return false;
        var csharpType = dbDriver.GetCsharpType(parameter.Column);
        return dbDriver.IsTypeNullable(csharpType);
    }
}