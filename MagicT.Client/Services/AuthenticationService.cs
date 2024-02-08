using MagicOnion;
using MagicT.Client.Filters;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

/// <inheritdoc cref="MagicT.Shared.Services.IAuthenticationService" />
[RegisterScoped]
public sealed class AuthenticationService : MagicClientServiceBase<IAuthenticationService>, IAuthenticationService
{
    /// <inheritdoc />
    public AuthenticationService(IServiceProvider provider)
        : base(provider, new AuthenticationFilter(provider))
    {
    }

    /// <inheritdoc />
    public UnaryResult<LoginResponse> LoginWithEmailAsync(LoginRequest loginRequest)
    {
        return Client.LoginWithEmailAsync(loginRequest);
    }

    /// <inheritdoc />
    public UnaryResult<LoginResponse> LoginWithUsername(LoginRequest loginRequest)
    {
        return Client.LoginWithUsername(loginRequest);
    }

    /// <inheritdoc />
    public UnaryResult<LoginResponse> LoginWithPhoneAsync(LoginRequest loginRequest)
    {
        return Client.LoginWithPhoneAsync(loginRequest);
    }

    /// <inheritdoc />
    public UnaryResult<LoginResponse> RegisterAsync(RegistrationRequest registrationRequest)
    {
        return Client.RegisterAsync(registrationRequest);
    }
}