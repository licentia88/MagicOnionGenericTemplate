using MagicOnion.Client;
using MagicT.Shared.Services;
using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using MagicT.Client.Extensions;
using MagicT.Shared.Models.ViewModels;
using System.Text;
//using Majorsoft.Blazor.Extensions.BrowserStorage;

namespace MagicT.Client.Filters;

/// <summary>
/// Diffie-Hellman key exchange filter
/// </summary>
public sealed class AuthenticationFilter : IClientFilter
{
    /// <summary>
    /// Local storage service
    /// </summary>
    public ILocalStorageService LocalStorageService { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public AuthenticationFilter(IServiceProvider provider)
    {
        LocalStorageService = provider.GetService<ILocalStorageService>();
    }

   
    /// <summary>
    /// Send public key to server and get server's public key
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        if (context.MethodPath == $"{nameof(IAuthenticationService)}/{nameof(IAuthenticationService.LoginWithPhoneAsync)}" ||
            context.MethodPath == $"{nameof(IAuthenticationService)}/{nameof(IAuthenticationService.LoginWithEmailAsync)}" ||
            context.MethodPath == $"{nameof(IAuthenticationService)}/{nameof(IAuthenticationService.RegisterAsync)}")
        {
            var publicKey = await LocalStorageService.GetItemAsync<byte[]>("public-bin");

            var publicKeyString = ASCIIEncoding.UTF8.GetString(publicKey);
            context.CallOptions.Headers.AddorUpdateItem("public-bin", publicKey);

            var response = await next(context);

            //Get UserResponse
            var userResponse = await response.GetResponseAs<LoginResponse>();

            //Set Token to localStorage
            await LocalStorageService.SetItemAsync("token-bin", userResponse.Token);

            return response;
        }

        return await next(context);
    }
}


 
 