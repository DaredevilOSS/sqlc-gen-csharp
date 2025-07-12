using System;
using System.IO;
using System.Threading.Tasks;

namespace SqlcGenCsharp;

public static class App
{
    public static void Main(string[] requestFiles)
    {
        for (int i = 0; i < requestFiles.Length; i++)
            ProcessRequestFile(requestFiles[i]);
    }

    private static async Task ProcessRequestFile(string requestFile)
    {
        if (!requestFile.EndsWith(".message"))
            return;
        Console.WriteLine($"Processing request file {requestFile}");
        var request = Plugin.GenerateRequest.Parser.ParseFrom(File.ReadAllBytes(requestFile));
        var response = await new CodeGenerator().Generate(request);
        Console.WriteLine($"Response: {response.Files.Count} output files");
    }
}