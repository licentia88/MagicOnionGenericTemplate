using MemoryPack;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public partial class UserResponse
{
    public string Identifier { get; set; }

    public byte[] Token { get; set; }
    
}

 