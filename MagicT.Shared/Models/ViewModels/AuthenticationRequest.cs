using System.ComponentModel.DataAnnotations;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public partial class AuthenticationRequest: AuthenticationBase
{
    [Required(ErrorMessage = "Password is required.")]
    public virtual string Password { get; set; }

    public AuthenticationRequest()
    {
        AuthenticationType = nameof(USERS);
    }
}