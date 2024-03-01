using Google.Protobuf;
using Microsoft.IO;
using Plugin;

namespace SqlcGenCsharpTest;

public static class PluginExtensions
{
    public static MemoryStream ToStream(this GenerateRequest generateRequest,
        RecyclableMemoryStreamManager memoryStreamManager)
    {
        return _toStream(generateRequest, memoryStreamManager);
    }
    
    public static MemoryStream ToStream(this GenerateResponse generateResponse,
        RecyclableMemoryStreamManager memoryStreamManager)
    {
        return _toStream(generateResponse, memoryStreamManager);
    }
    
    private static MemoryStream _toStream(IMessage obj, RecyclableMemoryStreamManager memoryStreamManager)
    {
        var stream = memoryStreamManager.GetStream();
        stream.WriteTo(obj.ToByteArray());
        stream.Position = 0;

        return stream;
    }
    
    public static bool ContentEquals(this MemoryStream ms1, MemoryStream ms2)
    {
        if (ms1.Length != ms2.Length)
            return false;
        ms1.Position = 0;
        ms2.Position = 0;

        var msArray1 = ms1.ToArray();
        var msArray2 = ms2.ToArray();

        return msArray1.SequenceEqual(msArray2);
    }
}