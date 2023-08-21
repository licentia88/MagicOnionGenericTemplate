using MemoryPack;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public sealed partial class LoginRequest
{
    public string Identifier { get; set; }
    public string Password { get; set; }

}

 