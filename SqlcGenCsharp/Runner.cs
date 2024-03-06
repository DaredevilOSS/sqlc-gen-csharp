namespace SqlcGenCsharp;
using ProtobufIO;

public static class Runner
{
    public static void Run()
    {
        var generateRequest = Utils.ReadInput();
        var generateResponse = CodeGenerator.CodeGenerator.Generate(generateRequest);
        Utils.WriteOutput(generateResponse);
    }
}