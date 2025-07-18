using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public record ConnectionGenCommands(string EstablishConnection, string ConnectionOpen);

public abstract class DbDriver
{
    public Options Options { get; }

    public string DefaultSchema { get; }

    public abstract string TransactionClassName { get; }

    public Dictionary<string, Dictionary<string, Table>> Tables { get; }

    public Dictionary<string, Dictionary<string, Plugin.Enum>> Enums { get; }

    private IList<Query> Queries { get; }

    private HashSet<string> NullableTypesInDotnetCore { get; } =
    [
        "string",
        "object",
        "byte[]"
    ]; // TODO add arrays in here in a non hard-coded manner

    private HashSet<string> NullableTypes { get; } =
    [
        "bool",
        "byte",
        "short",
        "int",
        "long",
        "float",
        "double",
        "decimal",
        "DateTime",
        "TimeSpan",
        "NpgsqlPoint",
        "NpgsqlLine",
        "NpgsqlLSeg",
        "NpgsqlBox",
        "NpgsqlPath",
        "NpgsqlPolygon",
        "NpgsqlCircle",
        "JsonElement"
    ];

    public abstract Dictionary<string, ColumnMapping> ColumnMappings { get; }

    private Dictionary<string, Tuple<string, string>> KnownMappings { get; } = new()
    {
        {
            "JsonElement",
            new (
                $"SqlMapper.AddTypeHandler(typeof(JsonElement), new JsonElementTypeHandler());",
                """
                    public class JsonElementTypeHandler : SqlMapper.TypeHandler<JsonElement>
                    {
                        public override JsonElement Parse(object value)
                        {
                            if (value is string s)
                                return JsonDocument.Parse(s).RootElement;
                            if (value is null)
                                return default;
                            throw new DataException($"Cannot convert {value?.GetType()} to JsonElement");
                        }

                        public override void SetValue(IDbDataParameter parameter, JsonElement value)
                        {
                            parameter.Value = value.GetRawText();
                        }
                    }
                """
            )
        }
    };

    protected const string TransformQueryForSliceArgsImpl = """
           public static string TransformQueryForSliceArgs(string originalSql, int sliceSize, string paramName)
           {
               var paramArgs = Enumerable.Range(0, sliceSize).Select(i => $"@{paramName}Arg{i}").ToList();
               return originalSql.Replace($"/*SLICE:{paramName}*/@{paramName}", string.Join(",", paramArgs));
           }
           """;

    protected DbDriver(
        Options options,
        string defaultSchema,
        Dictionary<string, Dictionary<string, Table>> tables,
        Dictionary<string, Dictionary<string, Plugin.Enum>> enums,
        IList<Query> queries)
    {
        Options = options;
        DefaultSchema = defaultSchema;
        Tables = tables;
        Enums = enums;
        Queries = queries;

        foreach (var schemaEnums in Enums)
        {
            foreach (var e in schemaEnums.Value)
            {
                NullableTypes.Add(e.Key.ToModelName(schemaEnums.Key, DefaultSchema));
            }
        }

        if (!Options.DotnetFramework.IsDotnetCore()) return;

        foreach (var t in NullableTypesInDotnetCore)
        {
            NullableTypes.Add(t);
        }
    }

    public virtual ISet<string> GetUsingDirectivesForQueries()
    {
        var usingDirectives = new HashSet<string>
            {
                "System",
                "System.Collections.Generic",
                "System.Threading.Tasks"
            }.AddIf("Dapper", Options.UseDapper);

        foreach (var query in Queries)
        {
            foreach (var column in query.Columns)
            {
                var csharpType = GetCsharpTypeWithoutNullableSuffix(column, query);
                if (!ColumnMappings.ContainsKey(csharpType))
                    continue;

                var columnMapping = ColumnMappings[GetCsharpTypeWithoutNullableSuffix(column, query)];
                usingDirectives.AddIfNotNull(columnMapping.UsingDirective);
            }
        }
        return usingDirectives;
    }

    public virtual ISet<string> GetUsingDirectivesForModels()
    {
        return GetUsingDirectivesForColumnMappings();
    }

    private ISet<string> GetUsingDirectivesForColumnMappings()
    {
        var usingDirectives = new HashSet<string>();
        foreach (var schemaTables in Tables)
        {
            foreach (var table in schemaTables.Value)
            {
                foreach (var column in table.Value.Columns)
                {
                    var csharpType = GetCsharpTypeWithoutNullableSuffix(column, null);
                    if (!ColumnMappings.ContainsKey(csharpType))
                        continue;

                    var columnMapping = ColumnMappings[GetCsharpTypeWithoutNullableSuffix(column, null)];
                    usingDirectives.AddIfNotNull(columnMapping.UsingDirective);
                }
            }
        }
        return usingDirectives;
    }

