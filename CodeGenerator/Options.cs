using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Plugin;

namespace SqlcGenCsharp;

public class Options
{
    [JsonPropertyName("driver")]
    public string Driver { get; init; } = null!;
}

public static class OptionsParser
{
    public static Options Parse(GenerateRequest generateRequest)
    {
        var text = Encoding.UTF8.GetString(generateRequest.PluginOptions.ToByteArray());
        return JsonSerializer.Deserialize<Options>(text) ?? throw new InvalidOperationException();
    }   
}