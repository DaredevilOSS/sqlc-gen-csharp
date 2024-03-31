using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using Google.Protobuf;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using File = Plugin.File;

namespace SqlcGenCsharp;

public class CodeGenerator
{
    private const string GeneratedNamespace = "GeneratedNamespace";

    public CodeGenerator(GenerateRequest generateRequest)
    {
        Options = ParseOptions(generateRequest);
        DbDriver = CreateNodeGenerator(Options.driver);
        GenerateResponse = Generate(generateRequest);
    }

    private Options Options { get; }
    private IDbDriver DbDriver { get; }
    public GenerateResponse GenerateResponse { get; }

    private Options ParseOptions(GenerateRequest generateRequest)
    {
        var text = Encoding.UTF8.GetString(generateRequest.PluginOptions.ToByteArray());
        return JsonSerializer.Deserialize<Options>(text) ?? throw new InvalidOperationException();
    }

    private MemberDeclarationSyntax GetClass(string className, MemberDeclarationSyntax[] methodDeclarations)
    {
        return ClassDeclaration(className)
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
            .AddMembers(methodDeclarations);
    }

    private ByteString ToByteString(CompilationUnitSyntax compilationUnit)
    {
        var syntaxTree = CSharpSyntaxTree.Create(compilationUnit);
        var sourceText = syntaxTree.GetText().ToString();
        return ByteString.CopyFromUtf8(sourceText);
    }

    private GenerateResponse Generate(GenerateRequest generateRequest)
    {
        var fileQueries = generateRequest.Queries
            .GroupBy(query => query.Filename)
            .ToImmutableDictionary(
                group => group.Key,
                group => group.ToArray());

        var files = fileQueries.Select(fq => GenerateFile(fq.Value, fq.Key));
        return new GenerateResponse { Files = { files } };
    }

    private File GenerateFile(Query[] queries, string filename)
    {
        var (usingDb, className, classDeclaration) = GenerateClass(queries, filename);
        var namespaceDeclaration = NamespaceDeclaration(IdentifierName(GeneratedNamespace))
            .AddMembers(classDeclaration);

        var compilationUnit = CompilationUnit()
            .AddUsings(usingDb)
            .AddMembers(namespaceDeclaration)
            .NormalizeWhitespace();

        return new File
        {
            Name = $"{className}.cs",
            Contents = ToByteString(compilationUnit)
        };
    }

    private (UsingDirectiveSyntax[], string, MemberDeclarationSyntax) GenerateClass(Query[] queries, string filename)
    {
        var (usingDirectives, sharedMemberDeclarations) = DbDriver.Preamble(queries);
        var classMembers = new List<MemberDeclarationSyntax>()
            .Concat(sharedMemberDeclarations)
            .Concat(GetClassMembersForAllQueries());
        var className = QueryFilenameToClassName(filename);
        return (usingDirectives, className, GetClass(className, classMembers.ToArray()));

        IEnumerable<MemberDeclarationSyntax> GetClassMembersForAllQueries()
        {
            return queries.SelectMany(GetMembersForSingleQuery).ToArray();
        }

        string QueryFilenameToClassName(string filenameWithExtension)
        {
            return string.Concat(
                Path.GetFileNameWithoutExtension(filenameWithExtension).FirstCharToUpper(),
                Path.GetExtension(filenameWithExtension)[1..].FirstCharToUpper());
        }
    }

    private MemberDeclarationSyntax[] GetMembersForSingleQuery(Query query)
    {
        return new[]
            {
                GetQueryTextConstant(query),
                GetQueryColumnsDataclass(query),
                GetQueryParamsDataclass(query),
                AddMethodDeclaration(query)
            }
            .Where(member => member != null)
            .Cast<MemberDeclarationSyntax>()
            .ToArray();
    }