    public virtual ISet<string> GetUsingDirectivesForUtils()
    {
        return new HashSet<string>()
            {
                "System.Linq"
            }
            .AddRangeIf(["System.Data", "Dapper"], Options.UseDapper)
            .AddRangeIf(GetUsingDirectivesForColumnMappings(), Options.UseDapper);
    }

    public virtual string[] GetConstructorStatements()
    {
        return [.. new List<string>
            {
                $"this.{Variable.ConnectionString.AsPropertyName()} = {Variable.ConnectionString.AsVarName()};"
            }
            .AppendIf("Utils.ConfigureSqlMapper();", Options.UseDapper)
            .AppendIf("Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;", Options.UseDapper)];
    }

    public virtual string[] GetTransactionConstructorStatements()
    {
        return [.. new List<string>
            {
                $"this.{Variable.Transaction.AsPropertyName()} = {Variable.Transaction.AsVarName()};"
            }
            .AppendIf("Utils.ConfigureSqlMapper();", Options.UseDapper)
            .AppendIf("Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;", Options.UseDapper)];
    }

    protected virtual ISet<string> GetConfigureSqlMappings()
    {
        return KnownMappings
            .Where(m => TypeExistsInQueries(m.Key))
            .Select(m => m.Value.Item1)
            .ToHashSet();
    }

    public virtual MemberDeclarationSyntax[] GetMemberDeclarationsForUtils()
    {
        var memberDeclarations = new List<MemberDeclarationSyntax>();
        if (!Options.UseDapper)
            return [.. memberDeclarations];

        memberDeclarations.AddRange(KnownMappings
            .Where(m => TypeExistsInQueries(m.Key))
            .Select(m => ParseMemberDeclaration(m.Value.Item2)!));

        return [.. memberDeclarations,
            ParseMemberDeclaration($$"""
                 public static void ConfigureSqlMapper()
                 {
                     {{GetConfigureSqlMappings().JoinByNewLine()}}
                 }
               """)!];
    }

    protected bool TypeExistsInQueries(string csharpType)
    {
        return Queries
            .SelectMany(query => query.Columns)
            .Any(column => csharpType == GetCsharpTypeWithoutNullableSuffix(column, null));
    }

    public string AddNullableSuffixIfNeeded(string csharpType, bool notNull)
    {
        if (notNull)
            return csharpType;
        return IsTypeNullable(csharpType) ? $"{csharpType}?" : csharpType;
    }

    public string GetCsharpType(Column column, Query? query)
    {
        var csharpType = GetCsharpTypeWithoutNullableSuffix(column, query);
        return AddNullableSuffixIfNeeded(csharpType, IsColumnNotNull(column, query));
    }

    public string GetCsharpTypeWithoutNullableSuffix(Column column, Query? query)
    {
        if (column.EmbedTable != null)
            return column.EmbedTable.Name.ToModelName(column.EmbedTable.Schema, DefaultSchema);

        if (string.IsNullOrEmpty(column.Type.Name))
            return "object";

        if (IsEnumType(column))
            return column.Type.Name.ToModelName(column.Table.Schema, DefaultSchema);

        if (FindOverrideForQueryColumn(query, column) is { CsharpType: var csharpType })
            return csharpType.Type;

        foreach (var columnMapping in ColumnMappings
                     .Where(columnMapping => DoesColumnMappingApply(columnMapping.Value, column)))
        {
            if (column.IsArray || column.IsSqlcSlice) return $"{columnMapping.Key}[]";
            return columnMapping.Key;
        }

        throw new NotSupportedException($"Column {column.Name} has unsupported column type: {column.Type.Name}");
    }

    private bool IsEnumType(Column column)
    {
        if (column.Table is null)
            return false;
        var enumSchema = column.Table.Schema == DefaultSchema ? string.Empty : column.Table.Schema;
        if (!Enums.TryGetValue(enumSchema, value: out var enumsInSchema))
            return false;
        return enumsInSchema.ContainsKey(column.Type.Name);
    }

    private static bool DoesColumnMappingApply(ColumnMapping columnMapping, Column column)
    {
        var columnType = column.Type.Name.ToLower();
        if (!columnMapping.DbTypes.TryGetValue(columnType, out var typeInfo))
            return false;
        if (typeInfo.Length is null)
            return true;
        return typeInfo.Length.Value == column.Length;
    }

    private string GetColumnReader(CsharpTypeOption csharpTypeOption, int ordinal)
    {
        if (ColumnMappings.TryGetValue(csharpTypeOption.Type, out var value))
            return value.ReaderFn(ordinal);
        throw new NotSupportedException($"Could not find column mapping for type override: {csharpTypeOption.Type}");
    }

