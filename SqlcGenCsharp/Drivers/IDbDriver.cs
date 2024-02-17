using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Type = System.Type;

namespace sqlc_gen_csharp.Drivers;

public interface IDbDriver
{
    Type ColumnType(string columnType, bool notNull);

    CompilationUnitSyntax Preamble(Query[] queries);

    IEnumerable<ParameterSyntax> FuncParamsDecl(string iface, IEnumerable<Parameter> parameters);
}