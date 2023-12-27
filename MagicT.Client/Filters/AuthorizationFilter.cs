using System.Security.Authentication;
using Blazored.LocalStorage;
using MagicOnion.Client;
using MagicT.Client.Extensions;
using MagicT.Client.Managers;
using MagicT.Shared.Extensions;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models.ServiceModels;
//using Majorsoft.Blazor.Extensions.BrowserStorage;
//using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Filters;

/// <summary>
/// Filter for adding token to gRPC client requests.
/// </summary>
public class AuthorizationFilter : IClientFilter
{
    private KeyExchangeData GlobalData { get; set; }

    public IStorageManager StorageManager { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationFilter"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public AuthorizationFilter(IServiceProvider provider)
    {
        
        StorageManager = provider.GetService<IStorageManager>();
        GlobalData = provider.GetService<KeyExchangeData>();
    }

    /// <summary>
    /// Adds the authentication token to the request headers and sends the request.
    /// </summary>
    /// <param name="context">The request context.</param>
    /// <param name="next">The next step in the filter pipeline.</param>
    /// <returns>The response context.</returns>
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        //Token ve Id yi Encryptleyip Server e gonderir.
        
        var loginData = await StorageManager.GetLoginDataAsync() ?? throw new AuthenticationException("Failed to SignIn");

        var token = await StorageManager.GetTokenAsync() ?? throw new AuthenticationException("Security Token not found");

        var contactIdentfier = loginData.Identifier;

        var authData = new AuthenticationData(token, contactIdentfier);

        var cyptedAuthData = CryptoHelper.EncryptData(authData, GlobalData.SharedBytes);

        var cryptedAuthBin = cyptedAuthData.SerializeToBytes();

        context.CallOptions.Headers.AddorUpdateItem("crypted-auth-bin", cryptedAuthBin);

        return await next(context);
    }
}