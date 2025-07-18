using File = System.IO.File;

namespace CodegenTests;

public static class TestRequestHelper
{
    public static Plugin.GenerateRequest ParseRequestFile(string filename)
    {
        var baseDirectory = AppContext.BaseDirectory;
        var filePath = Path.Combine(baseDirectory, "test-requests", filename);

        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found", filePath);
        return Plugin.GenerateRequest.Parser.ParseFrom(File.ReadAllBytes(filePath));
    }
}