namespace SqlcGenCsharp;

public static class Runner
{
    public static void Run()
    {
        var generateRequest = ProtobufStreams.ReadInput();
        var generateResponse = CodeGenerator.Generate(generateRequest);
        ProtobufStreams.WriteOutput(generateResponse);
    }
}