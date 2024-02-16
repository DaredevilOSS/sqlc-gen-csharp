using System.Collections.Generic;
using ProtoBuf;

namespace sqlc_gen_csharp.Protobuf;
// Ensure protobuf-net is referenced

[ProtoContract]
public class Catalog
{
    [ProtoMember(1)] public string Comment { get; set; } = "";

    [ProtoMember(2)] public string DefaultSchema { get; set; } = "";

    [ProtoMember(3)] public string Name { get; set; } = "";

    [ProtoMember(4)] public List<Schema> Schemas { get; set; } = new();

    // Additional constructor, methods, or logic as needed
}