using MagicOnion;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services.Base;

namespace MagicT.Shared.Services;

public interface IAuthenticationService : IMagicService<IAuthenticationService, AuthenticationModel>
{
    public UnaryResult<LoginResponse> LoginWithPhoneAsync(LoginRequest loginRequest);

    public UnaryResult<LoginResponse> LoginWithEmailAsync(LoginRequest loginRequest);

    public UnaryResult<LoginResponse> RegisterAsync(RegistrationRequest registrationRequest);
}