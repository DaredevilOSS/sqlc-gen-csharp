using System;
using System.Collections.Generic;
using ProtoBuf;

namespace sqlc_gen_csharp.Protobuf;
// Ensure protobuf-net is referenced

[ProtoContract]
public class Codegen
{
    [ProtoMember(1)] public string Out { get; set; } = "";

    [ProtoMember(2)] public string Plugin { get; set; } = "";

    [ProtoMember(3)] public byte[] Options { get; set; } = Array.Empty<byte>();

    [ProtoMember(4)] public List<string> Env { get; set; } = new();

    [ProtoMember(5)] public CodegenProcess? Process { get; set; }

    [ProtoMember(6)] public CodegenWasm? Wasm { get; set; }
}