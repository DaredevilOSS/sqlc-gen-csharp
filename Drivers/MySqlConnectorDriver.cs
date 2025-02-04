using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using OneDeclareGen = SqlcGenCsharp.Drivers.Generators.OneDeclareGen;

namespace SqlcGenCsharp.Drivers;

public partial class MySqlConnectorDriver(Options options, Dictionary<string, Table> tables) :
    DbDriver(options, tables), IOne, IMany, IExec, IExecRows, IExecLastId, ICopyFrom
{
    protected override List<ColumnMapping> ColumnMappings { get; } =
    [
        new("long",
            new Dictionary<string, string?> { { "bigint", null } }, ordinal => $"reader.GetInt64({ordinal})"),
        new("byte[]",
            new Dictionary<string, string?>
            {
                { "binary", null },
                { "bit", null },
                { "blob", null },
                { "longblob", null },
                { "mediumblob", null },
                { "tinyblob", null },
                { "varbinary", null }
            }, ordinal => $"Utils.GetBytes(reader, {ordinal})"),
        new("string",
            new Dictionary<string, string?>
            {
                { "char", null },
                { "decimal", null },
                { "longtext", null },
                { "mediumtext", null },
                { "text", null },
                { "time", null },
                { "tinytext", null },
                { "varchar", null }
            }, ordinal => $"reader.GetString({ordinal})"),
        new("DateTime",
            new Dictionary<string, string?> { { "date", null }, { "datetime", null }, { "timestamp", null } }, ordinal => $"reader.GetDateTime({ordinal})"),
        new("int",
            new Dictionary<string, string?>
            {
                { "int", null },
                { "mediumint", null },
                { "smallint", null },
                { "tinyint", null },
                { "year", null }
            }, ordinal => $"reader.GetInt32({ordinal})"),
        new("double",
            new Dictionary<string, string?> { { "double", null }, { "float", null } }, ordinal => $"reader.GetDouble({ordinal})"),
        new("object",
            new Dictionary<string, string?> { { "json", null } }, ordinal => $"reader.GetString({ordinal})")
    ];

    public override UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return base.GetUsingDirectives()
            .Append(UsingDirective(ParseName("MySqlConnector")))
            .Append(UsingDirective(ParseName("System.Globalization")))
            .Append(UsingDirective(ParseName("System.IO")))
            .Append(UsingDirective(ParseName("CsvHelper")))
            .Append(UsingDirective(ParseName("CsvHelper.Configuration")))
            .ToArray();
    }

    public override ConnectionGenCommands EstablishConnection(Query query)
    {
        return new ConnectionGenCommands(
            $"var {Variable.Connection.AsVarName()} = new MySqlConnection({GetConnectionStringField()})",
            $"{Variable.Connection.AsVarName()}.Open()"
        );
    }

    public override string CreateSqlCommand(string sqlTextConstant)
    {
        return $"var {Variable.Command.AsVarName()} = new MySqlCommand({sqlTextConstant}, {Variable.Connection.AsVarName()})";
    }

    public override string TransformQueryText(Query query)
    {
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

    public override string[] GetLastIdStatement()
    {
        return
        [
            $"await {Variable.Command.AsVarName()}.ExecuteNonQueryAsync();",
            $"return {Variable.Command.AsVarName()}.LastInsertedId;"
        ];
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

    public MemberDeclarationSyntax CopyFromDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new CopyFromDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }

    public string GetCopyFromImpl(Query query, string queryTextConstant)
    {
        const string tempCsvFilename = "input.csv";
        const string csvDelimiter = ",";

        var (establishConnection, connectionOpen) = EstablishConnection(query);
        return $$"""  
                 var {{Variable.Config.AsVarName()}} = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = "{{csvDelimiter}}" };
                 using (var {{Variable.Writer.AsVarName()}} = new StreamWriter("{{tempCsvFilename}}"))
                 using (var {{Variable.CsvWriter.AsVarName()}} = new CsvWriter({{Variable.Writer.AsVarName()}}, {{Variable.Config.AsVarName()}}))
                    await {{Variable.CsvWriter.AsVarName()}}.WriteRecordsAsync({{Variable.Args.AsVarName()}});
                 
                 using ({{establishConnection}})
                 {
                     {{connectionOpen.AppendSemicolonUnlessEmpty()}}
                     var {{Variable.Loader.AsVarName()}} = new MySqlBulkLoader({{Variable.Connection.AsVarName()}})
                     {
                         Local = true, 
                         TableName = "{{query.InsertIntoTable.Name}}", 
                         FieldTerminator = "{{csvDelimiter}}",
                         FileName = "{{tempCsvFilename}}",
                         NumberOfLinesToSkip = 1
                     };
                     await {{Variable.Loader.AsVarName()}}.LoadAsync();
                     await {{Variable.Connection.AsVarName()}}.CloseAsync();
                 }
                 """;
    }
}