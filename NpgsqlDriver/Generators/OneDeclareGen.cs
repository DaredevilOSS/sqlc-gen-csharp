using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SqlcGenCsharp.Drivers.Generators;

internal static class OneDeclareGen
{
    public static MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        return MethodDeclaration(IdentifierName($"Task<{returnInterface}?>"), funcName)
            .WithPublicAsync()
            .WithParameterList(ParseParameterList(NpgsqlDriver.Utils.GetParameterListAsString(argInterface, parameters)))
            .WithBody(GetMethodBody());

        BlockSyntax GetMethodBody()
        {
            return Block(new[]
            {
                NpgsqlDriver.Utils.EstablishConnection(),
                NpgsqlDriver.Utils.PrepareSqlCommand(queryTextConstant, parameters),
                ExecuteAndReturnOne(returnInterface, columns)
            }.SelectMany(x => x));
        }
    }
    
    private static IEnumerable<StatementSyntax> ExecuteAndReturnOne(string returnInterface, IEnumerable<Column> columns)
    {
        return
        [
            NpgsqlDriver.Utils.UsingDataReader(),
            IfStatement(
                NpgsqlDriver.Utils.AwaitReaderRow(),
                ReturnSingleRow(returnInterface, columns)
            ),
            ReturnStatement(LiteralExpression(SyntaxKind.NullLiteralExpression))
        ];
    }
    
    private static StatementSyntax ReturnSingleRow(String returnInterface, IEnumerable<Column> columns)
    {
        return ReturnStatement(
            ObjectCreationExpression(IdentifierName(returnInterface))
                .WithInitializer(NpgsqlDriver.Utils.GetRecordInitExpression(columns)));
    }
}