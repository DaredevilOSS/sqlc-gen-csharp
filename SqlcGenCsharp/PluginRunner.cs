using Google.Protobuf;
using Plugin;

namespace SqlcGenCsharp;

public static class PluginRunner
{
    public static void Run()
    {
        var generateRequest = ReadInput();
        var generateResponse = new CodeGenerator().Generate(generateRequest);
        WriteOutput(generateResponse.Result);
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