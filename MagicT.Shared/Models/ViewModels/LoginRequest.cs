using System.ComponentModel.DataAnnotations;
using MemoryPack;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public sealed partial class LoginRequest
{
    [Required]
    public string Identifier { get; set; }

    [Required]
    public string Password { get; set; }

}

 