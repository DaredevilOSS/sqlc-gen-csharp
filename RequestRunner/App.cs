using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharp;

public static class App
{
    public static async Task Main(string[] requestFiles)
    {
        foreach (var requestFile in requestFiles)
        {
            await ProcessRequestFile(requestFile);
            break;
        }
    }

    private static async Task ProcessRequestFile(string requestFile)
    {
        if (!requestFile.EndsWith(".message"))
            return;
        Console.WriteLine($"Processing request file {requestFile}");
        var request = Plugin.GenerateRequest.Parser.ParseFrom(File.ReadAllBytes(requestFile));
        var response = await new CodeGenerator().Generate(request);
        Console.WriteLine($"Response files: {response.Files.Select(f => f.Name).JoinByComma()}");
        Console.WriteLine("--------------------------------");
    }
}