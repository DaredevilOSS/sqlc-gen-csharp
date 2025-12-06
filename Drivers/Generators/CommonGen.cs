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

    public static string GetDapperArgs(Query query)
    {
        return query.Params.Count == 0 ? string.Empty : $", {Variable.QueryParams.AsVarName()}";
    }
    
    public string ConstructDapperParamsDict(Query query)
    {
        if (!query.Params.Any()) return string.Empty;

        var objectType = dbDriver.AddNullableSuffixIfNeeded("object", false);
        var initParamsDict = $"var {Variable.QueryParams.AsVarName()} = new Dictionary<string, {objectType}>();";
        var argsVar = Variable.Args.AsVarName();
        var queryParamsVar = Variable.QueryParams.AsVarName();

        var dapperParamsCommands = query.Params.Select(p =>
        {
            var param = p.Column.Name.ToPascalCase();
            if (p.Column.IsSqlcSlice)
                return $$"""
                        for (int i = 0; i < {{argsVar}}.{{param}}.Length; i++)
                            {{queryParamsVar}}.Add($"@{{p.Column.Name}}Arg{i}", {{argsVar}}.{{param}}[i]);
                        """;

            if (dbDriver is EnumDbDriver enumDbDriver && enumDbDriver.Enums.ContainsKey(p.Column.Type.Name))
                param += "?.ToEnumString()";

            var notNull = dbDriver.IsColumnNotNull(p.Column, query);
            var writerFn = dbDriver.GetWriterFn(p.Column, query);
            var paramToWrite = writerFn is null ? $"{argsVar}.{param}" : writerFn($"{argsVar}.{param}", p.Column.Type.Name, notNull, dbDriver.Options.UseDapper, dbDriver.Options.DotnetFramework.IsDotnetLegacy());
            var addParamToDict = $"{queryParamsVar}.Add(\"{p.Column.Name}\", {paramToWrite});";
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
        var initVariable = $"var {Variable.TransformedSql.AsVarName()} = {queryTextConstant};";

        var sqlcSliceCommands = query.Params
            .Where(p => p.Column.IsSqlcSlice)
            .Select(c =>
            {
                var sqlTextVar = Variable.TransformedSql.AsVarName();
                return $"""
                         {sqlTextVar} = Utils.TransformQueryForSliceArgs({sqlTextVar}, {Variable.Args.AsVarName()}.{c.Column.Name.ToPascalCase()}.Length, "{c.Column.Name}");
                         """;
            });

        return $"""
                 {initVariable}
                 {sqlcSliceCommands.JoinByNewLine()}
                 """;
    }

    public string InstantiateDataclass(Column[] columns, string returnInterface, Query query)
    {
        var columnsInit = new List<string>();
        var actualOrdinal = 0;
        var seenEmbed = new Dictionary<string, int>();

        foreach (var column in columns)
        {
            if (column.EmbedTable is null)
            {
                columnsInit.Add(GetAsSimpleAssignment(column, actualOrdinal, query));
                actualOrdinal++;
                continue;
            }

            var tableFieldType = column.EmbedTable.Name.ToModelName(column.EmbedTable.Schema, dbDriver.DefaultSchema);
            var tableFieldName = seenEmbed.TryGetValue(tableFieldType, out var value)
                ? $"{tableFieldType}{value}" : tableFieldType;
            seenEmbed.TryAdd(tableFieldType, 1);
            seenEmbed[tableFieldType]++;

            var tableColumnsInit = GetAsEmbeddedTableColumnAssignment(column, actualOrdinal, query);
            columnsInit.Add($"{tableFieldName} = {InstantiateDataclassInternal(tableFieldType, tableColumnsInit)}");
            actualOrdinal += tableColumnsInit.Length;
        }

        return InstantiateDataclassInternal(returnInterface, columnsInit);

        string[] GetAsEmbeddedTableColumnAssignment(Column tableColumn, int ordinal, Query query)
        {
            var schemaName = tableColumn.EmbedTable.Schema == dbDriver.DefaultSchema ? string.Empty : tableColumn.EmbedTable.Schema;
            var tableColumns = dbDriver.Tables[schemaName][tableColumn.EmbedTable.Name].Columns;
            return tableColumns
                .Select((c, o) => GetAsSimpleAssignment(c, o + ordinal, query))
                .ToArray();
        }

        string GetAsSimpleAssignment(Column column, int ordinal, Query query)
        {
            var readExpression = GetReadExpression(column, ordinal, query);
            return $"{column.Name.ToPascalCase()} = {readExpression}";
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

    private string GetNullExpression(Column column, Query? query)
    {
        var csharpType = dbDriver.GetCsharpType(column, query);
        if (dbDriver.Options.DotnetFramework.IsDotnetCore()) return "null";
        return dbDriver.IsTypeNullable(csharpType) ? $"({csharpType}) null" : "null";
    }

    private static string CheckNullExpression(int ordinal)
    {
        return $"{Variable.Reader.AsVarName()}.IsDBNull({ordinal})";
    }

    private string GetReadExpression(Column column, int ordinal, Query query)
    {
        if (dbDriver.IsColumnNotNull(column, query))
            return dbDriver.GetColumnReader(column, ordinal, query);
        return $"{CheckNullExpression(ordinal)} ? {GetNullExpression(column, query)} : {dbDriver.GetColumnReader(column, ordinal, query)}";
    }
}