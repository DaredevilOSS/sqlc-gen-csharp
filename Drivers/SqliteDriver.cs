using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public sealed partial class SqliteDriver(
    Options options,
    Catalog catalog,
    IList<Query> queries) :
    DbDriver(options, catalog, queries), IOne, IMany, IExec, IExecRows, IExecLastId, ICopyFrom
{
    private const string DefaultSqliteVersion = "9.0.0";

    private static readonly HashSet<string> IntegerDbTypes = ["integer", "integernotnulldefaultunixepoch"];

    private const string DateTimeStringFormat = "yyyy-MM-dd HH:mm:ss"; // Default format for DateTime strings - TODO make configurable via Options

    private static readonly SqlMapperImplFunc NodaInstantTypeHandler = _ => $$"""
        private class NodaInstantTypeHandler : SqlMapper.TypeHandler<Instant>
        {
            public override Instant Parse(object value)
            {
                if (value is string s)
                    return InstantPattern.CreateWithInvariantCulture("{{DateTimeStringFormat}}").Parse(s).Value;
                if (value is long l)
                    return Instant.FromUnixTimeSeconds(l);
                throw new DataException($"Cannot convert {value?.GetType()} to Instant");
            }

            public override void SetValue(IDbDataParameter parameter, Instant value)
            {
                parameter.Value = value;
            }
        }
        """;

    protected override Dictionary<string, ColumnMapping> ColumnMappings { get; } =
        new()
        {
            ["byte[]"] = new(
                new()
                {
                    {"blob", new()}
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<byte[]>({ordinal})"
            ),
            ["string"] = new(
                new()
                {
                    {"text", new()}
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetString({ordinal})"
            ),
            ["int"] = new(
                new()
                {
                    { "integer", new() },
                    { "integernotnulldefaultunixepoch", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetInt32({ordinal})",
                convertFunc: x => $"Convert.ToInt32({x})"
            ),
            ["decimal"] = new(
                new()
                {
                    {"real", new()}
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetDecimal({ordinal})"
            ),
            ["DateTime"] = new(
                [],
                readerFn: (ordinal, dbType) =>
                {
                    if (IntegerDbTypes.Contains(dbType.ToLower()))
                        return $"DateTimeOffset.FromUnixTimeSeconds({Variable.Reader.AsVarName()}.GetInt32({ordinal})).DateTime";
                    return $"DateTime.Parse({Variable.Reader.AsVarName()}.GetString({ordinal}))";
                },
                writerFn: (el, dbType, notNull, isDapper, isLegacy) =>
                {
                    var nullValue = isDapper ? "null" : "(object)DBNull.Value";
                    var elWithOptionalNull = notNull ? el : $"{el}.Value";
                    if (IntegerDbTypes.Contains(dbType.ToLower()))
                        return $"{el} != null ? (int?) new DateTimeOffset({elWithOptionalNull}.ToUniversalTime()).ToUnixTimeSeconds() : {nullValue}";
                    return $"{el} != null ? {elWithOptionalNull}.ToString(\"{DateTimeStringFormat}\") : {nullValue}";
                },
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(DateTime), new DateTimeTypeHandler());",
                sqlMapperImpl: DateTimeTypeHandler
            ),
            ["Instant"] = new(
                [],
                readerFn: (ordinal, dbType) =>
                {
                    if (IntegerDbTypes.Contains(dbType.ToLower()))
                        return $"Instant.FromUnixTimeSeconds({Variable.Reader.AsVarName()}.GetInt32({ordinal}))";
                    return $"InstantPattern.CreateWithInvariantCulture(\"{DateTimeStringFormat}\").Parse({Variable.Reader.AsVarName()}.GetString({ordinal})).Value";
                },
                writerFn: (el, dbType, notNull, isDapper, isLegacy) =>
                {
                    var nullValue = isDapper ? "null" : "(object)DBNull.Value";
                    if (IntegerDbTypes.Contains(dbType.ToLower()))
                        return $"{el} != null ? (long?) {el}.Value.ToUnixTimeSeconds() : {nullValue}";
                    return $"{el} != null ? InstantPattern.CreateWithInvariantCulture(\"{DateTimeStringFormat}\").Format({el}.Value) : {nullValue}";
                },
                usingDirectives: ["NodaTime", "NodaTime.Extensions", "NodaTime.Text"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(Instant), new NodaInstantTypeHandler());",
                sqlMapperImpl: NodaInstantTypeHandler
            ),
            ["bool"] = new(
                [],
                readerFn: (ordinal, dbType) =>
                {
                    var getFunc = IntegerDbTypes.Contains(dbType.ToLower()) ? "GetInt32" : "GetString";
                    return $"Convert.ToBoolean({Variable.Reader.AsVarName()}.{getFunc}({ordinal}))";
                },
                writerFn: (el, dbType, notNull, isDapper, isLegacy) =>
                {
                    var nullValue = isDapper ? "null" : "(object)DBNull.Value";
                    var optionalCast = notNull ? string.Empty : "(int?)";
                    var convertFunc = IntegerDbTypes.Contains(dbType.ToLower()) ? $"{optionalCast} Convert.ToInt32" : "Convert.ToString";
                    return $"{el} != null ? {convertFunc}({el}) : {nullValue}";
                }
            ),
            ["object"] = new(
                new()
                {
                    { "any", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetValue({ordinal})"
            )
        };

    private static readonly SqlMapperImplFunc DateTimeTypeHandler = _ => $$"""
        private class DateTimeTypeHandler : SqlMapper.TypeHandler<DateTime>
        {
            public override DateTime Parse(object value)
            {
                if (value is string s)
                    return DateTime.Parse(s);
                if (value is long l)
                    return DateTimeOffset.FromUnixTimeSeconds(l).DateTime;
                throw new DataException($"Cannot convert {value?.GetType()} to DateTime");
            }

            public override void SetValue(IDbDataParameter parameter, DateTime value)
            {
                parameter.Value = value;
            }
        }
        """;

    public override string TransactionClassName => "SqliteTransaction";

    public override IDictionary<string, string> GetPackageReferences()
    {
        return base
            .GetPackageReferences()
            .Merge(new Dictionary<string, string>
            {
                { "Microsoft.Data.Sqlite", Options.OverrideDriverVersion != string.Empty ? Options.OverrideDriverVersion : DefaultSqliteVersion }
            });
    }

    public override ISet<string> GetUsingDirectivesForQueries()
    {
        var usingDirectives = base.GetUsingDirectivesForQueries();
        return usingDirectives.AddRangeExcludeNulls(
            [
                "Microsoft.Data.Sqlite"
            ]
        );
    }

    public override ISet<string> GetUsingDirectivesForUtils()
    {
        var usingDirectives = base.GetUsingDirectivesForUtils();
        return usingDirectives.AddRangeExcludeNulls(
            [
                "System",
                "System.Text.RegularExpressions"
            ]
        );
    }

    private const string ValuesRegex = """
        private static readonly Regex ValuesRegex = new Regex(@"VALUES\s*\((?<params>[^)]*)\)", RegexOptions.IgnoreCase);
    """;

    private const string TransformQueryForBatch = """
        public static string TransformQueryForSqliteBatch(string originalSql, int cntRecords)
        {
            var match = ValuesRegex.Match(originalSql);
            if (!match.Success)
                throw new ArgumentException("The query does not contain a valid VALUES clause.");
            
            var valuesParams = match.Groups["params"].Value
                .Split(',')
                .Select(p => p.Trim())
                .ToList();
            var batchRows = Enumerable.Range(0, cntRecords)
                .Select(i => "(" + string.Join(", ", valuesParams.Select(p => $"{p}{i}")) + ")");
                
            var batchValuesClause = "VALUES " + string.Join(",\n", batchRows);
            return ValuesRegex.Replace(originalSql, batchValuesClause);
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

    public override MemberDeclarationSyntax[] GetMemberDeclarationsForUtils()
    {
        return base
            .GetMemberDeclarationsForUtils()
            .AddRangeIf([
                ParseMemberDeclaration(TransformQueryForSliceArgsImpl)!
            ], SliceQueryExists())
            .AddRangeIf([
                ParseMemberDeclaration(ValuesRegex)!,
                ParseMemberDeclaration(TransformQueryForBatch)!
            ], CopyFromQueryExists())
            .ToArray();
    }

    public override ConnectionGenCommands EstablishConnection(Query query)
    {
        return new(
            $"var {Variable.Connection.AsVarName()} = new SqliteConnection({Variable.ConnectionString.AsPropertyName()})",
            $"await {Variable.Connection.AsVarName()}.OpenAsync()"
        );
    }

    public override string CreateSqlCommand(string sqlTextConstant)
    {
        return $"var {Variable.Command.AsVarName()} = new SqliteCommand({sqlTextConstant}, {Variable.Connection.AsVarName()})";
    }

    public override string TransformQueryText(Query query)
    {
        var queryText = query.Text;
        var areArgumentsNumbered = NumberedArgumentsRegex().IsMatch(queryText);

        foreach (var p in query.Params)
        {
            var column = GetColumnFromParam(p, query);
            queryText = areArgumentsNumbered
                // For numbered parameters, we replace all occurrences of each parameter number.
                ? new Regex($@"\?{p.Number}\b").Replace(queryText, $"@{column.Name}")
                // For positional '?' parameters, we must replace them one by one in order.
                : QueryParamRegex().Replace(queryText, $"@{column.Name}", 1);
        }
        return queryText;
    }

    // Regex to detect numbered parameters like ?1, ?2
    [GeneratedRegex(@"\?\d+\b")]
    private static partial Regex NumberedArgumentsRegex();

    [GeneratedRegex(@"\?")]
    private static partial Regex QueryParamRegex();

    public string GetCopyFromImpl(Query query, string queryTextConstant)
    {
        var sqlTextVar = Variable.TransformedSql.AsVarName();
        var (establishConnection, connectionOpen) = EstablishConnection(query);
        var sqlTransformation = $"var {sqlTextVar} = Utils.TransformQueryForSqliteBatch({queryTextConstant}, {Variable.Args.AsVarName()}.Count);";
        var commandParameters = AddParametersToCommand();
        var createSqlCommand = CreateSqlCommand(sqlTextVar);
        var executeScalar = $"await {Variable.Command.AsVarName()}.ExecuteScalarAsync();";

        return $$"""
                 using ({{establishConnection}})
                 {
                     {{connectionOpen.AppendSemicolonUnlessEmpty()}}
                     {{sqlTransformation}}
                     using ({{createSqlCommand}})
                     {
                         {{commandParameters}}
                         {{executeScalar}}
                     }
                 }
                 """;

        string AddParametersToCommand()
        {
            var commandVar = Variable.Command.AsVarName();
            var argsVar = Variable.Args.AsVarName();
            var addRecordParamsToCommand = query.Params.Select(p =>
            {
                var param = p.Column.Name.ToPascalCase();
                var nullParamCast = p.Column.NotNull ? string.Empty : " ?? (object)DBNull.Value";
                var addParamToCommand = $$"""
                    {{commandVar}}.Parameters.AddWithValue($"@{{p.Column.Name}}{i}", {{argsVar}}[i].{{param}}{{nullParamCast}});
                """;
                return addParamToCommand;
            }).JoinByNewLine();

            return $$"""
                     for (int i = 0; i < {{argsVar}}.Count; i++)
                     {
                         {{addRecordParamsToCommand}}
                     }
                     """;
        }
    }
}