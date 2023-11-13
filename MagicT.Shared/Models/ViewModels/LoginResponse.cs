using MemoryPack;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public partial class LoginResponse: AuthenticationBase
{
    public byte[] Token { get; set; }
    
}

 