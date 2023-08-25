using MagicOnion;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services.Base;

namespace MagicT.Shared.Services;

public interface IUserService : IMagicService<IUserService, USERS>
{
    public UnaryResult<UserResponse> LoginWithPhoneAsync(LoginRequest loginRequest);
    
    public UnaryResult<UserResponse> LoginWithEmailAsync(LoginRequest loginRequest);

    public UnaryResult<UserResponse> RegisterAsync(RegistrationRequest registrationRequest);
}