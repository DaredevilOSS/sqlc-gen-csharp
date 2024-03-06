using Google.Protobuf;
using SqlcGenCsharp;

namespace SqlcGenCsharpSetup;

public static class App
{
    private static readonly JsonFormatter JsonFormatter = new(JsonFormatter.Settings.Default.WithIndentation());
    private static string? _directory;
    
    public static void Main()
    {
        _runFromSqlc();
    }

    private static void _runFromSqlc()
    {
        var generateRequest = ProtobufStreams.ReadInput();
        _directory = _createDirectory(generateRequest.Settings.Codegen.Out);
        _dump(generateRequest, "generate-request");
        var generateResponse = CodeGenerator.Generate(generateRequest);
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