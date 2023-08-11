using MemoryPack;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
[MemoryPackUnion(1,typeof(LoginRequest))]
[MemoryPackUnion(2,typeof(UserResponse))]
public abstract partial class LoginBase
{
    public int UserId { get; set; }
    
 
}