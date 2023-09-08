using MemoryPack;

namespace MagicT.Server.ZoneTree.Models;

[MemoryPackable]
public partial class UsersZone
{
    public int UserId { get; set; }

    public string Identifier { get; set; }

    public byte[] SharedKey { get; set; }
}

 