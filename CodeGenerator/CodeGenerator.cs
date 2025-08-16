using Google.Protobuf;
using Plugin;
using SqlcGenCsharp.Drivers;
using SqlcGenCsharp.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Enum = Plugin.Enum;

namespace SqlcGenCsharp;

public class CodeGenerator
{
    private Options? _options;
    private Catalog? _catalog;
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

    private Catalog Catalog
    {
        get => _catalog!;
        set => _catalog = value;
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
        Options = new(generateRequest);
        if (Options.DebugRequest)
            return;

        Queries = generateRequest.Queries.ToList();
        var namespaceName = Options.NamespaceName == string.Empty ? projectName : Options.NamespaceName;
        Catalog = generateRequest.Catalog;
        DbDriver = InstantiateDriver();

        // initialize file generators
        CsprojGen = new(outputDirectory, projectName, namespaceName, Options);
        QueriesGen = new(DbDriver, namespaceName);
        ModelsGen = new(DbDriver, namespaceName);
        UtilsGen = new(DbDriver, namespaceName);
    }

    private DbDriver InstantiateDriver()
    {
        return Options.DriverName switch
        {
            DriverName.MySqlConnector => new MySqlConnectorDriver(Options, Catalog, Queries),
            DriverName.Npgsql => new NpgsqlDriver(Options, Catalog, Queries),
            DriverName.Sqlite => new SqliteDriver(Options, Catalog, Queries),
            _ => throw new ArgumentException($"unknown driver: {Options.DriverName}")
        };
    }

    public Task<GenerateResponse> Generate(GenerateRequest generateRequest)
    {
        InitGenerators(generateRequest); // the request is necessary in order to know which generators are needed
        if (Options.DebugRequest)
            return Task.FromResult(new GenerateResponse
            {
                Files =
                {
                    RequestToJsonFile(generateRequest),
                    RequestToProtobufFile(generateRequest)
                }
            });

        var files = GetFileQueries()
            .Select(fq => QueriesGen.GenerateFile(fq.Value, fq.Key))
            .AddRangeExcludeNulls([
                ModelsGen.GenerateFile(DbDriver.Tables, DbDriver.Enums),
                UtilsGen.GenerateFile()
            ])
            .AddRangeIf([CsprojGen.GenerateFile()], Options.GenerateCsproj);

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

    private static ByteString GetOptionsWithoutDebugRequest(GenerateRequest request)
    {
        var text = Encoding.UTF8.GetString(request.PluginOptions.ToByteArray());
        var rawOptions = JsonSerializer.Deserialize<RawOptions>(text) ?? throw new InvalidOperationException();
        var newOptions = rawOptions with { DebugRequest = false };
        return ByteString.CopyFromUtf8(JsonSerializer.Serialize(newOptions));
    }

    private static Plugin.File RequestToJsonFile(GenerateRequest request)
    {
        var formatter = new JsonFormatter(JsonFormatter.Settings.Default.WithIndentation());
        request.PluginOptions = GetOptionsWithoutDebugRequest(request);
        return new() { Name = "request.json", Contents = ByteString.CopyFromUtf8(formatter.Format(request)) };
    }

    private static Plugin.File RequestToProtobufFile(GenerateRequest request)
    {
        request.PluginOptions = GetOptionsWithoutDebugRequest(request);
        return new() { Name = "request.message", Contents = request.ToByteString() };
    }
}