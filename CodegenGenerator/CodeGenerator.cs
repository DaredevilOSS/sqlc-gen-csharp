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
    private static CompilationUnitSyntax MergeCompilationUnit(CompilationUnitSyntax a, CompilationUnitSyntax b)
    {
        a = a.WithUsings(b.Usings);
        a = a.WithMembers(b.Members);
        a = a.NormalizeWhitespace();
        return a;
    }

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
    
    private static string _queryFilenameToCsharpFilename(string filenameWithExtension)
    {
        var filename = Path.GetFileNameWithoutExtension(filenameWithExtension);
        var extension = Path.GetExtension(filenameWithExtension);
        return string.Concat(
            filename.FirstCharToUpper(), 
            extension[1..].FirstCharToUpper(),
            ".cs");
    }
    
    // TODO once uncommented it fails sqlc miserably with un-being able to import Roslyn lib
    public static GenerateResponse Generate(GenerateRequest generateRequest)
    {
        var options = ParseOptions(generateRequest);
        var dbDriver = CreateNodeGenerator(options.driver);
        var queryMap = generateRequest.Queries
            .GroupBy(query => query.Filename)
            .ToDictionary(group => group.Key, group => group.ToList());
        var files = new RepeatedField<File>();
        
        // loop over dictionary of query files
        foreach (var fileQueries in queryMap)
        {
            var (usingDirective, exportedMethods) = dbDriver.Preamble(fileQueries.Value);
        
            // loop over queries
            foreach (var query in fileQueries.Value)
            {
                var updatedColumns = ConstructUpdatedColumns(query);
                
                var lowerName = char.ToLower(query.Name[0]) + query.Name.Substring(1);
                var textName = $"{lowerName}Query";
                var queryDeclaration = QueryDecl(
                textName,
                $"-- name: {query.Name} {query.Cmd}\n{query.Text}"
                );
                
                // (var argDecleration, var argInterface) = AddArgsDeclaration(query, dbDriver);
                // (var rowDeclare, var returnInterface) = AddRowDeclaration(query, dbDriver);
                // var methodToAdd = AddMethodDeclaration(query, dbDriver, argInterface, returnInterface);

                // add members to class
                //var newClass = topDeclarations.Item3.AddMembers(methodToAdd);
                // var newNamespace = topDeclarations.Item2.ReplaceNode(topDeclarations.Item3,newClass);


                // Compilation unit (root of the syntax tree) with using directives and namespace
                var compilationUnit = CompilationUnit()
                    .AddUsings(usingDirective)
                    .AddMembers(exportedMethods)
                    .NormalizeWhitespace(); // Format the code for readability

        
                files.Add(new File
                {
                    Name = _queryFilenameToCsharpFilename(fileQueries.Key),
                    Contents = compilationUnit.ToByteString()
                });
                
            }
        }
        return new GenerateResponse { Files = {files} };
    }

    private static MethodDeclarationSyntax AddMethodDeclaration(Query query,
        IDbDriver dbDriver,
        string argInterface, string returnInterface)
    {
        MethodDeclarationSyntax methodToAdd = null;
        switch (query.Cmd)
        {
            case ":exec":
                methodToAdd = dbDriver.ExecDeclare(query.Name, query.Text, argInterface, query.Params);
                break;
            case ":one":
                methodToAdd = dbDriver.OneDeclare(query.Name, query.Text, argInterface, returnInterface,
                    query.Params, query.Columns);
                break;
            case ":many":
                methodToAdd = dbDriver.ManyDeclare(query.Name, query.Text, argInterface, returnInterface,
                    query.Params, query.Columns);
                break;
        }

        return methodToAdd;
    }

    // private static (InterfaceDeclarationSyntax, string) AddRowDeclaration(Query query, IDbDriver dbDriver)
    // {
    //     if (query.Columns.Count <= 0) return (null, string.Empty); // TODO
    //     var returnInterface = $"{query.Name}Row";
    //     // TODO this is pure guess
    //     var unitToAdd = RowDeclare(returnInterface,
    //         column => dbDriver.ColumnType(column.Type.Name, column.NotNull), query.Columns);
    //     return (unitToAdd, returnInterface);
    //     // nodes = nodes.WithMembers(unitToAdd.Members);
    //     // return (nodes.NormalizeWhitespace(), returnInterface);
    // }

    private static (CompilationUnitSyntax, string) AddArgsDeclaration(Query query, IDbDriver dbDriver)
    {
        if (query.Params.Count <= 0) return (null, string.Empty); // TODO String.Empty?
        var argInterface = $"{query.Name}Args";
        var argsDeclaration = ArgsDeclare(argInterface,
            column => dbDriver.ColumnType(column.Type.Name, column.NotNull), query.Params);
        
        return (argsDeclaration,argInterface);
        // return (MergeCompilationUnit(nodes, argsDeclaration), argInterface);
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
    
    private static RecordDeclarationSyntax RowDeclare(string name, Func<Column, TypeSyntax> columnToType,
        IEnumerable<Column> columns)
    {
        var columnsList = columns.ToList();
        var parameterList = GenerateParameters(columnToType, columnsList);
        
        return RecordDeclaration(Token(SyntaxKind.RecordDeclaration), $"{name}Row")
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .WithParameterList(parameterList);

        static ParameterListSyntax GenerateParameters(Func<Column, TypeSyntax> columnToType, IEnumerable<Column> columns)
        {
            return ParameterList(SeparatedList(columns
                .Select(column => Parameter(Identifier(column.Name))
                    .WithType(columnToType(column))
                )));
        }
    }

    private static CompilationUnitSyntax QueryDecl(string name, string sql)
    {
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
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
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