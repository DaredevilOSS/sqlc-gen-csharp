using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public sealed class MssqlDriver :
    DbDriver, IOne, IMany, IExec, IExecRows, IExecLastId, ICopyFrom
{
    private const string DefaultSqlClientVersion = "7.1.1";
    private const string DefaultSqlServerTypesVersion = "160.1000.6";

    // CLR types of the spatial and hierarchyid columns, from Microsoft.SqlServer.Types
    private static readonly HashSet<string> SqlServerTypes = ["SqlGeography", "SqlGeometry", "SqlHierarchyId"];

    public MssqlDriver(
        Options options,
        Catalog catalog,
        IList<Query> queries) : base(options, catalog, queries)
    {
        NullableTypes.Add("DateTimeOffset");
        NullableTypes.Add("SqlHierarchyId");
    }

    // Dapper sends a null byte[] as nvarchar, which SQL Server refuses to convert to a binary type
    private const string BinaryParameterImpl = """
        public class BinaryParameter : SqlMapper.ICustomQueryParameter
        {
            private readonly byte[]? _value;

            public BinaryParameter(byte[]? value)
            {
                _value = value;
            }

            public void AddParameter(IDbCommand command, string name)
            {
                var parameter = new SqlParameter(name, SqlDbType.VarBinary) { Value = (object?)_value ?? DBNull.Value };
                command.Parameters.Add(parameter);
            }
        }
        """;

    private static readonly SqlMapperImplFunc NodaInstantTypeHandler = _ => """
        private class NodaInstantTypeHandler : SqlMapper.TypeHandler<Instant>
        {
            public override Instant Parse(object value)
            {
                if (value is DateTime dt)
                    return DateTime.SpecifyKind(dt, DateTimeKind.Utc).ToInstant();
                if (value is DateTimeOffset dto)
                    return dto.ToInstant();
                throw new DataException($"Cannot convert {value?.GetType()} to Instant");
            }

            public override void SetValue(IDbDataParameter parameter, Instant value)
            {
                parameter.Value = value.ToDateTimeUtc();
                parameter.DbType = DbType.DateTime2;
            }
        }
        """;

    protected override Dictionary<string, ColumnMapping> ColumnMappings { get; } =
        new()
        {
            /* Integer data types */
            ["bool"] = new(
                new()
                {
                    { "bit", new(DbTypeOverride: "SqlDbType.Bit") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetBoolean({ordinal})",
                convertFunc: x => $"Convert.ToBoolean({x})"
            ),
            ["byte"] = new(
                new()
                {
                    { "tinyint", new(DbTypeOverride: "SqlDbType.TinyInt") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetByte({ordinal})",
                convertFunc: x => $"Convert.ToByte({x})"
            ),
            ["short"] = new(
                new()
                {
                    { "smallint", new(DbTypeOverride: "SqlDbType.SmallInt") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetInt16({ordinal})",
                convertFunc: x => $"Convert.ToInt16({x})"
            ),
            ["int"] = new(
                new()
                {
                    { "int", new(DbTypeOverride: "SqlDbType.Int") },
                    { "integer", new(DbTypeOverride: "SqlDbType.Int") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetInt32({ordinal})",
                convertFunc: x => $"Convert.ToInt32({x})"
            ),
            ["long"] = new(
                new()
                {
                    { "bigint", new(DbTypeOverride: "SqlDbType.BigInt") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetInt64({ordinal})",
                convertFunc: x => $"Convert.ToInt64({x})"
            ),

            /* Floating point data types */
            ["float"] = new(
                new()
                {
                    { "real", new(DbTypeOverride: "SqlDbType.Real") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFloat({ordinal})"
            ),
            ["double"] = new(
                new()
                {
                    { "float", new(DbTypeOverride: "SqlDbType.Float") },
                    { "double precision", new(DbTypeOverride: "SqlDbType.Float") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetDouble({ordinal})"
            ),
            // decimal types are left untyped on purpose: the precision and scale are inferred from the value
            ["decimal"] = new(
                new()
                {
                    { "decimal", new() },
                    { "numeric", new() },
                    { "money", new() },
                    { "smallmoney", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetDecimal({ordinal})"
            ),

            /* String data types */
            ["string"] = new(
                new()
                {
                    { "char", new(DbTypeOverride: "SqlDbType.Char") },
                    { "nchar", new(DbTypeOverride: "SqlDbType.NChar") },
                    { "varchar", new(DbTypeOverride: "SqlDbType.VarChar") },
                    { "nvarchar", new(DbTypeOverride: "SqlDbType.NVarChar") },
                    { "text", new(DbTypeOverride: "SqlDbType.Text") },
                    { "ntext", new(DbTypeOverride: "SqlDbType.NText") },
                    { "xml", new(DbTypeOverride: "SqlDbType.Xml") },
                    { "sysname", new(DbTypeOverride: "SqlDbType.NVarChar") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetString({ordinal})"
            ),

            /* Binary data types */
            ["byte[]"] = new(
                new()
                {
                    { "binary", new(DbTypeOverride: "SqlDbType.Binary") },
                    { "varbinary", new(DbTypeOverride: "SqlDbType.VarBinary") },
                    { "image", new(DbTypeOverride: "SqlDbType.Image") },
                    { "rowversion", new(DbTypeOverride: "SqlDbType.Timestamp") },
                    { "timestamp", new(DbTypeOverride: "SqlDbType.Timestamp") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<byte[]>({ordinal})",
                writerFn: (el, _, notNull, isDapper, _) =>
                {
                    if (isDapper)
                        return $"new Utils.BinaryParameter({el})";
                    return notNull ? el : $"{el} ?? (object)DBNull.Value";
                }
            ),

            /* Date and time data types */
            ["DateTime"] = new(
                new()
                {
                    { "date", new(DbTypeOverride: "SqlDbType.Date") },
                    { "datetime", new(DbTypeOverride: "SqlDbType.DateTime") },
                    { "datetime2", new(DbTypeOverride: "SqlDbType.DateTime2") },
                    { "smalldatetime", new(DbTypeOverride: "SqlDbType.SmallDateTime") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetDateTime({ordinal})",
                // Dapper sends DateTime as the less precise datetime unless told otherwise
                sqlMapper: "SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime2);"
            ),
            ["DateTimeOffset"] = new(
                new()
                {
                    { "datetimeoffset", new(DbTypeOverride: "SqlDbType.DateTimeOffset") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<DateTimeOffset>({ordinal})"
            ),
            ["TimeSpan"] = new(
                new()
                {
                    { "time", new(DbTypeOverride: "SqlDbType.Time") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<TimeSpan>({ordinal})"
            ),

            /* Other data types */
            ["Guid"] = new(
                new()
                {
                    { "uniqueidentifier", new(DbTypeOverride: "SqlDbType.UniqueIdentifier") }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetGuid({ordinal})",
                convertFunc: x => $"Guid.Parse({x}.ToString())"
            ),
            ["SqlGeography"] = new(
                new()
                {
                    { "geography", new(DbTypeOverride: "SqlDbType.Udt") }
                },
                readerFn: (ordinal, _) => $"SqlGeography.Deserialize({Variable.Reader.AsVarName()}.GetSqlBytes({ordinal}))",
                usingDirectives: ["Microsoft.SqlServer.Types"]
            ),
            ["SqlGeometry"] = new(
                new()
                {
                    { "geometry", new(DbTypeOverride: "SqlDbType.Udt") }
                },
                readerFn: (ordinal, _) => $"SqlGeometry.Deserialize({Variable.Reader.AsVarName()}.GetSqlBytes({ordinal}))",
                usingDirectives: ["Microsoft.SqlServer.Types"]
            ),
            ["SqlHierarchyId"] = new(
                new()
                {
                    { "hierarchyid", new(DbTypeOverride: "SqlDbType.Udt") }
                },
                readerFn: (ordinal, _) => $$"""
                    (new Func<System.Data.SqlTypes.SqlBytes, SqlHierarchyId>(b =>
                    {
                        var id = new SqlHierarchyId();
                        id.Read(new System.IO.BinaryReader(b.Stream));
                        return id;
                    }))({{Variable.Reader.AsVarName()}}.GetSqlBytes({{ordinal}}))
                    """,
                usingDirectives: ["Microsoft.SqlServer.Types"]
            ),

            /* Reachable only through overrides (datetime, datetime2, datetimeoffset columns) */
            ["Instant"] = new(
                [],
                readerFn: (ordinal, dbType) => dbType.ToLower() == "datetimeoffset"
                    ? $"{Variable.Reader.AsVarName()}.GetFieldValue<DateTimeOffset>({ordinal}).ToInstant()"
                    : $"DateTime.SpecifyKind({Variable.Reader.AsVarName()}.GetDateTime({ordinal}), DateTimeKind.Utc).ToInstant()",
                writerFn: (el, dbType, notNull, isDapper, _) =>
                {
                    var convert = dbType.ToLower() == "datetimeoffset" ? "ToDateTimeOffset()" : "ToDateTimeUtc()";
                    if (notNull)
                        return $"{el}.{convert}";
                    var nullValue = isDapper ? "null" : "(object)DBNull.Value";
                    return $"{el} is null ? {nullValue} : {el}.Value.{convert}";
                },
                usingDirectives: ["System", "NodaTime", "NodaTime.Extensions"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(Instant), new NodaInstantTypeHandler());",
                sqlMapperImpl: NodaInstantTypeHandler
            ),

            ["object"] = new(
                new()
                {
                    { "sql_variant", new(DbTypeOverride: "SqlDbType.Variant") },
                    { "any", new() },
                    // sqlc leaves the type of many expressions empty; they are read as object rather than failing
                    { "", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetValue({ordinal})"
            )
        };

    public override string TransactionClassName => "SqlTransaction";

    public override IDictionary<string, string> GetPackageReferences()
    {
        return base
            .GetPackageReferences()
            .Merge(new Dictionary<string, string>
            {
                { "Microsoft.Data.SqlClient", Options.OverrideDriverVersion != string.Empty ? Options.OverrideDriverVersion : DefaultSqlClientVersion }
            })
            .MergeIf(new Dictionary<string, string>
            {
                { "Microsoft.SqlServer.Types", DefaultSqlServerTypesVersion }
            }, SqlServerTypesInUse());
    }

    // Parameters get an explicit SqlDbType, otherwise a null is sent as nvarchar and
    // SQL Server refuses to convert it to binary types, and DateTime is always sent as the less precise datetime
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
            var paramToWrite = writerFn is null ? param : writerFn(
                param,
                p.Column.Type.Name,
                IsColumnNotNull(p.Column, query),
                Options.UseDapper,
                Options.DotnetFramework.IsDotnetLegacy());

            var dbType = GetColumnDbTypeOverride(p.Column);
            if (dbType == "SqlDbType.Udt")
                return $$"""{{commandVar}}.Parameters.Add(new SqlParameter("@{{p.Column.Name}}", SqlDbType.Udt) { UdtTypeName = "{{p.Column.Type.Name.ToLower()}}", Value = {{paramToWrite}} });""";
            return dbType is null
                ? $"""{commandVar}.Parameters.AddWithValue("@{p.Column.Name}", {paramToWrite});"""
                : $"""{commandVar}.Parameters.Add("@{p.Column.Name}", {dbType}).Value = {paramToWrite};""";
        }).JoinByNewLine();
    }

    public override ISet<string> GetUsingDirectivesForQueries()
    {
        return base
            .GetUsingDirectivesForQueries()
            .AddRangeExcludeNulls(["Microsoft.Data.SqlClient"])
            .AddRangeIf(["Microsoft.SqlServer.Types"], SqlServerTypes.Any(TypeExistsInQueries));
    }

    public override ISet<string> GetUsingDirectivesForModels()
    {
        return base
            .GetUsingDirectivesForModels()
            .AddRangeExcludeNulls(["System"])
            .AddRangeIf(["Microsoft.SqlServer.Types"], SqlServerTypesInUse());
    }

    // table models are generated for every table, so the spatial types count when any table column uses them
    private bool SqlServerTypesInUse()
    {
        return SqlServerTypes.Any(TypeExistsInQueries) || Tables.Values
            .SelectMany(t => t.Values)
            .SelectMany(t => t.Columns)
            .Any(c => SqlServerTypes.Contains(GetCsharpTypeWithoutNullableSuffix(c, null)));
    }

    public override ISet<string> GetUsingDirectivesForUtils()
    {
        return base
            .GetUsingDirectivesForUtils()
            .AddRangeExcludeNulls(["System"])
            .AddRangeIf(["Microsoft.Data.SqlClient"], Options.UseDapper);
    }

    public override MemberDeclarationSyntax[] GetMemberDeclarationsForUtils()
    {
        return [.. base
            .GetMemberDeclarationsForUtils()
            .AddRangeIf([
                ParseMemberDeclaration(TransformQueryForSliceArgsImpl)!
            ], SliceQueryExists())
            .AddRangeIf([
                ParseMemberDeclaration(BinaryParameterImpl)!
            ], Options.UseDapper && TypeExistsInQueries("byte[]"))];
    }

    public override ConnectionGenCommands EstablishConnection(Query query)
    {
        return new(
            GetConnectionOrDataSource: new(
                $"var {Variable.Connection.AsVarName()} = new SqlConnection({Variable.ConnectionString.AsPropertyName()})",
                true
            ),
            ConnectionOpen: $"await {Variable.Connection.AsVarName()}.OpenAsync({Cancellation.Argument()})"
        );
    }

    public override CommandGenCommands CreateSqlCommand(string sqlTextConstant)
    {
        var commandVar = Variable.Command.AsVarName();
        return new CommandGenCommands(
            CommandCreation: new(
                $"var {commandVar} = new SqlCommand({sqlTextConstant}, {Variable.Connection.AsVarName()})",
                true),
            SetCommandText: string.Empty,
            PrepareCommand: string.Empty
        );
    }

    // sqlc already writes named parameters as @name for this engine
    public override string TransformQueryText(Query query)
    {
        return query.Text;
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

    // :copyfrom is implemented with SqlBulkCopy over an in-memory DataTable
    public string GetCopyFromImpl(Query query, string queryTextConstant)
    {
        var connectionCommands = EstablishConnection(query);
        var connectionVar = Variable.Connection.AsVarName();
        var rowVar = Variable.Row.AsVarName();
        var tableVar = "dataTable";
        var bulkCopyVar = "bulkCopy";

        var qualifiedTableName = !string.IsNullOrEmpty(query.InsertIntoTable.Schema)
            ? $"[{query.InsertIntoTable.Schema}].[{query.InsertIntoTable.Name}]"
            : $"[{query.InsertIntoTable.Name}]";

        var addColumns = query.Params.Select(p =>
        {
            var csharpType = GetCsharpTypeWithoutNullableSuffix(p.Column, query);
            return $"""{tableVar}.Columns.Add("{p.Column.Name}", typeof({csharpType}));""";
        }).JoinByNewLine();

        var mapColumns = query.Params.Select(p =>
            $"""{bulkCopyVar}.ColumnMappings.Add("{p.Column.Name}", "{p.Column.Name}");""").JoinByNewLine();

        var rowValues = query.Params.Select(p =>
        {
            var param = $"{rowVar}.{p.Column.Name.ToPascalCase()}";
            var writerFn = GetWriterFn(p.Column, query);
            return writerFn is null ? param : writerFn(
                param,
                p.Column.Type.Name,
                IsColumnNotNull(p.Column, query),
                false,
                Options.DotnetFramework.IsDotnetLegacy());
        }).JoinByComma();

        return connectionCommands.GetConnectionOrDataSource.WrapBlock(
            $$"""
            {{connectionCommands.ConnectionOpen.AppendSemicolonUnlessEmpty()}}
            var {{tableVar}} = new DataTable();
            {{addColumns}}
            foreach (var {{rowVar}} in args)
                {{tableVar}}.Rows.Add({{rowValues}});
            using (var {{bulkCopyVar}} = new SqlBulkCopy({{connectionVar}}))
            {
                {{bulkCopyVar}}.DestinationTableName = "{{qualifiedTableName}}";
                {{mapColumns}}
                await {{bulkCopyVar}}.WriteToServerAsync({{tableVar}}{{Cancellation.TrailingArgument()}});
            }
            """
        );
    }
}