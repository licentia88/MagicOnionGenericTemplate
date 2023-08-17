using System.Security.Cryptography;
using MagicOnion.Client;
using MagicT.Shared.Enums;
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
 
        //Get server's public key bytes
        var serverPublicKey = await response.GetResponseAs<byte[]>();

        //Create client's public key bytes and private key
        var clientPublicKey = DiffieHellmanKeyExchange.CreatePublicKey();
        
        //Create shared key from server's public key and store it in LocalStorage
        var clientSharedKey = DiffieHellmanKeyExchange.CreateSharedKey(serverPublicKey, clientPublicKey.PrivateKey);

         //Store shared key in LocalStorage for data encryption
        await LocalStorageService.SetItemAsync("shared-bin", clientSharedKey);
  
        //Store client's public key in LocalStorage for sending to server on login or register
        await LocalStorageService.SetItemAsync("public-bin", clientPublicKey.PublicKeyBytes);

        return response;
    }
}


 
 