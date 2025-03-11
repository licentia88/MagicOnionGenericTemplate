using System.ComponentModel.DataAnnotations;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
[MemoryPackUnion(1, typeof(AuthenticationRequest))]
[MemoryPackUnion(2, typeof(AuthenticationResponse))]
public abstract partial class AuthenticationBase
{
  
    public virtual string Identifier { get; set; }

    [MemoryPackIgnore] 
    public string AuthenticationType { get; set; }
}

