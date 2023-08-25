using MagicOnion;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;
using MagicT.Client.Filters;

namespace MagicT.Client.Services;

/// <summary>
/// User service
/// </summary>
public sealed class UserService : MagicClientServiceBase<IUserService, USERS>, IUserService
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="filters"></param>
    public UserService(IServiceProvider provider) : base(provider, new UserAuthenticationFilter(provider))
    {
    }


   

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="registrationRequest"></param>
    /// <returns></returns>
    public UnaryResult<UserResponse> RegisterAsync(RegistrationRequest registrationRequest)
    {
        return Client.RegisterAsync(registrationRequest);
    }

    /// <summary>
    /// Login with phone number
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    public UnaryResult<UserResponse> LoginWithPhoneAsync(LoginRequest loginRequest)
    {
        return Client.LoginWithPhoneAsync(loginRequest);
    }
    
    /// <summary>
    /// Login with email
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    public UnaryResult<UserResponse> LoginWithEmailAsync(LoginRequest loginRequest)
    {
        return Client.LoginWithEmailAsync(loginRequest);
    }
}