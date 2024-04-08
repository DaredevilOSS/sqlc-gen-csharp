using System;
using System.IO;
using Google.Protobuf;
using Plugin;

namespace SqlcGenCsharp;

public static class App
{
    public static void Main()
    {
        Run();
    }

    private static void Run()
    {
        var generateRequest = ReadInput();
        var codeGenerator = new CodeGenerator(generateRequest);
        WriteOutput(codeGenerator.GenerateResponse);
    }

    private static GenerateRequest ReadInput()
    {
        using var memoryStream = new MemoryStream();
        Console.OpenStandardInput().CopyTo(memoryStream);
        memoryStream.Position = 0;
        var generateRequest = GenerateRequest.Parser.ParseFrom(memoryStream);
        return generateRequest;
    }

    private static void WriteOutput(GenerateResponse output)
    {
        var encodedOutput = output.ToByteArray();
        using var stdout = Console.OpenStandardOutput();
        stdout.Write(encodedOutput, 0, encodedOutput.Length);
    }
}