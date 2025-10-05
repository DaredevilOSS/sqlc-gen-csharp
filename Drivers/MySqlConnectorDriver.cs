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

public sealed partial class MySqlConnectorDriver(
    Options options,
    Catalog catalog,
    IList<Query> queries) :
    EnumDbDriver(options, catalog, queries),
    IOne, IMany, IExec, IExecRows, IExecLastId, ICopyFrom
{
    private const string DefaultMysqlConnectorVersion = "2.4.0";
    private const string DefaultCsvHelperVersion = "33.0.1";

    protected override Dictionary<string, ColumnMapping> ColumnMappings { get; } =
        new()
        {
            /* Numeric data types */
            ["bool"] = new(
                new()
                {
                    { "tinyint", new(Length: 1) }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetBoolean({ordinal})"
            ),
            ["short"] = new(
                new()
                {
                    { "tinyint", new() },
                    { "smallint", new() },
                    { "year", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetInt16({ordinal})"
            ),
            ["int"] = new(
                new()
                {
                    { "int", new() },
                    { "integer", new() },
                    { "mediumint", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetInt32({ordinal})",
                convertFunc: x => $"Convert.ToInt32{x}"
            ),
            ["long"] = new(
                new()
                {
                    { "bigint", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetInt64({ordinal})",
                convertFunc: x => $"Convert.ToInt64{x}"
            ),
            ["double"] = new(
                new()
                {
                    { "double", new() },
                    { "float", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetDouble({ordinal})"
            ),
            ["decimal"] = new(
                new()
                {
                    { "decimal", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetDecimal({ordinal})"
            ),

            /* Binary data types */
            ["byte"] = new(
                new()
                {
                    { "bit", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<byte>({ordinal})"
            ),
            ["byte[]"] = new(
                new()
                {
                    { "binary", new() },
                    { "blob", new() },
                    { "longblob", new() },
                    { "mediumblob", new() },
                    { "tinyblob", new() },
                    { "varbinary", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<byte[]>({ordinal})"
            ),

            /* String data types */
            ["string"] = new(
                new()
                {
                    { "char", new() },
                    { "longtext", new() },
                    { "mediumtext", new() },
                    { "text", new() },
                    { "tinytext", new() },
                    { "varchar", new() },
                    { "var_string", new() },
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetString({ordinal})"
            ),

            /* Date and time data types */
            ["DateTime"] = new(
                new()
                {
                    { "date", new() },
                    { "datetime", new() },
                    { "timestamp", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetDateTime({ordinal})"
            ),
            ["TimeSpan"] = new(
                new()
                {
                    { "time", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<TimeSpan>({ordinal})"
            ),
            ["Instant"] = new(
                [],
                readerFn: (ordinal, _) => $$"""
                    (new Func<MySqlDataReader, int, Instant>((r, o) =>
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
                    { "json", new() }
                },
                readerFn: (ordinal, _) => $"JsonSerializer.Deserialize<JsonElement>(reader.GetString({ordinal}))",
                writerFn: (el, _, notNull, isDapper, isLegacy) =>
                {
                    if (notNull)
                        return $"{el}.GetRawText()";
                    var nullValue = isDapper ? "null" : "(object)DBNull.Value";
                    return $"{el}?.GetRawText() ?? {nullValue}";
                },
                usingDirectives: ["System.Text.Json"],
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(JsonElement), new JsonElementTypeHandler());",
                sqlMapperImpl: JsonElementTypeHandler
            ),

            /* Other data types */
            ["object"] = new(
                new()
                {
                    { "any", new() }
                },
                readerFn: (ordinal, _) => $"reader.GetValue({ordinal})"
            )
        };

    public override string TransactionClassName => "MySqlTransaction";

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
                parameter.Value = value.GetRawText();
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
                { "MySqlConnector", Options.OverrideDriverVersion != string.Empty ? Options.OverrideDriverVersion : DefaultMysqlConnectorVersion }
            })
            .MergeIf(new Dictionary<string, string>
            {
                { "CsvHelper", DefaultCsvHelperVersion }
            }, CopyFromQueryExists());
    }

    public override ISet<string> GetUsingDirectivesForQueries()
    {
        return base
            .GetUsingDirectivesForQueries()
            .AddRangeExcludeNulls(
                [
                    "MySqlConnector",
                    "System.Globalization",
                    "System.IO",
                    "System.Text"
                ]
            )
            .AddRangeIf(
                [
                    "CsvHelper",
                    "CsvHelper.Configuration",
                    "CsvHelper.TypeConversion"
                ],
                CopyFromQueryExists()
            );
    }

    public override ISet<string> GetUsingDirectivesForModels()
    {
        return base
            .GetUsingDirectivesForModels()
            .AddRangeExcludeNulls(
            [
                "System",
                "System.Collections.Generic"
            ]
        );
    }

    public override ISet<string> GetUsingDirectivesForUtils()
    {
        return base
            .GetUsingDirectivesForUtils()
            .AddRangeIf(
                [
                    "CsvHelper.TypeConversion",
                    "CsvHelper",
                    "CsvHelper.Configuration"
                ],
                CopyFromQueryExists()
            )
            .AddRangeExcludeNulls(
                [
                    "System.Collections.Generic"
                ]
            );
    }

    protected override ISet<string> GetConfigureSqlMappings()
    {
        var setSqlMappings = Queries
            .SelectMany(q => q.Columns)
            .Where(IsSetDataType)
            .Select(c =>
            {
                var enumName = EnumToModelName(c);
                return $"SqlMapper.AddTypeHandler(typeof(HashSet<{enumName}>), new {enumName}TypeHandler());";
            })
            .Distinct();

        return base
            .GetConfigureSqlMappings()
            .AddRangeExcludeNulls(setSqlMappings);
    }

    public override MemberDeclarationSyntax[] GetEnumExtensionsMembers(string name, IList<string> possibleValues)
    {
        return [.. base
            .GetEnumExtensionsMembers(name, possibleValues)
            .AddRangeExcludeNulls([
                ParseMemberDeclaration($$"""
                    public static HashSet<{{name}}> To{{name}}Set(this string me)
                    {
                        return new HashSet<{{name}}>(me.Split(',').ToList().Select(v => StringToEnum[v]));
                    }
                """)!
            ])];
    }

    private MemberDeclarationSyntax[] GetSetTypeHandlers()
    {
        var optionalNullableSuffix = Options.DotnetFramework.IsDotnetCore() ? "?" : string.Empty;
        string setTypeHandlerFunc(string x) =>
            $$"""
            private class {{x}}TypeHandler : SqlMapper.TypeHandler<HashSet<{{x}}>>
            {
                public override HashSet<{{x}}> Parse(object value)
                {
                    if (value is string s)
                        return s.To{{x}}Set();
                    throw new DataException($"Cannot convert {value?.GetType()} to HashSet<{{x}}>");
                }
            
                public override void SetValue(IDbDataParameter parameter, HashSet<{{x}}>{{optionalNullableSuffix}} value)
                {
                    if (value is null)
                        return;
                    parameter.Value = string.Join(",", value);
                }
            }
            """;

        return [.. Queries
            .SelectMany(q => q.Columns)
            .Where(c =>
            {
                var enumType = GetEnumType(c);
                return enumType is not null && IsSetDataType(c, enumType);
            })
            .Select(c => setTypeHandlerFunc(EnumToModelName(c)))
            .Distinct()
            .Select(m => ParseMemberDeclaration(m)!)];
    }

    public override MemberDeclarationSyntax[] GetMemberDeclarationsForUtils()
    {
        var memberDeclarations = base
            .GetMemberDeclarationsForUtils()
            .AddRangeIf([ParseMemberDeclaration(TransformQueryForSliceArgsImpl)!], SliceQueryExists())
            .AddRangeIf(GetSetTypeHandlers(), Options.UseDapper);

        if (!CopyFromQueryExists())
            return memberDeclarations.ToArray();

        foreach (var query in Queries)
        {
            if (query.Cmd != ":copyfrom")
                continue;
            foreach (var p in query.Params)
            {
                if (!IsSetDataType(p.Column))
                    continue;
                var enumName = EnumToModelName(p.Column);
                memberDeclarations = memberDeclarations.AddRangeExcludeNulls([ParseMemberDeclaration(SetCsvConverterFunc(enumName))!]);
            }
        }

        return memberDeclarations
            .AddRangeIf([
                ParseMemberDeclaration($$"""
                 public class {{NullToStringCsvConverter}} : DefaultTypeConverter
                 {
                     public override {{AddNullableSuffixIfNeeded("string", false)}} ConvertToString(
                         {{AddNullableSuffixIfNeeded("object", false)}} value, IWriterRow row, MemberMapData memberMapData)
                     {
                         return value == null ? @"\N" : base.ConvertToString(value, row, memberMapData);
                     }
                 }
                 """)!,
            ], CopyFromQueryExists())
            .AddRangeIf([
            ParseMemberDeclaration($$"""
                 public class BoolToBitCsvConverter : DefaultTypeConverter
                 {
                     public override {{AddNullableSuffixIfNeeded("string", false)}} ConvertToString(
                     {{AddNullableSuffixIfNeeded("object", false)}} value, IWriterRow row, MemberMapData memberMapData)
                     {
                         switch (value)
                         {
                             case null:
                                 return @"\N";
                             case bool b:
                                 return b ? "1" : "0";
                             default:
                                 return base.ConvertToString(value, row, memberMapData);
                         }
                     }
                 }
                 """)!,
            ], CopyFromQueryExists() && TypeExistsInQueries("bool"))
            .AddRangeIf([
                ParseMemberDeclaration($$"""
                 public class ByteCsvConverter : DefaultTypeConverter
                 {
                     public override {{AddNullableSuffixIfNeeded("string", false)}} ConvertToString(
                     {{AddNullableSuffixIfNeeded("object", false)}} value, IWriterRow row, MemberMapData memberMapData)
                     {
                         if (value == null)
                             return @"\N";
                         if (value is byte byteVal)
                             return System.Text.Encoding.UTF8.GetString(new byte[] { byteVal });
                         return base.ConvertToString(value, row, memberMapData);
                     }
                 }
                 """)!,
            ], CopyFromQueryExists() && TypeExistsInQueries("byte"))
            .AddRangeIf([
                ParseMemberDeclaration($$"""
                 public class ByteArrayCsvConverter : DefaultTypeConverter
                 {
                     public override {{AddNullableSuffixIfNeeded("string", false)}} ConvertToString(
                     {{AddNullableSuffixIfNeeded("object", false)}} value, IWriterRow row, MemberMapData memberMapData)
                     {
                         if (value == null)
                             return @"\N";
                         if (value is byte[] byteArray)
                             return System.Text.Encoding.UTF8.GetString(byteArray);
                         return base.ConvertToString(value, row, memberMapData);
                     }
                 }
                 """)!,
            ], CopyFromQueryExists() && TypeExistsInQueries("byte[]"))
            .ToArray();

        string SetCsvConverterFunc(string x) =>
            $$"""
              public class {{x}}CsvConverter : DefaultTypeConverter
              {
                  public override {{AddNullableSuffixIfNeeded("string", false)}} ConvertToString(
                  {{AddNullableSuffixIfNeeded("object", false)}} value, IWriterRow row, MemberMapData memberMapData)
                  {
                      if (value == null)
                          return @"\N";
                      if (value is HashSet<{{x}}> setVal)
                          return string.Join(",", setVal);
                      return base.ConvertToString(value, row, memberMapData);
                  }
              }
              """;
    }

    public override ConnectionGenCommands EstablishConnection(Query query)
    {
        return new(
            $"var {Variable.Connection.AsVarName()} = new MySqlConnection({Variable.ConnectionString.AsPropertyName()})",
            $"await {Variable.Connection.AsVarName()}.OpenAsync()"
        );
    }

    public override string CreateSqlCommand(string sqlTextConstant)
    {
        return $"var {Variable.Command.AsVarName()} = new MySqlCommand({sqlTextConstant}, {Variable.Connection.AsVarName()})";
    }

    public override string TransformQueryText(Query query)
    {
        if (query.Cmd == ":copyfrom")
            return string.Empty;

        var counter = 0;
        var queryText = query.Text;
        queryText = QueryParamRegex().Replace(queryText, _ => "@" + query.Params[counter++].Column.Name);
        queryText = Options.UseDapper && query.Cmd == ":execlastid"
            ? $"{queryText}; SELECT LAST_INSERT_ID()"
            : queryText;

        return queryText;
    }

    [GeneratedRegex(@"\?")]
    private static partial Regex QueryParamRegex();

    public override string[] GetLastIdStatement(Query query)
    {
        return
        [
            $"await {Variable.Command.AsVarName()}.ExecuteNonQueryAsync();",
            $"return {Variable.Command.AsVarName()}.LastInsertedId;"
        ];
    }

    /* :copyfrom methods */
    public const string NullToStringCsvConverter = "NullToStringCsvConverter";
    private const string BoolToBitCsvConverter = "BoolToBitCsvConverter";
    private const string ByteCsvConverter = "ByteCsvConverter";
    private const string ByteArrayCsvConverter = "ByteArrayCsvConverter";

    public string GetCopyFromImpl(Query query, string queryTextConstant)
    {
        const string tempCsvFilename = "input.csv";
        const string csvDelimiter = ",";

        var csvWriterVar = Variable.CsvWriter.AsVarName();
        var loaderVar = Variable.Loader.AsVarName();
        var optionsVar = Variable.Options.AsVarName();
        var connectionVar = Variable.Connection.AsVarName();
        var nullConverterFn = Variable.NullConverterFn.AsVarName();

        var loaderColumns = query.Params.Select(p => $"\"{p.Column.Name}\"").JoinByComma();
        var (establishConnection, connectionOpen) = EstablishConnection(query);

        return $$"""
                 const string supportedDateTimeFormat = "yyyy-MM-dd H:mm:ss";
                 var {{Variable.Config.AsVarName()}} = new CsvConfiguration(CultureInfo.CurrentCulture) 
                 { 
                    Delimiter = "{{csvDelimiter}}",
                    NewLine = "\n"
                 };
                 var {{nullConverterFn}} = new Utils.{{NullToStringCsvConverter}}();
                 using (var {{Variable.Writer.AsVarName()}} = new StreamWriter("{{tempCsvFilename}}", false, new UTF8Encoding(false)))
                 using (var {{csvWriterVar}} = new CsvWriter({{Variable.Writer.AsVarName()}}, {{Variable.Config.AsVarName()}}))
                 {
                    var {{optionsVar}} = new TypeConverterOptions { Formats = new[] { supportedDateTimeFormat } };
                    {{csvWriterVar}}.Context.TypeConverterOptionsCache.AddOptions<DateTime>({{optionsVar}});
                    {{csvWriterVar}}.Context.TypeConverterOptionsCache.AddOptions<DateTime?>({{optionsVar}});
                    {{GetCsvConverters(query).JoinByNewLine()}}
                    await {{csvWriterVar}}.WriteRecordsAsync({{Variable.Args.AsVarName()}});
                 }
                 
                 using ({{establishConnection}})
                 {
                     {{connectionOpen.AppendSemicolonUnlessEmpty()}}
                     var {{loaderVar}} = new MySqlBulkLoader({{connectionVar}})
                     {
                         Local = true, 
                         TableName = "{{query.InsertIntoTable.Name}}", 
                         FileName = "{{tempCsvFilename}}",
                         FieldTerminator = "{{csvDelimiter}}",
                         FieldQuotationCharacter = '"',
                         FieldQuotationOptional = true,
                         NumberOfLinesToSkip = 1,
                         LineTerminator = "\n"
                     };
                     {{loaderVar}}.Columns.AddRange(new List<string> { {{loaderColumns}} });
                     await {{loaderVar}}.LoadAsync();
                     await {{connectionVar}}.CloseAsync();
                 }
                 """;
    }

    private readonly ISet<string> _boolAndByteTypes = new HashSet<string>
    {
        "bool",
        "byte",
        "byte[]"
    };

    private ISet<string> GetCsvConverters(Query query)
    {
        var csvWriterVar = Variable.CsvWriter.AsVarName();
        return new HashSet<string>()
            .AddRangeIf(
                [
                    $"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded("bool", true)}>(new Utils.{BoolToBitCsvConverter}());",
                    $"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded("bool", false)}>(new Utils.{BoolToBitCsvConverter}());"
                ],
                TypeExistsInQuery("bool", query)
            )
            .AddRangeIf(
                [
                    $"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded("byte", true)}>(new Utils.{ByteCsvConverter}());",
                    $"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded("byte", false)}>(new Utils.{ByteCsvConverter}());",
                ],
                TypeExistsInQuery("byte", query)
            )
            .AddRangeIf(
                [
                    $"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded("byte[]", true)}>(new Utils.{ByteArrayCsvConverter}());",
                    $"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded("byte[]", false)}>(new Utils.{ByteArrayCsvConverter}());",
                ],
                TypeExistsInQuery("byte[]", query)
            )
            .AddRangeExcludeNulls(GetSetConverters(query))
            .AddRangeExcludeNulls(GetCsvNullConverters(query));
    }

    private ISet<string> GetCsvNullConverters(Query query)
    {
        var nullConverterFn = Variable.NullConverterFn.AsVarName();
        var converters = new HashSet<string>();
        foreach (var p in query.Params)
        {
            var csharpType = GetCsharpTypeWithoutNullableSuffix(p.Column, query);
            if (
                !_boolAndByteTypes.Contains(csharpType) &&
                !IsSetDataType(p.Column) &&
                TypeExistsInQuery(csharpType, query))
            {
                var nullableCsharpType = AddNullableSuffixIfNeeded(csharpType, false);
                converters.Add($"{Variable.CsvWriter.AsVarName()}.Context.TypeConverterCache.AddConverter<{nullableCsharpType}>({nullConverterFn});");
            }
        }
        return converters;
    }

    private ISet<string> GetSetConverters(Query query)
    {
        var converters = new HashSet<string>();
        foreach (var p in query.Params)
        {
            if (!IsSetDataType(p.Column))
                continue;

            var enumName = EnumToModelName(p.Column);
            var csvWriterVar = Variable.CsvWriter.AsVarName();
            converters.Add($"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded($"HashSet<{enumName}>", true)}>(new Utils.{enumName}CsvConverter());");
            converters.Add($"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded($"HashSet<{enumName}>", false)}>(new Utils.{enumName}CsvConverter());");
        }
        return converters;
    }

    /* Enum methods */
    private static bool IsSetDataType(Column column, Enum enumType)
    {
        return column.Length > enumType.Vals.Select(v => v.Length).Sum();
    }

    private bool IsSetDataType(Column column)
    {
        var enumType = GetEnumType(column);
        return enumType is not null && IsSetDataType(column, enumType);
    }

    public override WriterFn? GetWriterFn(Column column, Query query)
    {
        var csharpType = GetCsharpTypeWithoutNullableSuffix(column, query);
        var writerFn = ColumnMappings.GetValueOrDefault(csharpType)?.WriterFn;
        if (writerFn is not null)
            return writerFn;

        if (GetEnumType(column) is { } enumType && IsSetDataType(column, enumType))
            return (el, dbType, notNull, isDapper, isLegacy) =>
            {
                var stringJoinStmt = $"string.Join(\",\", {el})";
                var nullValue = isDapper ? "null" : "(object)DBNull.Value";
                return notNull
                    ? stringJoinStmt
                    : $"{el} != null ? {stringJoinStmt} : {nullValue}";
            };

        static string DefaultWriterFn(string el, string dbType, bool notNull, bool isDapper, bool isLegacy) => notNull ? el : $"{el} ?? (object)DBNull.Value";
        return Options.UseDapper ? null : DefaultWriterFn;
    }

    protected override string GetEnumReader(Column column, int ordinal)
    {
        var enumName = EnumToModelName(column);
        var readStmt = $"{Variable.Reader.AsVarName()}.GetString({ordinal})";
        return IsSetDataType(column)
            ? $"{readStmt}.To{enumName}Set()"
            : $"{readStmt}.To{enumName}()";
    }

    protected override Enum? GetEnumType(Column column)
    {
        if (!Enums.TryGetValue(string.Empty, value: out var enumsInSchema))
            return null;
        return enumsInSchema.GetValueOrDefault(column.Type.Name);
    }

    protected override string EnumToCsharpDataType(Column column)
    {
        var enumName = EnumToModelName(column);
        return IsSetDataType(column) ? $"HashSet<{enumName}>" : enumName;
    }

    public override string EnumToModelName(string _, Enum enumType)
    {
        return enumType.Name.ToPascalCase();
    }

    protected override string EnumToModelName(Column column)
    {
        return column.Type.Name.ToPascalCase();
    }
}