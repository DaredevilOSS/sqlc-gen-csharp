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

    protected IList<Query> Queries { get; }

    private HashSet<string> NullableTypesInDotnetCore { get; } =
    [
        "string",
        "object",
        "PhysicalAddress",
        "IPAddress"
    ];

    protected HashSet<string> NullableTypes { get; } =
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
        "Guid",
        "NpgsqlPoint",
        "NpgsqlLine",
        "NpgsqlLSeg",
        "NpgsqlBox",
        "NpgsqlPath",
        "NpgsqlPolygon",
        "NpgsqlCircle",
        "JsonElement",
        "NpgsqlCidr",
    ];

    protected abstract Dictionary<string, ColumnMapping> ColumnMappings { get; }

    protected const string TransformQueryForSliceArgsImpl = """
           public static string TransformQueryForSliceArgs(string originalSql, int sliceSize, string paramName)
           {
               var paramArgs = Enumerable.Range(0, sliceSize).Select(i => $"@{paramName}Arg{i}").ToList();
               return originalSql.Replace($"/*SLICE:{paramName}*/@{paramName}", string.Join(",", paramArgs));
           }
           """;

    public readonly string TransactionConnectionNullExcetionThrow = $"""
         if (this.{Variable.Transaction.AsPropertyName()}?.Connection == null || this.{Variable.Transaction.AsPropertyName()}?.Connection.State != System.Data.ConnectionState.Open)
             throw new InvalidOperationException("Transaction is provided, but its connection is null.");
         """;

    protected DbDriver(
        Options options,
        Catalog catalog,
        IList<Query> queries)
    {
        Options = options;
        DefaultSchema = catalog.DefaultSchema;
        Tables = ConstructTablesLookup(catalog);
        Queries = queries;

        if (!Options.DotnetFramework.IsDotnetCore())
            return;

        foreach (var t in NullableTypesInDotnetCore)
            NullableTypes.Add(t);
    }

    private static readonly HashSet<string> _excludedSchemas =
    [
        "pg_catalog",
        "information_schema"
    ];

    private static Dictionary<string, Dictionary<string, Table>> ConstructTablesLookup(Catalog catalog)
    {
        return catalog.Schemas
            .Where(s => !_excludedSchemas.Contains(s.Name))
            .ToDictionary(
                s => s.Name == catalog.DefaultSchema ? string.Empty : s.Name,
                s => s.Tables.ToDictionary(t => t.Rel.Name, t => t)
            );
    }

    public virtual ISet<string> GetUsingDirectivesForQueries()
    {
        return new HashSet<string>
            {
                "System",
                "System.Collections.Generic",
                "System.Threading.Tasks"
            }
            .AddRangeIf([
                "Dapper"
            ], Options.UseDapper)
            .AddRangeExcludeNulls(GetUsingDirectivesForColumnMappings());
    }

    private ISet<string> GetUsingDirectivesForColumnMappings()
    {
        var usingDirectives = new HashSet<string>();
        foreach (var schemaTables in Tables.Values)
            foreach (var table in schemaTables.Values)
                foreach (var column in table.Columns)
                {
                    var csharpType = GetCsharpTypeWithoutNullableSuffix(column, null);
                    if (!ColumnMappings.ContainsKey(csharpType))
                        continue;

                    var columnMapping = ColumnMappings[csharpType];
                    usingDirectives.AddRangeExcludeNulls([columnMapping.UsingDirective]);
                }
        return usingDirectives;
    }

    public virtual ISet<string> GetUsingDirectivesForUtils()
    {
        return new HashSet<string>
            {
                "System.Linq"
            }
            .AddRangeIf(["System.Data", "Dapper"], Options.UseDapper)
            .AddRangeIf(GetUsingDirectivesForColumnMappings(), Options.UseDapper);
    }

    public virtual ISet<string> GetUsingDirectivesForModels()
    {
        return new HashSet<string>
            {
                "System.Linq"
            }
            .AddRangeExcludeNulls(GetUsingDirectivesForColumnMappings());
    }

    public string[] GetConstructorStatements()
    {
        return [$"this.{Variable.ConnectionString.AsPropertyName()} = {Variable.ConnectionString.AsVarName()};"];
    }

    public string[] GetTransactionConstructorStatements()
    {
        return [$"this.{Variable.Transaction.AsPropertyName()} = {Variable.Transaction.AsVarName()};"];
    }

    protected virtual ISet<string> GetConfigureSqlMappings()
    {
        return ColumnMappings
            .Where(m => TypeExistsInQueries(m.Key) && m.Value.SqlMapper is not null)
            .Select(m => m.Value.SqlMapper!)
            .ToHashSet();
    }

    public virtual MemberDeclarationSyntax[] GetMemberDeclarationsForUtils()
    {
        if (!Options.UseDapper)
            return [];
        return [..
            GetSqlMapperMemberDeclarations(),
            ParseMemberDeclaration($$"""
                 public static void ConfigureSqlMapper()
                 {
                     {{GetConfigureSqlMappings().JoinByNewLine()}}
                 }
               """)!];
    }

    private MemberDeclarationSyntax[] GetSqlMapperMemberDeclarations()
    {
        return [.. ColumnMappings
            .Where(m => TypeExistsInQueries(m.Key) && m.Value.SqlMapperImpl is not null)
            .Select(m => ParseMemberDeclaration(m.Value.SqlMapperImpl!(Options.DotnetFramework.IsDotnetCore()))!)];
    }

    public abstract string TransformQueryText(Query query);

    public abstract ConnectionGenCommands EstablishConnection(Query query);

    public abstract string CreateSqlCommand(string sqlTextConstant);

    /* Since there is no indication of the primary key column in SQLC protobuf (assuming it is a single column),
       this method uses a few heuristics to assess the data type of the id column
    */
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
        var idColumnType = GetIdColumnType(query);
        var convertFunc = ColumnMappings[idColumnType].ConvertFunc ??
            throw new InvalidOperationException($"ConvertFunc is missing for id column type {idColumnType}");
        var convertFuncCall = convertFunc(Variable.Result.AsVarName());
        return
        [
            $"var {Variable.Result.AsVarName()} = await {Variable.Command.AsVarName()}.ExecuteScalarAsync();",
            $"return {convertFuncCall};"
        ];
    }

    public Column GetColumnFromParam(Parameter queryParam, Query query)
    {
        if (string.IsNullOrEmpty(queryParam.Column.Name))
            queryParam.Column.Name = $"{GetCsharpType(queryParam.Column, query).Replace("[]", "Arr")}_{queryParam.Number}";
        return queryParam.Column;
    }

    protected bool TypeExistsInQueries(string csharpType)
    {
        return Queries.Any(q => TypeExistsInQuery(csharpType, q));
    }

    protected bool TypeExistsInQuery(string csharpType, Query query)
    {
        return query.Columns
            .Any(column => csharpType == GetCsharpTypeWithoutNullableSuffix(column, query)) ||
               query.Params
            .Any(p => csharpType == GetCsharpTypeWithoutNullableSuffix(p.Column, query));
    }

    protected bool SliceQueryExists()
    {
        return Queries.Any(q => q.Params.Any(p => p.Column.IsSqlcSlice));
    }

    protected bool CopyFromQueryExists()
    {
        return Queries.Any(q => q.Cmd is ":copyfrom");
    }

    private OverrideOption? FindOverrideForQueryColumn(Query? query, Column column)
    {
        if (query is null)
            return null;
        foreach (var overrideOption in Options.Overrides)
            if (overrideOption.Column == $"{query.Name}:{column.Name}" || overrideOption.Column == $"*:{column.Name}")
                return overrideOption;
        return null;
    }

    // If the column data type is overridden, we need to check for nulls in generated code
    public bool IsColumnNotNull(Column column, Query? query)
    {
        if (FindOverrideForQueryColumn(query, column) is { CsharpType: var csharpType })
            return csharpType.NotNull;
        return column.NotNull;
    }

    /* Data type methods */
    public string GetCsharpType(Column column, Query? query)
    {
        var csharpType = GetCsharpTypeWithoutNullableSuffix(column, query);
        return AddNullableSuffixIfNeeded(csharpType, IsColumnNotNull(column, query));
    }

    public string AddNullableSuffixIfNeeded(string csharpType, bool notNull)
    {
        if (notNull)
            return csharpType;
        return IsTypeNullable(csharpType) ? $"{csharpType}?" : csharpType;
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

    public bool IsTypeNullable(string csharpType)
    {
        if (NullableTypes.Contains(csharpType.Replace("?", ""))) return true;
        return Options.DotnetFramework.IsDotnetCore(); // non-primitives in .Net Core are inherently nullable
    }

    protected virtual string GetCsharpTypeWithoutNullableSuffix(Column column, Query? query)
    {
        if (column.EmbedTable != null)
            return column.EmbedTable.Name.ToModelName(column.EmbedTable.Schema, DefaultSchema);

        if (string.IsNullOrEmpty(column.Type.Name))
            return "object";

        if (FindOverrideForQueryColumn(query, column) is { CsharpType: var csharpType })
            return csharpType.Type;

        foreach (var columnMapping in ColumnMappings
                     .Where(columnMapping => DoesColumnMappingApply(columnMapping.Value, column)))
        {
            if (column.IsArray || column.IsSqlcSlice) return $"{columnMapping.Key}[]";
            return columnMapping.Key;
        }
        throw new NotSupportedException($"Column {column.Name} has unsupported column type: {column.Type.Name} in {GetType().Name}");
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

    public virtual WriterFn? GetWriterFn(Column column, Query query)
    {
        var csharpType = GetCsharpTypeWithoutNullableSuffix(column, query);
        var writerFn = ColumnMappings.GetValueOrDefault(csharpType)?.WriterFn;
        if (writerFn is not null)
            return writerFn;

        static string DefaultWriterFn(string el, bool notNull, bool isDapper) => notNull ? el : $"{el} ?? (object)DBNull.Value";
        return Options.UseDapper ? null : DefaultWriterFn;
    }

    /* Column reader methods */
    private string GetColumnReader(CsharpTypeOption csharpTypeOption, int ordinal)
    {
        if (ColumnMappings.TryGetValue(csharpTypeOption.Type, out var value))
            return value.ReaderFn(ordinal);
        throw new NotSupportedException($"Could not find column mapping for type override: {csharpTypeOption.Type}");
    }

    public virtual string GetColumnReader(Column column, int ordinal, Query? query)
    {
        if (FindOverrideForQueryColumn(query, column) is { CsharpType: var csharpType })
            return GetColumnReader(csharpType, ordinal);

        foreach (var columnMapping in ColumnMappings.Values
                     .Where(columnMapping => DoesColumnMappingApply(columnMapping, column)))
        {
            if (column.IsArray)
                return columnMapping.ReaderArrayFn?.Invoke(ordinal) ?? throw new InvalidOperationException("ReaderArrayFn is null");
            return columnMapping.ReaderFn(ordinal);
        }
        throw new NotSupportedException($"column {column.Name} has unsupported column type: {column.Type.Name} in {GetType().Name}");
    }
}