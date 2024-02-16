using System.Collections.Generic;

namespace sqlc_gen_csharp.Protobuf;

using ProtoBuf; // Ensure protobuf-net is referenced

[ProtoContract]
public class Settings
{
    [ProtoMember(1)]
    public string Version { get; set; } = "";

    [ProtoMember(2)]
    public string Engine { get; set; } = "";

    [ProtoMember(3)]
    public List<string> Schema { get; set; } = new List<string>();

    [ProtoMember(4)]
    public List<string> Queries { get; set; } = new List<string>();

    [ProtoMember(12)]
    public Codegen? Codegen { get; set; }

    // Additional constructor, methods, or logic as needed
}