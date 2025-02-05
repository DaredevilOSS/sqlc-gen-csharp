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
    public NpgsqlDriver(Options options, Dictionary<string, Table> tables) : base(options, tables)
    {
        foreach (var columnMapping in ColumnMappings)
        {
            foreach (var dbType in columnMapping.DbTypes.ToList())
            {
                var dbTypeToAdd = $"pg_catalog.{dbType.Key}";
                columnMapping.DbTypes.Add(dbTypeToAdd, dbType.Value);
            }
        }
    }

    protected sealed override List<ColumnMapping> ColumnMappings { get; } =
    [
        new("long",
            new Dictionary<string, string?> { { "int8", null }, { "bigint", null }, { "bigserial", null } }
            , ordinal => $"reader.GetInt64({ordinal})"),
        new("byte[]",
            new Dictionary<string, string?>
            {
                { "binary", null },
                { "bit", null },
                { "bytea", null },
                { "blob", null },
                { "longblob", null },
                { "mediumblob", null },
                { "tinyblob", null },
                { "varbinary", null }
            }, ordinal => $"Utils.GetBytes(reader, {ordinal})"),
        new("string",
            new Dictionary<string, string?>
            {
                { "longtext", null },
                { "mediumtext", null },
                { "text", null },
                { "bpchar", null },
                { "time", null },
                { "tinytext", null },
                { "varchar", "NpgsqlDbType.Varchar" }
            }, ordinal => $"reader.GetString({ordinal})",
             ordinal => $"reader.GetFieldValue<string[]>({ordinal})"),
        new("DateTime",
            new Dictionary<string, string?>
            {
                { "date", "NpgsqlDbType.Date" }, { "timestamp", "NpgsqlDbType.Timestamp" },
            }, ordinal => $"reader.GetDateTime({ordinal})"),
        new("object",
            new Dictionary<string, string?> { { "json", null } }, ordinal => $"reader.GetString({ordinal})"),
        new("int",
            new Dictionary<string, string?>
            {
                { "integer", "NpgsqlDbType.Integer" },
                { "int", "NpgsqlDbType.Integer" },
                { "int2", null },
                { "int4", "NpgsqlDbType.Integer" },
                { "serial", null }
            }, ordinal => $"reader.GetInt32({ordinal})",
               ordinal => $"reader.GetFieldValue<int[]>({ordinal})"),
        new("float",
            new Dictionary<string, string?> { { "numeric", null }, { "float4", null }, { "float8", null } }
            , ordinal => $"reader.GetFloat({ordinal})"),
        new("decimal",
            new Dictionary<string, string?> { { "decimal", null } }, ordinal => $"reader.GetDecimal({ordinal})"),
        new("bool",
            new Dictionary<string, string?> { { "bool", null }, { "boolean", null } }, ordinal => $"reader.GetBoolean({ordinal})")
    ];

    public override UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return base.GetUsingDirectives()
            .Append(UsingDirective(ParseName("Npgsql")))
            .Append(UsingDirective(ParseName("NpgsqlTypes")))
            .ToArray();
    }

    // TODO different operations require different types of connections - improve code and docs to make it clearer
    public override ConnectionGenCommands EstablishConnection(Query query)
    {
        var connectionStringField = GetConnectionStringField();
        if (query.Cmd == ":copyfrom")
            return new ConnectionGenCommands(
                $"var ds = NpgsqlDataSource.Create({connectionStringField})",
                $"var {Variable.Connection.AsVarName()} = ds.CreateConnection()"
            );

        var embedTableExists = query.Columns.Any(c => c.EmbedTable is not null);
        if (Options.UseDapper && !embedTableExists)
            return new ConnectionGenCommands(
                $"var {Variable.Connection.AsVarName()} = new NpgsqlConnection({connectionStringField})",
                ""
            );
        return new ConnectionGenCommands(
            $"var {Variable.Connection.AsVarName()} = NpgsqlDataSource.Create({connectionStringField})",
            ""
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
            var column = GetColumnFromParam(currentParameter);
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
                        var partialStmt = $"await {writerVar}.WriteAsync({rowVar}.{p.Column.Name.ToPascalCase()}";
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