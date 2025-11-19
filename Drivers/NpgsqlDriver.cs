using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Enum = Plugin.Enum;

namespace SqlcGenCsharp.Drivers;

public sealed class NpgsqlDriver : EnumDbDriver, IOne, IMany, IExec, IExecRows, IExecLastId, ICopyFrom
{
    private const string DefaultNpgsqlVersion = "8.0.6";

    public NpgsqlDriver(
        Options options,
        Catalog catalog,
        IList<Query> queries) :
        base(options, catalog, queries)
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

    protected override Dictionary<string, ColumnMapping> ColumnMappings { get; } =
        new()
        {
            /* Numeric data types */
            ["bool"] = new(
                new()
                {
                    { "bool", new() },
                    { "boolean", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetBoolean({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<bool[]>({ordinal})"
            ),
            ["short"] = new(
                new()
                {
                    { "int2", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetInt16({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<short[]>({ordinal})",
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
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetInt32({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<int[]>({ordinal})",
                convertFunc: x => $"Convert.ToInt32({x})"
            ),
            ["long"] = new(
                new()
                {
                    { "int8", new() },
                    { "bigint", new() },
                    { "bigserial", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetInt64({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<long[]>({ordinal})",
                convertFunc: x => $"Convert.ToInt64({x})"
            ),
            ["float"] = new(
                new()
                {
                    { "float4", new() },
                    { "real", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFloat({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<float[]>({ordinal})"
            ),
            ["decimal"] = new(
                new()
                {
                    { "numeric", new() },
                    { "decimal", new() },
                    { "money", new(NpgsqlTypeOverride: "NpgsqlDbType.Money") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetDecimal({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<decimal[]>({ordinal})"
            ),
            ["double"] = new(
                new()
                {
                    { "float8", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetDouble({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<double[]>({ordinal})"
            ),

            /* String data types */
            ["string"] = new(
                new()
                {
                    { "longtext", new() },
                    { "mediumtext", new() },
                    { "text", new() },
                    { "bpchar", new() },
                    { "tinytext", new() },
                    { "varchar", new() },
                    { "jsonpath", new() },
                    { "macaddr8", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetString({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<string[]>({ordinal})"
            ),

            /* Date and time data types */
            ["TimeSpan"] = new(
                new()
                {
                    { "time", new(NpgsqlTypeOverride: "NpgsqlDbType.Time") },
                    { "interval", new(NpgsqlTypeOverride: "NpgsqlDbType.Interval") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<TimeSpan>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<TimeSpan[]>({ordinal})"
            ),
            ["DateTime"] = new(
                new()
                {
                    { "date", new(NpgsqlTypeOverride: "NpgsqlDbType.Date") },
                    { "timestamp", new(NpgsqlTypeOverride: "NpgsqlDbType.Timestamp") },
                    { "timestamptz", new(NpgsqlTypeOverride: "NpgsqlDbType.TimestampTz") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetDateTime({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<DateTime[]>({ordinal})"
            ),
            ["Instant"] = new(
                [],
                readerFn: (ordinal, _) => $$"""
                    (new Func<NpgsqlDataReader, int, Instant>((r, o) =>
                    {
                       var dt = {{Variable.Reader.AsVarName()}}.GetDateTime(o);
                       if (dt.Kind != DateTimeKind.Utc)
                           dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                       return dt.ToInstant();
                    }))({{Variable.Reader.AsVarName()}}, {{ordinal}})
                """,
                writerFn: (el, _, notNull, isDapper, isLegacy) =>
                {
                    if (notNull)
                        return $"DateTime.SpecifyKind({el}.ToDateTimeUtc(), DateTimeKind.Unspecified)";
                    var nullValue = isDapper ? "null" : "(object)DBNull.Value";
                    return $"{el} is null ? {nullValue} : (DateTime?) DateTime.SpecifyKind({el}.Value.ToDateTimeUtc(), DateTimeKind.Unspecified)";
                },
                usingDirectives: ["System", "NodaTime", "NodaTime.Extensions"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(Instant), new NodaInstantTypeHandler());",
                sqlMapperImpl: DateTimeNodaInstantTypeHandler
            ),

            /* Unstructured data types */
            ["JsonElement"] = new(
                new()
                {
                    { "json", new(NpgsqlTypeOverride: "NpgsqlDbType.Json") },
                    { "jsonb", new(NpgsqlTypeOverride: "NpgsqlDbType.Jsonb") }
                },
                readerFn: (ordinal, _) => $"JsonSerializer.Deserialize<JsonElement>(reader.GetString({ordinal}))",
                writerFn: (el, _, notNull, isDapper, isLegacy) =>
                {
                    if (notNull)
                        return $"{el}";
                    var nullValue = isDapper ? "null" : "(object)DBNull.Value";
                    return $"{el}.HasValue ? (object) {el}.Value : {nullValue}";
                },
                usingDirectives: ["System.Text.Json"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(JsonElement), new JsonElementTypeHandler());",
                sqlMapperImpl: JsonElementTypeHandler
            ),
            ["XmlDocument"] = new(
                new()
                {
                    { "xml", new(NpgsqlTypeOverride: "NpgsqlDbType.Xml") }
                },
                readerFn: (ordinal, dbType) => $$"""
                    (new Func<NpgsqlDataReader, int, XmlDocument>((r, o) =>
                    {
                       var xmlDoc = new XmlDocument(); 
                       xmlDoc.LoadXml(r.GetString(o)); 
                       return xmlDoc; 
                    }))({{Variable.Reader.AsVarName()}}, {{ordinal}})
                """,
                writerFn: (el, dbType, notNull, isDapper, isLegacy) =>
                {
                    if (notNull)
                        return $"{el}.OuterXml";
                    var nullValue = isDapper ? "null" : "(object)DBNull.Value";
                    return $"{el} != null ? {el}.OuterXml : {nullValue}";
                },
                usingDirectives: ["System.Xml"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(XmlDocument), new XmlDocumentTypeHandler());",
                sqlMapperImpl: XmlDocumentTypeHandler
            ),

            /* Geometric data types */
            ["NpgsqlPoint"] = new(
                new()
                {
                    { "point", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlPoint>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlPoint[]>({ordinal})",
                usingDirectives: ["NpgsqlTypes"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlPoint), new NpgsqlTypeHandler<NpgsqlPoint>());"
            ),
            ["NpgsqlLine"] = new(
                new()
                {
                    { "line", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlLine>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlLine[]>({ordinal})",
                usingDirectives: ["NpgsqlTypes"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlLine), new NpgsqlTypeHandler<NpgsqlLine>());"
            ),
            ["NpgsqlLSeg"] = new(
                new()
                {
                    { "lseg", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlLSeg>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlLSeg[]>({ordinal})",
                usingDirectives: ["NpgsqlTypes"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlLSeg), new NpgsqlTypeHandler<NpgsqlLSeg>());"
            ),
            ["NpgsqlBox"] = new(
                new()
                {
                    { "box", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlBox>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlBox[]>({ordinal})",
                usingDirectives: ["NpgsqlTypes"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlBox), new NpgsqlTypeHandler<NpgsqlBox>());"
            ),
            ["NpgsqlPath"] = new(
                new()
                {
                    { "path", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlPath>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlPath[]>({ordinal})",
                usingDirectives: ["NpgsqlTypes"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlPath), new NpgsqlTypeHandler<NpgsqlPath>());"
            ),
            ["NpgsqlPolygon"] = new(
                new()
                {
                    { "polygon", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlPolygon>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlPolygon[]>({ordinal})",
                usingDirectives: ["NpgsqlTypes"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlPolygon), new NpgsqlTypeHandler<NpgsqlPolygon>());"
            ),
            ["NpgsqlCircle"] = new(
                new()
                {
                    { "circle", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlCircle>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlCircle[]>({ordinal})",
                usingDirectives: ["NpgsqlTypes"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlCircle), new NpgsqlTypeHandler<NpgsqlCircle>());"
            ),

            /* Network data types */
            ["NpgsqlCidr"] = new(
                new()
                {
                    { "cidr", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlCidr>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlCidr[]>({ordinal})",
                usingDirectives: ["NpgsqlTypes"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlCidr), new NpgsqlTypeHandler<NpgsqlCidr>());"
            ),
            ["IPAddress"] = new(
                new()
                {
                    { "inet", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<IPAddress>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<IPAddress[]>({ordinal})",
                usingDirectives: ["System.Net"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(IPAddress), new NpgsqlTypeHandler<IPAddress>());"
            ),
            ["PhysicalAddress"] = new(
                new()
                {
                    { "macaddr", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<PhysicalAddress>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<PhysicalAddress[]>({ordinal})",
                usingDirectives: ["System.Net.NetworkInformation"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(PhysicalAddress), new NpgsqlTypeHandler<PhysicalAddress>());"
            ),

            /* Full-text search data types */
            ["NpgsqlTsQuery"] = new(
                new()
                {
                    { "tsquery", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlTsQuery>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlTsQuery[]>({ordinal})",
                usingDirectives: ["NpgsqlTypes"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlTsQuery), new NpgsqlTypeHandler<NpgsqlTsQuery>());"
            ),
            ["NpgsqlTsVector"] = new(
                new()
                {
                    { "tsvector", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlTsVector>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlTsVector[]>({ordinal})",
                usingDirectives: ["NpgsqlTypes"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlTsVector), new NpgsqlTypeHandler<NpgsqlTsVector>());"
            ),

            /* Other data types */
            ["Guid"] = new(
                new()
                {
                    { "uuid", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<Guid>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<Guid[]>({ordinal})",
                convertFunc: x => $"Guid.Parse({x}?.ToString())"
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
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<byte[]>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<byte[][]>({ordinal})"
            ),
            ["object"] = new(
                new()
                {
                    { "anyarray", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetValue({ordinal})"
            )
        };

    public override string TransactionClassName => "NpgsqlTransaction";

    private static readonly SqlMapperImplFunc JsonElementTypeHandler = _ => $$"""
        private class JsonElementTypeHandler : SqlMapper.TypeHandler<JsonElement>
        {
            public override JsonElement Parse(object value)
            {
                if (value is string s)
                    return JsonDocument.Parse(s).RootElement;
                throw new DataException($"Cannot convert {value?.GetType()} to JsonElement");
            }

            public override void SetValue(IDbDataParameter parameter, JsonElement value)
            {
                parameter.Value = value;
            }
        }
        """;

    private static readonly SqlMapperImplFunc XmlDocumentTypeHandler = isDotnetCore => $$"""
        private class XmlDocumentTypeHandler : SqlMapper.TypeHandler<XmlDocument>
        {
            public override XmlDocument Parse(object value)
            {
                if (value is string s)
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(s);
                    return xmlDoc;
                }
                throw new DataException($"Cannot convert {value?.GetType()} to XmlDocument");
            }

            public override void SetValue(IDbDataParameter parameter, XmlDocument{{(isDotnetCore ? "?" : string.Empty)}} value)
            {
                if (value is null)
                    return;
                parameter.Value = value.OuterXml;
            }
        }
        """;

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

    public override IDictionary<string, string> GetPackageReferences()
    {
        return base
            .GetPackageReferences()
            .Merge(new Dictionary<string, string>
            {
                { "Npgsql", Options.OverrideDriverVersion != string.Empty ? Options.OverrideDriverVersion : DefaultNpgsqlVersion }
            });
    }

    public override ISet<string> GetUsingDirectivesForQueries()
    {
        return base.GetUsingDirectivesForQueries().AddRangeExcludeNulls(
        [
            "Npgsql",
        ]);
    }

    public override ISet<string> GetUsingDirectivesForModels()
    {
        return base.GetUsingDirectivesForModels().AddRangeExcludeNulls(
        [
            "System",
            "System.Collections.Generic"
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
                 """)!
        ];
    }

    public override MemberDeclarationSyntax[] GetEnumExtensionsMembers(string name, IList<string> possibleValues)
    {
        return [.. base
            .GetEnumExtensionsMembers(name, possibleValues)
            .AddRangeExcludeNulls([
                ParseMemberDeclaration($$"""
                    private static readonly Dictionary<{{name}}, string> EnumToString = new Dictionary<{{name}}, string>()
                    {
                        [{{name}}.Invalid] = string.Empty,
                        {{possibleValues
                                .Select(v => $"[{name}.{v.ToPascalCase()}] = \"{v}\"")
                                .JoinByComma()}}
                    };
                    """)!,
                ParseMemberDeclaration($$"""
                    public static string Stringify(this {{name}} me)
                    {
                        return EnumToString[me];
                    }
                    """)!
            ])];
    }

    public override ConnectionGenCommands EstablishConnection(Query query)
    {
        var connectionStringVar = Variable.ConnectionString.AsPropertyName();
        var connectionVar = Variable.Connection.AsVarName();
        var embedTableExists = query.Columns.Any(c => c.EmbedTable is not null);
        var useOpenConnection = query.Cmd == ":copyfrom" || (Options.UseDapper && !embedTableExists);
        var optionalNotNullVerify = Options.DotnetFramework.IsDotnetCore() ? "!" : string.Empty;

        return useOpenConnection
            ? new ConnectionGenCommands(
                EstablishConnection: $"var {connectionVar} = new NpgsqlConnection({connectionStringVar})",
                ConnectionOpen: string.Empty,
                WrapInUsing: true
            )
            : new ConnectionGenCommands(
                EstablishConnection: $"var {connectionVar} = GetDataSource()",
                ConnectionOpen: string.Empty,
                WrapInUsing: false
            );
    }

    public override string[] GetClassBaseTypes()
    {
        return ["IDisposable"];
    }

    public override string[] GetConstructorStatements()
    {
        var baseStatements = base.GetConstructorStatements();
        var dataSourceVar = Variable.DataSource.AsFieldName();
        var connectionStringVar = Variable.ConnectionString.AsVarName();
        var optionalNotNullVerify = Options.DotnetFramework.IsDotnetCore() ? "!" : string.Empty;

        return
        [
            .. baseStatements,
            $"""
                {dataSourceVar} = new Lazy<NpgsqlDataSource>(() => NpgsqlDataSource.Create({connectionStringVar}{optionalNotNullVerify}), LazyThreadSafetyMode.ExecutionAndPublication);
            """,
        ];
    }

    public override MemberDeclarationSyntax[] GetAdditionalClassMembers()
    {
        var dataSourceField = Variable.DataSource.AsFieldName();
        var optionalNotNullVerify = Options.DotnetFramework.IsDotnetCore() ? "?" : string.Empty;
        var fieldDeclaration = ParseMemberDeclaration($$"""
            private readonly Lazy<NpgsqlDataSource>{{optionalNotNullVerify}} {{dataSourceField}};
        """)!;
        
        var getDataSourceMethod = ParseMemberDeclaration($$"""
            private NpgsqlDataSource GetDataSource()
            {
                if ({{dataSourceField}} == null)
                    throw new InvalidOperationException("ConnectionString is required when not using a transaction");
                return {{dataSourceField}}.Value;
            }
            """)!;
        
        return [fieldDeclaration, getDataSourceMethod];
    }

    public override MemberDeclarationSyntax? GetDisposeMethodImpl()
    {
        return ParseMemberDeclaration($$"""
            public void Dispose()
            {
                GC.SuppressFinalize(this);
                if ({{Variable.DataSource.AsFieldName()}}?.IsValueCreated == true)
                    {{Variable.DataSource.AsFieldName()}}.Value.Dispose();
            }
        """)!;
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
            var qualifiedTableName = !string.IsNullOrEmpty(query.InsertIntoTable.Schema) 
                ? $"{query.InsertIntoTable.Schema}.{query.InsertIntoTable.Name}" 
                : query.InsertIntoTable.Name;
            return $"COPY {qualifiedTableName} ({copyParams}) FROM STDIN (FORMAT BINARY)";
        }
    }

    public string GetCopyFromImpl(Query query, string queryTextConstant)
    {
        var connectionCommands = EstablishConnection(query);
        var beginBinaryImport = $"{Variable.Connection.AsVarName()}.BeginBinaryImportAsync({queryTextConstant}";
        var connectionVar = Variable.Connection.AsVarName();
        var writerVar = Variable.Writer.AsVarName();

        var addRowsToCopyCommand = AddRowsToCopyCommand();
        return $$"""
            using ({{connectionCommands.EstablishConnection}})
            {
                {{connectionCommands.ConnectionOpen.AppendSemicolonUnlessEmpty()}}
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
                        var param = $"{rowVar}.{p.Column.Name.ToPascalCase()}";
                        var writerFn = GetWriterFn(p.Column, query);
                        var paramToWrite = writerFn is null ? param : writerFn(
                            param,
                            p.Column.Type.Name,
                            IsColumnNotNull(p.Column, query),
                            false,
                            Options.DotnetFramework.IsDotnetLegacy());

                        var partialStmt = $"await {writerVar}.WriteAsync({paramToWrite}";
                        var typeOverride = GetColumnDbTypeOverride(p.Column);
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

    private string? GetColumnDbTypeOverride(Column column)
    {
        if (column.IsArray)
            return null; // TODO: handle array columns
        var columnType = column.Type.Name.ToLower();
        foreach (var columnMapping in ColumnMappings.Values)
        {
            if (columnMapping.DbTypes.TryGetValue(columnType, out var dbTypeOverride))
                return dbTypeOverride.NpgsqlTypeOverride;
        }
        return null;
    }

    public override WriterFn? GetWriterFn(Column column, Query query)
    {
        var csharpType = GetCsharpTypeWithoutNullableSuffix(column, query);
        var writerFn = ColumnMappings.GetValueOrDefault(csharpType)?.WriterFn;
        if (writerFn is not null)
            return writerFn;

        if (GetEnumType(column) is not null)
        {
            return (el, dbType, notNull, isDapper, isLegacy) =>
            {
                var nullValue = isDapper ? "null" : "(object)DBNull.Value";
                return notNull
                    ? $"{el}.Stringify()"
                    : $"{el} != null ? {el}.Value.Stringify() : {nullValue}";
            };
        }

        static string DefaultWriterFn(string el, string dbType, bool notNull, bool isDapper, bool isLegacy) => notNull ? el : $"{el} ?? (object)DBNull.Value";
        return Options.UseDapper ? null : DefaultWriterFn;
    }

    public override string AddParametersToCommand(Query query)
    {
        return query.Params.Select(p =>
        {
            var commandVar = Variable.Command.AsVarName();
            var param = $"{Variable.Args.AsVarName()}.{p.Column.Name.ToPascalCase()}";

            if (p.Column.IsSqlcSlice)
                return $$"""
                         for (int i = 0; i < {{param}}.Length; i++)
                             {{commandVar}}.Parameters.AddWithValue($"@{{p.Column.Name}}Arg{i}", {{param}}[i]);
                         """;

            var writerFn = GetWriterFn(p.Column, query);
            var paramToWrite = writerFn is null
                ? param
                : writerFn(
                    param,
                    p.Column.Type.Name,
                    IsColumnNotNull(p.Column, query),
                    Options.UseDapper,
                    Options.DotnetFramework.IsDotnetLegacy());

            var typeOverride = GetColumnDbTypeOverride(p.Column);
            var optionalNpgsqlTypeOverride = typeOverride is null
                ? string.Empty
                : $"{typeOverride}, ";
            var addParamToCommand = $"""{commandVar}.Parameters.AddWithValue("@{p.Column.Name}", {optionalNpgsqlTypeOverride}{paramToWrite});""";
            return addParamToCommand;
        }).JoinByNewLine();
    }

    private static (string, string) GetEnumSchemaAndName(Column column)
    {
        var schemaName = column.Type.Schema;
        var enumName = column.Type.Name;
        if (schemaName == string.Empty && enumName.Contains('.'))
        {
            var schemaAndEnum = enumName.Split('.');
            schemaName = schemaAndEnum[0];
            enumName = schemaAndEnum[1];
        }
        return (schemaName, enumName);
    }

    protected override string GetEnumReader(Column column, int ordinal)
    {
        var enumName = EnumToModelName(column);
        var readStmt = $"{Variable.Reader.AsVarName()}.GetString({ordinal})";
        return $"{readStmt}.To{enumName}()";
    }

    protected override Enum? GetEnumType(Column column)
    {
        var (schemaName, enumName) = GetEnumSchemaAndName(column);
        if (!Enums.TryGetValue(schemaName, value: out var enumsInSchema))
            return null;
        return enumsInSchema.GetValueOrDefault(enumName);
    }

    protected override string EnumToCsharpDataType(Column column)
    {
        var (schemaName, enumName) = GetEnumSchemaAndName(column);
        return $"{schemaName}.{enumName}".ToPascalCase();
    }

    public override string EnumToModelName(string schemaName, Enum enumType)
    {
        return $"{schemaName}.{enumType.Name}".ToPascalCase();
    }

    protected override string EnumToModelName(Column column)
    {
        var (schemaName, enumName) = GetEnumSchemaAndName(column);
        return $"{schemaName}.{enumName}".ToPascalCase();
    }
}