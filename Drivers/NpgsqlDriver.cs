using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public class NpgsqlDriver : DbDriver, IOne, IMany, IExec, IExecRows, IExecLastId, ICopyFrom
{
    public NpgsqlDriver(
        Options options,
        string defaultSchema,
        Dictionary<string, Dictionary<string, Table>> tables,
        Dictionary<string, Dictionary<string, Enum>> enums,
        IList<Query> queries) :
        base(options, defaultSchema, tables, enums, queries)
    {
        foreach (var columnMapping in ColumnMappings.Values)
        {
            foreach (var dbType in columnMapping.DbTypes.ToList())
            {
                var dbTypeToAdd = $"pg_catalog.{dbType.Key}";
                columnMapping.DbTypes.Add(dbTypeToAdd, dbType.Value);
            }
        }
    }

    public sealed override Dictionary<string, ColumnMapping> ColumnMappings { get; } =
        new()
        {
            ["long"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "int8", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Bigint") },
                    { "bigint", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Bigint") },
                    { "bigserial", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Bigint") }
                },
                ordinal => $"reader.GetInt64({ordinal})"
            ),
            ["byte[]"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "binary", new DbTypeInfo() },
                    { "bit", new DbTypeInfo() },
                    { "bytea", new DbTypeInfo() },
                    { "blob", new DbTypeInfo() },
                    { "longblob", new DbTypeInfo() },
                    { "mediumblob", new DbTypeInfo() },
                    { "tinyblob", new DbTypeInfo() },
                    { "varbinary", new DbTypeInfo() }
                },
                ordinal => $"reader.GetFieldValue<byte[]>({ordinal})"
            ),
            ["string"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "longtext", new DbTypeInfo() },
                    { "mediumtext", new DbTypeInfo() },
                    { "text", new DbTypeInfo() },
                    { "bpchar", new DbTypeInfo() },
                    { "tinytext", new DbTypeInfo() },
                    { "varchar", new DbTypeInfo() }
                },
                readerFn: ordinal => $"reader.GetString({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<string[]>({ordinal})"
            ),
            ["TimeSpan"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "time", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Time") }
                },
                ordinal => $"reader.GetFieldValue<TimeSpan>({ordinal})"
            ),
            ["DateTime"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "date", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Date") },
                    { "timestamp", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Timestamp") },
                    { "timestamptz", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.TimestampTz") }
                },
                ordinal => $"reader.GetDateTime({ordinal})"
            ),
            ["JsonElement"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "json", new DbTypeInfo() }
                },
                readerFn: ordinal => $"JsonSerializer.Deserialize<JsonElement>(reader.GetString({ordinal}))",
                writerFn: (el, notNull, isDapper) =>
                {
                    if (notNull)
                        return $"{el}.GetRawText()";
                    var nullValue = isDapper ? "null" : "(object)DBNull.Value";
                    return $"{el}.HasValue ? {el}.Value.GetRawText() : {nullValue}";
                },
                usingDirective: "System.Text.Json"
            ),
            ["short"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "int2", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Smallint") }
                },
                ordinal => $"reader.GetInt16({ordinal})",
                ordinal => $"reader.GetFieldValue<short[]>({ordinal})"
            ),
            ["int"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "integer", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Integer") },
                    { "int", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Integer") },
                    { "int4", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Integer") },
                    { "serial", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Integer") }
                },
                ordinal => $"reader.GetInt32({ordinal})",
                ordinal => $"reader.GetFieldValue<int[]>({ordinal})"
            ),
            ["float"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "float4", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Real") }
                },
                ordinal => $"reader.GetFloat({ordinal})"
            ),
            ["decimal"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "numeric", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Numeric") },
                    { "decimal", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Numeric") },
                    { "money", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Money") }
                },
                ordinal => $"reader.GetDecimal({ordinal})"
            ),
            ["double"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "float8", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Double") }
                },
                ordinal => $"reader.GetDouble({ordinal})"
            ),
            ["bool"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "bool", new DbTypeInfo() },
                    { "boolean", new DbTypeInfo() }
                },
                ordinal => $"reader.GetBoolean({ordinal})"
            ),
            ["NpgsqlPoint"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "point", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Point") }
                },
                ordinal => $"reader.GetFieldValue<NpgsqlPoint>({ordinal})"
            ),
            ["NpgsqlLine"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "line", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Line") }
                },
                ordinal => $"reader.GetFieldValue<NpgsqlLine>({ordinal})"
            ),
            ["NpgsqlLSeg"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "lseg", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.LSeg") }
                },
                ordinal => $"reader.GetFieldValue<NpgsqlLSeg>({ordinal})"
            ),
            ["NpgsqlBox"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "box", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Box") }
                },
                ordinal => $"reader.GetFieldValue<NpgsqlBox>({ordinal})"
            ),
            ["NpgsqlPath"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "path", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Path") }
                },
                ordinal => $"reader.GetFieldValue<NpgsqlPath>({ordinal})"
            ),
            ["NpgsqlPolygon"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "polygon", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Polygon") }
                },
                ordinal => $"reader.GetFieldValue<NpgsqlPolygon>({ordinal})"
            ),
            ["NpgsqlCircle"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "circle", new DbTypeInfo(NpgsqlTypeOverride: "NpgsqlDbType.Circle") }
                },
                ordinal => $"reader.GetFieldValue<NpgsqlCircle>({ordinal})"
            ),
            ["object[]"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "anyarray", new DbTypeInfo() }
                },
                ordinal => $"reader.GetValue({ordinal})"
            )
        };

    public override string TransactionClassName => "NpgsqlTransaction";

    public override ISet<string> GetUsingDirectivesForQueries()
    {
        return base.GetUsingDirectivesForQueries().AddRange(
        [
            "Npgsql",
            "NpgsqlTypes",
            "System.Data"
        ]);
    }

    public override ISet<string> GetUsingDirectivesForModels()
    {
        return base.GetUsingDirectivesForModels().AddRange(
        [
            "NpgsqlTypes",
            "System"
        ]);
    }

    public override ISet<string> GetUsingDirectivesForUtils()
    {
        var usingDirectives = base.GetUsingDirectivesForUtils();
        if (!Options.UseDapper)
            return usingDirectives;

        return usingDirectives.AddRange(
        [
            "NpgsqlTypes"
        ]);
    }

    public override MemberDeclarationSyntax[] GetMemberDeclarationsForUtils()
    {
        var memberDeclarations = base.GetMemberDeclarationsForUtils();
        if (!Options.UseDapper)
            return memberDeclarations;

        var optionalDotnetCoreSuffix = Options.DotnetFramework.IsDotnetCore()
            ? " where T : notnull"
            : string.Empty;
        var optionalDotnetCoreNullable = Options.DotnetFramework.IsDotnetCore()
            ? "?"
            : string.Empty;

        return
        [
            .. memberDeclarations,
            ParseMemberDeclaration($$"""
                 private class NpgsqlTypeHandler<T> : SqlMapper.TypeHandler<T>{{optionalDotnetCoreSuffix}}
                 {
                     public override T Parse(object value)
                     {
                         return (T)value;
                     }
                     
                     public override void SetValue(IDbDataParameter parameter, T{{optionalDotnetCoreNullable}} value)
                     {
                         parameter.Value = value;
                     }
                 }
                 """)!,
            ParseMemberDeclaration($$"""
                 private static void RegisterNpgsqlTypeHandler<T>(){{optionalDotnetCoreSuffix}}
                 {
                    SqlMapper.AddTypeHandler(typeof(T), new NpgsqlTypeHandler<T>());
                 }
                 """)!,
        ];
    }

    protected override ISet<string> GetConfigureSqlMappings()
    {
        return base.GetConfigureSqlMappings().AddRange(
        [
            "RegisterNpgsqlTypeHandler<NpgsqlPoint>();",
            "RegisterNpgsqlTypeHandler<NpgsqlLine>();",
            "RegisterNpgsqlTypeHandler<NpgsqlLSeg>();",
            "RegisterNpgsqlTypeHandler<NpgsqlBox>();",
            "RegisterNpgsqlTypeHandler<NpgsqlPath>();",
            "RegisterNpgsqlTypeHandler<NpgsqlPolygon>();",
            "RegisterNpgsqlTypeHandler<NpgsqlCircle>();",
            "SqlMapper.AddTypeHandler(typeof(JsonElement), new JsonElementTypeHandler());"
        ]);
    }

    // TODO different operations require different types of connections - improve code and docs to make it clearer
    public override ConnectionGenCommands EstablishConnection(Query query)
    {
        var connectionStringVar = Variable.ConnectionString.AsPropertyName();
        var connectionVar = Variable.Connection.AsVarName();

        if (query.Cmd == ":copyfrom")
            return new ConnectionGenCommands(
                $"var ds = NpgsqlDataSource.Create({connectionStringVar})",
                $"var {connectionVar} = ds.CreateConnection()"
            );

        var embedTableExists = query.Columns.Any(c => c.EmbedTable is not null);
        if (Options.UseDapper && !embedTableExists)
            return new ConnectionGenCommands(
                $"var {connectionVar} = new NpgsqlConnection({connectionStringVar})",
                string.Empty
            );
        return new ConnectionGenCommands(
            $"var {connectionVar} = NpgsqlDataSource.Create({connectionStringVar})",
            string.Empty
        );
    }

    public override string CreateSqlCommand(string sqlTextConstant)
    {
        return $"var {Variable.Command.AsVarName()} = {Variable.Connection.AsVarName()}.CreateCommand({sqlTextConstant})";
    }

    public override string TransformQueryText(Query query)
    {
        if (query.Cmd == ":copyfrom")
            return GetCopyCommand();

        var queryText = query.Text;
        for (var i = 0; i < query.Params.Count; i++)
        {
            var currentParameter = query.Params[i];
            var column = GetColumnFromParam(currentParameter, query);
            queryText = Regex.Replace(queryText, $@"\$\s*{i + 1}\b", $"@{column.Name}");
        }

        return queryText;

        string GetCopyCommand()
        {
            var copyParams = query.Params.Select(p => p.Column.Name).JoinByComma();
            return $"COPY {query.InsertIntoTable.Name} ({copyParams}) FROM STDIN (FORMAT BINARY)";
        }
    }

    public MemberDeclarationSyntax OneDeclare(string queryTextConstant, string argInterface,
        string returnInterface, Query query)
    {
        return new OneDeclareGen(this).Generate(queryTextConstant, argInterface, returnInterface, query);
    }

    public MemberDeclarationSyntax ExecDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new ExecDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }

    public MemberDeclarationSyntax ManyDeclare(string queryTextConstant, string argInterface,
        string returnInterface, Query query)
    {
        return new ManyDeclareGen(this).Generate(queryTextConstant, argInterface, returnInterface, query);
    }

    public MemberDeclarationSyntax ExecRowsDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new ExecRowsDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }

    public MemberDeclarationSyntax ExecLastIdDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new ExecLastIdDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }

    public MemberDeclarationSyntax CopyFromDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new CopyFromDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }

    public string GetCopyFromImpl(Query query, string queryTextConstant)
    {
        var (establishConnection, connectionOpen) = EstablishConnection(query);
        var beginBinaryImport = $"{Variable.Connection.AsVarName()}.BeginBinaryImportAsync({queryTextConstant}";
        var connectionVar = Variable.Connection.AsVarName();
        var writerVar = Variable.Writer.AsVarName();

        var addRowsToCopyCommand = AddRowsToCopyCommand();
        return $$"""
                 using ({{establishConnection}})
                 {
                     {{connectionOpen.AppendSemicolonUnlessEmpty()}}
                     await {{connectionVar}}.OpenAsync();
                     using (var {{writerVar}} = await {{beginBinaryImport}}))
                     {
                        {{addRowsToCopyCommand}}
                        await {{writerVar}}.CompleteAsync();
                     }
                     await {{connectionVar}}.CloseAsync();
                 }
                 """;

        string AddRowsToCopyCommand()
        {
            var rowVar = Variable.Row.AsVarName();
            var constructRowFields = query.Params
                    .Select(p =>
                    {
                        var typeOverride = GetColumnDbTypeOverride(p.Column);
                        var partialStmt = $"await {writerVar}.WriteAsync({rowVar}.{p.Column.Name.ToPascalCase()}";
                        return typeOverride is null
                            ? $"{partialStmt});"
                            : $"{partialStmt}, {typeOverride});";
                    })
                .JoinByNewLine();
            return $$"""
                     foreach (var {{rowVar}} in args) 
                     {
                          await {{writerVar}}.StartRowAsync();
                          {{constructRowFields}}
                     }
                     """;
        }
    }
}