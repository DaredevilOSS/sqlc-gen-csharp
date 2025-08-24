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
                readerFn: ordinal => $"reader.GetBoolean({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<bool[]>({ordinal})"
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
            ["float"] = new(
                new()
                {
                    { "float4", new() },
                    { "real", new() }
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
                readerFn: ordinal => $"reader.GetString({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<string[]>({ordinal})"
            ),

            /* Date and time data types */
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

            /* Unstructured data types */
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
            ["XmlDocument"] = new(
                new()
                {
                    { "xml", new(NpgsqlTypeOverride: "NpgsqlDbType.Xml") }
                },
                readerFn: ordinal => $$"""
                    (new Func<NpgsqlDataReader, int, XmlDocument>((r, o) =>
                    {
                       var xmlDoc = new XmlDocument(); 
                       xmlDoc.LoadXml(r.GetString(o)); 
                       return xmlDoc; 
                    }))({{Variable.Reader.AsVarName()}}, {{ordinal}})
                """,
                writerFn: (el, notNull, isDapper) =>
                {
                    if (notNull)
                        return $"{el}.OuterXml";
                    var nullValue = isDapper ? "null" : "(object)DBNull.Value";
                    return $"{el} != null ? {el}.OuterXml : {nullValue}";
                },
                usingDirective: "System.Xml",
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(XmlDocument), new XmlDocumentTypeHandler());",
                sqlMapperImpl: XmlDocumentTypeHandler
            ),

            /* Geometric data types */
            ["NpgsqlPoint"] = new(
                new()
                {
                    { "point", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlPoint>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlPoint[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlPoint), new NpgsqlTypeHandler<NpgsqlPoint>());"
            ),
            ["NpgsqlLine"] = new(
                new()
                {
                    { "line", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlLine>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlLine[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlLine), new NpgsqlTypeHandler<NpgsqlLine>());"
            ),
            ["NpgsqlLSeg"] = new(
                new()
                {
                    { "lseg", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlLSeg>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlLSeg[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlLSeg), new NpgsqlTypeHandler<NpgsqlLSeg>());"
            ),
            ["NpgsqlBox"] = new(
                new()
                {
                    { "box", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlBox>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlBox[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlBox), new NpgsqlTypeHandler<NpgsqlBox>());"
            ),
            ["NpgsqlPath"] = new(
                new()
                {
                    { "path", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlPath>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlPath[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlPath), new NpgsqlTypeHandler<NpgsqlPath>());"
            ),
            ["NpgsqlPolygon"] = new(
                new()
                {
                    { "polygon", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlPolygon>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlPolygon[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlPolygon), new NpgsqlTypeHandler<NpgsqlPolygon>());"
            ),
            ["NpgsqlCircle"] = new(
                new()
                {
                    { "circle", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlCircle>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlCircle[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlCircle), new NpgsqlTypeHandler<NpgsqlCircle>());"
            ),

            /* Network data types */
            ["NpgsqlCidr"] = new(
                new()
                {
                    { "cidr", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlCidr>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlCidr[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlCidr), new NpgsqlTypeHandler<NpgsqlCidr>());"
            ),
            ["IPAddress"] = new(
                new()
                {
                    { "inet", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<IPAddress>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<IPAddress[]>({ordinal})",
                usingDirective: "System.Net",
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(IPAddress), new NpgsqlTypeHandler<IPAddress>());"
            ),
            ["PhysicalAddress"] = new(
                new()
                {
                    { "macaddr", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<PhysicalAddress>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<PhysicalAddress[]>({ordinal})",
                usingDirective: "System.Net.NetworkInformation",
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(PhysicalAddress), new NpgsqlTypeHandler<PhysicalAddress>());"
            ),

            /* Full-text search data types */
            ["NpgsqlTsQuery"] = new(
                new()
                {
                    { "tsquery", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlTsQuery>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlTsQuery[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlTsQuery), new NpgsqlTypeHandler<NpgsqlTsQuery>());"
            ),
            ["NpgsqlTsVector"] = new(
                new()
                {
                    { "tsvector", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<NpgsqlTsVector>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<NpgsqlTsVector[]>({ordinal})",
                usingDirective: "NpgsqlTypes",
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlTsVector), new NpgsqlTypeHandler<NpgsqlTsVector>());"
            ),

            /* Other data types */
            ["Guid"] = new(
                new()
                {
                    { "uuid", new() }
                },
                readerFn: ordinal => $"reader.GetFieldValue<Guid>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<Guid[]>({ordinal})",
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
                readerFn: ordinal => $"reader.GetFieldValue<byte[]>({ordinal})",
                readerArrayFn: ordinal => $"reader.GetFieldValue<byte[][]>({ordinal})"
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

    private const string XmlDocumentTypeHandler =
        """
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

            public override void SetValue(IDbDataParameter parameter, XmlDocument value)
            {
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

    public override ConnectionGenCommands EstablishConnection(Query query)
    {
        var connectionStringVar = Variable.ConnectionString.AsPropertyName();
        var connectionVar = Variable.Connection.AsVarName();
        var embedTableExists = query.Columns.Any(c => c.EmbedTable is not null);
        var useOpenConnection = query.Cmd == ":copyfrom" || (Options.UseDapper && !embedTableExists);
        var optionalNotNullVerify = Options.DotnetFramework.IsDotnetCore() ? "!" : string.Empty;

        return useOpenConnection
            ? new ConnectionGenCommands(
                $"var {connectionVar} = new NpgsqlConnection({connectionStringVar})",
                string.Empty
            )
            : new ConnectionGenCommands(
                $"var {connectionVar} = NpgsqlDataSource.Create({connectionStringVar}{optionalNotNullVerify})",
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
                        var writerFn = GetWriterFn(p.Column, query);
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

    public override Func<string, bool, bool, string>? GetWriterFn(Column column, Query query)
    {
        var csharpType = GetCsharpTypeWithoutNullableSuffix(column, query);
        var writerFn = ColumnMappings.GetValueOrDefault(csharpType)?.WriterFn;
        if (writerFn is not null)
            return writerFn;

        if (GetEnumType(column) is not null)
        {
            return (el, notNull, isDapper) =>
            {
                var enumToStringStmt = $"{el}.Value.Stringify()";
                var nullValue = isDapper ? "null" : "(object)DBNull.Value";
                return notNull
                    ? enumToStringStmt
                    : $"{el} != null ? {enumToStringStmt} : {nullValue}";
            };
        }

        static string DefaultWriterFn(string el, bool notNull, bool isDapper) => notNull ? el : $"{el} ?? (object)DBNull.Value";
        return Options.UseDapper ? null : DefaultWriterFn;
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