using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.NpgsqlDriver.Generators;

internal static class ManyDeclareGen
{
    public static MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IEnumerable<Column> columns, IDbDriver dbDriver)
    {
        var methodDeclaration =
            MethodDeclaration(IdentifierName($"Task<List<{returnInterface}>>"), Identifier(funcName))
                .WithPublicAsync()
                .WithParameterList(ParseParameterList(CommonExpressions.GetParameterListAsString(argInterface, parameters)))
                .WithBody(GetMethodBody(queryTextConstant, returnInterface, columns, parameters, dbDriver));

        return methodDeclaration;
    }

    private static BlockSyntax GetMethodBody(string queryTextConstant, string returnInterface,
        IEnumerable<Column> columns, IEnumerable<Parameter> parameters, IDbDriver dbDriver)
    {
        return Block(new[]
        {
            Utils.EstablishConnection(),
            Utils.PrepareSqlCommand(queryTextConstant, parameters),
            new[]
            {
                CommonExpressions.UsingDataReader(),
                ParseStatement($"var {Variable.Rows.Name()} = new List<{returnInterface}>();"),
                GetWhileStatement(returnInterface, columns, dbDriver),
                ReturnStatement(IdentifierName(Variable.Rows.Name()))
            }
        }.SelectMany(x => x));
    }

    private static StatementSyntax GetWhileStatement(string returnInterface, IEnumerable<Column> columns, IDbDriver dbDriver)
    {
        return WhileStatement(
            CommonExpressions.AwaitReaderRow(),
            Block(
                ExpressionStatement(
                    InvocationExpression(ParseExpression($"{Variable.Rows.Name()}.Add"))
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList(
                                    Argument(ObjectCreationExpression(IdentifierName(returnInterface))
                                        .WithInitializer(CommonExpressions.GetRecordInitExpression(columns, dbDriver))
                                    )
                                )
                            )
                        )
                )));
    }
}