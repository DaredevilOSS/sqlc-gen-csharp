using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Common;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.MySqlConnector;

public static class OneDeclareMembers
{
    public static BlockSyntax GetBlock(string returnInterface, string queryTextConstant, IEnumerable<Parameter> parameters, IList<Column> columns)
    {
        var blockStatements = new StatementSyntax[]
            {
                Common.UsingConnectionVar(),
                Common.ConnectionOpen(),
                Common.UsingCommandVar(queryTextConstant)
            }
            .Concat(Common.AddParametersToCommand(parameters))
            .Concat(new[]
            {
                Common.UsingReaderVar(),
                GetIfRowExistsStatement(returnInterface, columns),
                ReturnStatement(Generators.NullExpression())
            })
            .ToArray();
        return Block(blockStatements);
    }

    private static IfStatementSyntax GetIfRowExistsStatement(string returnInterface, IList<Column> columns)
    {
        return IfStatement(
            Common.AwaitReaderRow(),
            Block(
                SingletonList<StatementSyntax>(
                    ReturnStatement(
                        ObjectCreationExpression(IdentifierName(returnInterface))
                            .WithInitializer(
                                InitializerExpression(
                                    SyntaxKind.ObjectInitializerExpression,
                                    SeparatedList(Types.GetColumnsAssignments(columns)
                                        .ToArray())
                                )
                            )
                    )
                )
            )
        );
    }
}