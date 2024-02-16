using ProtoBuf;

// Ensure protobuf-net is referenced

namespace sqlc_gen_csharp.Protobuf;

[ProtoContract]
public class CompositeType
{
    [ProtoMember(1)] public string Name { get; set; } = "";

    [ProtoMember(2)] public string Comment { get; set; } = "";

    // Constructor and additional logic as needed
}