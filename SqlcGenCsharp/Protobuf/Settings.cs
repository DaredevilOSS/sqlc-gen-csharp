using System.Collections.Generic;
using ProtoBuf;

namespace sqlc_gen_csharp.Protobuf;
// Ensure protobuf-net is referenced

[ProtoContract]
public class Settings
{
    [ProtoMember(1)] public string Version { get; set; } = "";

    [ProtoMember(2)] public string Engine { get; set; } = "";

    [ProtoMember(3)] public List<string> Schema { get; set; } = new();

    [ProtoMember(4)] public List<string> Queries { get; set; } = new();

    [ProtoMember(12)] public Codegen? Codegen { get; set; }

    // Additional constructor, methods, or logic as needed
}