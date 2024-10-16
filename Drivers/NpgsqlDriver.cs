using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public class NpgsqlDriver(DotnetFramework dotnetFramework) : DbDriver(dotnetFramework), ICopyFrom
{
    protected override List<(string, Func<int, string>, HashSet<string>)> GetColumnMapping()
    {
        return
        [
            ("long", ordinal => $"reader.GetInt64({ordinal})", ["serial", "bigserial"]),
            ("byte[]", ordinal => $"Utils.GetBytes(reader, {ordinal})",
                ["binary", "bit", "bytea", "blob", "longblob", "mediumblob", "tinyblob", "varbinary"]),
            ("string", ordinal => $"reader.GetString({ordinal})",
            [
                "char",
                "date",
                "datetime",
                "longtext",
                "mediumtext",
                "text",
                "bpchar",
                "time",
                "timestamp",
                "tinytext",
                "varchar"
            ]),
            ("object", ordinal => $"reader.GetString({ordinal})", ["json"]),
            ("int", ordinal => $"reader.GetInt32({ordinal})", ["int2", "int4", "int8"]),
            ("float", ordinal => $"reader.GetFloat({ordinal})", ["numeric", "float4", "float8"]),
            ("decimal", ordinal => $"reader.GetDecimal({ordinal})", ["decimal"]),
            ("bool", ordinal => $"reader.GetBoolean({ordinal})", ["bool", "boolean"])
        ];
    }

    public override UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return base.GetUsingDirectives()
            .Append(UsingDirective(ParseName("Npgsql")))
            .ToArray();
    }

    public override GenExpression[] EstablishConnection(Query query)
    {
        if (query.Cmd == ":copyfrom")
        {
            return
            [
                new GenExpression($"var ds = NpgsqlDataSource.Create({Variable.ConnectionString.Name()})", true, true),
                new GenExpression($"var {Variable.Connection.Name()} = ds.CreateConnection()", true, true),
            ];
        }
        return [
            new GenExpression($"var {Variable.Connection.Name()} = NpgsqlDataSource.Create({Variable.ConnectionString.Name()})", true, true)
        ];
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
        var establishConnection = EstablishConnection(query);
        var beginBinaryImport = $"{Variable.Connection.Name()}.BeginBinaryImportAsync({queryTextConstant}";
        var methodBody = DotnetFramework.LatestDotnetSupported() ? GetAsModernDotnet() : GetAsLegacyDotnet();

        return ParseMemberDeclaration($$"""
                                        public async Task {{query.Name}}(List<{{argInterface}}> args)
                                        {
                                            {{methodBody}}
                                        }
                                        """)!;

        string GetAsModernDotnet()
        {
            var addRowsToCopyCommand = AddRowsToCopyCommand();
            return $$"""
                     {
                         await using {{establishConnection}};
                         await {{Variable.Connection.Name()}}.OpenAsync();
                         await using var {{Variable.Writer.Name()}} = await {{beginBinaryImport}});
                         {{addRowsToCopyCommand}}
                         await {{Variable.Writer.Name()}}.CompleteAsync();
                         await {{Variable.Connection.Name()}}.CloseAsync();
                     }
                     """;
        }

        string GetAsLegacyDotnet()
        {
            var addRowsToCopyCommand = AddRowsToCopyCommand();
            return $$"""
                     {
                         using ({{establishConnection}})
                         {
                             await {{Variable.Connection.Name()}}.OpenAsync();
                             using (var {{Variable.Writer.Name()}} = await {{beginBinaryImport}}))
                             {
                                {{addRowsToCopyCommand}}
                                await {{Variable.Writer.Name()}}.CompleteAsync();
                             }
                             await {{Variable.Connection.Name()}}.CloseAsync();
                         }
                     }
                     """;
        }

        string AddRowsToCopyCommand()
        {
            var constructRow = new List<string>()
                    .Append($"await {Variable.Writer.Name()}.StartRowAsync();")
                    .Concat(query.Params.Select(p => $"await {Variable.Writer.Name()}.WriteAsync({Variable.Row.Name()}.{p.Column.Name.FirstCharToUpper()});"))
                    .JoinByNewLine();
            return $$"""
                   foreach (var {{Variable.Row.Name()}} in args) 
                   {
                        {{constructRow}}
                   }
                   """;
        }
    }
}