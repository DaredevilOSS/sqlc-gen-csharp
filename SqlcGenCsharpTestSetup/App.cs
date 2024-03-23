using Google.Protobuf;
using SqlcGenCsharp;

namespace SqlcGenCsharpTest
{
    public static class App
    {
        private static readonly JsonFormatter JsonFormatter = new(JsonFormatter.Settings.Default.WithIndentation());
        private static string? _directory;
    
        public static void Main()
        {
            RunFromSqlc();
        }

        private static void RunFromSqlc()
        {
            var generateRequest = ProtobufStreams.ReadInput();
            _directory = CreateDirectory(generateRequest.Settings.Codegen.Out);
            Dump(generateRequest, "generate-request");
            var codeGenerator = new CodeGenerator(generateRequest);
            Dump(codeGenerator.GenerateResponse, "generate-response");
        }

        private static void Dump(IMessage message, string messageType)
        {
            DumpMessageToJson(message, messageType);
            DumpProto(message, messageType);
        }
    
        private static string CreateDirectory(string baseDirectory)
        {
            var parentDirectory = Directory.GetParent(baseDirectory);
            var exampleName = parentDirectory?.Name;
            var examplesDirectory = $"{parentDirectory?.Parent?.FullName}/examples";
            var directory = $"{examplesDirectory}/{exampleName}/input";
        
            Directory.CreateDirectory(directory);
            return directory;
        }

        private static void DumpProto(IMessage message, string messageType)
        {
            var protoFilePath = $"{_directory}/{messageType}.proto";
            using var outputFileStream = File.Create(protoFilePath);
            message.WriteTo(outputFileStream);
        }

        private static void DumpMessageToJson(IMessage message, string messageType)
        {
            var jsonFilePath = $"{_directory}/{messageType}.json";
            using var debugOutputFileStream = File.CreateText(jsonFilePath);
            var stringRequest = JsonFormatter.Format(message);
            debugOutputFileStream.Write(stringRequest);
        }
    }
}