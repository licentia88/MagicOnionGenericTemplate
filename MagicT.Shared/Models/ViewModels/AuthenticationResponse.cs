namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public partial class AuthenticationResponse: AuthenticationBase
{
    public byte[] Token { get; set; }
    
}

 