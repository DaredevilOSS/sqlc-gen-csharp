using System.Collections.Generic;
using ProtoBuf;

namespace sqlc_gen_csharp.Protobuf;
// Ensure protobuf-net is referenced

[ProtoContract]
public class Enum
{
    [ProtoMember(1)]
    public string Name { get; set; } = "";

    [ProtoMember(2)]
    public List<string> Vals { get; set; } = new List<string>();

    [ProtoMember(3)]
    public string Comment { get; set; } = "";

    // Constructor and additional logic as needed
}