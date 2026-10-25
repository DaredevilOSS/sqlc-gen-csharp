using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
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
        // `string?` as a type argument is only valid with nullable reference types, which legacy targets lack
        var hstoreValueType = Options.DotnetFramework.IsDotnetCore() ? "string?" : "string";
        ColumnMappings[$"Dictionary<string, {hstoreValueType}>"] = HstoreMapping(hstoreValueType);

        NullableTypes.UnionWith(
        [
            "NpgsqlRange<int>",
            "NpgsqlRange<long>",
            "NpgsqlRange<decimal>",
            "NpgsqlRange<DateTime>",
            "uint",
            "ulong",
            "NpgsqlTid",
            "NpgsqlLogSequenceNumber"
        ]);

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
                    { "int2", new() },
                    { "smallserial", new() },
                    { "serial2", new() }
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
                    { "serial", new() },
                    { "serial4", new() }
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
                    { "bigserial", new() },
                    { "serial8", new() }
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
                    { "money", new(DbTypeOverride: "NpgsqlDbType.Money") }
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
                    { "macaddr8", new() },
                    // Fully qualified so that existing generated files do not need an additional `using NpgsqlTypes;`
                    { "citext", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.Citext") },
                    { "ltree", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.LTree") },
                    { "lquery", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.LQuery") },
                    { "ltxtquery", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.LTxtQuery") },
                    { "name", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.Name") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetString({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<string[]>({ordinal})"
            ),

            /* Date and time data types */
            ["TimeSpan"] = new(
                new()
                {
                    { "time", new(DbTypeOverride: "NpgsqlDbType.Time") },
                    { "interval", new(DbTypeOverride: "NpgsqlDbType.Interval") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<TimeSpan>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<TimeSpan[]>({ordinal})"
            ),
            ["DateTime"] = new(
                new()
                {
                    { "date", new(DbTypeOverride: "NpgsqlDbType.Date") },
                    { "timestamp", new(DbTypeOverride: "NpgsqlDbType.Timestamp") },
                    { "timestamptz", new(DbTypeOverride: "NpgsqlDbType.TimestampTz") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetDateTime({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<DateTime[]>({ordinal})",
                usingDirectives: ["NpgsqlTypes"]
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

            /* Bit string data types */
            ["BitArray"] = new(
                new()
                {
                    { "bit", new(DbTypeOverride: "NpgsqlDbType.Bit") },
                    { "varbit", new(DbTypeOverride: "NpgsqlDbType.Varbit") },
                    { "bit varying", new(DbTypeOverride: "NpgsqlDbType.Varbit") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<BitArray>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<BitArray[]>({ordinal})",
                usingDirectives: ["System.Collections", "NpgsqlTypes"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(BitArray), new BitArrayTypeHandler());",
                sqlMapperImpl: BitArrayTypeHandler
            ),

            /* System data types */
            ["uint"] = new(
                new()
                {
                    { "oid", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.Oid") },
                    { "xid", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.Xid") },
                    { "cid", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.Cid") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<uint>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<uint[]>({ordinal})"
            ),
            ["ulong"] = new(
                new()
                {
                    { "xid8", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.Xid8") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<ulong>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<ulong[]>({ordinal})"
            ),
            ["NpgsqlTid"] = new(
                new()
                {
                    { "tid", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.Tid") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlTid>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlTid[]>({ordinal})",
                usingDirectives: ["NpgsqlTypes"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlTid), new NpgsqlTypeHandler<NpgsqlTid>());"
            ),
            ["NpgsqlLogSequenceNumber"] = new(
                new()
                {
                    { "pg_lsn", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.PgLsn") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlLogSequenceNumber>({ordinal})",
                readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<NpgsqlLogSequenceNumber[]>({ordinal})",
                usingDirectives: ["NpgsqlTypes"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(NpgsqlLogSequenceNumber), new NpgsqlTypeHandler<NpgsqlLogSequenceNumber>());"
            ),

            /* Range data types */
            ["NpgsqlRange<int>"] = RangeMapping("int", new()
            {
                { "int4range", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.IntegerRange") }
            }),
            ["NpgsqlRange<long>"] = RangeMapping("long", new()
            {
                { "int8range", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.BigIntRange") }
            }),
            ["NpgsqlRange<decimal>"] = RangeMapping("decimal", new()
            {
                { "numrange", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.NumericRange") }
            }),
            ["NpgsqlRange<DateTime>"] = RangeMapping("DateTime", new()
            {
                { "daterange", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.DateRange") },
                { "tsrange", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.TimestampRange") },
                { "tstzrange", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.TimestampTzRange") }
            }),

            /* Multirange data types (PostgreSQL 14+) */
            ["NpgsqlRange<int>[]"] = MultirangeMapping("int", new()
            {
                { "int4multirange", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.IntegerMultirange") }
            }),
            ["NpgsqlRange<long>[]"] = MultirangeMapping("long", new()
            {
                { "int8multirange", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.BigIntMultirange") }
            }),
            ["NpgsqlRange<decimal>[]"] = MultirangeMapping("decimal", new()
            {
                { "nummultirange", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.NumericMultirange") }
            }),
            ["NpgsqlRange<DateTime>[]"] = MultirangeMapping("DateTime", new()
            {
                { "datemultirange", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.DateMultirange") },
                { "tsmultirange", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.TimestampMultirange") },
                { "tstzmultirange", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.TimestampTzMultirange") }
            }),

            /* Unstructured data types */
            ["JsonElement"] = new(
                new()
                {
                    { "json", new(DbTypeOverride: "NpgsqlDbType.Json") },
                    { "jsonb", new(DbTypeOverride: "NpgsqlDbType.Jsonb") }
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
                    { "xml", new(DbTypeOverride: "NpgsqlDbType.Xml") }
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
                    { "any", new() },
                    { "anyarray", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetValue({ordinal})"
            )
        };

    private static ColumnMapping HstoreMapping(string valueType)
    {
        var dictionaryType = $"Dictionary<string, {valueType}>";
        return new(
            new()
            {
                { "hstore", new(DbTypeOverride: "NpgsqlTypes.NpgsqlDbType.Hstore") }
            },
            readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<{dictionaryType}>({ordinal})",
            readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<{dictionaryType}[]>({ordinal})"
        );
    }

    private static ColumnMapping RangeMapping(string subtype, Dictionary<string, DbTypeInfo> dbTypes)
    {
        var rangeType = $"NpgsqlRange<{subtype}>";
        return new(
            dbTypes,
            readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<{rangeType}>({ordinal})",
            readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<{rangeType}[]>({ordinal})",
            usingDirectives: ["NpgsqlTypes"],
            sqlMapper: $"SqlMapper.AddTypeHandler(typeof({rangeType}), new NpgsqlTypeHandler<{rangeType}>());"
        );
    }

    // A multirange is read and written as an array of ranges
    private static ColumnMapping MultirangeMapping(string subtype, Dictionary<string, DbTypeInfo> dbTypes)
    {
        var rangeType = $"NpgsqlRange<{subtype}>";
        return new(
            dbTypes,
            readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<{rangeType}[]>({ordinal})",
            readerArrayFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<{rangeType}[][]>({ordinal})",
            usingDirectives: ["NpgsqlTypes"]
        );
    }

    public override string TransactionClassName => "NpgsqlTransaction";

    // Npgsql returns bool for bit(1) and BitArray for bit(n > 1) / varbit when read through GetValue (which Dapper uses)
    private static readonly SqlMapperImplFunc BitArrayTypeHandler = isDotnetCore => $$"""
        private class BitArrayTypeHandler : SqlMapper.TypeHandler<BitArray>
        {
            public override BitArray Parse(object value)
            {
                if (value is BitArray bits)
                    return bits;
                if (value is bool b)
                    return new BitArray(new[] { b });
                throw new DataException($"Cannot convert {value?.GetType()} to BitArray");
            }

            public override void SetValue(IDbDataParameter parameter, BitArray{{(isDotnetCore ? "?" : string.Empty)}} value)
            {
                parameter.Value = value;
            }
        }
        """;

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

        return usingDirectives
            .AddRangeExcludeNulls(["NpgsqlTypes"])
            .AddRangeIf(["Npgsql"], TypedParameterExistsInQueries());
    }

    private static bool IsRangeCsharpType(string csharpType) => csharpType.StartsWith("NpgsqlRange<");

    // Types for which Dapper cannot infer the right NpgsqlDbType from the CLR type alone
    private static bool RequiresTypedDapperParameter(string csharpType) =>
        IsRangeCsharpType(csharpType) || csharpType is "uint" or "ulong" or "NpgsqlTid" or "NpgsqlLogSequenceNumber";

    private bool TypedParameterExistsInQueries()
    {
        return Queries.Any(q => q.Params.Any(p => RequiresTypedDapperParameter(GetCsharpTypeWithoutNullableSuffix(p.Column, q))));
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
            .. GetNpgsqlTypedParameterMembers(optionalDotnetCoreNullable),
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

    // Dapper infers the parameter type from the CLR type, which is ambiguous for ranges (NpgsqlRange<DateTime> could be
    // daterange, tsrange or tstzrange) and for oid/xid/cid (all uint), or not inferred at all (tid), so these are sent with an explicit NpgsqlDbType.
    private MemberDeclarationSyntax[] GetNpgsqlTypedParameterMembers(string optionalNullable)
    {
        if (!TypedParameterExistsInQueries())
            return [];

        return
        [
            ParseMemberDeclaration($$"""
                 internal class NpgsqlTypedParameter : SqlMapper.ICustomQueryParameter
                 {
                     private readonly object{{optionalNullable}} _value;
                     private readonly NpgsqlDbType _dbType;

                     public NpgsqlTypedParameter(object{{optionalNullable}} value, NpgsqlDbType dbType)
                     {
                         _value = value;
                         _dbType = dbType;
                     }

                     public void AddParameter(IDbCommand command, string name)
                     {
                         command.Parameters.Add(new NpgsqlParameter(name, _dbType) { Value = _value ?? DBNull.Value });
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
        var connectionVar = Variable.Connection.AsVarName();
        return new(
            GetConnectionOrDataSource: new($"var {connectionVar} = await GetDataSource().OpenConnectionAsync({Cancellation.Argument()})", true)
        // ConnectionOpen: string.Empty
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

        return [
            ParseMemberDeclaration($$"""
                private readonly Lazy<NpgsqlDataSource>{{optionalNotNullVerify}} {{dataSourceField}};
            """)!,
            ParseMemberDeclaration($$"""
            private NpgsqlDataSource GetDataSource()
            {
                if ({{dataSourceField}} == null)
                    throw new InvalidOperationException("ConnectionString is required when not using a transaction");
                return {{dataSourceField}}.Value;
            }
            """)!,
            ParseMemberDeclaration($$"""
            public void Dispose()
            {
                GC.SuppressFinalize(this);
                if ({{dataSourceField}}?.IsValueCreated == true)
                    {{dataSourceField}}.Value.Dispose();
            }
            """)!
        ];
    }

    public override CommandGenCommands CreateSqlCommand(string sqlTextConstant)
    {
        var commandVar = Variable.Command.AsVarName();
        return new CommandGenCommands(
            CommandCreation: new(
                $"var {commandVar} = {Variable.Connection.AsVarName()}.CreateCommand()",
                true),
            SetCommandText: $"{commandVar}.CommandText = {sqlTextConstant}",
            PrepareCommand: string.Empty
        );
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
        var beginBinaryImport = $"{Variable.Connection.AsVarName()}.BeginBinaryImportAsync({queryTextConstant}{Cancellation.TrailingArgument()}";
        var connectionVar = Variable.Connection.AsVarName();
        var writerVar = Variable.Writer.AsVarName();
        var commandBlock = $$"""
            using (var {{writerVar}} = await {{beginBinaryImport}}))
            {
                {{AddRowsToCopyCommand()}}
                await {{writerVar}}.CompleteAsync({{Cancellation.Argument()}});
            }
        """;
        return connectionCommands.GetConnectionOrDataSource.WrapBlock(
            $$"""
            {{connectionCommands.ConnectionOpen.AppendSemicolonUnlessEmpty()}}
            {{commandBlock}}
            """
        );

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
                            ? $"{partialStmt}{Cancellation.TrailingArgument()});"
                            : $"{partialStmt}, {typeOverride}{Cancellation.TrailingArgument()});";
                    })
                .JoinByNewLine();
            return $$"""
                     foreach (var {{rowVar}} in args) 
                     {
                          await {{writerVar}}.StartRowAsync({{Cancellation.Argument()}});
                          {{constructRowFields}}
                     }
                     """;
        }
    }

    // `NpgsqlRange<T>[]` is both an array of ranges and a multirange to Npgsql, which infers the multirange.
    // Array columns have no override otherwise, so only arrays of ranges are given an explicit one.
    public override string? GetColumnDbTypeOverride(Column column)
    {
        if (!column.IsArray)
            return base.GetColumnDbTypeOverride(column);

        var elementOverride = ColumnMappings
            .Where(m => m.Key.StartsWith("NpgsqlRange<"))
            .Select(m => m.Value.DbTypes.GetValueOrDefault(column.Type.Name.ToLower())?.DbTypeOverride)
            .FirstOrDefault(o => o is not null);
        return elementOverride is null ? null : $"NpgsqlTypes.NpgsqlDbType.Array | {elementOverride}";
    }

    public override WriterFn? GetWriterFn(Column column, Query query)
    {
        var csharpType = GetCsharpTypeWithoutNullableSuffix(column, query);
        if (Options.UseDapper && RequiresTypedDapperParameter(csharpType) && GetColumnDbTypeOverride(column) is { } explicitDbType)
            return (el, _, _, isDapper, _) => isDapper ? $"new Utils.NpgsqlTypedParameter({el}, {explicitDbType})" : el;

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
            var optionalTypeOverride = typeOverride is null
                ? string.Empty
                : $"{typeOverride}, ";
            var addParamToCommand = $"""{commandVar}.Parameters.AddWithValue("@{p.Column.Name}", {optionalTypeOverride}{paramToWrite});""";
            return addParamToCommand;
        }).JoinByNewLine();
    }

    private (string, string) GetEnumSchemaAndName(Column column)
    {
        var schemaName = column.Type.Schema;
        var enumName = column.Type.Name;
        if (schemaName == string.Empty && enumName.Contains('.'))
        {
            var schemaAndEnum = enumName.Split('.');
            schemaName = schemaAndEnum[0];
            enumName = schemaAndEnum[1];
        }
        if (schemaName == DefaultSchema)
            schemaName = string.Empty;
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