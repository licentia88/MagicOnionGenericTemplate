using MemoryPack;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public sealed partial class LoginRequest:LoginBase
{
    public string Password { get; set; }

}