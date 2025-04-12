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

    public Dictionary<string, Dictionary<string, Table>> Tables { get; }

    public Dictionary<string, Dictionary<string, Plugin.Enum>> Enums { get; }

    private IList<Query> Queries { get; }

    private HashSet<string> NullableTypesInDotnetCore { get; } = ["string", "object", "byte[]"]; // TODO add arrays in here in a non hard-coded manner

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
        "NpgsqlCircle"
    ];

    protected abstract List<ColumnMapping> ColumnMappings { get; }

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

    public virtual UsingDirectiveSyntax[] GetUsingDirectivesForQueries()
    {
        return new List<UsingDirectiveSyntax>
            {
                UsingDirective(ParseName("System")),
                UsingDirective(ParseName("System.Collections.Generic")),
                UsingDirective(ParseName("System.Threading.Tasks"))
            }
            .AppendIf(UsingDirective(ParseName("Dapper")), Options.UseDapper)
            .ToArray();
    }

    public virtual UsingDirectiveSyntax[] GetUsingDirectivesForModels()
    {
        return [];
    }

    public virtual UsingDirectiveSyntax[] GetUsingDirectivesForUtils()
    {
        return
        [
            UsingDirective(ParseName("System.Linq"))
        ];
    }

    public virtual string[] GetConstructorStatements()
    {
        return new List<string>
        {
            $"this.{Variable.ConnectionString.AsPropertyName()} = {Variable.ConnectionString.AsVarName()};"
        }
        .AppendIf("Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;", Options.UseDapper)
        .ToArray();
    }

    public virtual MemberDeclarationSyntax[] GetMemberDeclarationsForUtils()
    {
        return [];
    }


    public string AddNullableSuffixIfNeeded(string csharpType, bool notNull)
    {
        if (notNull) return csharpType;
        return IsTypeNullable(csharpType) ? $"{csharpType}?" : csharpType;
    }

    public string GetCsharpType(Column column)
    {
        var csharpType = GetCsharpTypeWithoutNullableSuffix(column);
        return AddNullableSuffixIfNeeded(csharpType, column.NotNull);
    }

    private string GetCsharpTypeWithoutNullableSuffix(Column column)
    {
        if (column.EmbedTable != null)
            return column.EmbedTable.Name.ToModelName(column.EmbedTable.Schema, DefaultSchema);

        if (string.IsNullOrEmpty(column.Type.Name))
            return "object";

        if (IsEnumType(column))
            return column.Type.Name.ToModelName(column.Table.Schema, DefaultSchema);

        foreach (var columnMapping in ColumnMappings
                     .Where(columnMapping => DoesColumnMappingApply(columnMapping, column)))
        {
            if (column.IsArray || column.IsSqlcSlice) return $"{columnMapping.CsharpType}[]";
            return columnMapping.CsharpType;
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

    public string GetColumnReader(Column column, int ordinal)
    {
        if (IsEnumType(column))
        {
            var enumName = column.Type.Name.ToModelName(column.Table.Schema, DefaultSchema);
            return $"{Variable.Reader.AsVarName()}.GetString({ordinal}).To{enumName}()";
        }

        foreach (var columnMapping in ColumnMappings
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
        foreach (var columnMapping in ColumnMappings)
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

    /*
    Since there is no indication of the primary key column in SQLC protobuf (assuming it is a single column),
    this method uses a few heuristics to assess the data type of the id column
    */
    public string GetIdColumnType(Query query)
    {
        var tableColumns = Tables[query.InsertIntoTable.Schema][query.InsertIntoTable.Name].Columns;
        var idColumn = tableColumns.First(c => c.Name.Equals("id", StringComparison.OrdinalIgnoreCase));
        if (idColumn is not null)
            return GetCsharpType(idColumn);

        idColumn = tableColumns.First(c => c.Name.Contains("id", StringComparison.CurrentCultureIgnoreCase));
        return GetCsharpType(idColumn ?? tableColumns[0]);
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

    public Column GetColumnFromParam(Parameter queryParam)
    {
        if (string.IsNullOrEmpty(queryParam.Column.Name))
            queryParam.Column.Name = $"{GetCsharpType(queryParam.Column).Replace("[]", "Arr")}_{queryParam.Number}";
        return queryParam.Column;
    }

    protected bool SliceQueryExists()
    {
        return Queries.Any(q =>
        {
            return q.Params.Any(p => p.Column.IsSqlcSlice);
        });
    }

    protected bool BatchQueryExists()
    {
        return Queries.Any(q => q.Cmd is ":copyfrom");
    }
}