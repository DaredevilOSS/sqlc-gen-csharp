using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.NpgsqlDriver.Generators;

public class ExecLastIdDeclareGen(IDbDriver dbDriver)
{
    public MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        var methodDeclaration = MethodDeclaration(IdentifierName("Task<long>"), Identifier(funcName))
            .WithPublicAsync()
            .WithParameterList(
                ParseParameterList(CommonExpressions.GetParameterListAsString(argInterface, parameters)))
            .WithBody(Block(
                Array.Empty<StatementSyntax>()
                    .Concat(Utils.EstablishConnection())
                    .Concat(Utils.PrepareSqlCommand(queryTextConstant, parameters))
                    .Append(ParseStatement($"await {Variable.Command.Name()}.ExecuteNonQueryAsync();"))
                    .Append(ParseStatement($"return {Variable.Command.Name()}.LastInsertedId;"))));
        return methodDeclaration;
    }
}