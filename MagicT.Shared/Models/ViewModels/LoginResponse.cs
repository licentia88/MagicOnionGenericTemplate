using MemoryPack;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public partial class UserResponse:LoginBase
{
    public byte[] Token { get; set; }
    
}