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
            ["bool"] = new(
                new()
                {
                    { "tinyint", new(Length: 1) }
                },
                ordinal => $"reader.GetBoolean({ordinal})"
            ),
            ["short"] = new(
                new()
                {
                    { "tinyint", new() },
                    { "smallint", new() },
                    { "year", new() }
                },
                ordinal => $"reader.GetInt16({ordinal})"
            ),
            ["long"] = new(
                new()
                {
                    { "bigint", new() }
                },
                ordinal => $"reader.GetInt64({ordinal})",
                convertFunc: x => $"Convert.ToInt64{x}"
            ),
            ["byte"] = new ColumnMapping(
                new()
                {
                    { "bit", new() }
                },
                ordinal => $"reader.GetFieldValue<byte>({ordinal})"
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
                ordinal => $"reader.GetFieldValue<byte[]>({ordinal})"
            ),
            ["string"] = new(
                new()
                {
                    { "char", new() },
                    { "longtext", new() },
                    { "mediumtext", new() },
                    { "text", new() },
                    { "time", new() },
                    { "tinytext", new() },
                    { "varchar", new() },
                    { "var_string", new() },
                },
                ordinal => $"reader.GetString({ordinal})"
            ),
            ["DateTime"] = new(
                new()
                {
                    { "date", new() },
                    { "datetime", new() },
                    { "timestamp", new() }
                },
                ordinal => $"reader.GetDateTime({ordinal})"
            ),
            ["int"] = new(
                new()
                {
                    { "int", new() },
                    { "integer", new() },
                    { "mediumint", new() }
                },
                ordinal => $"reader.GetInt32({ordinal})",
                convertFunc: x => $"Convert.ToInt32{x}"
            ),
            ["double"] = new(
                new()
                {
                    { "double", new() },
                    { "float", new() }
                },
                ordinal => $"reader.GetDouble({ordinal})"
            ),
            ["decimal"] = new(
                new()
                {
                    { "decimal", new() }
                },
                ordinal => $"reader.GetDecimal({ordinal})"
            ),
            ["JsonElement"] = new(
                new()
                {
                    { "json", new() }
                },
                readerFn: ordinal => $"JsonSerializer.Deserialize<JsonElement>(reader.GetString({ordinal}))",
                writerFn: (el, notNull, isDapper) =>
                {
                    if (notNull)
                        return $"{el}.GetRawText()";
                    var nullValue = isDapper ? "null" : "(object)DBNull.Value";
                    return $"{el}?.GetRawText() ?? {nullValue}";
                },
                usingDirective: "System.Text.Json",
                sqlMapper: "SqlMapper.AddTypeHandler(typeof(JsonElement), new JsonElementTypeHandler());",
                sqlMapperImpl: JsonElementTypeHandler
            ),
            ["object"] = new(
                new()
                {
                    { "any", new() }
                },
                ordinal => $"reader.GetValue({ordinal})"
            )
        };

    public override string TransactionClassName => "MySqlTransaction";

    private readonly Func<string, string> _setTypeHandlerFunc = x =>
        $$"""
          private class {{x}}TypeHandler : SqlMapper.TypeHandler<{{x}}[]>
          {
              public override {{x}}[] Parse(object value)
              {
                  if (value is string s)
                      return s.To{{x}}Arr();
                  throw new DataException($"Cannot convert {value?.GetType()} to {{x}}[]");
              }
          
              public override void SetValue(IDbDataParameter parameter, {{x}}[] value)
              {
                  parameter.Value = string.Join(",", value);
              }
          }
          """;

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

    private bool IsSetType(Column column)
    {
        var enumType = GetEnumType(column);
        return enumType is not null && IsEnumOfTypeSet(column, enumType);
    }

    protected override ISet<string> GetConfigureSqlMappings()
    {
        var setSqlMappings = Queries
            .SelectMany(q => q.Columns)
            .Where(IsSetType)
            .Select(c =>
            {
                var enumName = c.Type.Name.ToModelName(GetColumnSchema(c), DefaultSchema);
                return $"SqlMapper.AddTypeHandler(typeof({enumName}[]), new {enumName}TypeHandler());";
            })
            .Distinct();

        return base
            .GetConfigureSqlMappings()
            .AddRangeExcludeNulls(setSqlMappings);
    }

    private MemberDeclarationSyntax[] GetSetTypeHandlers()
    {
        return Queries
            .SelectMany(q => q.Columns)
            .Where(c =>
            {
                var enumType = GetEnumType(c);
                return enumType is not null && IsEnumOfTypeSet(c, enumType);
            })
            .Select(c => _setTypeHandlerFunc(c.Type.Name.ToModelName(GetColumnSchema(c), DefaultSchema)))
            .Distinct()
            .Select(m => ParseMemberDeclaration(m)!)
            .ToArray();
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
                if (!IsSetType(p.Column))
                    continue;
                var enumName = p.Column.Type.Name.ToModelName(GetColumnSchema(p.Column), DefaultSchema);
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
                      if (value is {{x}}[] arrVal)
                          return string.Join(",", arrVal);
                      return base.ConvertToString(value, row, memberMapData);
                  }
              }
              """;
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
                    {{GetBoolAndByteConverters(query).JoinByNewLine()}}
                    {{GetCsvNullConverters(query).JoinByNewLine()}}
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

    private readonly ISet<string> BoolAndByteTypes = new HashSet<string>
    {
        "bool",
        "byte",
        "byte[]"
    };

    private ISet<string> GetCsvNullConverters(Query query)
    {
        var nullConverterFn = Variable.NullConverterFn.AsVarName();
        var converters = new HashSet<string>();
        foreach (var p in query.Params)
        {
            var csharpType = GetCsharpTypeWithoutNullableSuffix(p.Column, query);
            if (
                !BoolAndByteTypes.Contains(csharpType) &&
                !IsSetType(p.Column) &&
                TypeExistsInQuery(csharpType, query))
            {
                var nullableCsharpType = AddNullableSuffixIfNeeded(csharpType, false);
                converters.Add($"{Variable.CsvWriter.AsVarName()}.Context.TypeConverterCache.AddConverter<{nullableCsharpType}>({nullConverterFn});");
            }
        }
        return converters;
    }

    private ISet<string> GetBoolAndByteConverters(Query query)
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
            .AddRangeExcludeNulls(GetSetConverters(query));
    }

    private ISet<string> GetSetConverters(Query query)
    {
        var converters = new HashSet<string>();
        foreach (var p in query.Params)
        {
            if (!IsSetType(p.Column))
                continue;

            var enumName = p.Column.Type.Name.ToModelName(GetColumnSchema(p.Column), DefaultSchema);
            var csvWriterVar = Variable.CsvWriter.AsVarName();
            converters.Add($"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded($"{enumName}[]", true)}>(new Utils.{enumName}CsvConverter());");
            converters.Add($"{csvWriterVar}.Context.TypeConverterCache.AddConverter<{AddNullableSuffixIfNeeded($"{enumName}[]", false)}>(new Utils.{enumName}CsvConverter());");
        }
        return converters;
    }

    private static bool IsEnumOfTypeSet(Column column, Plugin.Enum enumType)
    {
        return column.Length > enumType.Vals.Select(v => v.Length).Sum();
    }

    public override string GetEnumTypeAsCsharpType(Column column, Plugin.Enum enumType)
    {
        var enumName = column.Type.Name.ToModelName(GetColumnSchema(column), DefaultSchema);
        return IsEnumOfTypeSet(column, enumType) ? $"{enumName}[]" : enumName;
    }
}