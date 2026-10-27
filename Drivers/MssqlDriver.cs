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
    private const string DefaultSqlClientVersion = "6.1.1";

    public MssqlDriver(
        Options options,
        Catalog catalog,
        IList<Query> queries) : base(options, catalog, queries)
    {
        NullableTypes.Add("DateTimeOffset");
    }

    protected override Dictionary<string, ColumnMapping> ColumnMappings { get; } =
        new()
        {
            /* Integer data types */
            ["bool"] = new(
                new()
                {
                    { "bit", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetBoolean({ordinal})",
                convertFunc: x => $"Convert.ToBoolean({x})"
            ),
            ["byte"] = new(
                new()
                {
                    { "tinyint", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetByte({ordinal})",
                convertFunc: x => $"Convert.ToByte({x})"
            ),
            ["short"] = new(
                new()
                {
                    { "smallint", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetInt16({ordinal})",
                convertFunc: x => $"Convert.ToInt16({x})"
            ),
            ["int"] = new(
                new()
                {
                    { "int", new() },
                    { "integer", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetInt32({ordinal})",
                convertFunc: x => $"Convert.ToInt32({x})"
            ),
            ["long"] = new(
                new()
                {
                    { "bigint", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetInt64({ordinal})",
                convertFunc: x => $"Convert.ToInt64({x})"
            ),

            /* Floating point data types */
            ["float"] = new(
                new()
                {
                    { "real", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFloat({ordinal})"
            ),
            ["double"] = new(
                new()
                {
                    { "float", new() },
                    { "double precision", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetDouble({ordinal})"
            ),
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
                    { "char", new() },
                    { "nchar", new() },
                    { "varchar", new() },
                    { "nvarchar", new() },
                    { "text", new() },
                    { "ntext", new() },
                    { "xml", new() },
                    { "sysname", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetString({ordinal})"
            ),

            /* Binary data types */
            ["byte[]"] = new(
                new()
                {
                    { "binary", new() },
                    { "varbinary", new() },
                    { "image", new() },
                    { "rowversion", new() },
                    { "timestamp", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<byte[]>({ordinal})"
            ),

            /* Date and time data types */
            ["DateTime"] = new(
                new()
                {
                    { "date", new() },
                    { "datetime", new() },
                    { "datetime2", new() },
                    { "smalldatetime", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetDateTime({ordinal})"
            ),
            ["DateTimeOffset"] = new(
                new()
                {
                    { "datetimeoffset", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<DateTimeOffset>({ordinal})"
            ),
            ["TimeSpan"] = new(
                new()
                {
                    { "time", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetFieldValue<TimeSpan>({ordinal})"
            ),

            /* Other data types */
            ["Guid"] = new(
                new()
                {
                    { "uniqueidentifier", new() }
                },
                readerFn: (ordinal, _) => $"{Variable.Reader.AsVarName()}.GetGuid({ordinal})",
                convertFunc: x => $"Guid.Parse({x}.ToString())"
            ),
            ["object"] = new(
                new()
                {
                    { "sql_variant", new() },
                    { "any", new() }
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
            });
    }

    public override ISet<string> GetUsingDirectivesForQueries()
    {
        return base
            .GetUsingDirectivesForQueries()
            .AddRangeExcludeNulls(["Microsoft.Data.SqlClient"]);
    }

    public override ISet<string> GetUsingDirectivesForUtils()
    {
        return base
            .GetUsingDirectivesForUtils()
            .AddRangeExcludeNulls(["System"]);
    }

    public override MemberDeclarationSyntax[] GetMemberDeclarationsForUtils()
    {
        return [.. base
            .GetMemberDeclarationsForUtils()
            .AddRangeIf([
                ParseMemberDeclaration(TransformQueryForSliceArgsImpl)!
            ], SliceQueryExists())];
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
