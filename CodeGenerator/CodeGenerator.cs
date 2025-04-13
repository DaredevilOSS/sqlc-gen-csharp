using Google.Protobuf;
using Plugin;
using SqlcGenCsharp.Drivers;
using SqlcGenCsharp.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enum = Plugin.Enum;

namespace SqlcGenCsharp;

public class CodeGenerator
{
    private readonly HashSet<string> _excludedSchemas =
    [
        "pg_catalog",
        "information_schema"
    ];

    private Options? _options;
    private Dictionary<string, Dictionary<string, Table>>? _tables;
    private Dictionary<string, Dictionary<string, Enum>>? _enums;
    private List<Query>? _queries;
    private DbDriver? _dbDriver;
    private QueriesGen? _queriesGen;
    private ModelsGen? _modelsGen;
    private UtilsGen? _utilsGen;
    private CsprojGen? _csprojGen;

    private Options Options
    {
        get => _options!;
        set => _options = value;
    }

    private Dictionary<string, Dictionary<string, Table>> Tables
    {
        get => _tables!;
        set => _tables = value;
    }

    private Dictionary<string, Dictionary<string, Enum>> Enums
    {
        get => _enums!;
        set => _enums = value;
    }

    private List<Query> Queries
    {
        get => _queries!;
        set => _queries = value;
    }

    private DbDriver DbDriver
    {
        get => _dbDriver!;
        set => _dbDriver = value;
    }

    private QueriesGen QueriesGen
    {
        get => _queriesGen!;
        set => _queriesGen = value;
    }

    private ModelsGen ModelsGen
    {
        get => _modelsGen!;
        set => _modelsGen = value;
    }

    private UtilsGen UtilsGen
    {
        get => _utilsGen!;
        set => _utilsGen = value;
    }

    private CsprojGen CsprojGen
    {
        get => _csprojGen!;
        set => _csprojGen = value;
    }

    private void InitGenerators(GenerateRequest generateRequest)
    {
        var outputDirectory = generateRequest.Settings.Codegen.Out;
        var projectName = new DirectoryInfo(outputDirectory).Name;
        Options = new Options(generateRequest);
        if (Options.DebugRequest)
            return;

        Queries = generateRequest.Queries.ToList();
        Tables = ConstructTablesLookup(generateRequest.Catalog);
        Enums = ConstructEnumsLookup(generateRequest.Catalog);

        var namespaceName = Options.NamespaceName == string.Empty ? projectName : Options.NamespaceName;
        DbDriver = InstantiateDriver(generateRequest.Catalog.DefaultSchema);

        // initialize file generators
        CsprojGen = new CsprojGen(outputDirectory, projectName, namespaceName, Options);
        QueriesGen = new QueriesGen(DbDriver, namespaceName);
        ModelsGen = new ModelsGen(DbDriver, namespaceName);
        UtilsGen = new UtilsGen(DbDriver, namespaceName);
    }

    private Dictionary<string, Dictionary<string, Table>> ConstructTablesLookup(Catalog catalog)
    {
        return catalog.Schemas
            .Where(s => !_excludedSchemas.Contains(s.Name))
            .ToDictionary(
                s => s.Name == catalog.DefaultSchema ? string.Empty : s.Name,
                s => s.Tables.ToDictionary(t => t.Rel.Name, t => t));
    }

    /// <summary>
    /// Enums in the request exist only in the default schema (in mysql), this remaps enums to their original schema.
    /// Unusual behavior we might have to fix in the future - TODO
    /// </summary>
    /// <param name="catalog"></param>
    /// <returns></returns>
    private Dictionary<string, Dictionary<string, Enum>> ConstructEnumsLookup(Catalog catalog)
    {
        var defaultSchemaCatalog = catalog.Schemas.First(s => s.Name == catalog.DefaultSchema);
        var schemaEnumTuples = defaultSchemaCatalog.Enums
            .Select(e => new
            {
                EnumItem = e,
                Schema = FindEnumSchema(e)
            });
        var schemaToEnums = schemaEnumTuples
            .GroupBy(x => x.Schema)
            .ToDictionary(
                group => group.Key,
                group => group.ToDictionary(
                    x => x.EnumItem.Name,
                    x => x.EnumItem)
            );
        return schemaToEnums;
    }

    private string FindEnumSchema(Enum e)
    {
        foreach (var schemaTables in Tables)
        {
            foreach (var table in schemaTables.Value)
            {
                var isEnumColumn = table.Value.Columns.Any(c => c.Type.Name == e.Name);
                if (isEnumColumn)
                    return schemaTables.Key;
            }
        }
        throw new InvalidDataException($"No enum {e.Name} schema found.");
    }

    private DbDriver InstantiateDriver(string defaultSchema)
    {
        return Options.DriverName switch
        {
            DriverName.MySqlConnector => new MySqlConnectorDriver(Options, defaultSchema, Tables, Enums, Queries),
            DriverName.Npgsql => new NpgsqlDriver(Options, defaultSchema, Tables, Enums, Queries),
            DriverName.Sqlite => new SqliteDriver(Options, defaultSchema, Tables, Enums, Queries),
            _ => throw new ArgumentException($"unknown driver: {Options.DriverName}")
        };
    }

    public Task<GenerateResponse> Generate(GenerateRequest generateRequest)
    {
        InitGenerators(generateRequest); // the request is necessary in order to know which generators are needed
        if (Options.DebugRequest)
            return Task.FromResult(new GenerateResponse
            {
                Files = { RequestToFile(generateRequest) }
            });

        var fileQueries = GetFileQueries();
        var files = fileQueries
            .Select(fq => QueriesGen.GenerateFile(fq.Value, fq.Key))
            .Append(ModelsGen.GenerateFile(Tables, Enums))
            .AppendIfNotNull(UtilsGen.GenerateFile())
            .AppendIf(CsprojGen.GenerateFile(), Options.GenerateCsproj);

        return Task.FromResult(new GenerateResponse { Files = { files } });

        Dictionary<string, Query[]> GetFileQueries()
        {
            return generateRequest.Queries
                .GroupBy(query => QueryFilenameToClassName(query.Filename))
                .ToDictionary(
                    group => group.Key,
                    group => group.ToArray());
        }

        string QueryFilenameToClassName(string filenameWithExtension)
        {
            return string.Concat(
                Path.GetFileNameWithoutExtension(filenameWithExtension).ToPascalCase(),
                Path.GetExtension(filenameWithExtension)[1..].ToPascalCase());
        }
    }

    private static Plugin.File RequestToFile(GenerateRequest request)
    {
        var formatter = new JsonFormatter(JsonFormatter.Settings.Default.WithIndentation());
        request.PluginOptions = ByteString.CopyFrom("{}", Encoding.UTF8);
        return new Plugin.File { Name = "request.json", Contents = ByteString.CopyFromUtf8(formatter.Format(request)) };
    }
}