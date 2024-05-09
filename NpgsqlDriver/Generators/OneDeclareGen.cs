using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.NpgsqlDriver.Generators;

internal static class OneDeclareGen
{
    public static MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IEnumerable<Column> columns, IDbDriver dbDriver)
    {
        return MethodDeclaration(IdentifierName($"Task<{returnInterface}?>"), funcName)
            .WithPublicAsync()
            .WithParameterList(
                ParseParameterList(CommonExpressions.GetParameterListAsString(argInterface, parameters)))
            .WithBody(GetMethodBody());

        BlockSyntax GetMethodBody()
        {
            return Block(new[]
            {
                Utils.EstablishConnection(),
                Utils.PrepareSqlCommand(queryTextConstant, parameters),
                ExecuteAndReturnOne(returnInterface, columns, dbDriver)
            }.SelectMany(x => x));
        }
    }

    private static IEnumerable<StatementSyntax> ExecuteAndReturnOne(string returnInterface, IEnumerable<Column> columns,
        IDbDriver dbDriver)
    {
        return new[]
        {
            CommonExpressions.UsingDataReader(),
            IfStatement(
                CommonExpressions.AwaitReaderRow(),
                ReturnSingleRow(returnInterface, columns, dbDriver)
            ),
            ReturnStatement(LiteralExpression(SyntaxKind.NullLiteralExpression))
        };
    }

    private static StatementSyntax ReturnSingleRow(string returnInterface, IEnumerable<Column> columns, IDbDriver dbDriver)
    {
        return ReturnStatement(
            ObjectCreationExpression(IdentifierName(returnInterface))
                .WithInitializer(CommonExpressions.GetRecordInitExpression(columns, dbDriver)));
    }
}