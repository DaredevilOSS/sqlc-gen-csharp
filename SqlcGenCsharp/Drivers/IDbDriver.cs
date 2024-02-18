using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;

namespace sqlc_gen_csharp.Drivers;

public interface IDbDriver
{
    System.Type ColumnType(string columnType, bool notNull);

    CompilationUnitSyntax Preamble(List<Query> queries);

    CompilationUnitSyntax ExecDeclare(string name, string text, string iface, List<Parameter> parameters);
    
    CompilationUnitSyntax ManyDeclare(string name, string text, string argIface, string returnIface, 
        List<Parameter> parameters, List<Column> columns);
    
    CompilationUnitSyntax OneDeclare(string name, string text, string argIface, string returnIface, 
        List<Parameter> parameters, List<Column> columns);
}