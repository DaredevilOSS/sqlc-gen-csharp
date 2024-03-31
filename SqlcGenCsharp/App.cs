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
        var codeGenerator = new CodeGenerator(generateRequest);
        ProtobufStreams.WriteOutput(codeGenerator.GenerateResponse);
    }
}