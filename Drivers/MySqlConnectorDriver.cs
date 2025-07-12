using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using OneDeclareGen = SqlcGenCsharp.Drivers.Generators.OneDeclareGen;

namespace SqlcGenCsharp.Drivers;

public partial class MySqlConnectorDriver(
    Options options,
    string defaultSchema,
    Dictionary<string, Dictionary<string, Table>> tables,
    Dictionary<string, Dictionary<string, Plugin.Enum>> enums,
    IList<Query> queries) :
    DbDriver(options, defaultSchema, tables, enums, queries), IOne, IMany, IExec, IExecRows, IExecLastId, ICopyFrom
{
    public const string NullToStringCsvConverter = "NullToStringCsvConverter";
    public const string BoolToBitCsvConverter = "BoolToBitCsvConverter";
    public const string ByteCsvConverter = "ByteCsvConverter";
    public const string ByteArrayCsvConverter = "ByteArrayCsvConverter";

    public override Dictionary<string, ColumnMapping> ColumnMappings { get; } =
        new()
        {
            ["bool"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "tinyint", new DbTypeInfo(Length: 1) }
                },
                ordinal => $"reader.GetBoolean({ordinal})"
            ),
            ["short"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "tinyint", new DbTypeInfo() },
                    { "smallint", new DbTypeInfo() },
                    { "year", new DbTypeInfo() }
                },
                ordinal => $"reader.GetInt16({ordinal})"
            ),
            ["long"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "bigint", new DbTypeInfo() }
                },
                ordinal => $"reader.GetInt64({ordinal})"
            ),
            ["byte"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "bit", new DbTypeInfo() }
                },
                ordinal => $"reader.GetFieldValue<byte>({ordinal})"
            ),
            ["byte[]"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "binary", new DbTypeInfo() },
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
                    { "char", new DbTypeInfo() },
                    { "longtext", new DbTypeInfo() },
                    { "mediumtext", new DbTypeInfo() },
                    { "text", new DbTypeInfo() },
                    { "time", new DbTypeInfo() },
                    { "tinytext", new DbTypeInfo() },
                    { "varchar", new DbTypeInfo() },
                    { "var_string", new DbTypeInfo() },
                },
                ordinal => $"reader.GetString({ordinal})"
            ),
            ["DateTime"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "date", new DbTypeInfo() },
                    { "datetime", new DbTypeInfo() },
                    { "timestamp", new DbTypeInfo() }
                },
                ordinal => $"reader.GetDateTime({ordinal})"
            ),
            ["int"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "int", new DbTypeInfo() },
                    { "mediumint", new DbTypeInfo() }
                },
                ordinal => $"reader.GetInt32({ordinal})"
            ),
            ["double"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "double", new DbTypeInfo() },
                    { "float", new DbTypeInfo() }
                },
                ordinal => $"reader.GetDouble({ordinal})"
            ),
            ["decimal"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "decimal", new DbTypeInfo() }
                },
                ordinal => $"reader.GetDecimal({ordinal})"
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
            ["object"] = new ColumnMapping(
                new Dictionary<string, DbTypeInfo>
                {
                    { "any", new DbTypeInfo() }
                },
                ordinal => $"reader.GetValue({ordinal})"
            )
        };

    protected sealed override Dictionary<string, Tuple<string, string?>> KnownMappings { get; } = new()
    {
        {
            "JsonElement",
            new (
                $"SqlMapper.AddTypeHandler(typeof(JsonElement), new JsonElementTypeHandler());",
                JsonElementTypeHandler
            )
        }
    };
    public override string TransactionClassName => "MySqlTransaction";

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
            );
    }

    public override MemberDeclarationSyntax[] GetMemberDeclarationsForUtils()
    {
        var memberDeclarations = base
            .GetMemberDeclarationsForUtils()
            .AddRangeIf([ParseMemberDeclaration(TransformQueryForSliceArgsImpl)!], SliceQueryExists());

        if (!CopyFromQueryExists())
            return [.. memberDeclarations];

        var csvConverters = new List<MemberDeclarationSyntax>
        {
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
                 """)!
        };

        return [.. memberDeclarations, .. csvConverters];
    }

    public override ConnectionGenCommands EstablishConnection(Query query)
    {
        return new ConnectionGenCommands(
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

        var queryText = Options.UseDapper ? $"{query.Text}; SELECT LAST_INSERT_ID()" : query.Text;
        var counter = 0;
        return QueryParamRegex().Replace(queryText, _ => "@" + query.Params[counter++].Column.Name);
    }

    [GeneratedRegex(@"\?")]
    private static partial Regex QueryParamRegex();

    public MemberDeclarationSyntax OneDeclare(string queryTextConstant, string argInterface,
        string returnInterface, Query query)
    {
        return new OneDeclareGen(this).Generate(queryTextConstant, argInterface, returnInterface, query);
    }

    public MemberDeclarationSyntax ExecDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new ExecDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }

    public MemberDeclarationSyntax ExecLastIdDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new ExecLastIdDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }

    public override string[] GetLastIdStatement(Query query)
    {
        return
        [
            $"await {Variable.Command.AsVarName()}.ExecuteNonQueryAsync();",
            $"return {Variable.Command.AsVarName()}.LastInsertedId;"
        ];
    }

    public MemberDeclarationSyntax ManyDeclare(string queryTextConstant, string argInterface, string returnInterface, Query query)
    {
        return new ManyDeclareGen(this).Generate(queryTextConstant, argInterface, returnInterface, query);
    }

    public MemberDeclarationSyntax ExecRowsDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new ExecRowsDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }

    public MemberDeclarationSyntax CopyFromDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new CopyFromDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }

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
                    {{GetBoolAndByteConverters().JoinByNewLine()}}
                    {{GetCsvNullConverters().JoinByNewLine()}}
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

        string GetCsvNullConverter(string csharpType)
        {
            var nullableCsharpType = AddNullableSuffixIfNeeded(csharpType, false);
            return $"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{nullableCsharpType}>({nullConverterFn});";
        }

        string[] GetCsvNullConverters()
        {
            var primitivesConverters = new HashSet<string>()
                .AddRangeIf(
                    [GetCsvNullConverter("short")],
                    TypeExistsInQueries("short")
                )
                .AddRangeIf(
                    [GetCsvNullConverter("int")],
                    TypeExistsInQueries("int")
                )
                .AddRangeIf(
                    [GetCsvNullConverter("long")],
                    TypeExistsInQueries("long")
                )
                .AddRangeIf(
                    [GetCsvNullConverter("float")],
                    TypeExistsInQueries("float")
                )
                .AddRangeIf(
                    [GetCsvNullConverter("decimal")],
                    TypeExistsInQueries("decimal")
                )
                .AddRangeIf(
                    [GetCsvNullConverter("double")],
                    TypeExistsInQueries("double")
                )
                .AddRangeIf(
                    [GetCsvNullConverter("DateTime")],
                    TypeExistsInQueries("DateTime")
                )
                .AddRangeIf(
                    [GetCsvNullConverter("string")],
                    TypeExistsInQueries("string")
                )
                .AddRangeIf(
                    [GetCsvNullConverter("JsonElement")],
                    TypeExistsInQueries("JsonElement")
                )
                .AddRangeIf(
                    [GetCsvNullConverter("object")],
                    TypeExistsInQueries("object")
                );

            var enumConverters = Enums.SelectMany(s =>
            {
                return s.Value.Select(e =>
                    GetCsvNullConverter(e.Key.ToModelName(s.Key, DefaultSchema)));
            });

            return [.. primitivesConverters, .. enumConverters];
        }

        ISet<string> GetBoolAndByteConverters()
        {
            return new HashSet<string>()
                .AddRangeIf(
                    [
                        $"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded("bool", true)}>(new Utils.{BoolToBitCsvConverter}());",
                        $"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded("bool", false)}>(new Utils.{BoolToBitCsvConverter}());"
                    ],
                    TypeExistsInQueries("bool")
                )
                .AddRangeIf(
                    [
                        $"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded("byte", true)}>(new Utils.{ByteCsvConverter}());",
                        $"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded("byte", false)}>(new Utils.{ByteCsvConverter}());",
                    ],
                    TypeExistsInQueries("byte")
                )
                .AddRangeIf(
                    [
                        $"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded("byte[]", true)}>(new Utils.{ByteArrayCsvConverter}());",
                        $"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded("byte[]", false)}>(new Utils.{ByteArrayCsvConverter}());",
                    ],
                    TypeExistsInQueries("byte[]")
                );
        }
    }
}