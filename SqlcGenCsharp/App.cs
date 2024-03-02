using System;
using System.IO;
using Google.Protobuf;
using Plugin;
using File = System.IO.File;

namespace sqlc_gen_csharp;

public static class App
{
    private static void _dumpRequestIfNeeded(GenerateRequest generateRequest)
    {
        if (Environment.GetEnvironmentVariable("DEBUG")!.Length == 0) return;
        var outputFilePath = $"{typeof(GenerateRequest)}_{new Random().NextInt64()}.protobuf";
        using var outputFileStream = File.Create(outputFilePath);
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