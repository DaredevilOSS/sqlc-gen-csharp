using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

public class ExecDeclareGen(DbDriver dbDriver)
{
    public MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        IList<Parameter> parameters)
    {
        var methodDeclaration = MethodDeclaration(IdentifierName("Task"), Identifier(funcName))
            .WithPublicAsync()
            .WithParameterList(ParseParameterList(CommonGen.GetParameterListAsString(argInterface, parameters)))
            .WithBody(GetMethodBody(queryTextConstant, parameters));
        return methodDeclaration;
    }

    private BlockSyntax GetMethodBody(string queryTextConstant, IEnumerable<Parameter> parameters)
    {
        return Block(new[]
            {
                dbDriver.EstablishConnection(),
                dbDriver.PrepareSqlCommand(queryTextConstant, parameters),
                ExecuteScalar()
            }
            .SelectMany(x => x));

        IEnumerable<StatementSyntax> ExecuteScalar()
        {
            return new[]
            {
                ParseStatement($"await {Variable.Command.Name()}.ExecuteScalarAsync();")
            };
        }
    }
}