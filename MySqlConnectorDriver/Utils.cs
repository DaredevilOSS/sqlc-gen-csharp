using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.MySqlConnectorDriver;

public static class Utils
{
    public static IEnumerable<StatementSyntax> EstablishConnection()
    {
        return new[]
        {
            ParseStatement($"await using var {Variable.Connection.Name()} = new MySqlConnection({Variable.ConnectionString.Name()});"),
            ParseStatement($"{Variable.Connection.Name()}.Open();")
        };
    }
    
    public static IEnumerable<StatementSyntax> PrepareSqlCommand(string sqlTextConstant,
        IEnumerable<Parameter> parameters)
    {
        return new[]
        {
            ParseStatement(
                $"await using var {Variable.Command.Name()} = " +
                $"new MySqlCommand({sqlTextConstant}, {Variable.Connection.Name()});")
        }.Concat(
            parameters.Select(param => ParseStatement(
                $"{Variable.Command.Name()}.Parameters.AddWithValue(\"@{param.Column.Name}\", " +
                $"args.{param.Column.Name.FirstCharToUpper()});"))
        );
    }
}