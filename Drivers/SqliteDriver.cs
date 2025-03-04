using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers;

public partial class SqliteDriver(Options options, Dictionary<string, Table> tables) :
    DbDriver(options, tables), IOne, IMany, IExec, IExecRows, IExecLastId, ICopyFrom
{
    protected override List<ColumnMapping> ColumnMappings { get; } = [
        new("byte[]", new Dictionary<string, DbTypeInfo>
            {
                {"blob", new DbTypeInfo()}
            }, ordinal => $"reader.GetFieldValue<byte[]>({ordinal})"),
        new("string",
            new Dictionary<string, DbTypeInfo>
            {
                {"text", new DbTypeInfo()}
            }, ordinal => $"reader.GetString({ordinal})"),
        new("int",
            new Dictionary<string, DbTypeInfo>
            {
                { "integer", new DbTypeInfo() },
                { "integernotnulldefaultunixepoch", new DbTypeInfo() } // return type of UNIXEPOCH function
            }, ordinal => $"reader.GetInt32({ordinal})"),
        new("decimal",
            new Dictionary<string, DbTypeInfo>
            {
                {"real", new DbTypeInfo()}
            }, ordinal => $"reader.GetDecimal({ordinal})"),
    ];

    public override UsingDirectiveSyntax[] GetUsingDirectives()
    {
        return base.GetUsingDirectives()
            .Append(UsingDirective(ParseName("Microsoft.Data.Sqlite")))
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