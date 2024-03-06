namespace SqlcGenCsharp;

public static class App
{
    public static void Main()
    {
        Run();
    }
    
    public static void Run()
    {
        var generateRequest = ProtobufStreams.ReadInput();
        var generateResponse = CodeGenerator.Generate(generateRequest);
        ProtobufStreams.WriteOutput(generateResponse);
    }
}