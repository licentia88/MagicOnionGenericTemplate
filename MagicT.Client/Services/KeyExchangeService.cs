using MagicOnion;
using MagicT.Client.Filters;
using MagicT.Client.Models;
using MagicT.Client.Services.Base;
using MagicT.Shared.Helpers;
using MagicT.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Services;

/// <summary>
/// Diffie-Hellman key exchange service
/// </summary>
public sealed class KeyExchangeService : MagicTClientServiceBase<IKeyExchangeService, byte[]>, IKeyExchangeService
{

    public GlobalData GlobalData { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="filters"></param>
    public KeyExchangeService(IServiceProvider provider) : base(provider, new KeyExchangeFilter(provider))
    {
        GlobalData = provider.GetRequiredService<GlobalData>();
    }

  

    /// <summary>
    /// Makes a request to get a public Key from the server
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public UnaryResult<byte[]> RequestServerPublicKeyAsync()
    {
        return Client.RequestServerPublicKeyAsync();
    }

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