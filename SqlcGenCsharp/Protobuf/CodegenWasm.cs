using ProtoBuf;

// Make sure protobuf-net is referenced

namespace sqlc_gen_csharp.Protobuf;

[ProtoContract]
public class CodegenWasm
{
    [ProtoMember(1)]
    public string Url { get; set; } = "";

    [ProtoMember(2)]
    public string Sha256 { get; set; } = "";
}