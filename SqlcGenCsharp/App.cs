using System;
using System.IO;
using Google.Protobuf;
using Plugin;
using File = System.IO.File;

namespace sqlc_gen_csharp;

public static class App
{
    private static readonly JsonFormatter JsonFormatter = new(JsonFormatter.Settings.Default.WithIndentation());

    // TODO refactor
    private static string _createDebugDirectory(string baseDirectory)
    {
        var parentDirectory = Directory.GetParent(baseDirectory);
        var exampleName = parentDirectory?.Name;
        var examplesDirectory = $"{parentDirectory?.Parent?.FullName}/examples";
        var debugDirectory = $"{examplesDirectory}/{exampleName}/debug";
        
        Directory.CreateDirectory(debugDirectory);
        return debugDirectory;
    }
    
    private static void _dumpRequestIfNeeded(GenerateRequest generateRequest)
    {
        if (Environment.GetEnvironmentVariable("DEBUG")!.Length == 0) return;
        var debugDirectory = _createDebugDirectory(generateRequest.Settings.Codegen.Out);
        
        var jsonFilePath = $"{debugDirectory}/generate-request.json";
        using var debugOutputFileStream = File.CreateText(jsonFilePath);
        var stringRequest = JsonFormatter.Format(generateRequest);
        debugOutputFileStream.Write(stringRequest);
        
        var protoFilePath = $"{debugDirectory}/generate-request.proto";
        using var outputFileStream = File.Create(protoFilePath);
        generateRequest.WriteTo(outputFileStream);
    }
    
    private static GenerateRequest ReadInput()
    {
        using var memoryStream = new MemoryStream();
        Console.OpenStandardInput().CopyTo(memoryStream);
        memoryStream.Position = 0;
        var generateRequest = GenerateRequest.Parser.ParseFrom(memoryStream);
        _dumpRequestIfNeeded(generateRequest);
        return generateRequest;
    }

    private static void WriteOutput(GenerateResponse output)
    {
        var encodedOutput = output.ToByteArray(); // Convert the Protobuf message to a byte array
        using var stdout = Console.OpenStandardOutput();
        stdout.Write(encodedOutput, 0, encodedOutput.Length);
    }

    public static void Run()
    {
        var generateRequest = ReadInput();
        var generateResponse = CodeGenerator.Generate(generateRequest);
        WriteOutput(generateResponse);
    }

    public static void Main()
    {
        Run();
    }
}