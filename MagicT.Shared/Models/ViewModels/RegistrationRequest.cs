using MemoryPack;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public sealed partial class RegistrationRequest:LoginBase
{
    public string Name { get; set; }

    public string Surname { get; set; }

    public string PhoneNumber { get; set; }
    
    public string Password { get; set; }

    public string Email { get; set; }
}