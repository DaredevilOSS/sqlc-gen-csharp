using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public class NpgsqlDriver : DbDriver, ICopyFrom, IExecRows
{
    public NpgsqlDriver(DotnetFramework dotnetFramework) : base(dotnetFramework)
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
        new("long", ordinal => $"reader.GetInt64({ordinal})",
            new Dictionary<string, string?> { { "int8", null }, { "bigint", null }, { "bigserial", null } }),
        new("byte[]", ordinal => $"Utils.GetBytes(reader, {ordinal})",
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
            }),
        new("string", ordinal => $"reader.GetString({ordinal})",
            new Dictionary<string, string?>
            {
                { "longtext", null },
                { "mediumtext", null },
                { "text", null },
                { "bpchar", null },
                { "time", null },
                { "tinytext", null },
                { "varchar", "NpgsqlDbType.Varchar" }
            }),
        new("DateTime", ordinal => $"reader.GetDateTime({ordinal})",
            new Dictionary<string, string?>
            {
                { "date", "NpgsqlDbType.Date" }, { "timestamp", "NpgsqlDbType.Timestamp" },
            }),
        new("object", ordinal => $"reader.GetString({ordinal})",
            new Dictionary<string, string?> { { "json", null } }),
        new("int", ordinal => $"reader.GetInt32({ordinal})",
            new Dictionary<string, string?>
            {
                { "integer", "NpgsqlDbType.Integer" },
                { "int", "NpgsqlDbType.Integer" },
                { "int2", null },
                { "int4", "NpgsqlDbType.Integer" },
                { "serial", null }
            }),
        new("float", ordinal => $"reader.GetFloat({ordinal})",
            new Dictionary<string, string?> { { "numeric", null }, { "float4", null }, { "float8", null } }),
        new("decimal", ordinal => $"reader.GetDecimal({ordinal})",
            new Dictionary<string, string?> { { "decimal", null } }),
        new("bool", ordinal => $"reader.GetBoolean({ordinal})",
            new Dictionary<string, string?> { { "bool", null }, { "boolean", null } })
    ];

    public override UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return base.GetUsingDirectives()
            .Append(UsingDirective(ParseName("Npgsql")))
            .Append(UsingDirective(ParseName("NpgsqlTypes")))
            .ToArray();
    }

    public override ConnectionGenCommands EstablishConnection(Query query)
    {
        if (query.Cmd == ":copyfrom")
        {
            return new ConnectionGenCommands(
                $"var ds = NpgsqlDataSource.Create({Variable.ConnectionString.Name()})",
                $"var {Variable.Connection.Name()} = ds.CreateConnection()"
            );
        }

        return new ConnectionGenCommands(
            $"var {Variable.Connection.Name()} = NpgsqlDataSource.Create({Variable.ConnectionString.Name()})",
            ""
        );
    }

    public override string CreateSqlCommand(string sqlTextConstant)
    {
        return $"var {Variable.Command.Name()} = {Variable.Connection.Name()}.CreateCommand({sqlTextConstant})";
    }

    public override string TransformQueryText(Query query)
    {
        if (query.Cmd == ":copyfrom")
            return GetCopyCommand();

        var queryText = query.Text;
        for (var i = 0; i < query.Params.Count; i++)
        {
            var currentParameter = query.Params[i];
            queryText = Regex.Replace(queryText, $@"\$\s*{i + 1}",
                $"@{currentParameter.Column.Name.FirstCharToLower()}");
        }

        return queryText;

        string GetCopyCommand()
        {
            var copyParams = query.Params.Select(p => p.Column.Name).JoinByComma();
            return $"COPY {query.InsertIntoTable.Name} ({copyParams}) FROM STDIN (FORMAT BINARY)";
        }
    }

    public override MemberDeclarationSyntax OneDeclare(string queryTextConstant, string argInterface,
        string returnInterface, Query query)
    {
        return new OneDeclareGen(this).Generate(queryTextConstant, argInterface, returnInterface, query);
    }

    public override MemberDeclarationSyntax ExecDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new ExecDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }

    public override MemberDeclarationSyntax ManyDeclare(string queryTextConstant, string argInterface,
        string returnInterface, Query query)
    {
        return new ManyDeclareGen(this).Generate(queryTextConstant, argInterface, returnInterface, query);
    }

    public MemberDeclarationSyntax CopyFromDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new CopyFromDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }

    public MemberDeclarationSyntax ExecRowsDeclare(string queryTextConstant, string argInterface, Query query)
    {
        return new ExecRowsDeclareGen(this).Generate(queryTextConstant, argInterface, query);
    }
}