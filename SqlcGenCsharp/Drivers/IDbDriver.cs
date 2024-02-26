using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;

namespace sqlc_gen_csharp.Drivers;

public interface IDbDriver
{
    TypeSyntax ColumnType(string columnType, bool notNull);

    CompilationUnitSyntax Preamble(IEnumerable<Query> queries);
    
    MethodDeclarationSyntax OneDeclare(string name, string text, string argInterface, string returnInterface, 
        IEnumerable<Parameter> parameters, IEnumerable<Column> columns);
    
    MethodDeclarationSyntax ManyDeclare(string name, string text, string argInterface, string returnInterface, 
        IEnumerable<Parameter> parameters, IEnumerable<Column> columns);
    
    MethodDeclarationSyntax ExecDeclare(string name, string text, string argInterface, IEnumerable<Parameter> parameters);
}