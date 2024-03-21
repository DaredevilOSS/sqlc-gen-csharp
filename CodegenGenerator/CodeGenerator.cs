using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using File = Plugin.File;

namespace SqlcGenCsharp;

public static class CodeGenerator
{
    private const string GeneratedNamespace = "GeneratedNamespace";
    
    private static ByteString ToByteString(this CompilationUnitSyntax compilationUnit)
    {
        var syntaxTree = CSharpSyntaxTree.Create(compilationUnit);
        var sourceText = syntaxTree.GetText().ToString();
        return ByteString.CopyFromUtf8(sourceText);
    }

    private static Options ParseOptions(GenerateRequest generateRequest)
    {
        var text = Encoding.UTF8.GetString(generateRequest.PluginOptions.ToByteArray());
        return JsonSerializer.Deserialize<Options>(text) ?? throw new InvalidOperationException();
    }
    
    private static string FirstCharToUpper(this string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };
    
    private static string _queryFilenameToClassName(string filenameWithExtension)
    {
        return string.Concat(
            Path.GetFileNameWithoutExtension(filenameWithExtension).FirstCharToUpper(), 
            Path.GetExtension(filenameWithExtension)[1..].FirstCharToUpper());
    }

    private static MemberDeclarationSyntax _getClassDeclaration(string className, MemberDeclarationSyntax[] methodDeclarations)
    {
        return ClassDeclaration(className)
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
            .AddMembers(methodDeclarations);
    }
    
    public static GenerateResponse Generate(GenerateRequest generateRequest)
    {
        var options = ParseOptions(generateRequest);
        var dbDriver = CreateNodeGenerator(options.driver);
        var fileQueries = GetQueryMap(generateRequest.Queries);

        var outputFiles = new RepeatedField<File>();
        foreach (var (filename, queries) in fileQueries)
        {
            var (usingDb, methodDeclarations) = dbDriver.Preamble(queries);
            var recordDeclarations = _getRecordDeclarations(queries, dbDriver);

            var className = _queryFilenameToClassName(filename);
            var classDeclaration = _getClassDeclaration(className, methodDeclarations);

            var memberDeclarations = new[] { classDeclaration };
            memberDeclarations = memberDeclarations.Concat(recordDeclarations).ToArray();
            
            var namespaceDeclaration = NamespaceDeclaration(IdentifierName(GeneratedNamespace))
                .AddMembers(memberDeclarations);
            
            var compilationUnit = CompilationUnit()
                .AddUsings(usingDb)
                .AddMembers(namespaceDeclaration)
                .NormalizeWhitespace();
            
            outputFiles.Add(new File
            {
                Name = $"{className}.cs",
                Contents = compilationUnit.ToByteString()
            });
        }
        return new GenerateResponse { Files = {outputFiles} };
        
        static ImmutableDictionary<string, Query[]> GetQueryMap(RepeatedField<Query> queries)
        {
            return queries
                .GroupBy(query => query.Filename)
                .ToImmutableDictionary(
                    group => group.Key, 
                    group => group.ToArray());
        }
        
        static MemberDeclarationSyntax AddMethodDeclaration(Query query, IDbDriver dbDriver, 
            string argInterface, string returnInterface)
        {
            return query.Cmd switch
            {
                ":exec" => dbDriver.ExecDeclare(query.Name, query.Text, argInterface, query.Params),
                ":one" => dbDriver.OneDeclare(query.Name, query.Text, argInterface, returnInterface, query.Params, query.Columns),
                ":many" => dbDriver.ManyDeclare(query.Name, query.Text, argInterface, returnInterface, query.Params, query.Columns),
                _ => throw new InvalidDataException()
            };
        }
    }

    private static MemberDeclarationSyntax[] _getRecordDeclarations(Query[] queries, IDbDriver dbDriver)
    {
        return queries
            .Select(query => new
            {
                QueryName = query.Name,
                SqlText = $"-- name: {query.Name} {query.Cmd}\n{query.Text}",
                RecordDeclaration = GenerateRecordDeclarations(dbDriver, query.Name, query.Columns)
            })
            .Where(x => x.RecordDeclaration.ParameterList?.Parameters.Count > 0)
            .Select(x => x.RecordDeclaration)
            .Cast<MemberDeclarationSyntax>()
            .ToArray();
    }

