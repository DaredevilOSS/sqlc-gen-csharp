using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace SqlcGenCsharp.Drivers.Generators;

public class ManyDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IEnumerable<Column> columns)
    {
        var methodDeclaration =
            MethodDeclaration(IdentifierName($"Task<List<{returnInterface}>>"), Identifier(funcName))
                .WithPublicAsync()
                .WithParameterList(ParseParameterList(CommonGen.GetParameterListAsString(argInterface, parameters)))
                .WithBody(GetMethodBody(queryTextConstant, returnInterface, columns, parameters));

        return methodDeclaration;
    }

    private BlockSyntax GetMethodBody(string queryTextConstant, string returnInterface,
        IEnumerable<Column> columns, IEnumerable<Parameter> parameters)
    {
        return Block(new[]
            {
                dbDriver.EstablishConnection(),
                dbDriver.PrepareSqlCommand(queryTextConstant, parameters),
                ReadWhileExistsAndReturn()
            }
            .SelectMany(x => x));

        IEnumerable<StatementSyntax> ReadWhileExistsAndReturn()
        {
            return new[]
            {
                CommonGen.UsingDataReader(),
                ParseStatement($"var {Variable.Result.Name()} = new List<{returnInterface}>();"),
                WhileStatement(
                    CommonGen.AwaitReaderRow(),
                    Block(
                        ExpressionStatement(
                            InvocationExpression(ParseExpression($"{Variable.Result.Name()}.Add"))
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList(
                                            Argument(CommonGen.InstantiateDataclass(columns, returnInterface))
                                        )
                                    )
                                )
                        )
                    )
                ),
                ParseStatement($"return {Variable.Result.Name()};")
            };
        }
    }
}