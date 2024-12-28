using Benutomo;
using MagicOnion;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services;
//using Majorsoft.Blazor.Extensions.BrowserStorage;
// using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.Extensions.DependencyInjection;
using MagicT.Shared.Managers;

namespace MagicT.Client.Services;

/// <summary>
/// Diffie-Hellman key exchange service
/// </summary>
[RegisterScoped]
[AutomaticDisposeImpl]
public partial class KeyExchangeService : MagicClientService<IKeyExchangeService, byte[]>, IKeyExchangeService, IDisposable
{
    /// <summary>
    /// Key-exchangeManager
    /// </summary>
    public IKeyExchangeManager KeyExchangeManager { get; set; }
 

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public KeyExchangeService(IServiceProvider provider) : base(provider) //new KeyExchangeFilter(provider)
    {
        // KeyExchangeData = provider.GetService<KeyExchangeData>();
        LocalStorageService = provider.GetService<ISessionStorageService>();
        KeyExchangeManager = provider.GetService<IKeyExchangeManager>();
    }
    
    ~KeyExchangeService()
    {
        Dispose(false);
    }
 
    /// <summary>
    ///    Makes a request to get a public Key from the server and creates a shared Key
    ///    This Key is to be used during Register & Login to encrypt username and password on request
    /// </summary>
    public async Task GlobalKeyExchangeAsync()
    {
        var clientPpKeyPair = KeyExchangeManager.CreatePublicKey();

        KeyExchangeManager.KeyExchangeData.SelfPublicBytes = clientPpKeyPair.PublicBytes;

        KeyExchangeManager.KeyExchangeData.PrivateKey = clientPpKeyPair.PrivateKey;

        var serverPublic = await Client.GlobalKeyExchangeAsync(KeyExchangeManager.KeyExchangeData.SelfPublicBytes);

        var sharedKey = KeyExchangeManager.CreateSharedKey(serverPublic, KeyExchangeManager.KeyExchangeData.PrivateKey);

        KeyExchangeManager.KeyExchangeData.SharedBytes = sharedKey;

        KeyExchangeManager.KeyExchangeData.OtherPublicBytes = serverPublic;
    }

    UnaryResult<byte[]> IKeyExchangeService.GlobalKeyExchangeAsync(byte[] clientPublic)
    {
        var serverPublicKeyBytes = Client.GlobalKeyExchangeAsync(clientPublic);

        return serverPublicKeyBytes;
    }

   
}