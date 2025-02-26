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
    private Dictionary<string, Table>? _tables;
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

    private Dictionary<string, Table> Tables
    {
        get => _tables!;
        set => _tables = value;
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

        // TODO currently only default schema is considered - should consider all non-internal schemas
        Tables = generateRequest.Catalog.Schemas
            .Where(schema => schema.Name == generateRequest.Catalog.DefaultSchema)
            .SelectMany(schema => schema.Tables)
            .ToDictionary(table => table.Rel.Name, table => table);

        var namespaceName = Options.NamespaceName == string.Empty ? projectName : Options.NamespaceName;
        DbDriver = InstantiateDriver();

        // initialize file generators
        CsprojGen = new CsprojGen(outputDirectory, projectName, namespaceName, Options);
        QueriesGen = new QueriesGen(DbDriver, namespaceName);
        ModelsGen = new ModelsGen(DbDriver, namespaceName);
        UtilsGen = new UtilsGen(DbDriver, namespaceName);
    }

    private DbDriver InstantiateDriver()
    {
        return Options.DriverName switch
        {
            DriverName.MySqlConnector => new MySqlConnectorDriver(Options, Tables),
            DriverName.Npgsql => new NpgsqlDriver(Options, Tables),
            DriverName.Sqlite => new SqliteDriver(Options, Tables),
            _ => throw new ArgumentException($"unknown driver: {Options.DriverName}")
        };
    }

    public Task<GenerateResponse> Generate(GenerateRequest generateRequest)
    {
        InitGenerators(generateRequest); // the request is necessary in order to know which generators are needed
        var fileQueries = GetFileQueries();
        var files = fileQueries
            .Select(fq => QueriesGen.GenerateFile(fq.Value, fq.Key))
            .Append(ModelsGen.GenerateFile(Tables))
            .Append(UtilsGen.GenerateFile())
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