using MagicOnion;
using MagicOnion.Client;
using MagicT.Client.Filters;
using MagicT.Client.Services.Base;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

/// <summary>
/// Diffie-Hellman key exchange service
/// </summary>
public sealed class KeyExchangeService : MagicTClientServiceBase<IKeyExchangeService, byte[]>, IKeyExchangeService
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="filters"></param>
    public KeyExchangeService(IServiceProvider provider) : base(provider, new KeyExchangeFilter(provider))
    {
    }

    /// <summary>
    /// Exchange public keys
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public UnaryResult<byte[]> RequestServerPublicKeyAsync()
    {
        return Client.RequestServerPublicKeyAsync();
    }
}