using System;
using System.IO;
using Google.Protobuf;
using Plugin;

namespace sqlc_gen_csharp;

public static class App
{
    private static GenerateRequest ReadInput()
    {
        using (var memoryStream = new MemoryStream())
        {
            Console.OpenStandardInput().CopyTo(memoryStream);
            memoryStream.Position = 0;
            return GenerateRequest.Parser.ParseFrom(memoryStream);
        }
    }

    private static void WriteOutput(GenerateResponse output)
    {
        var encodedOutput = output.ToByteArray(); // Convert the Protobuf message to a byte array
        using var stdout = Console.OpenStandardOutput();
        stdout.Write(encodedOutput, 0, encodedOutput.Length);
    }

    public static void Main()
    {
        var generateRequest = ReadInput();
        var generateResponse = CodeGenerator.Generate(generateRequest);
        WriteOutput(generateResponse);
    }
}