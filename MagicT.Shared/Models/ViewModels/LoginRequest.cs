using System.ComponentModel.DataAnnotations;
using MemoryPack;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public sealed partial class LoginRequest: AuthenticationBase
{
    [Required]
    public string Password { get; set; }

 }
