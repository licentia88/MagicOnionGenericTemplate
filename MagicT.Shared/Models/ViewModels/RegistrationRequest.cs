using MemoryPack;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public sealed partial class RegistrationRequest
{
    public string PhoneNumber { get; set; }

    public string Name { get; set; }

    public string Lastname { get; set; }
    
    public string Password { get; set; }

    public string Email{ get; set; }
}
