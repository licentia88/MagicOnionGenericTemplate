using System.Security.Cryptography;
using MagicOnion.Client;
using MagicT.Shared.Helpers;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Filters;

/// <summary>
/// Diffie-Hellman key exchange filter
/// </summary>
public sealed class KeyExchangeFilter : IClientFilter
{
    /// <summary>
    /// Local storage service
    /// </summary>
    public ILocalStorageService LocalStorageService { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public KeyExchangeFilter(IServiceProvider provider)
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
        var response = await next(context);
        
        //Triggers when Client does not have a sharedKey
        using ECDiffieHellmanCng clientDh = new();
 
        //Get server's public key
        var serverPublicKey = await response.GetResponseAs<byte[]>();


        //Create shared key from server's public key and store it in LocalStorage
        var clientSharedKey = clientDh.CreateSharedKey(serverPublicKey);

        //Store shared key in LocalStorage for data encryption
        await LocalStorageService.SetItemAsync("shared-bin", clientSharedKey);


        //Create public key and storage it in LocalStorage for later use in login or register
        var publicKey = clientDh.CreatePublicKey();

        await LocalStorageService.SetItemAsync("public-bin", publicKey);

        return response;
    }
}


 
 