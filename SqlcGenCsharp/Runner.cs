using System;
using System.IO;
using Google.Protobuf;
using Plugin;

namespace SqlcGenCsharp;

public static class Runner
{
    public static GenerateRequest ReadInput()
    {
        using var memoryStream = new MemoryStream();
        Console.OpenStandardInput().CopyTo(memoryStream);
        memoryStream.Position = 0;
        var generateRequest = GenerateRequest.Parser.ParseFrom(memoryStream);
        return generateRequest;
    }

    private static void _writeOutput(GenerateResponse output)
    {
        var encodedOutput = output.ToByteArray();
        using var stdout = Console.OpenStandardOutput();
        stdout.Write(encodedOutput, 0, encodedOutput.Length);
    }

    public static void Run()
    {
        var generateRequest = ReadInput();
        var generateResponse = CodeGenerator.Generate(generateRequest);
        _writeOutput(generateResponse);
    }
}