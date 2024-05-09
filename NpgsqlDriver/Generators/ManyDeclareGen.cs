using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.NpgsqlDriver.Generators;

public class ManyDeclareGen(IDbDriver dbDriver)
{
    public MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IEnumerable<Column> columns)
    {
        var methodDeclaration =
            MethodDeclaration(IdentifierName($"Task<List<{returnInterface}>>"), Identifier(funcName))
                .WithPublicAsync()
                .WithParameterList(ParseParameterList(CommonExpressions.GetParameterListAsString(argInterface, parameters)))
                .WithBody(GetMethodBody(queryTextConstant, returnInterface, columns, parameters));

        return methodDeclaration;
    }

    private BlockSyntax GetMethodBody(string queryTextConstant, string returnInterface,
        IEnumerable<Column> columns, IEnumerable<Parameter> parameters)
    {
        return Block(new[]
        {
            Utils.EstablishConnection(),
            Utils.PrepareSqlCommand(queryTextConstant, parameters),
            new[]
            {
                CommonExpressions.UsingDataReader(),
                ParseStatement($"var {Variable.Rows.Name()} = new List<{returnInterface}>();"),
                GetWhileStatement(returnInterface, columns),
                ReturnStatement(IdentifierName(Variable.Rows.Name()))
            }
        }.SelectMany(x => x));
    }

    private StatementSyntax GetWhileStatement(string returnInterface, IEnumerable<Column> columns)
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