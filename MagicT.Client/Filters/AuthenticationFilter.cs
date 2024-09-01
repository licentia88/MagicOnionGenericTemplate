using MagicOnion.Client;
using MagicT.Shared.Services;
using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using MagicT.Client.Extensions;
using MagicT.Shared.Models.ViewModels;
using System.Text;

namespace MagicT.Client.Filters;

/// <summary>
/// Diffie-Hellman key exchange filter.
/// </summary>
public sealed class AuthenticationFilter : IClientFilter
{
    /// <summary>
    /// Gets the local storage service.
    /// </summary>
    private ILocalStorageService LocalStorageService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationFilter"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public AuthenticationFilter(IServiceProvider provider)
    {
        LocalStorageService = provider.GetService<ILocalStorageService>();
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
        
        var publicKey = await LocalStorageService.GetItemAsync<byte[]>("public-bin");
        // var publicKeyString = Encoding.UTF8.GetString(publicKey);
        context.CallOptions.Headers.AddOrUpdateItem("public-bin", publicKey);

        var response = await next(context);

        var userResponse = await response.GetResponseAs<LoginResponse>();
        await LocalStorageService.SetItemAsync("token-bin", userResponse.Token);

        return response;

    }

    /// <summary>
    /// Determines if the method path is an authentication method.
    /// </summary>
    /// <param name="methodPath">The method path.</param>
    /// <returns><c>true</c> if the method path is an authentication method; otherwise, <c>false</c>.</returns>
    private static bool IsAuthenticationMethod(string methodPath)
    {
        return methodPath == $"{nameof(IAuthenticationService)}/{nameof(IAuthenticationService.LoginWithPhoneAsync)}" ||
               methodPath == $"{nameof(IAuthenticationService)}/{nameof(IAuthenticationService.LoginWithEmailAsync)}" ||
               methodPath == $"{nameof(IAuthenticationService)}/{nameof(IAuthenticationService.LoginWithUsername)}" ||
               methodPath == $"{nameof(IAuthenticationService)}/{nameof(IAuthenticationService.RegisterAsync)}";
    }
}