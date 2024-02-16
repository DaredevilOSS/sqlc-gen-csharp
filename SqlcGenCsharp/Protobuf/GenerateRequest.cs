using System;
using System.Collections.Generic;

namespace sqlc_gen_csharp.Protobuf;

using ProtoBuf; // Import protobuf-net namespace

[ProtoContract]
public class GenerateRequest
{
    [ProtoMember(1)]
    public Settings? Settings { get; set; }

    [ProtoMember(2)]
    public Catalog? Catalog { get; set; }

    [ProtoMember(3)]
    public List<Query> Queries { get; set; } = new List<Query>();

    [ProtoMember(4)]
    public string SqlcVersion { get; set; } = "";

    [ProtoMember(5)]
    public byte[] PluginOptions { get; set; } = Array.Empty<byte>();

    [ProtoMember(6)]
    public byte[] GlobalOptions { get; set; } = Array.Empty<byte>();

    // Constructor, methods, and additional logic as needed
}