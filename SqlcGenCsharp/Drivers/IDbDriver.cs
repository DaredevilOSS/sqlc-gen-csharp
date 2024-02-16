using Google.Protobuf.WellKnownTypes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Type = System.Type;

namespace sqlc_gen_csharp.drivers;

public interface IDbDriver
{
    Type ColumnType(string columnType, bool notNull);

    CompilationUnitSyntax Preamble(Any queries);
}