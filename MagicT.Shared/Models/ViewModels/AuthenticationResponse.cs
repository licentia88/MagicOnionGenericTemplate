namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public partial class AuthenticationResponse: AuthenticationBase
{
    public byte[] Token { get; set; }

    public AuthenticationResponse(byte[] token, string identifier)
    {
        Token = token;
        Identifier = identifier;
    }   
    
}

 