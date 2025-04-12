using Google.Protobuf;
using Plugin;
using System;
using System.IO;
using File = System.IO.File;

namespace CodegenTests;

public static class RequestHelper
{
    public static void GenerateRequestFile(string filename)
    {
        var process = new System.Diagnostics.Process();
        var startInfo = new System.Diagnostics.ProcessStartInfo();
        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = "/C copy /b Image1.jpg + Archive.rar Image2.jpg";
        process.StartInfo = startInfo;
        process.Start();
    }

    public static GenerateRequest ParseRequestFile(string filename)
    {

        var baseDirectory = AppContext.BaseDirectory;
        var filePath = Path.Combine(baseDirectory, "test-requests", filename);

        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found", filePath);
        var contents = File.ReadAllText(filePath);
        var jsonParserSettings = JsonParser.Settings.Default.WithIgnoreUnknownFields(true);
        return new JsonParser(jsonParserSettings).Parse<GenerateRequest>(contents);
    }
}