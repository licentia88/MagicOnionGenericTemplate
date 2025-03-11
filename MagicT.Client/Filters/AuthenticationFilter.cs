using MagicOnion.Client;
using MagicT.Shared.Services;
// using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using MagicT.Client.Extensions;
using MagicT.Shared.Models.ViewModels;
using Benutomo;
using Blazored.SessionStorage;
using MagicT.Client.Managers;
using MagicT.Shared.Cryptography;
using MagicT.Shared.Models.ServiceModels;

namespace MagicT.Client.Filters;

/// <summary>
/// Diffie-Hellman key exchange filter.
/// </summary>
[AutomaticDisposeImpl]
public partial class AuthenticationFilter : IClientFilter,IDisposable
{
    [EnableAutomaticDispose]
    LoginManager LoginManager { get; set; }
    /// <summary>
    /// Gets the local storage service.
    /// </summary>
    private ISessionStorageService LocalStorageService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationFilter"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public AuthenticationFilter(IServiceProvider provider)
    {
        LocalStorageService = provider.GetService<ISessionStorageService>();
        LoginManager = provider.GetService<LoginManager>();
    }

    ~AuthenticationFilter()
    {
        Dispose(false);
    }
    /// <summary>
    /// Sends the public key to the server and gets the server's public key.
    /// </summary>
    /// <param name="context">The request context.</param>
    /// <param name="next">The next filter in the pipeline.</param>
    /// <returns>The response context.</returns>
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        if (!IsAuthenticationMethod(context.MethodPath))
            return await next(context);

        var userPublicBytes = await LoginManager.CreateAndStoreUserPublics();
        // var publicKey = await LocalStorageService.GetItemAsync<byte[]>("user-public-bin");
        // var publicKeyString = Encoding.UTF8.GetString(publicKey);
        context.CallOptions.Headers.AddOrUpdateItem("public-bin", userPublicBytes);

        var response = await next(context);

        var encryptedResponse = await response.GetResponseAs<EncryptedData<AuthenticationResponse>>();

        var decryptedResponse = CryptoHelper.DecryptData(encryptedResponse, LoginManager.UserShared);

        await LocalStorageService.SetItemAsync("token-bin", decryptedResponse.Token);


        return response;

    }

    /// <summary>
    /// Determines if the method path is an authentication method.
    /// </summary>
    /// <param name="methodPath">The method path.</param>
    /// <returns><c>true</c> if the method path is an authentication method; otherwise, <c>false</c>.</returns>
    private static bool IsAuthenticationMethod(string methodPath)
    {
        return methodPath is $"{nameof(IAuthenticationService)}/{nameof(IAuthenticationService.LoginWithPhoneAsync)}" or
            $"{nameof(IAuthenticationService)}/{nameof(IAuthenticationService.LoginWithEmailAsync)}" or
            $"{nameof(IAuthenticationService)}/{nameof(IAuthenticationService.LoginWithUsername)}" or
            $"{nameof(IAuthenticationService)}/{nameof(IAuthenticationService.RegisterAsync)}";
    }
}