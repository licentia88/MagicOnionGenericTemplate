namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public sealed partial class RegistrationRequest:AuthenticationRequest
{
    public string PhoneNumber { get; set; }

    public string Name { get; set; }

    public string Lastname { get; set; }
    
    /// <summary>
    /// Gets or sets the confirmation password of the user.
    /// </summary>
    [MemoryPackIgnore]
    public string ConfirmPassword { get; set; }

    public string Email{ get; set; }
}