using Grpc.Core;
using MagicOnion.Client;
using MagicT.Client.Exceptions;
using MagicT.Client.Extensions;
using MagicT.Client.Models;
using MagicT.Shared.Extensions;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models.ServiceModels;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Filters;

/// <summary>
/// Filter for adding authentication token to gRPC client requests.
/// </summary>
public class AuthenticationFilter : IClientFilter
{
    private GlobalData GlobalData { get; set; }

    public ILocalStorageService LocalStorageService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationFilter"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public AuthenticationFilter(IServiceProvider provider)
    {
        LocalStorageService = provider.GetService<ILocalStorageService>();
        GlobalData = provider.GetService<GlobalData>();
    }

    /// <summary>
    /// Adds the authentication token to the request headers and sends the request.
    /// </summary>
    /// <param name="context">The request context.</param>
    /// <param name="next">The next step in the filter pipeline.</param>
    /// <returns>The response context.</returns>
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        //var shared = await StorageService.GetItemAsync<byte[]>("shared-bin");

        var contactIdentfier = await LocalStorageService.GetItemAsync<string>("Identifier");

        var token = await LocalStorageService.GetItemAsync<byte[]>("token-bin");

        var authData = new AuthenticationData(token, contactIdentfier);

        var cyptedAuthData = CryptoHelper.EncryptData(authData, GlobalData.Shared);

        var cryptedAuthBin = cyptedAuthData.SerializeToBytes();

        if (token is null)
            throw new AuthException(StatusCode.NotFound, "Security Token not found");

        context.CallOptions.Headers.AddorUpdateItem("crypted-auth-bin", cryptedAuthBin);


        return await next(context);
    }
}