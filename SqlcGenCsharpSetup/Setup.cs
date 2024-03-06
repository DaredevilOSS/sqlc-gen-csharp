using System.Diagnostics;
using Google.Protobuf;
using ProtobufIO;

namespace SqlcGenCsharpSetup;

public static class Setup
{
    private static readonly JsonFormatter JsonFormatter = new(JsonFormatter.Settings.Default.WithIndentation());
    private static string? _directory;
        
    public static void Run()
    {
        // var bashCommand = "sqlc -f examples/sqlc.test.yaml generate";
        var bashCommand = "sqlc --help";
        var startInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{bashCommand}\"",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        using var process = Process.Start(startInfo)!;
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        Console.WriteLine($"sqlc output: {output}\nsqlc error: {error}");
        process.WaitForExit();
    }
    
    public static void RunFromSqlc()
    {
        var generateRequest = Utils.ReadInput();
        _directory = _createDirectory(generateRequest.Settings.Codegen.Out);
        _dump(generateRequest, "generate-request");
        var generateResponse = CodeGenerator.CodeGenerator.Generate(generateRequest);
        _dump(generateResponse, "generate-response");
    }

    private static void _dump(IMessage message, string messageType)
    {
        _dumpJson(message, messageType);
        _dumpProto(message, messageType);
    }
    
    private static string _createDirectory(string baseDirectory)
    {
        var parentDirectory = Directory.GetParent(baseDirectory);
        var exampleName = parentDirectory?.Name;
        var examplesDirectory = $"{parentDirectory?.Parent?.FullName}/examples";
        var directory = $"{examplesDirectory}/{exampleName}/input";
        
        Directory.CreateDirectory(directory);
        return directory;
    }

    private static void _dumpProto(IMessage message, string messageType)
    {
        var protoFilePath = $"{_directory}/{messageType}.proto";
        using var outputFileStream = File.Create(protoFilePath);
        message.WriteTo(outputFileStream);
    }

    private static void _dumpJson(IMessage message, string messageType)
    {
        var jsonFilePath = $"{_directory}/{messageType}.json";
        using var debugOutputFileStream = File.CreateText(jsonFilePath);
        var stringRequest = JsonFormatter.Format(message);
        debugOutputFileStream.Write(stringRequest);
    }
}