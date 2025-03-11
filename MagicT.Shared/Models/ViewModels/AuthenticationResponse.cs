using System.ComponentModel.DataAnnotations;

namespace MagicT.Shared.Models.ViewModels;


[MemoryPackable]
public partial class AuthenticationResponse: AuthenticationRequest// where TModel:class
{
    public byte[] Token { get; set; }

    public sealed override string Identifier { get; set; }

    [Required]
    public string CredentialType { get; set; }

    public AuthenticationResponse(byte[] token, string identifier)
    {
        Token = token;
        Identifier = identifier;
        CredentialType = nameof(USERS);
    }

    
}
 