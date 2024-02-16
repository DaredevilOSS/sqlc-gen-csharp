using ProtoBuf;

namespace sqlc_gen_csharp.Protobuf;
// Import the protobuf-net namespace

[ProtoContract]
public class CodegenProcess
{
    [ProtoMember(1)] public string Cmd { get; set; } = "";

    // Constructor and additional logic as needed
}

// Example usage and additional setup if necessary