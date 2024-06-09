using MagicOnion;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services;
//using Majorsoft.Blazor.Extensions.BrowserStorage;
using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using MagicT.Shared.Managers;

namespace MagicT.Client.Services;

/// <summary>
/// Diffie-Hellman key exchange service
/// </summary>
[RegisterScoped]
public sealed class KeyExchangeService : MagicClientService<IKeyExchangeService, byte[]>, IKeyExchangeService
{
    private IKeyExchangeManager KeyExchangeManager { get; set; }

    /// <summary>
    /// Global data
    /// </summary>
    private KeyExchangeData KeyExchangeData { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="filters"></param>
    public KeyExchangeService(IServiceProvider provider) : base(provider) //new KeyExchangeFilter(provider)
    {
        KeyExchangeData = provider.GetService<KeyExchangeData>();
        LocalStorageService = provider.GetService<ILocalStorageService>();
        KeyExchangeManager = provider.GetService<IKeyExchangeManager>();
    }
 
    /// <summary>
    ///    Makes a request to get a public Key from the server and creates a shared Key
    ///    This Key is to be used during Register & Login to encrypt username and password on request
    /// </summary>
    public async Task GlobalKeyExchangeAsync()
    {
        var ClientPPKeyPair = KeyExchangeManager.CreatePublicKey();

        KeyExchangeData.SelfPublicBytes = ClientPPKeyPair.PublicBytes;

        KeyExchangeData.PrivateKey = ClientPPKeyPair.PrivateKey;

        var serverPublic = await Client.GlobalKeyExchangeAsync(KeyExchangeData.SelfPublicBytes);

        var sharedKey = KeyExchangeManager.CreateSharedKey(serverPublic, KeyExchangeData.PrivateKey);

        KeyExchangeData.SharedBytes = sharedKey;

        KeyExchangeData.OtherPublicBytes = serverPublic;
    }

    UnaryResult<byte[]> IKeyExchangeService.GlobalKeyExchangeAsync(byte[] clientPublic)
    {
        var serverPublicKeyBytes = Client.GlobalKeyExchangeAsync(clientPublic);

        return serverPublicKeyBytes;
    }
 
}