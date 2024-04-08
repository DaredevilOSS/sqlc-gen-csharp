using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers.Generators;

namespace SqlcGenCsharp.Drivers;

public class MySqlConnectorDriver : IDbDriver
{
    public string ColumnType(string mysqlColumnType, bool notNull)
    {
        return mysqlColumnType.MySqlTypeToCsharpType(notNull);
    }

    public (UsingDirectiveSyntax[], MemberDeclarationSyntax[]) Preamble(string className)
    {
        return (
            PreambleGen.GetUsingDirectives(),
            PreambleGen.GetClassMembers()
        );
    }

    public MemberDeclarationSyntax OneDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        return OneDeclareGen.Generate(funcName, queryTextConstant, argInterface, returnInterface, parameters, columns);
    }

    public MemberDeclarationSyntax ExecDeclare(string funcName, string queryTextConstant, string argInterface,
        IList<Parameter> parameters)
    {
        return ExecDeclareGen.Generate(funcName, queryTextConstant, argInterface, parameters);
    }

    public MemberDeclarationSyntax ExecLastIdDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IList<Column> columns)
    {
        return ExecLastIdDeclareGen.Generate(funcName, queryTextConstant, argInterface, returnInterface, parameters,
            columns);
    }

    public MemberDeclarationSyntax ManyDeclare(string funcName, string queryTextConstant, string argInterface,
        string returnInterface, IList<Parameter> parameters, IEnumerable<Column> columns)
    {
        return ManyDeclareGen.Generate(funcName, queryTextConstant, argInterface, returnInterface, parameters, columns);
    }
}