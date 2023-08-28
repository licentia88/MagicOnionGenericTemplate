using MagicOnion;
using MagicT.Client.Models;
using MagicT.Client.Services.Base;
using MagicT.Shared.Helpers;
using MagicT.Shared.Services;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Services;

/// <summary>
/// Diffie-Hellman key exchange service
/// </summary>
public sealed class KeyExchangeService : MagicClientServiceBase<IKeyExchangeService, byte[]>, IKeyExchangeService
{

    /// <summary>
    ///  Local storage service
    /// </summary>
    public ILocalStorageService LocalStorageService { get; set; }
    
    /// <summary>
    /// Global data
    /// </summary>
    public GlobalData GlobalData { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="filters"></param>
    public KeyExchangeService(IServiceProvider provider) : base(provider)
    {
        GlobalData = provider.GetRequiredService<GlobalData>();
        LocalStorageService = provider.GetRequiredService<ILocalStorageService>();
    }


    UnaryResult<byte[]> IKeyExchangeService.RequestServerPublicKeyAsync()
    {
        return Client.RequestServerPublicKeyAsync();
    }

    /// <summary>
    /// Makes a request to get a public Key from the server
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task RequestServerPublicKeyAsync()
    {
        byte[] serverPublicKey = await ((IKeyExchangeService) this).RequestServerPublicKeyAsync();
        //Create client's public key bytes and private key
        var clientPublicKey = DiffieHellmanKeyExchange.CreatePublicKey();
        
        //Create shared key from server's public key and store it in LocalStorage
        var clientSharedKey = DiffieHellmanKeyExchange.CreateSharedKey(serverPublicKey, clientPublicKey.PrivateKey);

        //Store shared key in LocalStorage for data encryption
        await LocalStorageService.SetItemAsync("shared-bin", clientSharedKey);
  
        //Store client's public key in LocalStorage for sending to server on login or register
        await LocalStorageService.SetItemAsync("public-bin", clientPublicKey.PublicKeyBytes);

    }

    /// <summary>
    ///    Makes a request to get a public Key from the server
    /// </summary>
    public async Task GlobalKeyExchangeAsync()
    {
        var clientPublic = DiffieHellmanKeyExchange.CreatePublicKey();

        var serverPublic = await Client.GlobalKeyExchangeAsync(clientPublic.PublicKeyBytes);

        var sharedKey = DiffieHellmanKeyExchange.CreateSharedKey(serverPublic, clientPublic.PrivateKey);

        GlobalData.Shared = sharedKey;

    }

    UnaryResult<byte[]> IKeyExchangeService.GlobalKeyExchangeAsync(byte[] clientPublic)
    {
        var serverPublicKeyBytes = Client.GlobalKeyExchangeAsync(clientPublic);

        return serverPublicKeyBytes;
    }
}