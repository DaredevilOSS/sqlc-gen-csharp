using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public class NpgsqlDriver(DotnetFramework dotnetFramework, bool useDapper) : DbDriver(dotnetFramework, useDapper), ICopyFrom, IExecRows
{
    protected override List<(string, Func<int, string>, HashSet<string>)> GetColumnMapping()
    {
        return
        [
            ("long", ordinal => $"reader.GetInt64({ordinal})", ["serial", "bigserial"]),
            ("byte[]", ordinal => $"Utils.GetBytes(reader, {ordinal})", [
                "binary",
                "bit",
                "bytea",
                "blob",
                "longblob",
                "mediumblob",
                "tinyblob",
                "varbinary"
            ]),
            ("string", ordinal => $"reader.GetString({ordinal})",
            [
                "char",

                "longtext",
                "mediumtext",
                "text",
                "bpchar",
                "time",
                "tinytext",
                "varchar",
                "pg_catalog.varchar"
            ]),
            ("DateTime", ordinal => $"reader.GetDateTime({ordinal})", [
                "date",
                "datetime",
                "timestamp"
            ]),
            ("object", ordinal => $"reader.GetString({ordinal})", ["json"]),
            ("int", ordinal => $"reader.GetInt32({ordinal})", [
                "integer",
                "int2",
                "int4",
                "int8",
                "pg_catalog.int2",
                "pg_catalog.int4",
                "pg_catalog.int8"
            ]),
            ("float", ordinal => $"reader.GetFloat({ordinal})", ["numeric", "float4", "float8"]),
            ("decimal", ordinal => $"reader.GetDecimal({ordinal})", ["decimal"]),
            ("bool", ordinal => $"reader.GetBoolean({ordinal})", [
                "bool",
                "boolean",
                "pg_catalog.bool"
            ])
        ];
    }

    public override UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return base.GetUsingDirectives()
            .Append(UsingDirective(ParseName("Npgsql")))
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
        if (UseDapper)
        {
            return new ConnectionGenCommands(
            $"var {Variable.Connection.Name()} = new NpgsqlConnection({Variable.ConnectionString.Name()})",
            ""
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
        var (establishConnection, connectionOpen) = EstablishConnection(query);
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
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}
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
                             {{connectionOpen.AppendSemicolonUnlessEmpty()}}
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

    public MemberDeclarationSyntax ExecRowsDeclare(string queryTextConstant, string argInterface, Query query)
    {
        var parametersStr = CommonGen.GetParameterListAsString(argInterface, query.Params);
        var (establishConnection, connectionOpen) = EstablishConnection(query);
        var createSqlCommand = CreateSqlCommand(queryTextConstant);
        var commandParameters = CommonGen.GetCommandParameters(query.Params);
        var executeScalarAndReturnCreated = ExecuteScalarAndReturnCreated();



        var methodBody = DotnetFramework.LatestDotnetSupported()
            ? GetWithUsingAsStatement()
            : GetWithUsingAsBlock();

        if (UseDapper)
            methodBody = GetAsDapper();

        return ParseMemberDeclaration($$"""
                                        public async Task<long> {{query.Name}}({{parametersStr}})
                                        {
                                            {{methodBody}}
                                        }
                                        """)!;

        string GetAsDapper()
        {
            var argsParams = query.Params.Count > 0 ? $", args" : "";
            return $$"""
                        using ({{establishConnection}})
                        {
                            return await connection.ExecuteAsync({{queryTextConstant}}{{argsParams}});
                        }
                     """;
        }

        string GetWithUsingAsStatement()
        {
            return $$"""
                     {
                         await using {{establishConnection}};
                         {{connectionOpen.AppendSemicolonUnlessEmpty()}}
                         await using {{createSqlCommand}};
                         {{commandParameters.JoinByNewLine()}}
                         {{executeScalarAndReturnCreated.JoinByNewLine()}}
                     }
                     """;
        }

        string GetWithUsingAsBlock()
        {
            return $$"""
                     {
                         using ({{establishConnection}})
                         {
                             {{connectionOpen.AppendSemicolonUnlessEmpty()}}
                             using ({{createSqlCommand}})
                             {
                                {{commandParameters.JoinByNewLine()}}
                                {{executeScalarAndReturnCreated.JoinByNewLine()}}
                             }
                         }
                     }
                     """;
        }

        IEnumerable<string> ExecuteScalarAndReturnCreated()
        {
            return new[]
            {
                $"return await {Variable.Command.Name()}.ExecuteNonQueryAsync();",
            };
        }
    }
}