    private MemberDeclarationSyntax AddMethodDeclaration(Query query)
    {
        var queryTextConstant = GetInterfaceName(ClassMemberType.Sql);
        var argInterface = GetInterfaceName(ClassMemberType.Args);
        var returnInterface = GetInterfaceName(ClassMemberType.Row);
        return query.Cmd switch
        {
            ":exec" => DbDriver.ExecDeclare(query.Name, queryTextConstant, argInterface, query.Params),
            ":one" => DbDriver.OneDeclare(query.Name, queryTextConstant, argInterface, returnInterface, query.Params,
                query.Columns),
            ":many" => DbDriver.ManyDeclare(query.Name, queryTextConstant, argInterface, returnInterface, query.Params,
                query.Columns),
            _ => throw new InvalidDataException()
        };

        string GetInterfaceName(ClassMemberType classMemberType)
        {
            return $"{query.Name}{classMemberType.ToRealString()}";
        }
    }

    private MemberDeclarationSyntax? GetQueryColumnsDataclass(Query query)
    {
        // TODO add feature-flag for using C# records as data classes or not
        if (query.Columns.Count <= 0) return null;
        var recordParameters = QueryColumnsToRecordParams(query.Columns);
        return GenerateRecord(query.Name, ClassMemberType.Row, recordParameters);
    }

    private ParameterListSyntax QueryColumnsToRecordParams(IEnumerable<Column> columns)
    {
        return ParameterList(SeparatedList(columns
            .Select(column => Parameter(Identifier(column.Name.FirstCharToUpper()))
                .WithType(DbDriver.ColumnType(column.Type.Name, column.NotNull))
            )));
    }

    private MemberDeclarationSyntax? GetQueryParamsDataclass(Query query)
    {
        // TODO add feature-flag for using C# records as data classes or not
        if (query.Params.Count <= 0) return null;
        var recordParameters = QueryColumnsToRecordParams(QueryParamsToQueryColumns());
        return GenerateRecord(query.Name, ClassMemberType.Args, recordParameters);

        IEnumerable<Column> QueryParamsToQueryColumns()
        {
            return query.Params.Select(p => p.Column).ToArray();
        }
    }

    private MemberDeclarationSyntax GetQueryTextConstant(Query query)
    {
        return FieldDeclaration(
                VariableDeclaration(
                        PredefinedType(
                            Token(SyntaxKind.StringKeyword))
                    )
                    .AddVariables(
                        VariableDeclarator(
                                Identifier($"{query.Name}{ClassMemberType.Sql.ToRealString()}"))
                            .WithInitializer(
                                EqualsValueClause(
                                    LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        Literal(TransformQueryParamsToMatchDriverSyntax()))))))
            .AddModifiers(
                Token(SyntaxKind.PrivateKeyword), 
                Token(SyntaxKind.ConstKeyword));
        
        string TransformQueryParamsToMatchDriverSyntax()
        {
            return query.Params.Aggregate(
                query.Text, 
                (current, parameter) => current.Replace("?", $"@{parameter.Column.Name}"));
        }
    }

    // TODO find out if needed?
    private IEnumerable<Column> ConstructUpdatedColumns(Query query)
    {
        var colMap = new Dictionary<string, int>();
        return query.Columns
            .Where(column => !string.IsNullOrEmpty(column.Name)) // Filter out columns without a name
            .Select(column =>
            {
                var count = colMap.GetValueOrDefault(column.Name, 0);
                var updatedName = count > 0 ? $"{column.Name}_{count + 1}" : column.Name;
                colMap[column.Name] = count + 1; // Update the count for the current name
                return new Column { Name = updatedName };
            })
            .ToList();
    }

    private static IDbDriver CreateNodeGenerator(string driver)
    {
        return driver switch
        {
            "MySqlConnector" => new MySqlConnector(),
            _ => throw new ArgumentException($"unknown driver: {driver}", nameof(driver))
        };
    }


    private RecordDeclarationSyntax GenerateRecord(string name, ClassMemberType classMemberType,
        ParameterListSyntax parameterListSyntax)
    {
        return RecordDeclaration(
                Token(SyntaxKind.StructKeyword),
                $"{name}{classMemberType.ToRealString()}")
            .AddModifiers(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.ReadOnlyKeyword),
                Token(SyntaxKind.RecordKeyword)
            )
            .WithParameterList(parameterListSyntax)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }
}