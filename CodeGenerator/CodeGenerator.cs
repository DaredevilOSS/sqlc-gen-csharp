using Google.Protobuf;
using Plugin;
using SqlcGenCsharp.Drivers;
using SqlcGenCsharp.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharp;

public class CodeGenerator
{
    private Options? _options;
    private Dictionary<string, Dictionary<string, Table>> _tables;
    private Dictionary<string, Dictionary<string, Plugin.Enum>> _enums;
    private List<Query>? _queries;
    private DbDriver? _dbDriver;
    private QueriesGen? _queriesGen;
    private ModelsGen? _modelsGen;
    private UtilsGen? _utilsGen;
    private CsprojGen? _csprojGen;

    private readonly HashSet<string> _excludedSchemas =
    [
        "pg_catalog",
        "information_schema"
    ];

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

    private Dictionary<string, Dictionary<string, Plugin.Enum>> Enums
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

    private static void DebugWriteRequest(GenerateRequest generateRequest)
    {
        System.IO.File.WriteAllText($"/tmp/request_{generateRequest.Settings.Engine}.json", generateRequest.ToString());
    }

    private void InitGenerators(GenerateRequest generateRequest)
    {
        // DebugWriteRequest(generateRequest);

        var outputDirectory = generateRequest.Settings.Codegen.Out;
        var projectName = new DirectoryInfo(outputDirectory).Name;
        Options = new Options(generateRequest);

        Tables = generateRequest.Catalog.Schemas
            .Where(s => !_excludedSchemas.Contains(s.Name))
            .ToDictionary(
                s => s.Name == generateRequest.Catalog.DefaultSchema ? string.Empty : s.Name,
                s => s.Tables.ToDictionary(t => t.Rel.Name, t => t));

        Enums = generateRequest.Catalog.Schemas
            .Where(s => !_excludedSchemas.Contains(s.Name))
            .ToDictionary(
                s => s.Name == generateRequest.Catalog.DefaultSchema ? string.Empty : s.Name,
                s => s.Enums.ToDictionary(e => e.Name, e => e));

        Queries = generateRequest.Queries.ToList();

        var namespaceName = Options.NamespaceName == string.Empty ? projectName : Options.NamespaceName;
        DbDriver = InstantiateDriver(generateRequest.Catalog.DefaultSchema);

        // initialize file generators
        CsprojGen = new CsprojGen(outputDirectory, projectName, namespaceName, Options);
        QueriesGen = new QueriesGen(DbDriver, namespaceName);
        ModelsGen = new ModelsGen(DbDriver, namespaceName);
        UtilsGen = new UtilsGen(DbDriver, namespaceName);
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
}