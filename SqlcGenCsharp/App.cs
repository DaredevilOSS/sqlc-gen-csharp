using System;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plugin;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using File = System.IO.File;

namespace sqlc_gen_csharp;

public class App
{
    private static GenerateRequest ReadInput()
    {
        // Reading from standard input stream
        using var memoryStream = new MemoryStream();
        Console.OpenStandardInput().CopyTo(memoryStream);
        memoryStream.Position = 0; // Resetting position to the beginning of the stream

        // Deserializing from binary data
        return GenerateRequest.Parser.ParseFrom(memoryStream);
    }

    private static void WriteOutput(GenerateResponse output)
    {
        var encodedOutput = output.ToByteArray(); // Convert the Protobuf message to a byte array
        using var stdout = Console.OpenStandardOutput();
        stdout.Write(encodedOutput, 0, encodedOutput.Length);
    }

    public static void Main()
    {
        using var writer = new StreamWriter("/tmp/sqlc-gen-csharp.log");
        var generateRequest = ReadInput();
        var generateResponse = CodeGenerator.Generate(generateRequest);
        if (generateResponse is not null) WriteOutput(generateResponse);
        if (generateResponse is null) writer.WriteLine("response is null");
    }
}