using System.Collections.Generic;
using ProtoBuf;

namespace sqlc_gen_csharp.Protobuf;

[ProtoContract]
public class Table
{
    [ProtoMember(1)]
    public Identifier? Rel { get; set; }

    [ProtoMember(2)]
    public List<Column> Columns { get; set; } = new List<Column>();

    [ProtoMember(3)]
    public string Comment { get; set; } = "";

    // Additional constructor, methods, or logic as needed
}