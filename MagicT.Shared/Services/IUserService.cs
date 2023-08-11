using MagicOnion;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services.Base;

namespace MagicT.Shared.Services;

public interface IUserService : IMagicTService<IUserService, USERS>
{
    public UnaryResult<UserResponse> LoginAsync(LoginRequest loginRequest);

    public UnaryResult<UserResponse> RegisterAsync(RegistrationRequest registrationRequest);
}