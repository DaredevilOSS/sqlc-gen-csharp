using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using SqlcGenCsharp.Drivers;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace SqlcGenCsharp.Generators;

internal class ModelsGen(DbDriver dbDriver, string namespaceName)
{
    private const string ClassName = "Models";

    private RootGen RootGen { get; } = new(dbDriver.Options);

    private DataClassesGen DataClassesGen { get; } = new(dbDriver);


    private EnumsGen EnumsGen { get; } = new(dbDriver);

    public File GenerateFile(Dictionary<string, Table> tables, Dictionary<string, Enum> enums)
    {
        var dataclassModels = GenerateModelsDataClasses(tables);
        var enumModels = GenerateModelsEnums(enums);
        dataclassModels = dataclassModels.Concat(enumModels).ToArray();

        var directives = GetDirectives();
        var root = RootGen.CompilationRootGen(IdentifierName(namespaceName), directives, dataclassModels);
        root = root.AddCommentOnTop(Consts.AutoGeneratedComment);

        return new File
        {
            Name = $"{ClassName}.cs",
            Contents = root.ToByteString()
        };
    }

    private UsingDirectiveSyntax[] GetDirectives()
    {
        IEnumerable<UsingDirectiveSyntax> usingDirectives = [
            UsingDirective(ParseName("System")),
            UsingDirective(ParseName("System.Collections.Generic")),
            UsingDirective(ParseName("System.Linq"))
        ];

        if (dbDriver.Options.DriverName is DriverName.Npgsql)
            usingDirectives = usingDirectives.Concat([
                UsingDirective(ParseName("NpgsqlTypes")),
            ]);
        return usingDirectives.ToArray();
    }

    private MemberDeclarationSyntax[] GenerateModelsDataClasses(Dictionary<string, Table> tables)
    {
        return (
            from table in tables.Values
            let className = $"{table.Rel.Schema}_{table.Rel.Name}"
            select DataClassesGen.Generate(className.ToModelName(), ClassMember.Model, table.Columns, dbDriver.Options)
        ).ToArray();
    }

    private MemberDeclarationSyntax[] GenerateModelsEnums(Dictionary<string, Enum> enums)
    {
        return enums.Values.SelectMany(e => EnumsGen.Generate(e.Name, e.Vals)).ToArray();
    }
}