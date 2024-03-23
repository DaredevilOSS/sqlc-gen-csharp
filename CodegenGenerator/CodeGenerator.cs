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

public class CodeGenerator
{
    private const string GeneratedNamespace = "GeneratedNamespace";
    private Options Options { get; }
    
    private IDbDriver DbDriver { get; }

    public GenerateResponse GenerateResponse { get; }

    public CodeGenerator(GenerateRequest generateRequest)
    {
        Options = ParseOptions(generateRequest);
        var dbDriver = CreateNodeGenerator(Options.driver);
        DbDriver = dbDriver;
        GenerateResponse = Generate(generateRequest);
    }

    private Options ParseOptions(GenerateRequest generateRequest)
    {
        var text = Encoding.UTF8.GetString(generateRequest.PluginOptions.ToByteArray());
        return JsonSerializer.Deserialize<Options>(text) ?? throw new InvalidOperationException();
    }
    
    private string QueryFilenameToClassName(string filenameWithExtension)
    {
        return string.Concat(
            Path.GetFileNameWithoutExtension(filenameWithExtension).FirstCharToUpper(), 
            Path.GetExtension(filenameWithExtension)[1..].FirstCharToUpper());
    }

    private MemberDeclarationSyntax GetClassDeclaration(string className, MemberDeclarationSyntax[] methodDeclarations)
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
    
    public GenerateResponse Generate(GenerateRequest generateRequest)
    {
        var fileQueries = GetQueryMap(generateRequest.Queries);

        var outputFiles = new RepeatedField<File>();
        foreach (var (filename, queries) in fileQueries)
        {
            var (usingDb, queryMethodsDeclarations) = DbDriver.Preamble(queries);
            var queriesConstantsDeclarations = GetQueryStringsConstants(queries);
            var className = QueryFilenameToClassName(filename);
            var classDeclaration = GetClassDeclaration(className, 
                queriesConstantsDeclarations.Concat(queryMethodsDeclarations).ToArray());

            // var ListArgsInterfaceDeclarations = new[] {  };
                
            var memberDeclarations = new[] { classDeclaration };
            memberDeclarations = memberDeclarations.Concat(ListRowInterfaceDeclarations(queries)).ToArray();
            
            var namespaceDeclaration = NamespaceDeclaration(IdentifierName(GeneratedNamespace))
                .AddMembers(memberDeclarations);
            
            var compilationUnit = CompilationUnit()
                .AddUsings(usingDb)
                .AddMembers(namespaceDeclaration)
                .NormalizeWhitespace();
            
            outputFiles.Add(new File
            {
                Name = $"{className}.cs",
                Contents = ToByteString(compilationUnit)
            });
        }
        return new GenerateResponse { Files = {outputFiles} };
    }
    
    private ImmutableDictionary<string, Query[]> GetQueryMap(RepeatedField<Query> queries)
    {
        return queries
            .GroupBy(query => query.Filename)
            .ToImmutableDictionary(
                group => group.Key, 
                group => group.ToArray());
    }

    private MemberDeclarationSyntax AddMethodDeclaration(Query query, string argInterface, string returnInterface)
    {
        return query.Cmd switch
        {
            ":exec" => DbDriver.ExecDeclare(query.Name, query.Text, argInterface, query.Params),
            ":one" => DbDriver.OneDeclare(query.Name, query.Text, argInterface, returnInterface, query.Params, query.Columns),
            ":many" => DbDriver.ManyDeclare(query.Name, query.Text, argInterface, returnInterface, query.Params, query.Columns),
            _ => throw new InvalidDataException()
        };
    }
    
    private MemberDeclarationSyntax[] ListRowInterfaceDeclarations(Query[] queries)
    {
        // TODO: feature flag on whether to generate as C# records or not
        if (true)
        {
            return queries
                .Select(query => new { RecordDeclaration = GenerateRecordDeclarations(query.Name, 
                    "row", query.Columns) })
                .Where(x => x.RecordDeclaration.ParameterList?.Parameters.Count > 0)
                .Select(x => x.RecordDeclaration)
                .Cast<MemberDeclarationSyntax>()
                .ToArray();
        }
    }
    
    private MemberDeclarationSyntax[] ListArgsInterfaceDeclarations(Query[] queries)
    {
        // TODO: feature flag on whether to generate as C# records or not
        if (true)
        {
            return queries
                .Select(query => new { 
                    // query.Params.Select()
                    RecordDeclaration = GenerateRecordDeclarations(query.Name, "args", null) 
                })
                .Where(x => x.RecordDeclaration.ParameterList?.Parameters.Count > 0)
                .Select(x => x.RecordDeclaration)
                .Cast<MemberDeclarationSyntax>()
                .ToArray();
        }
    }
    
    private MemberDeclarationSyntax[] GetQueryStringsConstants(Query[] queries)
    {
        return queries
            .Select(query => new
            {
                FieldDeclaration = FieldDeclaration(
                        VariableDeclaration(
                                PredefinedType(
                                    Token(SyntaxKind.StringKeyword))
                            )
                            .AddVariables(
                                VariableDeclarator(
                                        Identifier($"{query.Name}Sql"))
                                    .WithInitializer(
                                        EqualsValueClause(
                                            LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                Literal(query.Text))))))
                    .AddModifiers(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ConstKeyword))
            })
            .Select(x => x.FieldDeclaration)
            .Cast<MemberDeclarationSyntax>()
            .ToArray();
    }

    private (CompilationUnitSyntax, string) AddArgsDeclaration(Query query)
    {
        if (query.Params.Count <= 0) return (null, string.Empty)!; // TODO String.Empty?
        var argInterface = $"{query.Name}Args";
        var argsDeclaration = ArgsDeclare(argInterface,
            column => DbDriver.ColumnType(column.Type.Name, column.NotNull), query.Params);
        
        return (argsDeclaration,argInterface);
    }

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

    private IDbDriver CreateNodeGenerator(string driver)
    {
        return driver switch
        {
            "MySqlConnector" => new MySqlConnector(),
            _ => throw new ArgumentException($"unknown driver: {driver}", nameof(driver))
        };
    }
    
    private RecordDeclarationSyntax GenerateRecordDeclarations(string name, string recordSuffix, IEnumerable<Column> columns)
    {
        return RecordDeclaration(Token(SyntaxKind.RecordKeyword), $"{name}{recordSuffix.FirstCharToUpper()}")
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .WithParameterList(ColumnsToRecordParameters(columns))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }

    private ParameterListSyntax ColumnsToRecordParameters(IEnumerable<Column> columns)
    {
        return ParameterList(SeparatedList(columns
            .Select(column => Parameter(Identifier(column.Name))
                .WithType(DbDriver.ColumnType(column.Type.Name, column.NotNull))
            )));
    }

    private CompilationUnitSyntax QueryDecl(string name, string sql)
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

    private CompilationUnitSyntax ArgsDeclare(string name, Func<Column, TypeSyntax> ctype,
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