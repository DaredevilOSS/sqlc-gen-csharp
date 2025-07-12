using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public partial class SqliteDriver(
    Options options,
    string defaultSchema,
    Dictionary<string, Dictionary<string, Table>> tables,
    Dictionary<string, Dictionary<string, Enum>> enums,
    IList<Query> queries) :
    DbDriver(options, defaultSchema, tables, enums, queries), IOne, IMany, IExec, IExecRows, IExecLastId, ICopyFrom
{
    public override Dictionary<string, ColumnMapping> ColumnMappings { get; } =
        new()
        {
            ["byte[]"] = new(
                new()
                {
                    {"blob", new()}
                },
                ordinal => $"reader.GetFieldValue<byte[]>({ordinal})"
            ),
            ["string"] = new(
                new()
                {
                    {"text", new()}
                },
                ordinal => $"reader.GetString({ordinal})"
            ),
            ["int"] = new(
                new()
                {
                    { "integer", new() },
                    { "integernotnulldefaultunixepoch", new() }
                },
                ordinal => $"reader.GetInt32({ordinal})"
            ),
            ["decimal"] = new(
                new()
                {
                    {"real", new()}
                },
                ordinal => $"reader.GetDecimal({ordinal})"
            ),
            ["object"] = new(
                new()
                {
                    { "any", new() }
                },
                ordinal => $"reader.GetValue({ordinal})"
            ),
            ["long"] = new(
                new()
                {
                    { "bigint", new() }
                },
                ordinal => $"reader.GetInt64({ordinal})"
            ),
        };

    public override string TransactionClassName => "SqliteTransaction";

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

    public override MemberDeclarationSyntax[] GetMemberDeclarationsForUtils()
    {
        var memberDeclarations = base
            .GetMemberDeclarationsForUtils()
            .AddRangeIf([ParseMemberDeclaration(TransformQueryForSliceArgsImpl)!], SliceQueryExists());

        if (!CopyFromQueryExists())
            return memberDeclarations.ToArray();

        return memberDeclarations
            .Append(ParseMemberDeclaration("""
                   private static readonly Regex ValuesRegex = new Regex(@"VALUES\s*\((?<params>[^)]*)\)", RegexOptions.IgnoreCase);
                   """)!)
            .Append(ParseMemberDeclaration("""
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
                   """)!)
            .ToArray();
    }

    public override ConnectionGenCommands EstablishConnection(Query query)
    {
        return new ConnectionGenCommands(
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
        var counter = 0;
        var transformedQueryText = QueryParamRegex().Replace(query.Text, _ => "@" + query.Params[counter++].Column.Name);
        return transformedQueryText;
    }

    [GeneratedRegex(@"\?\d*")]
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