using Google.Protobuf;
using Microsoft.IO;
using Plugin;

namespace SqlcGenCsharp;

public static class ProtobufExtensions
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

    private static RecyclableMemoryStream _toStream(IMessage obj, RecyclableMemoryStreamManager memoryStreamManager)
    {
        var stream = memoryStreamManager.GetStream();
        stream.WriteTo(obj.ToByteArray());
        stream.Position = 0;

        return stream;
    }
}