    private string GetEnumReader(Column column, int ordinal)
    {
        var enumName = column.Type.Name.ToModelName(column.Table.Schema, DefaultSchema);
        return $"{Variable.Reader.AsVarName()}.GetString({ordinal}).To{enumName}()";
    }

    public string GetColumnReader(Column column, int ordinal, Query? query)
    {
        if (IsEnumType(column))
            return GetEnumReader(column, ordinal);

        if (FindOverrideForQueryColumn(query, column) is { CsharpType: var csharpType })
            return GetColumnReader(csharpType, ordinal);

        foreach (var columnMapping in ColumnMappings.Values
                     .Where(columnMapping => DoesColumnMappingApply(columnMapping, column)))
        {
            if (column.IsArray)
                return columnMapping.ReaderArrayFn?.Invoke(ordinal) ?? throw new InvalidOperationException("ReaderArrayFn is null");
            return columnMapping.ReaderFn(ordinal);
        }
        throw new NotSupportedException($"Column {column.Name} has unsupported column type: {column.Type.Name}");
    }

    protected string? GetColumnDbTypeOverride(Column column)
    {
        var columnType = column.Type.Name.ToLower();
        foreach (var columnMapping in ColumnMappings.Values)
        {
            if (columnMapping.DbTypes.TryGetValue(columnType, out var dbTypeOverride))
                return dbTypeOverride.NpgsqlTypeOverride;
        }
        throw new NotSupportedException($"Column {column.Name} has unsupported column type: {column.Type.Name}");
    }

    public abstract string TransformQueryText(Query query);

    public abstract ConnectionGenCommands EstablishConnection(Query query);

    public abstract string CreateSqlCommand(string sqlTextConstant);

    public bool IsTypeNullable(string csharpType)
    {
        if (NullableTypes.Contains(csharpType.Replace("?", ""))) return true;
        return Options.DotnetFramework.IsDotnetCore(); // non-primitives in .Net Core are inherently nullable
    }

    /// <summary>
    /// Since there is no indication of the primary key column in SQLC protobuf (assuming it is a single column),
    /// this method uses a few heuristics to assess the data type of the id column
    /// </summary>
    /// <param name="query"></param>
    /// <returns>The data type of the id column</returns>
    public string GetIdColumnType(Query query)
    {
        var tableColumns = Tables[query.InsertIntoTable.Schema][query.InsertIntoTable.Name].Columns;
        var idColumn = tableColumns.First(c => c.Name.Equals("id", StringComparison.OrdinalIgnoreCase));
        if (idColumn is not null)
            return GetCsharpType(idColumn, query);

        idColumn = tableColumns.First(c => c.Name.Contains("id", StringComparison.CurrentCultureIgnoreCase));
        return GetCsharpType(idColumn ?? tableColumns[0], query);
    }

    public virtual string[] GetLastIdStatement(Query query)
    {
        var convertFunc = GetIdColumnType(query) == "int" ? "ToInt32" : "ToInt64"; // TODO refactor
        return
        [
            $"var {Variable.Result.AsVarName()} = await {Variable.Command.AsVarName()}.ExecuteScalarAsync();",
            $"return Convert.{convertFunc}({Variable.Result.AsVarName()});"
        ];
    }

    public Column GetColumnFromParam(Parameter queryParam, Query query)
    {
        if (string.IsNullOrEmpty(queryParam.Column.Name))
            queryParam.Column.Name = $"{GetCsharpType(queryParam.Column, query).Replace("[]", "Arr")}_{queryParam.Number}";
        return queryParam.Column;
    }

    protected bool SliceQueryExists()
    {
        return Queries.Any(q =>
        {
            return q.Params.Any(p => p.Column.IsSqlcSlice);
        });
    }

    protected bool CopyFromQueryExists()
    {
        return Queries.Any(q => q.Cmd is ":copyfrom");
    }

    public OverrideOption? FindOverrideForQueryColumn(Query? query, Column column)
    {
        if (query is null)
            return null;
        return Options.Overrides.FirstOrDefault(o => o.Column.Equals($"{query.Name}:{column.Name}"));
    }

    /// <summary>
    /// If the column data type is overridden, we need to check for nulls in generated code
    /// </summary>
    /// <param name="column"></param>
    /// <param name="query"></param>
    /// <returns>Adjusted not null value</returns>
    public bool IsColumnNotNull(Column column, Query? query)
    {
        if (FindOverrideForQueryColumn(query, column) is { CsharpType: var csharpType })
            return csharpType.NotNull;
        return column.NotNull;
    }
}