using System.ComponentModel.DataAnnotations;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public sealed partial class AuthenticationRequest: AuthenticationBase
{
    [Required]
    public string Password { get; set; }

 }