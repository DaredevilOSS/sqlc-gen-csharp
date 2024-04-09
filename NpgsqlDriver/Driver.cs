using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using SqlcGenCsharp.Drivers.Generators;
using SqlcGenCsharp.NpgsqlDriver.Generators;

namespace SqlcGenCsharp.NpgsqlDriver;

public partial class Driver : IDbDriver
{
    public string ColumnType(string mysqlColumnType, bool notNull)
    {
        return mysqlColumnType.PostgreSqlTypeToCsharpType(notNull);
    }

    public string TransformQuery(Query query)
    {
        var queryText = query.Text;
        for (var i = 0; i < query.Params.Count; i++)
        {
            var currentParameter = query.Params[i];
            queryText = Regex.Replace(queryText, $@"\$\s*{i + 1}",
                $"@{currentParameter.Column.Name.FirstCharToLower()}");
            DebugHelper.Append($"\n{queryText}");
        }
        return queryText;
    }

    public (UsingDirectiveSyntax[], MemberDeclarationSyntax[]) Preamble()
    {
        return (
            PreambleGen.GetUsingDirectives(),
            PreambleGen.GetClassMembers()
        );
    }

    public MemberDeclarationSyntax OneDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        return OneDeclareGen.Generate(funcName, queryTextConstant, argInterface, returnInterface, parameters, columns);
    }

    public MemberDeclarationSyntax ExecDeclare(string funcName, string queryTextConstant, string argInterface,
        IList<Parameter> parameters)
    {
        return ExecDeclareGen.Generate(funcName, queryTextConstant, argInterface, parameters);
    }

    public MemberDeclarationSyntax ExecLastIdDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        return ExecLastIdDeclareGen.Generate(funcName, queryTextConstant, argInterface, returnInterface, parameters,
            columns);
    }

    public MemberDeclarationSyntax ManyDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IEnumerable<Column> columns)
    {
        return ManyDeclareGen.Generate(funcName, queryTextConstant, argInterface, returnInterface, parameters, columns);
    }
}