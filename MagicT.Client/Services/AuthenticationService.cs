using Benutomo;
using MagicOnion;
using MagicT.Client.Filters;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

/// <inheritdoc cref="MagicT.Shared.Services.IAuthenticationService" />
[RegisterScoped]
[AutomaticDisposeImpl]
public partial class AuthenticationService : MagicClientServiceBase<IAuthenticationService>, IAuthenticationService
{
    /// <inheritdoc />
    public AuthenticationService(IServiceProvider provider)
        : base(provider, new AuthenticationFilter(provider))
    {
    }

    ~AuthenticationService()
    {
        Dispose(false);
    }
    /// <inheritdoc />
    public UnaryResult<AuthenticationResponse> LoginWithEmailAsync(AuthenticationRequest authenticationRequest)
    {
        return Client.LoginWithEmailAsync(authenticationRequest);
    }

    /// <inheritdoc />
    public UnaryResult<AuthenticationResponse> LoginWithUsername(AuthenticationRequest authenticationRequest)
    {
        return Client.LoginWithUsername(authenticationRequest);
    }

    /// <inheritdoc />
    public UnaryResult<AuthenticationResponse> LoginWithPhoneAsync(AuthenticationRequest authenticationRequest)
    {
        return Client.LoginWithPhoneAsync(authenticationRequest);
    }

    /// <inheritdoc />
    public UnaryResult<AuthenticationResponse> RegisterAsync(RegistrationRequest registrationRequest)
    {
        return Client.RegisterAsync(registrationRequest);
    }
}