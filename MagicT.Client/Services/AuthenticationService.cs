using Benutomo;
using MagicOnion;
using MagicT.Client.Filters;
using MagicT.Client.Managers;
using MagicT.Client.Services.Base;
using MagicT.Shared.Cryptography;
using MagicT.Shared.Managers;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Services;

/// <summary>
/// Provides secure authentication services with encrypted data.
/// </summary>
/// <inheritdoc cref="IAuthenticationService" />
[RegisterScoped(typeof(IAuthenticationService), typeof(ISecureAuthenticationService))]
[AutomaticDisposeImpl]
public partial class AuthenticationService : MagicClientServiceBase<IAuthenticationService>, IAuthenticationService, ISecureAuthenticationService
{
    /// <summary>
    /// Gets or sets the key exchange manager.
    /// </summary>
    public IKeyExchangeManager KeyExchangeManager { get; set; }

    [EnableAutomaticDispose]
    public LoginManager LoginManager { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public AuthenticationService(IServiceProvider provider) : base(provider, new AuthenticationFilter(provider))
    {
        KeyExchangeManager = provider.GetService<IKeyExchangeManager>();
        LoginManager = provider.GetService<LoginManager>();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="AuthenticationService"/> class.
    /// </summary>
    ~AuthenticationService()
    {
        Dispose(false);
    }

    /// <summary>
    /// Logs in a user with email using encrypted data.
    /// </summary>
    /// <param name="request">The authentication request.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the decrypted authentication response.</returns>
    public async UnaryResult<AuthenticationResponse> LoginWithEmailEncryptedAsync(AuthenticationRequest request)
    {
        var sharedKey = KeyExchangeManager.KeyExchangeData.SharedBytes;
        var encryptedData = CryptoHelper.EncryptData(request, sharedKey);
        var result = await Client.LoginWithEmailAsync(encryptedData);
        return CryptoHelper.DecryptData(result, LoginManager.UserShared);
    }

    /// <summary>
    /// Logs in a user with username using encrypted data.
    /// </summary>
    /// <param name="request">The authentication request.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the decrypted authentication response.</returns>
    public async UnaryResult<AuthenticationResponse> LoginWithUsernameEncryptedAsync(AuthenticationRequest request)
    {
        
        var sharedKey = KeyExchangeManager.KeyExchangeData.SharedBytes;
        var encryptedData = CryptoHelper.EncryptData(request, sharedKey);
        var result = await Client.LoginWithUsername(encryptedData);
        
        return CryptoHelper.DecryptData(result, LoginManager.UserShared);
    }

    /// <summary>
    /// Logs in a user with phone using encrypted data.
    /// </summary>
    /// <param name="request">The authentication request.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the decrypted authentication response.</returns>
    public async UnaryResult<AuthenticationResponse> LoginWithPhoneEncryptedAsync(AuthenticationRequest request)
    {
        var sharedKey = KeyExchangeManager.KeyExchangeData.SharedBytes;
        var encryptedData = CryptoHelper.EncryptData(request, sharedKey);
        var result = await Client.LoginWithPhoneAsync(encryptedData);
        
        return CryptoHelper.DecryptData(result, LoginManager.UserShared);
    }

    /// <summary>
    /// Registers a user using encrypted data.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the decrypted authentication response.</returns>
    public async UnaryResult<AuthenticationResponse> RegisterEncryptedAsync(RegistrationRequest request)
    {
        var sharedKey = KeyExchangeManager.KeyExchangeData.SharedBytes;
        var encryptedData = CryptoHelper.EncryptData(request, sharedKey);
        var result = await Client.RegisterAsync(encryptedData);
        return CryptoHelper.DecryptData(result, LoginManager.UserShared);
    }

    /// <summary>
    /// Logs in a user with phone using encrypted data.
    /// </summary>
    /// <param name="authenticationRequest">The encrypted authentication request.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted authentication response.</returns>
    public UnaryResult<EncryptedData<AuthenticationResponse>> LoginWithPhoneAsync(EncryptedData<AuthenticationRequest> authenticationRequest)
    {
        return Client.LoginWithPhoneAsync(authenticationRequest);
    }

    /// <summary>
    /// Logs in a user with email using encrypted data.
    /// </summary>
    /// <param name="authenticationRequest">The encrypted authentication request.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted authentication response.</returns>
    public UnaryResult<EncryptedData<AuthenticationResponse>> LoginWithEmailAsync(EncryptedData<AuthenticationRequest> authenticationRequest)
    {
        return Client.LoginWithEmailAsync(authenticationRequest);
    }

    /// <summary>
    /// Logs in a user with username using encrypted data.
    /// </summary>
    /// <param name="authenticationRequest">The encrypted authentication request.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted authentication response.</returns>
    public UnaryResult<EncryptedData<AuthenticationResponse>> LoginWithUsername(EncryptedData<AuthenticationRequest> authenticationRequest)
    {
        return Client.LoginWithUsername(authenticationRequest);
    }

    /// <summary>
    /// Registers a user using encrypted data.
    /// </summary>
    /// <param name="registrationRequest">The encrypted registration request.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the encrypted authentication response.</returns>
    public UnaryResult<EncryptedData<AuthenticationResponse>> RegisterAsync(EncryptedData<RegistrationRequest> registrationRequest)
    {
        return Client.RegisterAsync(registrationRequest);
    }
}