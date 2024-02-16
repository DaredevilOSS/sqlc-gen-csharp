using System;
using System.IO;
using ProtoBuf;
using sqlc_gen_csharp.drivers;
using sqlc_gen_csharp.Protobuf;

namespace sqlc_gen_csharp;

public class App
{
    public static IDbDriver CreateNodeGenerator(string driver)
    {
        switch (driver)
        {
            case "MySqlConnector":
                return new MySqlConnector();
            default:
                throw new ArgumentException($"unknown driver: {driver}", nameof(driver));
        }
    }
    
    public static GenerateRequest ReadInput()
    {
        // Reading from standard input stream
        using (var memoryStream = new MemoryStream())
        {
            Console.OpenStandardInput().CopyTo(memoryStream);
            memoryStream.Position = 0; // Resetting position to the beginning of the stream

            // Deserializing from binary data
            return Serializer.Deserialize<GenerateRequest>(memoryStream);
        }
    }
    
    public static void Main()
    {
        // The static call below is generated at build time, and will list the syntax trees used in the compilation
        
    }
}