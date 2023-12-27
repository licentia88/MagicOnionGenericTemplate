using System.ComponentModel.DataAnnotations;
using MemoryPack;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
[MemoryPackUnion(1, typeof(LoginRequest))]
[MemoryPackUnion(2, typeof(LoginResponse))]
[MemoryPackUnion(3, typeof(AuthenticationModel))]
public abstract partial class AuthenticationBase
{
    [Required]
    public string Identifier { get; set; }
}