    private static (CompilationUnitSyntax, string) AddArgsDeclaration(Query query, IDbDriver dbDriver)
    {
        if (query.Params.Count <= 0) return (null, string.Empty)!; // TODO String.Empty?
        var argInterface = $"{query.Name}Args";
        var argsDeclaration = ArgsDeclare(argInterface,
            column => dbDriver.ColumnType(column.Type.Name, column.NotNull), query.Params);
        
        return (argsDeclaration,argInterface);
    }

    private static IEnumerable<Column> ConstructUpdatedColumns(Query query)
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
        switch (driver)
        {
            case "MySqlConnector":
                return new MySqlConnector();
            default:
                throw new ArgumentException($"unknown driver: {driver}", nameof(driver));
        }
    }
    
    private static RecordDeclarationSyntax GenerateRecordDeclarations(IDbDriver dbDriver, string name, 
        IEnumerable<Column> columns)
    {
        return RecordDeclaration(Token(SyntaxKind.RecordKeyword), $"{name}Row")
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .WithParameterList(
                ParameterList(SeparatedList(columns
                    .Select(column => Parameter(Identifier(column.Name))
                        .WithType(dbDriver.ColumnType(column.Type.Name, column.NotNull))
                    ))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }

    private static CompilationUnitSyntax QueryDecl(string name, string sql)
    {
        var lowerName = char.ToLower(name[0]) + name[1..];
        var textName = $"{lowerName}Query";
        
        // Create the constant field declaration
        var fieldDeclaration = FieldDeclaration(
                VariableDeclaration(
                        PredefinedType(
                            Token(SyntaxKind.StringKeyword)
                        )
                    )
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(
                                    Identifier(name)
                                )
                                .WithInitializer(
                                    EqualsValueClause(
                                        LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(sql)
                                        )
                                    )
                                )
                        )
                    )
            )
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.ConstKeyword));

        // Create a class to contain the constant
        var classDeclaration = ClassDeclaration("Queries")
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddMembers(fieldDeclaration);

        // Create a namespace
        var namespaceDeclaration = NamespaceDeclaration(IdentifierName("YourNamespace"))
            .AddMembers(classDeclaration);

        // Create the compilation unit (root of the syntax tree) and add the namespace
        var compilationUnit = CompilationUnit()
            .AddUsings(UsingDirective(IdentifierName("System")))
            .AddMembers(namespaceDeclaration)
            .NormalizeWhitespace(); // Format the code for readability

        return compilationUnit;
    }

    private static CompilationUnitSyntax ArgsDeclare(string name, Func<Column, TypeSyntax> ctype,
        IEnumerable<Parameter> parameters)
    {
        // Create a list of property signatures based on the parameters
        var properties = parameters.Select((param, i) =>
            PropertyDeclaration(ctype(param.Column), Identifier(Utils.ArgName(i, param.Column)))
                .AddModifiers(Token(SyntaxKind
                    .PublicKeyword)) // Assuming properties in interfaces cannot have accessors
                .WithAccessorList(AccessorList(List(new[]
                {
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                })))
        ).ToArray();
    
        // Create the interface declaration
        var interfaceDeclaration = InterfaceDeclaration(name)
            .AddModifiers(Token(SyntaxKind.PublicKeyword)) // Making the interface public
            .AddMembers(properties); // Adding the properties
    
        // Optionally, wrap the interface in a namespace
        var namespaceDeclaration = NamespaceDeclaration(IdentifierName("YourNamespace"))
            .AddMembers(interfaceDeclaration);
    
        // Create the compilation unit (root of the syntax tree) and add the namespace
        var compilationUnit = CompilationUnit()
            .AddUsings(UsingDirective(IdentifierName("System")))
            .AddMembers(namespaceDeclaration)
            .NormalizeWhitespace(); // Format the code for readability
    
        return compilationUnit;
    }
}