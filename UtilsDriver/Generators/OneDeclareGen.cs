using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace SqlcGenCsharp.Drivers.Generators;

public class OneDeclareGen(DbDriver dbDriver)
{
    private CommonGen CommonGen { get; } = new(dbDriver);

    public MemberDeclarationSyntax Generate(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IEnumerable<Column> columns)
    {
        var returnType = $"Task<{dbDriver.AddNullableSuffix(returnInterface, false)}>";
        return MethodDeclaration(IdentifierName(returnType), funcName)
            .WithPublicAsync()
            .WithParameterList(
                ParseParameterList(CommonGen.GetParameterListAsString(argInterface, parameters)))
            .WithBody(GetMethodBody(queryTextConstant, returnInterface, columns, parameters));
    }

    private BlockSyntax GetMethodBody(string queryTextConstant, string returnInterface,
        IEnumerable<Column> columns, IEnumerable<Parameter> parameters)
    {
        return Block(new[]
            {
                dbDriver.EstablishConnection(),
                dbDriver.PrepareSqlCommand(queryTextConstant, parameters),
                ReadIfExistsAndReturn()
            }
            .SelectMany(x => x));

        IEnumerable<StatementSyntax> ReadIfExistsAndReturn()
        {
            return new[]
            {
                CommonGen.UsingDataReader(),
                IfStatement(
                    CommonGen.AwaitReaderRow(),
                    ReturnStatement(CommonGen.InstantiateDataclass(columns, returnInterface))
                ),
                ParseStatement("return null;")
            };
        }
    }
}