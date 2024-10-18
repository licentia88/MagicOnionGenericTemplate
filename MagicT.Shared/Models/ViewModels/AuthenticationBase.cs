using System.ComponentModel.DataAnnotations;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
[MemoryPackUnion(1, typeof(AuthenticationRequest))]
[MemoryPackUnion(2, typeof(AuthenticationResponse))]
[MemoryPackUnion(3, typeof(AuthenticationModel))]
public abstract partial class AuthenticationBase
{
    [Required]
    public string Identifier { get; set; }
}
