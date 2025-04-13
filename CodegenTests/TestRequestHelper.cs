using Google.Protobuf;
using Plugin;
using System;
using System.IO;
using File = System.IO.File;

namespace CodegenTests;

public static class TestRequestHelper
{
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