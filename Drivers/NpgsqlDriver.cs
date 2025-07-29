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
        Dictionary<string, Dictionary<string, Plugin.Enum>> enums,
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
        CommonGen = new CommonGen(this);
    }

    private CommonGen CommonGen { get; }

    public sealed override Dictionary<string, ColumnMapping> ColumnMappings { get; } =
        new()
        {
            ["long"] = new(
                new()
                {
                    { "int8", new() },
                    { "bigint", new() },
                    { "bigserial", new() }
                },
                readerFn: ordinal => $"reader.GetInt64({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<long[]>({ordinal})",
                convertFunc: x => $"Convert.ToInt64({x})"
            ),
            ["byte[]"] = new(
                new()
                {
                    { "binary", new() },
                    { "bit", new() },
                    { "bytea", new() },
                    { "blob", new() },
                    { "longblob", new() },
                    { "mediumblob", new() },
                    { "tinyblob", new() },
                    { "varbinary", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<byte[]>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<byte[][]>({ordinal})"
            ),
            ["string"] = new(
                new()
                {
                    { "longtext", new() },
                    { "mediumtext", new() },
                    { "text", new() },
                    { "bpchar", new() },
                    { "tinytext", new() },
                    { "varchar", new() },
                    { "jsonpath", new(NpgsqlTypeOverride: "NpgsqlDbType.JsonPath") },
                    { "macaddr8", new(NpgsqlTypeOverride: "NpgsqlDbType.MacAddr8") }
                },
                readerFn: ordinal => $"reader.GetString({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<string[]>({ordinal})"
            ),
            ["Guid"] = new(
                new()
                {
                    { "uuid", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<Guid>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<Guid[]>({ordinal})",
                convertFunc: x => $"Guid.Parse({x}?.ToString())"
            ),
            ["TimeSpan"] = new(
                new()
                {
                    { "time", new(NpgsqlTypeOverride: "NpgsqlDbType.Time") },
                    { "interval", new(NpgsqlTypeOverride: "NpgsqlDbType.Interval") }
                },
                readerFn: ordinal => $"reader.GetFieldValue<TimeSpan>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<TimeSpan[]>({ordinal})"
            ),
            ["DateTime"] = new(
                new()
                {
                    { "date", new(NpgsqlTypeOverride: "NpgsqlDbType.Date") },
                    { "timestamp", new() },
                    { "timestamptz", new() }
                },
                readerFn: ordinal => $"reader.GetDateTime({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<DateTime[]>({ordinal})"
            ),
            ["JsonElement"] = new(
                new()
                {
                    { "json", new() },
                    { "jsonb", new() }
                },
                readerFn: ordinal => $"JsonSerializer.Deserialize<JsonElement>(reader.GetString({ordinal}))",
                writerFn: (el, notNull, isDapper) =>
                {
                    if (notNull)
                        return $"{el}.GetRawText()";
                    var nullValue = isDapper ? "null" : "(object)DBNull.Value";
                    return $"{el}.HasValue ? {el}.Value.GetRawText() : {nullValue}";
                },
                usingDirective: "System.Text.Json",
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(JsonElement), new JsonElementTypeHandler());",
                sqlMapperImpl: JsonElementTypeHandler
            ),
            ["short"] = new(
                new()
                {
                    { "int2", new() }
                },
                readerFn: ordinal => $"reader.GetInt16({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<short[]>({ordinal})",
                convertFunc: x => $"Convert.ToInt16({x})"
            ),
            ["int"] = new(
                new()
                {
                    { "integer", new() },
                    { "int", new() },
                    { "int4", new() },
                    { "serial", new() }
                },
                readerFn: ordinal => $"reader.GetInt32({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<int[]>({ordinal})",
                convertFunc: x => $"Convert.ToInt32({x})"
            ),
            ["float"] = new(
                new()
                {
                    { "float4", new() }
                },
                readerFn: ordinal => $"reader.GetFloat({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<float[]>({ordinal})"
            ),
            ["decimal"] = new(
                new()
                {
                    { "numeric", new() },
                    { "decimal", new() },
                    { "money", new(NpgsqlTypeOverride: "NpgsqlDbType.Money") }
                },
                readerFn: ordinal => $"reader.GetDecimal({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<decimal[]>({ordinal})"
            ),
            ["double"] = new(
                new()
                {
                    { "float8", new() }
                },
                readerFn: ordinal => $"reader.GetDouble({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<double[]>({ordinal})"
            ),
            ["bool"] = new(
                new()
                {
                    { "bool", new() },
                    { "boolean", new() }
                },
                readerFn: ordinal => $"reader.GetBoolean({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<bool[]>({ordinal})"
            ),
            ["NpgsqlPoint"] = new(
                new()
                {
                    { "point", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlPoint>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlPoint[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "RegisterNpgsqlTypeHandler<NpgsqlPoint>();"
            ),
            ["NpgsqlLine"] = new(
                new()
                {
                    { "line", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlLine>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlLine[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "RegisterNpgsqlTypeHandler<NpgsqlLine>();"
            ),
            ["NpgsqlLSeg"] = new(
                new()
                {
                    { "lseg", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlLSeg>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlLSeg[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "RegisterNpgsqlTypeHandler<NpgsqlLSeg>();"
            ),
            ["NpgsqlBox"] = new(
                new()
                {
                    { "box", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlBox>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlBox[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "RegisterNpgsqlTypeHandler<NpgsqlBox>();"
            ),
            ["NpgsqlPath"] = new(
                new()
                {
                    { "path", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlPath>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlPath[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "RegisterNpgsqlTypeHandler<NpgsqlPath>();"
            ),
            ["NpgsqlPolygon"] = new(
                new()
                {
                    { "polygon", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlPolygon>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlPolygon[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "RegisterNpgsqlTypeHandler<NpgsqlPolygon>();"
            ),
            ["NpgsqlCircle"] = new(
                new()
                {
                    { "circle", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlCircle>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlCircle[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "RegisterNpgsqlTypeHandler<NpgsqlCircle>();"
            ),
            ["NpgsqlCidr"] = new(
                new()
                {
                    { "cidr", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlCidr>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlCidr[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "RegisterNpgsqlTypeHandler<NpgsqlCidr>();"
            ),
            ["IPAddress"] = new(
                new()
                {
                    { "inet", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<IPAddress>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<IPAddress[]>({ordinal})",
                usingDirective: "System.Net",
                sqlMapper: "RegisterNpgsqlTypeHandler<IPAddress>();"
            ),
            ["PhysicalAddress"] = new(
                new()
                {
                    { "macaddr", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<PhysicalAddress>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<PhysicalAddress[]>({ordinal})",
                usingDirective: "System.Net.NetworkInformation",
                sqlMapper: "RegisterNpgsqlTypeHandler<PhysicalAddress>();"
            ),
            ["object"] = new(
                new()
                {
                    { "anyarray", new() }
                },
                ordinal => $"reader.GetValue({ordinal})"
            )
        };

    public override string TransactionClassName => "NpgsqlTransaction";

    public override ISet<string> GetUsingDirectivesForQueries()
    {
        return base.GetUsingDirectivesForQueries().AddRangeExcludeNulls(
        [
            "Npgsql",
            "System.Data"
        ]);
    }

    public override ISet<string> GetUsingDirectivesForModels()
    {
        return base.GetUsingDirectivesForModels().AddRangeExcludeNulls(
        [
            "System"
        ]);
    }

    public override ISet<string> GetUsingDirectivesForUtils()
    {
        var usingDirectives = base.GetUsingDirectivesForUtils();
        if (!Options.UseDapper)
            return usingDirectives;

        return usingDirectives.AddRangeExcludeNulls(
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

    public override ConnectionGenCommands EstablishConnection(Query query)
    {
        var connectionStringVar = Variable.ConnectionString.AsPropertyName();
        var connectionVar = Variable.Connection.AsVarName();
        var embedTableExists = query.Columns.Any(c => c.EmbedTable is not null);
        var useOpenConnection = query.Cmd == ":copyfrom" || (Options.UseDapper && !embedTableExists);

        return useOpenConnection
            ? new ConnectionGenCommands(
                $"var {connectionVar} = new NpgsqlConnection({connectionStringVar})",
                string.Empty
            )
            : new ConnectionGenCommands(
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
                        var param = $"{rowVar}.{p.Column.Name.ToPascalCase()}";
                        var writerFn = CommonGen.GetWriterFn(p.Column, query);
                        var paramToWrite = writerFn is null ? param : writerFn(param, p.Column.NotNull, false);
                        var partialStmt = $"await {writerVar}.WriteAsync({paramToWrite}";
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