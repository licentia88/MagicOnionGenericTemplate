using MagicOnion;
using MagicT.Client.Filters;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

[RegisterScoped]
public sealed class AuthenticationService : MagicClientService<IAuthenticationService, AuthenticationModel>, IAuthenticationService
{
    public AuthenticationService(IServiceProvider provider)
        : base(provider, new AuthenticationFilter(provider))
    {
    }

    public UnaryResult<LoginResponse> LoginWithEmailAsync(LoginRequest loginRequest)
    {
        return Client.LoginWithEmailAsync(loginRequest);
    }

    public UnaryResult<LoginResponse> LoginWithPhoneAsync(LoginRequest loginRequest)
    {
        return Client.LoginWithPhoneAsync(loginRequest);
    }

    public UnaryResult<LoginResponse> RegisterAsync(RegistrationRequest registrationRequest)
    {
        return Client.RegisterAsync(registrationRequest);
    }
}